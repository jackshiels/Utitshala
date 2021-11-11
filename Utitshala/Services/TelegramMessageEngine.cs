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
        /// <param name="caption">Determines if the image is compressed.</param>
        /// <param name="chat">The chat to send to.</param>
        public void SendImageMessage(string imageUrl, string caption, bool compress, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send via url if false, or compress if true
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
                    try
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
                        FileStream compressedImage = new ImageHandler().CompressImage(image, path);
                        botClient.SendPhotoAsync(e.Message.Chat, new InputOnlineFile(compressedImage, e.Message.Chat.Id.ToString() + ".jpg"), caption);
                        // Sleep so that it doesn't crash by trying to close filestream while sending the image
                        // MAY CAUSE PROBLEMS IN THE FUTURE...
                        Thread.Sleep(500);
                        // Finally close
                        compressedImage.Close();
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
        /// Sends an album of images via Telegram.
        /// </summary>
        /// <param name="imageUrls">The string array of image urls.</param>
        /// <param name="compress">Determines if the images are compressed.</param>
        /// <param name="chat">The chat to send to.</param>
        public void SendAlbumMessage(string[] imageUrls, bool compress, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Construct the IAlbumInputMedia
            List<IAlbumInputMedia> images = new List<IAlbumInputMedia>();
            // Choose to compress or not
            if (!compress)
            {
                // Create the images
                foreach (var image in imageUrls)
                {
                    if (image != "")
                    {
                        images.Add(new InputMediaPhoto(image));
                    }
                }
                // Send
                botClient.SendMediaGroupAsync(
                    inputMedia: images,
                    chatId: e.Message.Chat);
            }
            // Else compress them all
            else
            {
                // Create a list of files
                List<FileStream> compressedImages = new List<FileStream>();
                // Get the image from url
                using (var client = new WebClient())
                {
                    foreach (var imageUrl in imageUrls.Where(c => c != ""))
                    {
                        try
                        {
                            // Create directory if it doesn't exist
                            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"DownloadedImages\");
                            // Create the unique path to be used
                            string path = AppDomain.CurrentDomain.BaseDirectory + @"DownloadedImages\" + e.Message.Chat.Id.ToString() + "_" + new Random().Next(0, 9999999).ToString();
                            client.Headers.Add("User-Agent: Other");
                            client.DownloadFile(imageUrl, path);
                            // Open the file
                            FileStream image = System.IO.File.Open(path, FileMode.Open);
                            // Try change into an imagemagick object
                            FileStream compressedImage = new ImageHandler().CompressImage(image, path);
                            compressedImages.Add(compressedImage);
                        }
                        catch (Exception ex)
                        {
                            // If crashed, send an error message
                            botClient.SendTextMessageAsync(e.Message.Chat, "Error in learning content. Please contact your teacher");
                            Console.WriteLine(ex.StackTrace);
                        }
                    }
                }
                // Send the full album
                foreach (var image in compressedImages)
                {
                    images.Add(new InputMediaPhoto(new InputMedia(image, image.Name.Split('\\').Last())));
                }
                // Send
                try
                {
                    botClient.SendMediaGroupAsync(
                    inputMedia: images,
                    chatId: e.Message.Chat);
                }
                catch (Exception ex)
                {
                    // If crashed, send an error message
                    botClient.SendTextMessageAsync(e.Message.Chat, "Error in learning content. Please contact your teacher");
                    Console.WriteLine(ex.StackTrace);
                }
                // Close filstreams and delete
                foreach (var image in compressedImages)
                {
                    // Finally close
                    image.Close();
                    // Delete the stored image to save disk space
                    System.IO.File.Delete(image.Name);
                }
            }
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