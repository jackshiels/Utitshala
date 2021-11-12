using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The close session element
    /// </summary>
    public class LearningDesignElementExecute : LearningDesignElement
    {
        public string ExecutionName { get; set; }
        public string Argument1 { get; set; }
        public string Argument2 { get; set; }
        public string Argument3 { get; set; }
    }
}