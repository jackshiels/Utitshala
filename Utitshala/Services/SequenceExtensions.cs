using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Spin;
using Spin.Attributes;
using Spin.Utility;
using Utitshala.Controllers;
using Utitshala.Models;

namespace Utitshala.Services
{
    public static class SequenceExtensions
    {
        #region Embedded Sequence Functions
        /// <summary>
        /// Handles text input optionsRegister by logging choices in the
        /// current optionsRegister list.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The list of optionsRegister and directions.</param>
        [SequenceCommand("opt")]
        public static void OptionTraversal(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("opt", arguments, 2);

            // Derive the argumentss
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);

            // Add optionsRegister to the optionsRegister list
            ChatEngine.optionsRegister.Add(new string[] { sequence.GetVariable("currentUserId").ToString(), arg1.ToString(), arg2.ToString(), sequence.CurrentLine.Value.Name });
        }

        /// <summary>
        /// Handles image messages.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The image URL, caption, and compression boolean.</param>
        [SequenceCommand("image")]
        public static void ImageMessageHandler(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("image", arguments, 3);

            // Derive the arguments
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);
            var arg3 = sequence.Resolve(arguments[2]);

            // Add the image URL to the send slot
            ChatEngine.messageClient.SendImageMessage(arg1.ToString(), arg2.ToString(), Convert.ToBoolean(arg3), sequence.GetVariable("currentChat"));
        }

        /// <summary>
        /// Handles presentation messages.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The image URLs and the compression boolean.</param>
        [SequenceCommand("presentation")]
        public static void PresentationMessageHandler(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("image", arguments, 11);

            // Derive the arguments
            List<string> argumentList = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                argumentList.Add(sequence.Resolve(arguments[i]).ToString());
            }
            var compress = sequence.Resolve(arguments[10]);

            // Add the album to the chat
            ChatEngine.messageClient.SendAlbumMessage(argumentList.ToArray(), Convert.ToBoolean(compress), sequence.GetVariable("currentChat"));
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
            ChatEngine.messageClient.SendStickerMessage(arg1.ToString(), sequence.GetVariable("currentChat"));
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
            ChatEngine.inputRegister.Add(new string[] { sequence.GetVariable("currentUserId").ToString(),
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
            switch (arg1.ToString())
            {
                // PERSONAL ---------------------------------------------------------
                case "viewprofile":
                    string profile = DatabaseController.GetStudentProfile(userId);
                    ChatEngine.messageClient.SendTextMessage(profile, sequence.GetVariable("currentChat"));
                    break;
                case "viewstudentrecord":
                    var record = DatabaseController.GetStudentRecord(userId);
                    // Create the message string
                    string messagerecord = "Lessons completed:\n\n";
                    // Write lessons:
                    if (record.Item1.Count() != 0)
                    {
                        foreach (var lesson in record.Item1)
                        {
                            messagerecord += lesson + "\n\n";
                        }
                    }
                    else
                    {
                        messagerecord += "No Lessons completed yet.\n\n";
                    }
                    messagerecord += "Assessments completed:\n\n";
                    // Write assessments
                    if (record.Item2.Count() != 0)
                    {
                        foreach (var assess in record.Item2)
                        {
                            messagerecord += assess + "\n\n";
                        }
                    }
                    else
                    {
                        messagerecord += "No Assessments completed yet.\n\n";
                    }
                    messagerecord += "0: Back";
                    ChatEngine.messageClient.SendTextMessage(messagerecord, sequence.GetVariable("currentChat"));
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
                    string toSendLessons = "Enter the number of the Lesson you want to start\n\n";
                    if (resultsLessons.Count() != 0)
                    {
                        foreach (var ent in resultsLessons)
                        {
                            toSendLessons += ent[0] + ": " + ent[1] + "\n";
                        }
                    }
                    else
                    {
                        toSendLessons += "No Lessons available";
                    }
                    toSendLessons += "\n0: Back\n\n";
                    ChatEngine.messageClient.SendTextMessage(toSendLessons, sequence.GetVariable("currentChat"));
                    break;
                case "getassessments":
                    List<string[]> resultsAssessments = DatabaseController.GetAssessments(userId);
                    // Construct a message and send
                    string toSendAssessments = "Enter the number of the Assessment you want to start\n\n";
                    if (resultsAssessments.Count() != 0)
                    {
                        foreach (var ent in resultsAssessments)
                        {
                            if (ent[2] != "")
                            {
                                toSendAssessments += ent[0] + ": " + ent[1] + ", Score: " + ent[2] + "%" + "\n";
                            }
                            else
                            {
                                toSendAssessments += ent[0] + ": " + ent[1] + ", not attempted" + "\n";
                            }
                        }
                    }
                    else
                    {
                        toSendAssessments += "No Assessments available";
                    }
                    toSendAssessments += "\n0: Back\n\n";
                    ChatEngine.messageClient.SendTextMessage(toSendAssessments, sequence.GetVariable("currentChat"));
                    break;
                case "getassignments":
                    List<string[]> resultsAssignments = DatabaseController.GetAssignments(userId);
                    // Construct a message and send
                    string toSendAssignments = "Enter the number of the Assignment you want to start\n\n";
                    if (resultsAssignments.Count != 0)
                    {
                        foreach (var ent in resultsAssignments)
                        {
                            toSendAssignments += ent[0] + ", " + ent[1] + ": " + ent[2] + "\n";
                        }
                    }
                    else
                    {
                        toSendAssignments += "No Assignments available";
                    }
                    toSendAssignments += "\n0: Back\n\n";
                    ChatEngine.messageClient.SendTextMessage(toSendAssignments, sequence.GetVariable("currentChat"));
                    break;
                case "closesession":
                    string[] userStateCloseSession = ChatEngine.userStateRegister.FirstOrDefault(c => c[0] == userId);
                    // Close the session
                    bool closeResult = DatabaseController.CloseSession(Convert.ToInt32(userStateCloseSession[4]), true);
                    if (closeResult)
                    {
                        // Remove the current user state and replace with default
                        ChatEngine.userStateRegister.Remove(userStateCloseSession);
                        ChatEngine.userStateRegister.Add(new string[] { userId, "registered" });
                        sequence.SetNextLine(arg3.ToString());
                    }
                    break;
                case "checkscore":
                    string[] userStateScore = ChatEngine.userStateRegister.FirstOrDefault(c => c[0] == userId);
                    // Get the user's score
                    int score = Convert.ToInt32(ChatEngine.userStateRegister.FirstOrDefault(c => c[0] == userId)[5]);
                    int sessionId = Convert.ToInt32(ChatEngine.userStateRegister.FirstOrDefault(c => c[0] == userId)[4]);
                    // Get the assessment's score
                    Tuple<bool, decimal> results = DatabaseController.CheckScore(score, sessionId, Convert.ToDecimal(arg2));
                    // Send the score to the user
                    string message = "Your score is: " + results.Item2.ToString() + "%";
                    ChatEngine.messageClient.SendTextMessage(message, sequence.GetVariable("currentChat"));
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

        /// <summary>
        /// Prepares the sequence to redirect after an upload, based on upload success or failure.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The name of the function to execute.</param>
        [SequenceCommand("upload")]
        public static void Upload(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("upload", arguments, 2);

            // Derive the arguments
            var arg1 = sequence.Resolve(arguments[0]);
            var arg2 = sequence.Resolve(arguments[1]);

            // Act
            try
            {
                // Add the arguments to the upload register element
                string[] uploadRegisterElement = ChatEngine.uploadRegister
                .FirstOrDefault(c => c[0] == sequence.GetVariable("currentUserId").ToString());
                string[] sa1 = uploadRegisterElement.Append(arg1.ToString()).ToArray();
                string[] sa2 = sa1.Append(arg2.ToString()).ToArray();
                // Replace
                ChatEngine.uploadRegister.Remove(uploadRegisterElement);
                ChatEngine.uploadRegister.Add(sa2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// Prepares the sequence to host a forum.
        /// </summary>
        /// <param name="sequence">The sequence this function is added to.</param>
        /// <param name="arguments">The name of the function to execute.</param>
        [SequenceCommand("forum")]
        public static void Forum(Sequence sequence, object[] arguments)
        {
            // Register the function
            ArgumentUtils.Count("upload", arguments, 2);

            // Derive the arguments
            string arg1 = sequence.Resolve(arguments[0]).ToString();
            string arg2 = sequence.Resolve(arguments[1]).ToString();

            // Attempt to convert the date into a DateTime
            DateTime endDate;
            try
            {
                endDate = DateTime.Parse(arg1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                // Otherwise, create a date in the far future
                endDate = new DateTime(2099, 12, 31);
            }

            // Set the user state register 
            ChatEngine.userStateRegister
                .FirstOrDefault(c => c[0] == sequence.GetVariable("currentUserId").ToString())
                [1] = "forum";

            // Get the userstate in memory
            string[] userState = ChatEngine.userStateRegister
                .FirstOrDefault(c => c[0] == sequence.GetVariable("currentUserId").ToString());
            string storageUrl = userState[3];
            // Send all the messages so far (if they exist)
            LearningDesign learningDesign = DatabaseController.GetLearningDesignByPath(storageUrl);
            // Send the notification
            ChatEngine.messageClient
                        .SendTextMessage("Add your comments by sending messages. Send 'exit' when you are done to continue with this lesson.", sequence.GetVariable("currentChat"));
            // Select the forum object and send its messages
            if (learningDesign.Forum != null)
            {
                // Get the messages and send them
                foreach (var message in learningDesign.Forum.ForumMessages.OrderBy(c => c.MessageDate))
                {
                    ChatEngine.messageClient
                        .SendTextMessage(message.GetMessage(), sequence.GetVariable("currentChat"));
                }
            }
            // Else, create a new forum within the learning design
            else
            {
                Forum newForum = new Forum()
                {
                    ForumMessages = new List<ForumMessage>()
                };
                // Set and save
                learningDesign.Forum = newForum;
                DatabaseController.UpdateLearningDesign(learningDesign);
                // Then send the opening message
                ChatEngine.messageClient
                        .SendTextMessage(arg2, sequence.GetVariable("currentChat"));
            }
        }
        #endregion
    }
}