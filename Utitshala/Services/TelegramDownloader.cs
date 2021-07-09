using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using static Utitshala.Services.Interfaces;
using System.Net;
using System.IO;
using Utitshala.Models.JSONModels;
using Newtonsoft.Json;
using System.Configuration;
using Telegram.Bot.Args;

namespace Utitshala.Services
{
    /// <summary>
    /// The Telegram implementation of the downloader class.
    /// </summary>
    public class TelegramDownloader : IDownloader
    {
        /// <summary>
        /// Downloads an image, based on the properties of a message sent containing such
        /// an image.
        /// </summary>
        /// <param name="e">The object containing the user's image message.</param>
        /// <returns>The image file, downloaded from Telegram's servers.</returns>
        public Image DownloadImage(object e)
        {
            try
            {
                // Convert the chat to MessageEventArgs
                MessageEventArgs chat = (MessageEventArgs)e;
                // Check to see if this is a photo upload
                if (chat.Message.Photo != null)
                {
                    // Get the Telegram photo URL
                    string url = String.Format(@"https://api.telegram.org/bot{0}/getFile?file_id={1}",
                                    ConfigurationManager.AppSettings.Get("telegramKey"), chat.Message.Photo[3].FileId);
                    // Get the request for this as json
                    string json = "";
                    // Execute the request
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                    // Deserialise 
                    ImageResult fileSpecs = JsonConvert.DeserializeObject<ImageRequestJson>(json).result;
                    // Download the image via the url
                    string imageUrl = String.Format(@"https://api.telegram.org/file/bot{0}/{1}",
                        ConfigurationManager.AppSettings.Get("telegramKey"), fileSpecs.file_path);
                    WebClient client = new WebClient();
                    Stream imageStream = client.OpenRead(imageUrl);
                    Bitmap bitmap = new Bitmap(imageStream);
                    // Return the image
                    return bitmap;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Downloads a voice note, based on the properties of a message sent containing such
        /// a voice note.
        /// </summary>
        /// <param name="e">The object containing the user's image message.</param>
        /// <returns>The voice note file, downloaded from Telegram's servers.</returns>
        public Stream DownloadVoiceNote(object e)
        {
            try
            {
                // Convert the chat to MessageEventArgs
                MessageEventArgs chat = (MessageEventArgs)e;
                // Check to see if this is a voice note
                if (chat.Message.Voice != null)
                {
                    // Get the Telegram photo URL
                    string url = String.Format(@"https://api.telegram.org/bot{0}/getFile?file_id={1}",
                                    ConfigurationManager.AppSettings.Get("telegramKey"), chat.Message.Voice.FileId);
                    // Get the request for this as json
                    string json = "";
                    // Execute the request
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        json = reader.ReadToEnd();
                    }
                    // Deserialise 
                    ImageResult fileSpecs = JsonConvert.DeserializeObject<ImageRequestJson>(json).result;
                    // Download the image via the url
                    string audioUrl = String.Format(@"https://api.telegram.org/file/bot{0}/{1}",
                        ConfigurationManager.AppSettings.Get("telegramKey"), fileSpecs.file_path);
                    WebClient client = new WebClient();
                    Stream audioStream = client.OpenRead(audioUrl);
                    // Return the image
                    return audioStream;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return null;
            }
        }
    }
}