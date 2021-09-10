using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The option element inherited class
    /// </summary>
    public class LearningDesignElementOption : LearningDesignElement
    {
        public List<List<string>> Options { get; set; }
    }
}