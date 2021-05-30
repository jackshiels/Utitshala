using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public DateTime DateTimeStarted { get; set; }
        public DateTime DateTimeEnded { get; set; }
        public bool Abandoned { get; set; }

        // Virtuals
        public virtual List<Assessment> Assessments { get; set; }
    }
}