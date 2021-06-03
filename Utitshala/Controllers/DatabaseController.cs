using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utitshala.Models;

namespace Utitshala.Controllers
{
    /// <summary>
    /// Handles I/O operations to the database.
    /// </summary>
    public static class DatabaseController
    {
        #region Variables
        public static ApplicationDbContext _context;
        #endregion

        /// <summary>
        /// Returns a boolean indicating the presence of a specific user.
        /// </summary>
        /// <param name="userId">The user ID to check.</param>
        /// <returns>A boolean indicating presence.</returns>
        public static bool CheckRegistration(string userId)
        {
            bool exists = false;

            // Check the database for the existence of a student with this ID
            if (_context.Students.FirstOrDefault(c => c.ServiceUserID == userId) != null)
            {
                exists = true;
            }

            return exists;
        }

        /// <summary>
        /// Registers a new student on the database.
        /// </summary>
        /// <param name="userId">The unique identifier from their chat.</param>
        /// <param name="name">The name of the student.</param>
        /// <returns></returns>
        public static bool RegisterStudent(string userId, string name)
        {
            bool success = false;

            // Create the student
            try
            {
                if (userId != "" && name != "")
                {
                    Student studentToAdd = new Student()
                    {
                        ServiceUserID = userId,
                        Name = name,
                        Language = Language.English
                    };
                    _context.Students.Add(studentToAdd);
                    _context.SaveChanges();

                    // Mark successful
                    success = true;
                }
            }
            // Write to console on failure
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return success;
        }
    }
}