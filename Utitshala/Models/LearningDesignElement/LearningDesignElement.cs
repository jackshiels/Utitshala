using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// Represents individual learning design elements, editable in the front end.
    /// </summary>
    public enum LearningDesignElementType { Text, Sticker, Image, Option, Audio, Video }
    public class LearningDesignElement
    {
        public string Name { get; set; }
        public string TextContent { get; set; }
        public LearningDesignElementType LearningDesignElementType { get; set; }
    }
}