using Microsoft.Owin;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Owin;
using Utitshala.Services;
using Utitshala.Controllers;
using Utitshala.Models;
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

            // Initiate the database
            DatabaseController._context = new ApplicationDbContext();

            // Initiate the state registers
            ChatEngine.downloadClient = new TelegramDownloader();
            ChatEngine.optionsRegister = new List<string[]>();
            ChatEngine.inputRegister = new List<string[]>();
            ChatEngine.userStateRegister = new List<string[]>();
            ChatEngine.uploadRegister = new List<string[]>();

            // Populate the state register with registered students
            // Might be prudent to create a maintenance routine that removes duplicate service ID entries here
            List<Student> students = DatabaseController.GetAllStudents();
            foreach(var student in students)
            {
                ChatEngine.userStateRegister.Add(new string[] { student.ServiceUserID, "registered" });
            }

            // Initiate the Telegram bot
            ChatEngine.messageClient = new TelegramMessageEngine(
                new TelegramBotClient(ConfigurationManager.AppSettings.Get("telegramKey")));

            // Mark sessions not completed as abandoned
            DatabaseController.UpdateAbandonedSessions();
        }
    }
}
