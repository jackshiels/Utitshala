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
using Telegram.Bot.Types.Enums;
using static Utitshala.Services.Interfaces;

namespace Utitshala.Services
{
    /// <summary>
    /// A class responsible for handling chat activities on the Telegram bot.
    /// </summary>
    public static class ChatEngine
    {
        #region Variables
        public static IMessageClient messageClient;
        // Holds in memory chat sequences
        public static List<string[]> options;
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

            // Get the path of the introductory dialogue document and load
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Dialogues\DialogueTest.spd";
            sequence.LoadAndStartDocument(path);
            sequence.AddCommand("opt", ChatEngine.OptionTraversal);
            sequence.AddCommand("image", ChatEngine.ImageMessageHandler);
            sequence.AddCommand("sticker", ChatEngine.StickerMessageHandler);
            sequence.AddCommand("wait", ChatEngine.WaitTimer);

            // Set variables
            sequence.SetVariable("currentStickerUrl", "");
            sequence.SetVariable("currentImageUrl", "");
            sequence.SetVariable("currentImageCaption", "");

            // The current ID of this chat, tracked for functions
            sequence.SetVariable("currentChatId", e.Message.Chat.Id.ToString());

            // Set options, if applicable
            OptionInitiator(sequence, e.Message.Chat.Id.ToString(), e.Message.Text);

            // Set the line to send
            string currentLine;

            // Send the opening message
            while (sequence.StartNextLine().HasValue)
            {
                // Get the current line
                currentLine = sequence.ExecuteCurrentLine().BuildString();
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

        /// <summary>
        /// Sets the current option line for a specific chat ID.
        /// </summary>
        /// <param name="sequence">The sequence to act on.</param>
        /// <param name="chatId">The ID of the chat to check options for.</param>
        public static void OptionInitiator(Sequence sequence, string chatId, string messageText)
        {
            // Check for an option choice before execution
            if (options.Where(c => c[0] == chatId).Count() != 0)
            {
                foreach (var opt in options.Where(c => c[0] == chatId))
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
                foreach (var opt in options.Where(c => c[0] == chatId).ToList())
                {
                    options.Remove(opt);
                }
            }
        }

        #region Spin Functions
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
            options.Add(new string[] { sequence.GetVariable("currentChatId").ToString(), arg1.ToString(), arg2.ToString() });
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
        #endregion
    }
}