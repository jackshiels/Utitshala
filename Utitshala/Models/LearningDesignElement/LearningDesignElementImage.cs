using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The image element inherited class
    /// </summary>
    public class LearningDesignElementImage : LearningDesignElement
    {
        public string ImageUrl { get; set; }
        public string Caption { get; set; }
        public bool Compressed { get; set; }
    }
}