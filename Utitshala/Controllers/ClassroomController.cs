using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Utitshala.Models;

namespace Utitshala.Controllers
{
    [Authorize]
    public class ClassroomController : Controller
    {
        private ApplicationDbContext _context;
        /// <summary>
        /// Returns the teacher's classroom, with accompanying students and learning designs.
        /// </summary>
        /// <returns>The classroom view.</returns>
        public ActionResult Index()
        {
            // Set context
            _context = new ApplicationDbContext();
            // Get the current user
            string userId = HttpContext.User.Identity.GetUserId();
            ApplicationUser user = _context.Users
                .FirstOrDefault(c => c.Id == userId);
            Classroom classroom = _context.Classrooms
                .FirstOrDefault(c => c.ID == user.ClassroomID);
            // Return the model
            return View(classroom);
        }

        /// <summary>
        /// Removes a student from a classroom.
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns>A redirect to the classroom index.</returns>
        public ActionResult RemoveStudent(int studentId)
        {
            try
            {
                // Set context
                _context = new ApplicationDbContext();
                // Find the student
                Student student = _context.Students
                    .FirstOrDefault(c => c.ID == studentId);
                if (student != null)
                {
                    // Get the user's classroom
                    string userId = HttpContext.User.Identity.GetUserId();
                    ApplicationUser user = _context.Users
                        .FirstOrDefault(c => c.Id == userId);
                    Classroom classroom = _context.Classrooms
                        .FirstOrDefault(c => c.ID == user.ClassroomID);
                    // Do they have permission to remove this student?
                    if (student.ClassroomID == classroom.ID)
                    {
                        student.ClassroomID = null;
                        _context.Entry(student).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return RedirectToAction("Index");
        }
    }
}