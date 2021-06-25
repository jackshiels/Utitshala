// System
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
// Chat-specific 
using Telegram.Bot.Args;
using Spin;
using static Utitshala.Services.Interfaces;
using Utitshala.Controllers;
using Utitshala.Models;

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
            #region Construct Sequence
            // Initialise Spin
            var sequence = new LearningSequence(new DictionaryBackend(), new FileDocumentLoader());
            sequence.RegisterStandardLibrary();

            // Get user ID
            string userId = e.Message.From.Id.ToString();

            // The current chat and ID of this chat, tracked for functions
            sequence.SetVariable("currentUserId", userId);
            sequence.SetVariable("currentChat", e);
            #endregion

            #region Select Dialogue File
            // The dialogue path
            string path = "";
            string[] userState = userStateRegister.FirstOrDefault(c => c[0] == userId);

            // Exit clause
            if (e.Message.Text != null && e.Message.Text.ToLower() == "exit")
            {
                if (userState != null && userState[1] == "learning"
                    || userState[1] == "assessing")
                {
                    // Close the session
                    DatabaseController.CloseSession(Convert.ToInt32(userState[4]), false);
                    path = AppDomain.CurrentDomain.BaseDirectory + @"Dialogues\Default.spd";
                    userStateRegister.Remove(userState);
                    userStateRegister.Add(new string[] { userId, "registered" });
                    // Send a message to confirm
                    messageClient.SendTextMessage("Exiting...", e);
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
                if (e.Message.Text != null)
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
                                        Bot_OnMessage(null, null);
                                        // sequence.SetNextLine(read[4]);
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
                                case "openassignment":
                                    // Get the assignment, if it exists and they have permission
                                    Assignment assignment = DatabaseController.GetAssignment(userId, Convert.ToInt32(input));

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
                                case "wordcheck":
                                    // Check if the input is valid
                                    if (input.ToLower() == read[3].ToLower())
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
                            // If failed regex, tell the fail condition to be met, but only if not an assessment
                            if (read[1] != "mcq")
                            {
                                sequence.SetNextLine(read[3]);
                            }
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
    }
}