using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utitshala.Models;

namespace Utitshala.Controllers
{
    public class LearningDesignController : Controller
    {
        #region Properties
        private ApplicationDbContext _context;
        #endregion

        /// <summary>
        /// Returns the list of learning designs associated with the user's
        /// classroom ID.
        /// </summary>
        /// <returns>The learning design index view.</returns>
        public ActionResult Index()
        {
            // Get the learning designs owned by this teacher
            _context = new ApplicationDbContext();
            // Get the current user
            string userId = HttpContext.User.Identity.GetUserId();
            ApplicationUser user = _context.Users
                .FirstOrDefault(c => c.Id == userId);
            Classroom classroom = _context.Classrooms
                .FirstOrDefault(c => c.ID == user.ClassroomID);
            List<LearningDesign> learningDesigns = _context.LearningDesigns
                .Where(c => c.ClassroomID == classroom.ID).ToList();
            // Class name viewbag
            ViewBag.ClassName = classroom.Name;
            // Return the model
            return View(learningDesigns);
        }
    }
}