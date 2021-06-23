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

            #region Select Dialogue File
            // The dialogue path
            string path = "";
            string[] userState = userStateRegister.FirstOrDefault(c => c[0] == userId);

            // Exit clause
            if (e.Message.Text.ToLower() == "exit")
            {
                if (userState != null && userState[1] == "learning"
                    || userState[1] == "assessing")
                {
                    // Close the session
                    DatabaseController.CloseSession(Convert.ToInt32(userState[4]), false);
                    path = AppDomain.CurrentDomain.BaseDirectory + @"Dialogues\Default.spd";
                    userStateRegister.Remove(userState);
                    userStateRegister.Add(new string[] { userId, "registered" });
                }
            }

            // Get the path of the introductory dialogue document and load, if a new user
            if (userState == null)
            {
                path = AppDomain.CurrentDomain.BaseDirectory + @"Dialogues\Register.spd";
            }
            else
            {
                switch (userStateRegister.FirstOrDefault(c => c[0] == userId)[1])
                {
                    case "registered":
                        // Set the path to default
                        path = AppDomain.CurrentDomain.BaseDirectory + @"Dialogues\Default.spd";
                        break;
                    case "learning":
                        path = AppDomain.CurrentDomain.BaseDirectory + @"LearningContent\Lessons\" + userStateRegister.FirstOrDefault(c => c[0] == userId)[3];
                        break;
                    case "assessing":
                        path = AppDomain.CurrentDomain.BaseDirectory + @"LearningContent\Assessments\" + userStateRegister.FirstOrDefault(c => c[0] == userId)[3];
                        break;
                }
            }
            sequence.LoadAndStartDocument(path);
            #endregion

            #region Construct Sequence
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
            #endregion

            #region Sequence Utilities
            // Set options, if applicable
            OptionInitiator(sequence, e.Message.Chat.Id.ToString(), e.Message.Text);

            // Grab input if applicable
            InputSaver(sequence, e.Message.Text, e.Message.Chat.Id.ToString());
            #endregion

            #region Input Cycle
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
            #endregion
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
        /// Save the requested input from chat according to its classification.
        /// </summary>
        /// <param name="sequence">The sequence the input is based on.</param>
        /// <param name="input">The text input.</param>
        /// <param name="userId">The user ID of the user conducting input.</param>
        public static void InputSaver(Sequence sequence, string input, string userId)
        {
            if (inputRegister.Where(c => c[0] == userId).Count() != 0)
            {
                foreach (var read in inputRegister.Where(c => c[0] == userId))
                {
                    // Back bavigation check
                    if (input == "0"
                        && read[5] != "")
                    {
                        sequence.SetNextLine(read[5].ToString());
                    }
                    else
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
                                    DatabaseController.RegisterStudent(userId, input);
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
                                    string resultLessonUrl = DatabaseController.GetLessonUrl(Convert.ToInt32(input));
                                    // Add it to the state machine
                                    if (resultLessonUrl != "")
                                    {
                                        try
                                        {
                                            // Create a session in the database
                                            int sessionId = DatabaseController.StartSessionLearningDesign(userId, Convert.ToInt32(input));
                                            userStateRegister.Remove(userStateRegister.FirstOrDefault(c => c[0] == userId));
                                            userStateRegister.Add(new string[] { userId, "learning", input, resultLessonUrl, sessionId.ToString() });
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
                                case "openassessment":
                                    // Get the learning design Url
                                    string resultAssessmentUrl = DatabaseController.GetAssessmentUrl(Convert.ToInt32(input));
                                    // Add it to the state machine
                                    if (resultAssessmentUrl != "")
                                    {
                                        try
                                        {
                                            // Create a session in the database
                                            int sessionId = DatabaseController.StartSessionAssessment(userId, Convert.ToInt32(input));
                                            userStateRegister.Remove(userStateRegister.FirstOrDefault(c => c[0] == userId));
                                            userStateRegister.Add(new string[] { userId, "assessing", input, resultAssessmentUrl, sessionId.ToString(), "0" });
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
                                // Assessment inputs
                                case "mcq":
                                    // Check if the input is valid
                                    if (input == read[3])
                                    {
                                        // Correct answer leads to the state register being updated
                                        int score = Convert.ToInt32(userStateRegister.FirstOrDefault(c => c[0] == userId)[5]);
                                        score += 1;
                                        userStateRegister.FirstOrDefault(c => c[0] == userId)[5] = score.ToString();
                                    }
                                    // Set the next line
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
            ArgumentUtils.Count("input", arguments, 5);

            // Derive the arguments
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);
            var arg3 = sequence.Resolve(arguments[2]);
            var arg4 = sequence.Resolve(arguments[3]);
            var arg5 = sequence.Resolve(arguments[4]);

            // Act
            inputRegister.Add(new string[] { sequence.GetVariable("currentUserId").ToString(), 
                arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString(), arg5.ToString() });
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
            ArgumentUtils.Count("input", arguments, 4);

            // Derive the argument
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);
            var arg3 = sequence.Resolve(arguments[2]);
            var arg4 = sequence.Resolve(arguments[3]);

            // Get the user ID from this sequence and check its presence
            string userId = sequence.GetVariable("currentUserId").ToString();

            // Act upon the function, based on name
            switch (arg1.ToString()) {
                // PERSONAL ---------------------------------------------------------
                case "viewprofile":
                    string profile = DatabaseController.GetStudentProfile(userId);
                    messageClient.SendTextMessage(profile, sequence.GetVariable("currentChat"));
                    break;
                // CLASSROOM --------------------------------------------------------
                case "classcheck":
                    // Get the user ID from this sequence and check its presence
                    bool result = DatabaseController.CheckClassPresence(userId);
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
                    bool leaveResult = DatabaseController.LeaveClassroom(userId);
                    // If successful, set the next line
                    if (leaveResult)
                    {
                        sequence.SetNextLine(arg3.ToString());
                    }
                    break;
                // LEARNING/WORKING --------------------------------------------------
                case "getlessons":
                    List<string[]> resultsLessons = DatabaseController.GetLessons(userId);
                    // Construct a message and send
                    string toSendLessons = "0: Back\n";
                    foreach (var ent in resultsLessons)
                    {
                        toSendLessons += ent[0] + ": " + ent[1] + "\n";
                    }
                    toSendLessons += "Enter the number of the lesson you want to start.";
                    messageClient.SendTextMessage(toSendLessons, sequence.GetVariable("currentChat"));
                    break;
                case "getassessments":
                    List<string[]> resultsAssessments = DatabaseController.GetAssessments(userId);
                    // Construct a message and send
                    string toSendAssessments = "0: Back\n";
                    foreach (var ent in resultsAssessments)
                    {
                        toSendAssessments += ent[0] + ": " + ent[1] + "\n";
                    }
                    toSendAssessments += "Enter the number of the assessment you want to start.";
                    messageClient.SendTextMessage(toSendAssessments, sequence.GetVariable("currentChat"));
                    break;
                case "closesession":
                    string[] userStateCloseSession = userStateRegister.FirstOrDefault(c => c[0] == userId);
                    // Close the session
                    bool closeResult = DatabaseController.CloseSession(Convert.ToInt32(userStateCloseSession[4]), true);
                    if (closeResult)
                    {
                        // Remove the current user state and replace with default
                        userStateRegister.Remove(userStateCloseSession);
                        userStateRegister.Add(new string[] { userId, "registered" });
                        sequence.SetNextLine(arg3.ToString());
                    }
                    break;
                case "checkscore":
                    string[] userStateScore = userStateRegister.FirstOrDefault(c => c[0] == userId);
                    // Get the user's score
                    int score = Convert.ToInt32(userStateRegister.FirstOrDefault(c => c[0] == userId)[5]);
                    int sessionId = Convert.ToInt32(userStateRegister.FirstOrDefault(c => c[0] == userId)[4]);
                    // Get the assessment's score
                    Tuple<bool, decimal> results = DatabaseController.CheckScore(score, sessionId, Convert.ToDecimal(arg2));
                    // Send the score to the user
                    string message = "Your score is: "+ results.Item2.ToString() + "%";
                    messageClient.SendTextMessage(message, sequence.GetVariable("currentChat"));
                    if (results.Item1) // If passed
                    {
                        sequence.SetNextLine(arg4.ToString());
                    }
                    else // If failed
                    {
                        sequence.SetNextLine(arg3.ToString());
                    }
                    break;
            }
        }
        #endregion
    }
}