using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Spin;
using Spin.Attributes;
using Spin.Utility;
using static Utitshala.Services.Interfaces;
using static Utitshala.Controllers.InputRegister;
using System.Text.RegularExpressions;
using Utitshala.Controllers;

namespace Utitshala.Services
{
    /// <summary>
    /// A class responsible for handling chat activities on the Telegram bot.
    /// </summary>
    public static class ChatEngine
    {
        #region Variables
        /* The message client to be used. An interface, meaning it can
         * be extended for other services (e.g., WhatsApp, FB Messenger, etc.) */
        public static IMessageClient messageClient;
        // Holds in memory chat sequences
        public static List<string[]> options;
        public static List<string[]> inputRegister;
        public static List<string[]> userStateRegister;
        #endregion

        /// <summary>
        /// Receives dialogue and initiates responses.
        /// </summary>
        /// <param name="sender">The object for this event.</param>
        /// <param name="e">The Telegram message arguments.</param>
        public static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            // Initialise Spin
            var sequence = new Sequence(new DictionaryBackend(), new FileDocumentLoader());
            sequence.RegisterStandardLibrary();

            // Get user ID
            string userId = e.Message.From.Id.ToString();

            // Get the path of the introductory dialogue document and load, if a new user
            string path = "";
            if (userStateRegister.FirstOrDefault(c => c[0] == userId)[1] == "registered"
                || e.Message.Text == "exit"
                || e.Message.Text == "Exit")
            {
                path = AppDomain.CurrentDomain.BaseDirectory + @"Dialogues\Default.spd";
            }
            else if (userStateRegister.FirstOrDefault(c => c[0] == userId)[1] == "learning")
            {
                path = AppDomain.CurrentDomain.BaseDirectory + @"Lessons\" + userStateRegister.FirstOrDefault(c => c[0] == userId)[2];
            }
            else if (userStateRegister.Where(c => c[0] == userId).Count() == 0)
            {
                path = AppDomain.CurrentDomain.BaseDirectory + @"Dialogues\Register.spd";
            }
            sequence.LoadAndStartDocument(path);

            // Add functional elements
            sequence.AddCommand("opt", ChatEngine.OptionTraversal);
            sequence.AddCommand("image", ChatEngine.ImageMessageHandler);
            sequence.AddCommand("sticker", ChatEngine.StickerMessageHandler);
            sequence.AddCommand("wait", ChatEngine.WaitTimer);
            sequence.AddCommand("input", ChatEngine.ReceiveInput);
            sequence.AddCommand("execute", ChatEngine.ExecuteFunction);

            // Set variables
            sequence.SetVariable("currentStickerUrl", "");
            sequence.SetVariable("currentImageUrl", "");
            sequence.SetVariable("currentImageCaption", "");
            sequence.SetVariable("currentInputRegex", "");
            sequence.SetVariable("currentUserId", userId);

            // The current ID of this chat, tracked for functions
            sequence.SetVariable("currentUserId", e.Message.From.Id);
            sequence.SetVariable("currentChat", e);

            // Set options, if applicable
            OptionInitiator(sequence, e.Message.Chat.Id.ToString(), e.Message.Text);

            // Grab input if applicable
            InputSaver(sequence, e.Message.Text, e.Message.Chat.Id.ToString());

            // Set the line to send
            string currentLine;

            // Send the opening message
            while (sequence.StartNextLine().HasValue)
            {
                // Get the current line
                currentLine = sequence.ExecuteCurrentLine().BuildString(true, false);
                // Sleep so that any executed lines can complete their message
                Thread.Sleep(100);
                // Message the line based on media type
                if (sequence.GetVariable("currentImageUrl").ToString() != "")
                {
                    // Send the message
                    messageClient.SendImageMessage(sequence.GetVariable("currentImageUrl").ToString(), 
                        sequence.GetVariable("currentImageCaption").ToString(), e);

                    // Zero the vars
                    sequence.SetVariable("currentImageUrl", "");
                    sequence.SetVariable("currentImageCaption", "");
                }
                else if (sequence.GetVariable("currentStickerUrl").ToString() != "")
                {
                    // Send the message
                    messageClient.SendStickerMessage(sequence.GetVariable("currentStickerUrl").ToString(), e);

                    // Zero the var
                    sequence.SetVariable("currentStickerUrl", "");
                }
                else if (e.Message.Text != null)
                {
                    // Send the message
                    messageClient.SendTextMessage(currentLine, e);
                }
            }
        }

        #region Helper Functions
        /// <summary>
        /// Sets the current option line for a specific chat ID.
        /// </summary>
        /// <param name="sequence">The sequence to act on.</param>
        /// <param name="chatId">The ID of the chat to check options for.</param>
        public static void OptionInitiator(Sequence sequence, string userId, string messageText)
        {
            // Check for an option choice before execution
            if (options.Where(c => c[0] == userId).Count() != 0)
            {
                foreach (var opt in options.Where(c => c[0] == userId))
                {
                    // Check to see if the input message is the same as the option identifier
                    if (messageText == opt[1])
                    {
                        // Set the next line and break
                        sequence.SetNextLine(opt[2]);
                        break;
                    }
                }
                // Clean the options from this list once used
                foreach (var opt in options.Where(c => c[0] == userId).ToList())
                {
                    options.Remove(opt);
                }
            }
        }

        /// <summary>
        /// Save the requested input from a chat.
        /// </summary>
        /// <param name="sequence">The sequence to act upon.</param>
        /// <param name="chatId">The ID of the current chat.</param>
        public static void InputSaver(Sequence sequence, string input, string userId)
        {
            if (inputRegister.Where(c => c[0] == userId).Count() != 0)
            {
                foreach (var read in inputRegister.Where(c => c[0] == userId))
                {
                    // Perform a regex check
                    Regex reg = new Regex("");
                    switch (read[2])
                    {
                        case "textspaced":
                            reg = new Regex(@"^[a-z A-Z,.'-]+$");
                            break;
                        case "textnonspaced":
                            reg = new Regex(@"^[A-Za-z]+$");
                            break;
                        case "anynumber":
                            reg = new Regex(@"^([-+]?[0-9]+[-,])*[+-]?[0-9]+$");
                            break;
                        case "positivenumber":
                            reg = new Regex(@"^[+]?\d+([.]\d+)?$");
                            break;
                        case "emailaddress":
                            reg = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                            break;
                    }
                    if (reg.IsMatch(input))
                    {
                        // Act on the input, depending on its descriptor
                        switch (read[1])
                        {
                            case "register":
                                // Do registration here
                                InputRegister.RegisterStudent(userId, input);
                                userStateRegister.Add(new string[] { userId, "registered" });
                                sequence.SetNextLine(read[4]);
                                break;
                            case "classregister":
                                bool result = DatabaseController.RegisterWithClass(userId, input);
                                // Go A or B, depending on success
                                if (result)
                                {
                                    sequence.SetNextLine(read[4]);
                                }
                                else
                                {
                                    sequence.SetNextLine(read[3]);
                                }
                                break;
                            case "openlesson":
                                // Get the learning design Url
                                string resultUrl = DatabaseController.GetLessonUrl(Convert.ToInt32(input));
                                // Add it to the state machine
                                if (resultUrl != "")
                                {
                                    try
                                    {
                                        userStateRegister.Remove(userStateRegister.FirstOrDefault(c => c[0] == userId));
                                        userStateRegister.Add(new string[] { userId, "learning", resultUrl });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.StackTrace);
                                    }
                                    // Set the true sequence output
                                    sequence.SetNextLine(read[4]);
                                }
                                else
                                {
                                    // Set the false sequence output
                                    sequence.SetNextLine(read[2]);
                                }
                                break;
                            case "chooselanguage":
                                // Do registration here
                                InputRegister.ChooseLanguage(userId, input);
                                sequence.SetNextLine(read[4]);
                                break;
                            case "viewprofile":
                                InputRegister.ViewProfile(userId, input);
                                sequence.SetNextLine(read[4]);
                                break;
                            case "viewrecord":
                                InputRegister.ViewRecord(userId, input);
                                sequence.SetNextLine(read[4]);
                                break;
                            case "editprofile":
                                InputRegister.EditProfile(userId, input);
                                sequence.SetNextLine(read[4]);
                                break;
                        }
                    }
                    else
                    {
                        // If failed regex, tell the fail condition to be met
                        sequence.SetNextLine(read[3]);
                    }
                }
                // Clean the inputs from this list once used
                foreach (var read in inputRegister.Where(c => c[0] == userId).ToList())
                {
                    inputRegister.Remove(read);
                }
            }
        }
        #endregion

        #region Embedded Sequence Functions
        /// <summary>
        /// Handles text input options by logging choices in the
        /// current options list.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The list of options and directions.</param>
        [SequenceCommand("opt")]
        public static void OptionTraversal(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("opt", arguments, 2);

            // Derive the argumentss
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);

            // Add options to the options list
            options.Add(new string[] { sequence.GetVariable("currentUserId").ToString(), arg1.ToString(), arg2.ToString() });
        }

        /// <summary>
        /// Handles image messages.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The image URL and caption.</param>
        [SequenceCommand("image")]
        public static void ImageMessageHandler(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("image", arguments, 2);

            // Derive the arguments
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);

            // Add the image URL to the send slot
            sequence.SetVariable("currentImageUrl", arg1.ToString());
            sequence.SetVariable("currentImageCaption", arg2.ToString());
        }

        /// <summary>
        /// Handles sticker messages.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The sticker URL.</param>
        [SequenceCommand("sticker")]
        public static void StickerMessageHandler(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("sticker", arguments, 1);

            // Derive the arguments
            var arg1 = sequence.Resolve(arguments[0]);

            // Add the sticker URL to the send slot
            sequence.SetVariable("currentStickerUrl", arg1.ToString());
        }

        /// <summary>
        /// Sets a timer that delays output.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The timer count in milliseconds.</param>
        [SequenceCommand("wait")]
        public static void WaitTimer(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("wait", arguments, 1);

            // Derive the argument
            var arg1 = sequence.Resolve(arguments[0]);

            // Act
            Thread.Sleep(Convert.ToInt32(arg1));
        }

        /// <summary>
        /// Sets an input flag for the next input.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The RegEx to apply to this input.</param>
        [SequenceCommand("input")]
        public static void ReceiveInput(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("input", arguments, 4);

            // Derive the arguments
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);
            var arg3 = sequence.Resolve(arguments[2]);
            var arg4 = sequence.Resolve(arguments[3]);

            // Act
            inputRegister.Add(new string[] { sequence.GetVariable("currentUserId").ToString(), 
                arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString() });
        }

        /// <summary>
        /// Executes a specific function, based on a sequence command.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The name of the function to execute.</param>
        [SequenceCommand("execute")]
        public static void ExecuteFunction(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("input", arguments, 3);

            // Derive the argument
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);
            var arg3 = sequence.Resolve(arguments[2]);

            // Get the user ID from this sequence and check its presence
            string user = sequence.GetVariable("currentUserId").ToString();

            // Act upon the function, based on name (extend this)
            switch (arg1.ToString()) {
                case "classcheck":
                    // Get the user ID from this sequence and check its presence
                    bool result = DatabaseController.CheckClassPresence(user);
                    // Go to an output based on the result
                    if (result)
                    {
                        sequence.SetNextLine(arg3.ToString());
                    }
                    else
                    {
                        sequence.SetNextLine(arg2.ToString());
                    }
                    break;
                case "leaveclassroom":
                    // Remove the student from the classroom
                    bool leaveResult = DatabaseController.LeaveClassroom(user);
                    // If successful, set the next line
                    if (leaveResult)
                    {
                        sequence.SetNextLine(arg3.ToString());
                    }
                    break;
                case "getlessons":
                    List<string[]> results = DatabaseController.GetLessons(user);
                    // Construct a message and send
                    string toSend = "";
                    foreach (var ent in results)
                    {
                        toSend += ent[0] + ": " + ent[1] + "\n";
                    }
                    toSend += "Enter the number of the lesson you want to run!";
                    messageClient.SendTextMessage(toSend, sequence.GetVariable("currentChat"));
                    break;
                case "viewprofile":
                    string profile = DatabaseController.GetStudentProfile(user);
                    messageClient.SendTextMessage(profile, sequence.GetVariable("currentChat"));
                    break;
            }
        }
        #endregion
    }
}