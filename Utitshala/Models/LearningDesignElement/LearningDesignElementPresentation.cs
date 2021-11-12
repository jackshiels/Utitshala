using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The presentation element inherited class
    /// </summary>
    public class LearningDesignElementPresentation : LearningDesignElement
    {
        public string[] ImageURLs { get; set; }
        public bool Compressed { get; set; }
    }
}