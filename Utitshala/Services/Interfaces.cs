﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Utitshala.Services
{
    public class Interfaces
    {
        /// <summary>
        /// Interface for sending messages to a chat client.
        /// </summary>
        public interface IMessageClient
        {
            void SendTextMessage(string message, object chat);
            void SendImageMessage(string imageUrl, string caption, object chat);
            void SendStickerMessage(string stickerUrl, object chat);
            void SendAudioMessage(string audioUrl, object chat);
            void SendVideoMessage(string videoUrl, string thumbnailUrl, object chat);
        }

        /// <summary>
        /// An interface for individual message client download operations.
        /// </summary>
        public interface IDownloader
        {
            Image DownloadImage(object e);
        }
    }
}