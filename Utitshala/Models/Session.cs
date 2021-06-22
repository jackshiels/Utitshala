using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// A session of connectivity throughout a lesson.
    /// </summary>
    public class Session
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("LearningDesign")]
        public int? LearningDesignID { get; set; }
        [ForeignKey("Assessment")]
        public int? AssessmentID { get; set; }
        public DateTime? DateTimeStarted { get; set; }
        public DateTime? DateTimeEnded { get; set; }
        public bool Completed { get; set; }
        public bool Abandoned { get; set; }

        // Virtuals
        public Assessment Assessment { get; set; }
        public virtual LearningDesign LearningDesign { get; set; }
    }
}