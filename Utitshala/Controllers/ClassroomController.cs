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
    }
}