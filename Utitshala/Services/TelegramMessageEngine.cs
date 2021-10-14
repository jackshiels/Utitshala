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
                    // Create the unique path to be used
                    string path = AppDomain.CurrentDomain.BaseDirectory + @"DownloadedImages\" + e.Message.MessageId.ToString();
                    client.Headers.Add("User-Agent: Other");
                    client.DownloadFile(imageUrl, path);
                    // Open the file
                    FileStream image = System.IO.File.Open(path, FileMode.Open);
                    // Try change into an imagemagick object
                    try
                    {
                        var resizedImage = new MagickImage(image);
                        // Close the existing filestream
                        image.Close();
                        // Delete the stored image
                        System.IO.File.Delete(path);
                        // Resize to 50%
                        var resize = new MagickGeometry(new Percentage(50), new Percentage(50));
                        resizedImage.Resize(resize);
                        // Write to path with .jpg
                        resizedImage.Write(path + ".jpg");
                        // Compress, starting with opening the file again
                        image = System.IO.File.Open(path + ".jpg", FileMode.Open);
                        // Create a new compressor and act
                        new ImageOptimizer().Compress(image);
                        // Convert to an image type stream
                        var compressedImage = new MagickImage(image);
                        // Close the old stream and write the new, compressed image stream
                        image.Close();
                        compressedImage.Write(path + ".jpg");
                        // Distribute the image by opening and sending into bot
                        image = System.IO.File.Open(path + ".jpg", FileMode.Open);
                        botClient.SendPhotoAsync(e.Message.Chat, new InputOnlineFile(image, "newupload1.jpg"), caption);
                        // Sleep so that it doesn't crash by trying to close filestream while sending the image
                        Thread.Sleep(500);
                        // Finally close
                        image.Close();
                        // Delete the stored image to save disk space
                        System.IO.File.Delete(path + ".jpg");
                    }
                    catch (Exception ex)
                    {
                        // If crashed, send an error message
                        botClient.SendTextMessageAsync(e.Message.Chat, "Error in learning content. Please contact your teacher");
                        Console.WriteLine(ex.StackTrace);
                    }
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