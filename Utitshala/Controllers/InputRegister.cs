using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Controllers
{
    /// <summary>
    /// A controller that handles unique, syntactical use cases for functionality,
    /// to be read from a dialogue file.
    /// </summary>
    public static class InputRegister
    {
        #region Student Services
        /// <summary>
        /// Registers a student, based on a chat/service ID.
        /// </summary>
        /// <param name="name">The student's name.</param>
        /// <param name="serviceId">The service ID used to identify the student.</param>
        public static void RegisterStudent(string name, string serviceId)
        {
            // Something
        }

        /// <summary>
        /// Selects a language for the student to learn in.
        /// </summary>
        /// <param name="name">The name of the language.</param>
        /// <param name="serviceId">The service ID used to identify the student.</param>
        public static void ChooseLanguage(string name, string serviceId)
        {
            // Something
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serviceId">The service ID used to identify the student.</param>
        public static void ViewProfile(string name, string serviceId)
        {
            // Something
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serviceId">The service ID used to identify the student.</param>
        public static void ViewRecord(string name, string serviceId)
        {
            // Something
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="serviceId">The service ID used to identify the student.</param>
        public static void EditProfile(string name, string serviceId)
        {
            // Something
        }
        #endregion

        #region Lesson System
        // To do
        #endregion
    }
}