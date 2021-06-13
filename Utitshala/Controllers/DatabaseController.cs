﻿using System;
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
        /// <returns>A boolean representing success or failure in registering.</returns>
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
                        Language = Language.English,
                        DateJoined = DateTime.Now
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

        /// <summary>
        /// Removes a student from their classroom.
        /// </summary>
        /// <param name="userId">The unique identifier from their chat.</param>
        /// <returns>A boolean representing success in removing.</returns>
        public static bool LeaveClassroom(string userId)
        {
            bool result = false;

            // Attempt to remove the student
            try
            {
                Student student = _context.Students
                    .FirstOrDefault(c => c.ServiceUserID == userId);
                student.ClassroomID = null;
                _context.Entry(student).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();

                // Mark true
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return result;
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

        /// <summary>
        /// Constructs and returns a student profile in string form.
        /// </summary>
        /// <param name="userId">The ID of the student to base the profile on.</param>
        /// <returns>A string containing profile details.</returns>
        public static string GetStudentProfile(string userId)
        {
            string profile = "";

            try
            {
                // Get the student
                Student student = _context.Students
                    .FirstOrDefault(c => c.ServiceUserID == userId);
                Classroom classroom = _context.Classrooms
                    .FirstOrDefault(c => c.ID == student.ClassroomID);
                // Construct the string, based on classroom presence
                if (classroom != null)
                {
                    profile += "Your Profile:\n" + "Name: " + student.Name + "\n"
                    + "Date Joined: " + student.DateJoined.ToShortDateString() + "\n"
                    + "Classroom: " + classroom.Name + "\n"
                    + "Chat ID: " + student.ServiceUserID;
                }
                else
                {
                    profile += "Your Profile:\n" + "Name: " + student.Name + "\n"
                    + "Date Joined: " + student.DateJoined.ToShortDateString() + "\n"
                    + "Classroom: None\n"
                    + "Chat ID: " + student.ServiceUserID;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return profile;
        }

        /// <summary>
        /// Gets the SPD file location for a learning design.
        /// </summary>
        /// <param name="ldId">The learning design ID.</param>
        /// <returns>A string of the file name.</returns>
        public static string GetLessonUrl(int ldId)
        {
            string url = "";
            // Attempt to find the learning design URL
            try
            {
                url = _context.LearningDesigns
                    .FirstOrDefault(c => c.ID == ldId).StorageURL;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return url;
        }
        #endregion
    }
}