using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The video element inherited class
    /// </summary>
    public class LearningDesignElementVideo : LearningDesignElement
    {
        public string VideoUrl { get; set; }

        public LearningDesignElementVideo()
        {
            LearningDesignElementType = LearningDesignElementType.Video;
        }
    }
}