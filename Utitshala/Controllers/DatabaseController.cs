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
    }
}