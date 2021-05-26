using Microsoft.Owin;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Owin;
using Utitshala.Services;
using System.Configuration;
using System.Collections.Generic;

[assembly: OwinStartupAttribute(typeof(Utitshala.Startup))]
namespace Utitshala
{
    /// <summary>
    /// Handles ASP MVC startup and Telegram I/O
    /// </summary>
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Configure the app
            ConfigureAuth(app);

            // Create and start the Telegram bot client
            ChatEngine.messageClient = new TelegramMessageEngine(
                new TelegramBotClient(ConfigurationManager.AppSettings.Get("telegramKey")));
            ChatEngine.options = new List<string[]>();
        }
    }
}
