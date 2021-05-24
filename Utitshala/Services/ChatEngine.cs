using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public static IMessageClient messageClient;
        private static List<string[]> options;
        private static string currentLine;
        private static string[] imageUrl;

        /// <summary>
        /// Receives dialogue and initiates responses.
        /// </summary>
        /// <param name="sender">The object for this event.</param>
        /// <param name="e">The Telegram message arguments.</param>
        public static void Bot_OnMessage(object sender, MessageEventArgs e)
        {
            // Initialise the options List<string[]>
            if (options == null)
            {
                options = new List<string[]>();
            }

            // Initialise Spin
            var sequence = new Sequence(new DictionaryBackend(), new FileDocumentLoader());
            sequence.RegisterStandardLibrary();

            // Get the path of the introductory dialogue document and load
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Dialogues\DialogueTest.spd";
            sequence.LoadAndStartDocument(path);
            sequence.AddCommand("opt", ChatEngine.OptionTraversal);
            sequence.AddCommand("image", ChatEngine.ImageMessageHandler);
            sequence.AddCommand("sticker", ChatEngine.StickerMessageHandler);

            // Set variables
            sequence.SetVariable("currentStickerUrl", "");

            // Check for an option choice before execution
            if (options.Count() != 0)
            {
                foreach (var opt in options)
                {
                    if (e.Message.Text == opt[0])
                    {
                        // Set the next line and break
                        sequence.SetNextLine(opt[1]);
                        options = null;
                        break;
                    }
                }
            }

            // Send the opening message
            while (sequence.StartNextLine().HasValue)
            {
                currentLine = sequence.ExecuteCurrentLine().BuildString();
                if (imageUrl != null)
                {
                    messageClient.SendImageMessage(imageUrl[0], imageUrl[1], e);
                    imageUrl = null;
                }
                else if (sequence.GetVariable("currentStickerUrl").ToString() != "")
                {
                    messageClient.SendStickerMessage(sequence.GetVariable("currentStickerUrl").ToString(), e);
                    sequence.SetVariable("currentStickerUrl", "");
                }
                else if (e.Message.Text != null)
                {
                    messageClient.SendTextMessage(currentLine, e);
                }
            }
        }

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
            options.Add(new string[] { arg1.ToString(), arg2.ToString() });
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
            imageUrl = new string[] { arg1.ToString(), arg2.ToString() };
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
    }
}