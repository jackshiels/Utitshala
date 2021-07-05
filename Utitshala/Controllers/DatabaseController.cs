using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utitshala.Models;

namespace Utitshala.Controllers
{
    /// <summary>
    /// Handles I/O operations into/ out of the database.
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
                        DateJoined = DateTime.Now,
                        StudentRecord = new StudentRecord()
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
        /// Marks sessions that have been abandoned as abandoned.
        /// </summary>
        public static void UpdateAbandonedSessions()
        {
            // Get the list of sessions
            List<Session> sessions = _context.Sessions.ToList();
            foreach (var session in sessions)
            {
                // If not abandoned or completed, mark as abandoned
                if (session.Abandoned == false
                    && session.DateTimeEnded == null)
                {
                    session.Abandoned = true;
                    _context.Entry(session).State = System.Data.Entity.EntityState.Modified;
                }
            }
            // Save changes
            _context.SaveChanges();
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
        /// Returns a string array containing assessment IDs and names.
        /// </summary>
        /// <param name="userId">The user to base this GET on.</param>
        /// <returns>The string[] containing assessment details.</returns>
        public static List<string[]> GetAssessments(string userId)
        {
            // Get the user's sessions
            List<Session> sessions = _context.Students
                .Include("StudentRecord")
                .FirstOrDefault(c => c.ServiceUserID == userId)
                .StudentRecord
                .Sessions;
            // Get the available assessments, based on Learning Designs available and completed.
            List<Assessment> assessments = new List<Assessment>();
            foreach (var sesh in sessions)
            {
                if (sesh.Completed == true)
                {
                    LearningDesign lesson = _context
                        .LearningDesigns
                        .Include("Assessment")
                        .FirstOrDefault(c => c.ID == sesh.LearningDesignID);
                    if (lesson != null && lesson.Assessment != null)
                    {
                        assessments.Add(lesson.Assessment);
                    }
                }
            }
            // Get the available assessments, based on a direct link to the student's classroom
            int classroomId = (int)_context.Students
                .FirstOrDefault(c => c.ServiceUserID == userId)
                .ClassroomID;
            Classroom classroom = _context.Classrooms
                .Include("Assessments")
                .FirstOrDefault(c => c.ID == classroomId);
            List<Assessment> notWithinList = classroom.Assessments
                .Where(c => !assessments.Any(y => y == c)).ToList();
            assessments.AddRange(notWithinList);
            // Convert the lessons into string[] { ID, Name, Score (if exists) } and return
            List<string[]> assessmentStrings = new List<string[]>();
            foreach (var assessment in assessments)
            {
                assessmentStrings.Add(new string[] { assessment.ID.ToString(), assessment.Name, 
                    sessions.Where(c => c.AssessmentID == (int)assessment.ID).Max(c => c.Score).ToString() });
            }
            return assessmentStrings;
        }

        /// <summary>
        /// Gets a list of assignments available to a student.
        /// </summary>
        /// <param name="userId">The user to base this GET on.</param>
        /// <returns>A list<string[]> of assignment values in { ID, Name } form.</returns>
        public static List<string[]> GetAssignments(string userId)
        {
            List<string[]> results = new List<string[]>();
            // Ge the student to find the classroom ID
            Student student = _context.Students
                .FirstOrDefault(c => c.ServiceUserID == userId);
            // Get all assignments not closed
            List<Assignment> assignments = _context.Assignments
                .Where(c => c.ClassroomID == student.ClassroomID
                && c.DateDue != null
                && DateTime.Compare(c.DateDue.Value, DateTime.Now) > 0).ToList();
            assignments.AddRange(_context.Assignments
                .Where(c => c.ClassroomID == student.ClassroomID
                && c.DateDue == null).ToList());
            // Format the string list
            foreach (var assign in assignments)
            {
                results.Add(new string[] { assign.ID.ToString(), assign.Name });
            }
            // Return the list
            return results;
        }

        /// <summary>
        /// Returns a specific assignment, if that user has access.
        /// </summary>
        /// <param name="userId">The user to base this GET on.</param>
        /// <param name="assignmentId">The ID of the assignment.</param>
        /// <returns>The assignment if allowed, null if not.</returns>
        public static Assignment GetAssignment(string userId, int assignmentId)
        {
            // Get the assignment and the student
            Assignment assignment = _context.Assignments
                .FirstOrDefault(c => c.ID == assignmentId);
            Student student = _context.Students
                .FirstOrDefault(c => c.ServiceUserID == userId);
            // Authenticate their access, return null if disallowed.
            if (assignment.Public || 
                student.ClassroomID == assignment.ClassroomID)
            {
                // Check if there is a due date, and if it has passed
                if (assignment.DateDue != null)
                {
                    if (DateTime.Compare((DateTime)assignment.DateDue, DateTime.Now) > 0)
                    {
                        return assignment;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return assignment;
                }
            }
            else
            {
                return null;
            }
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
                    profile += "Your Profile:\n\n" + "Name: " + student.Name + "\n"
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
        /// Returns the student's record of lessons and assessments in a string list tuple.
        /// </summary>
        /// <param name="userId">The ID of the student to base the record on.</param>
        /// <returns>A tuple of <List<string>,List<string>> containing lessons and assessments, respectively.</returns>
        public static Tuple<List<string>, List<string>> GetStudentRecord(string userId)
        {
            List<string> lessons = new List<string>();
            // Get the lessons completed
            List<Session> sessions = _context.Students
                .Include("StudentRecord")
                .FirstOrDefault(c => c.ServiceUserID == userId)
                .StudentRecord
                .Sessions.ToList();
            foreach (var ses in sessions)
            {
                if (ses.Completed && ses.AssessmentID == null)
                {
                    // Handle nulls
                    lessons.Add(ses.LearningDesign.Name
                        + "\nCompleted: " + ses.DateTimeEnded.Value.ToShortDateString() 
                        + ", " + ses.DateTimeEnded.Value.ToShortTimeString());
                }
            }
            List<string> assessments = new List<string>();
            foreach (var ses in sessions)
            {
                if (ses.Completed && ses.AssessmentID != null)
                {
                    assessments.Add(_context.Assessments.FirstOrDefault(c => c.ID == ses.AssessmentID).Name
                        + "\nCompleted: " + ses.DateTimeEnded.Value.ToShortDateString()
                        + ", " + ses.DateTimeEnded.Value.ToShortTimeString());
                }
            }
            return new Tuple<List<string>, List<string>>(lessons, assessments);
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

        /// <summary>
        /// Gets the SPD file location for an assessment.
        /// </summary>
        /// <param name="ldId">The assessment ID.</param>
        /// <returns>A string of the file name.</returns>
        public static string GetAssessmentUrl(int assessId)
        {
            string url = "";
            // Attempt to find the learning design URL
            try
            {
                url = _context.Assessments
                    .FirstOrDefault(c => c.ID == assessId).StorageURL;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return url;
        }

        /// <summary>
        /// Calculates if a quiz has been passed.
        /// </summary>
        /// <param name="userScore">The user's correct answer count.</param>
        /// <param name="sessionId">The ID of the current user session.</param>
        /// <param name="passMark">The percentage grade to pass.</param>
        /// <returns>A tuple of the pass mark, and a decimal score.</returns>
        public static Tuple<bool, decimal> CheckScore(decimal userScore, decimal sessionId, decimal passMark)
        {
            bool result = false;
            decimal score = 0;
            try
            {
                // Get the session
                Session session = _context.Sessions
                    .Include("Assessment")
                    .FirstOrDefault(c => c.ID == sessionId);
                // If assessment not null, calculate the score
                if (session.Assessment != null)
                {
                    // Calculate the score if userScore != 0
                    if (userScore != 0)
                    {
                        score = (userScore
                        / session.Assessment.QuestionsCount) * 100;
                        if (score >= passMark) // If passed
                        {
                            result = true;
                        }
                    }
                    // Save the score into the session
                    session.Score = Math.Round(score, 2);
                    _context.Entry(session).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            // Return values as a <bool, decimal> tuple
            return new Tuple<bool, decimal>(result, Math.Round(score, 2));
        }
        #endregion

        #region POST Methods
        /// <summary>
        /// Creates a session object in a student's record, based on a learning design.
        /// </summary>
        /// <param name="userId">The service ID of the student to create a session for.</param>
        /// <param name="learningDesignId">The ID of the learning design being learned.</param>
        /// <returns>The ID of the session as an int.</returns>
        public static int StartSessionLearningDesign(string userId, int learningDesignId)
        {
            int result = -1;
            // Create a session object and save
            try
            {
                // First, close all other sessions
                Student student = _context.Students
                    .Include("StudentRecord")
                    .FirstOrDefault(c => c.ServiceUserID == userId);
                // Iterate over the non-abandoned sessions and mark abandoned
                foreach (var ses in student.StudentRecord.Sessions
                    .Where(c => c.Abandoned == false
                    && c.DateTimeEnded == null).ToList())
                {
                    ses.Abandoned = true;
                    ses.DateTimeEnded = DateTime.Now;
                    _context.Entry(ses).State = System.Data.Entity.EntityState.Modified;
                }
                // Save
                _context.SaveChanges();
                _context = new ApplicationDbContext();
                // Create the new session
                Session session = new Session()
                {
                    DateTimeStarted = DateTime.Now,
                    LearningDesignID = learningDesignId,
                    Abandoned = false,
                };
                // Get the student record by user service ID
                StudentRecord record = _context.Students
                    .FirstOrDefault(c => c.ServiceUserID == userId).StudentRecord;
                record.Sessions.Add(session);
                _context.SaveChanges();
                // Return the ID of the session
                result = session.ID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Creates a session object in a student's record, based on an assessment.
        /// </summary>
        /// <param name="userId">The service ID of the student to create a session for.</param>
        /// <param name="learningDesignId">The ID of the assessment being conducted.</param>
        /// <returns>The ID of the session as an int.</returns>
        public static int StartSessionAssessment(string userId, int assessmentId)
        {
            int result = -1;
            // Create a session object and save
            try
            {
                // First, close all other sessions
                Student student = _context.Students
                    .Include("StudentRecord")
                    .FirstOrDefault(c => c.ServiceUserID == userId);
                // Iterate over the non-abandoned sessions and mark abandoned
                foreach (var ses in student.StudentRecord.Sessions
                    .Where(c => c.Abandoned == false
                    && c.DateTimeEnded == null).ToList())
                {
                    ses.Abandoned = true;
                    ses.DateTimeEnded = DateTime.Now;
                    _context.Entry(ses).State = System.Data.Entity.EntityState.Modified;
                }
                // Save
                _context.SaveChanges();
                _context = new ApplicationDbContext();
                // Create the new session
                Session session = new Session()
                {
                    DateTimeStarted = DateTime.Now,
                    AssessmentID = assessmentId,
                    Abandoned = false,
                    Score = 0
                };
                // Get the student record by user service ID
                StudentRecord record = _context.Students
                    .FirstOrDefault(c => c.ServiceUserID == userId).StudentRecord;
                record.Sessions.Add(session);
                _context.SaveChanges();
                // Return the ID of the session
                result = session.ID;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return result;
        }

        /// <summary>
        /// Closes an open session.
        /// </summary>
        /// <param name="sessionId">The ID of the open session to close.</param>
        /// <param name="completed">A boolean representing a fair completion.</param>
        /// <returns>A boolean representing success or failure in closing.</returns>
        public static bool CloseSession(int sessionId, bool completed)
        {
            bool result = false;
            // Create a session object and save
            try
            {
                // Mark the session with an end datetime
                Session session = _context.Sessions
                    .FirstOrDefault(c => c.ID == sessionId);
                // Set abandoned at this time
                session.DateTimeEnded = DateTime.Now;
                // Was it completed correctly?
                if (completed)
                {
                    session.Completed = true;
                }
                else
                {
                    // Mark score as zero if abandoned
                    if (session.AssessmentID != null)
                    {
                        session.Score = 0;
                    }
                    session.Abandoned = true;
                }
                // Save changes
                _context.Entry(session).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                // Mark successful
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return result;
        }
        #endregion
    }
}