using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utitshala.Models;

namespace Utitshala.ViewModels
{
    /// <summary>
    /// A viewmodel for transporting the learning design and its
    /// physical file to the editor view.
    /// </summary>
    public class LearningDesignEditor
    {
        public LearningDesign LearningDesign { get; set; }
        public string LearningDesignCode { get; set; }
    }
}