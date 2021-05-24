using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static Utitshala.Services.Interfaces;

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
        public async void SendAudioMessage(string audioUrl, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            await botClient.SendAudioAsync(
                chatId: e.Message.Chat,
                audioUrl);
        }

        /// <summary>
        /// Sends an image message via Telegram.
        /// </summary>
        /// <param name="imageUrl">The URL of the image file.</param>
        /// <param name="caption">The caption to include with the image.</param>
        /// <param name="chat">The chat to send to.</param>
        public async void SendImageMessage(string imageUrl, string caption, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            await botClient.SendPhotoAsync(
                  chatId: e.Message.Chat,
                  photo: imageUrl,
                  caption: caption,
                  parseMode: ParseMode.Html);
        }

        /// <summary>
        /// Sends a sticker message via Telegram.
        /// </summary>
        /// <param name="stickerUrl">The URL of the sticker.</param>
        /// <param name="chat">The chat to send to.</param>
        public async void SendStickerMessage(string stickerUrl, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            await botClient.SendStickerAsync(
                  chatId: e.Message.Chat,
                  sticker: stickerUrl);
        }

        /// <summary>
        /// Sends a text message via Telegram.
        /// </summary>
        /// <param name="message">The text message to send.</param>
        /// <param name="chat">The chat to send to.</param>
        public async void SendTextMessage(string message, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            await botClient.SendTextMessageAsync(
                  chatId: e.Message.Chat,
                  text: message);
        }

        /// <summary>
        /// Sends a video message via Telegram.
        /// </summary>
        /// <param name="videoUrl">The URL of the video file.</param>
        /// <param name="thumbnailUrl">The URL of the video thumbnail.</param>
        /// <param name="chat">The chat to send to.</param>
        public async void SendVideoMessage(string videoUrl, string thumbnailUrl, object chat)
        {
            // Convert the chat to MessageEventArgs
            MessageEventArgs e = (MessageEventArgs)chat;
            // Send
            await botClient.SendVideoAsync(
                   chatId: e.Message.Chat,
                   video: videoUrl,
                   thumb: thumbnailUrl,
                   supportsStreaming: true);
        }
    }
}