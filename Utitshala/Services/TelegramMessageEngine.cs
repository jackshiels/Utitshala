using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using static Utitshala.Services.Interfaces;
using ImageMagick;

namespace Utitshala.Services
{
    /// <summary>
    /// Implements the MessageClient interface for Telegram.
    /// </summary>
    public class TelegramMessageEngine : IMessageClient
    {
        // Variables
        public ITelegramBotClient botClient;

        // Constructors
        public TelegramMessageEngine() { }
        public TelegramMessageEngine(ITelegramBotClient botClientIn)
        {
            botClient = botClientIn;
            botClient.OnMessage += ChatEngine.Bot_OnMessage;
            botClient.StartReceiving();
        }

        /// <summary>
        /// Sends an audio message via Telegram.
        /// </summary>
        /// <param name="audioUrl">The URL of the audio file.</param>
        /// <param name="chat">The chat to send to.</param>
        public void SendAudioMessage(string audioUrl, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            botClient.SendAudioAsync(
                chatId: e.Message.Chat,
                audioUrl);
            // Sleep so messages are staggered
            Thread.Sleep(500);
        }

        /// <summary>
        /// Sends an image message via Telegram.
        /// </summary>
        /// <param name="imageUrl">The URL of the image file.</param>
        /// <param name="caption">The caption to include with the image.</param>
        /// <param name="chat">The chat to send to.</param>
        public void SendImageMessage(string imageUrl, string caption, bool compress, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            if (!compress)
            {
                botClient.SendPhotoAsync(
                  chatId: e.Message.Chat,
                  photo: imageUrl,
                  caption: caption,
                  parseMode: ParseMode.Html);
            }
            else
            {
                // Get the image from url
                using (var client = new WebClient())
                {
                    // Create directory if it doesn't exist
                    Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"DownloadedImages\");
                    string path = AppDomain.CurrentDomain.BaseDirectory + @"DownloadedImages\" + e.Message.MessageId.ToString() + ".jpg";
                    client.Headers.Add("User-Agent: Other");
                    client.DownloadFile(imageUrl, path);
                    // Open the file
                    FileStream image = System.IO.File.Open(path, FileMode.Open);
                    // Compress the file
                    // Change into an imagemagick object
                    var compressedImage = new MagickImage(image);
                    image.Close();
                    // Resize to 50%
                    var resize = new MagickGeometry(new Percentage(50), new Percentage(50));
                    compressedImage.Resize(resize);
                    // Write
                    compressedImage.Write(path);
                    // Compress
                    image = System.IO.File.Open(path, FileMode.Open);
                    var optimiser = new ImageOptimizer().Compress(image);
                    // Send the image
                    botClient.SendPhotoAsync(e.Message.Chat, new InputOnlineFile(image, "newupload1.jpg"), caption);
                    Thread.Sleep(500);
                    image.Close();
                    // Delete the stored image
                    System.IO.File.Delete(path);
                }
            }
            // Sleep so messages are staggered
            Thread.Sleep(500);
        }

        /// <summary>
        /// Sends a sticker message via Telegram.
        /// </summary>
        /// <param name="stickerUrl">The URL/ File ID of the sticker.</param>
        /// <param name="chat">The chat to send to.</param>
        public void SendStickerMessage(string stickerUrl, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            botClient.SendStickerAsync(
                  chatId: e.Message.Chat,
                  sticker: stickerUrl);
            // Sleep so messages are staggered
            Thread.Sleep(500);
        }

        /// <summary>
        /// Sends a text message via Telegram.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <param name="chat">The chat to send to.</param>
        public void SendTextMessage(string message, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send if not null/ empty
            if (message != null 
                && message != ""
                && message != "\n")
            {
                botClient.SendTextMessageAsync(
                  chatId: e.Message.Chat,
                  text: message);
            }
            // Sleep so messages are staggered
            Thread.Sleep(500);
        }

        /// <summary>
        /// Sends a video message via Telegram.
        /// </summary>
        /// <param name="videoUrl">The URL of the video file.</param>
        /// <param name="thumbnailUrl">The URL of the video thumbnail.</param>
        /// <param name="chat">The chat to send to.</param>
        public void SendVideoMessage(string videoUrl, string thumbnailUrl, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            botClient.SendVideoAsync(
                   chatId: e.Message.Chat,
                   video: videoUrl,
                   thumb: thumbnailUrl,
                   supportsStreaming: true);
            // Sleep so messages are staggered
            Thread.Sleep(500);
        }
    }
}