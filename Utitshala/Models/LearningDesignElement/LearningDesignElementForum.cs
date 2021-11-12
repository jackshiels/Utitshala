using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The forum element inherited class
    /// </summary>
    public class LearningDesignElementForum : LearningDesignElement
    {
        public DateTime EndDate { get; set; }
        public string WelcomeMessage { get; set; }
    }
}