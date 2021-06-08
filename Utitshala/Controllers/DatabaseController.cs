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

        #region Utility Methods
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
        /// Returns a boolean indicating if a student is part of a class.
        /// </summary>
        /// <param name="userId">The user ID to check.</param>
        /// <returns>A boolean indicating presence.</returns>
        public static bool CheckClassPresence(string userId)
        {
            bool exists = false;

            // Check the database for the existence of a student with this ID
            if (_context.Students.FirstOrDefault(c => c.ServiceUserID == userId).ClassroomID != null)
            {
                exists = true;
            }

            return exists;
        }

        /// <summary>
        /// Attempts to add a student to a classroom.
        /// </summary>
        /// <param name="userId">The student to add to a classroom.</param>
        /// <param name="classId">The database ID of the classroom to add the student to.</param>
        /// <returns>A bool indicating success (true) or failure (false).</returns>
        public static bool RegisterWithClass(string userId, string classId)
        {
            bool success = false;
            try
            {
                // Convert the ID into an int
                int classroomId = Convert.ToInt32(classId);
                // Get the student
                Student student = _context.Students
                    .FirstOrDefault(c => c.ServiceUserID == userId);
                // Modify the student record and save into the DB
                student.ClassroomID = classroomId;
                _context.Entry(student).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                // If made it this far, success
                success = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return success;
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
        #endregion

        #region GET Methods
        /// <summary>
        /// Returns a list of all students on the database.
        /// </summary>
        /// <returns>The list of students.</returns>
        public static List<Student> GetAllStudents()
        {
            return _context.Students.ToList();
        }


        /// <summary>
        /// Returns a string array containing Learning Design IDs and names.
        /// </summary>
        /// <param name="userId">The user to base this GET on.</param>
        /// <returns>The string[] containing Learning Design details.</returns>
        public static List<string[]> GetLessons(string userId)
        {
            // Get the classroom ID
            int classroomId = (int)_context.Students
                .FirstOrDefault(c => c.ServiceUserID == userId).ClassroomID;
            // Get the available learning designs, based on the classroom ID
            List<LearningDesign> lessons = _context.LearningDesigns
                .Where(c => c.ClassroomID == classroomId).ToList();
            // Convert the lessons into string[] { ID, Name } and return
            List<string[]> lessonsStrings = new List<string[]>();
            foreach (var lesson in lessons)
            {
                lessonsStrings.Add(new string[] { lesson.ID.ToString(), lesson.Name });
            }
            return lessonsStrings;
        }
        #endregion
    }
}