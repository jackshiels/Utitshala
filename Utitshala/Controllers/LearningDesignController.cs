using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using Utitshala.Models;
using Utitshala.ViewModels;

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
        [HttpGet]
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

        /// <summary>
        /// Loads a learning design for use in the learning design editor.
        /// </summary>
        /// <param name="learningDesignId">The ID of the learning design to load.</param>
        /// <returns>The learning design editor view, with the learning design
        /// to edit.</returns>
        [HttpGet]
        public ActionResult Editor(int learningDesignId)
        {
            _context = new ApplicationDbContext();
            // Get the current user and their classroom
            string userId = HttpContext.User.Identity.GetUserId();
            ApplicationUser user = _context.Users
                .FirstOrDefault(c => c.Id == userId);
            Classroom classroom = _context.Classrooms
                .FirstOrDefault(c => c.ID == user.ClassroomID);
            LearningDesign learningDesign = _context.LearningDesigns
                .FirstOrDefault(c => c.ID == learningDesignId);
            // Check ownership of this learning design
            if (learningDesign != null)
            {
                if (learningDesign.ClassroomID == classroom.ID)
                {
                    // Construct the viewmodel
                    string fileLocation = AppDomain.CurrentDomain.BaseDirectory 
                        + @"LearningContent\Lessons\" 
                        + learningDesign.StorageURL;
                    LearningDesignEditor model = new LearningDesignEditor()
                    {
                        LearningDesign = learningDesign,
                        LearningDesignCode = System.IO.File.ReadAllText(fileLocation)
                    };
                    // Send it to the view
                    return View(model);
                }
            }
            // Else go back to the index
            return RedirectToAction("Index", "LearningDesign");
        }
    }
}