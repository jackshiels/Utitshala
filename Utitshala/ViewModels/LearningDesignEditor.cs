using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utitshala.Models;
using Utitshala.Services;

namespace Utitshala.ViewModels
{
    /// <summary>
    /// A viewmodel for transporting the learning design and its
    /// physical file to the editor view.
    /// </summary>
    public class LearningDesignEditor
    {
        [VueData("LearningDesign")]
        public LearningDesign LearningDesign { get; set; }
        [VueData("LearningDesignCode")]
        public string LearningDesignCode { get; set; }
        public Dictionary<string, object> VueData { get; set; } = new Dictionary<string, object>();
    }
}