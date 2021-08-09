using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// A object that represents a class of students and teams.
    /// </summary>
    public class Classroom
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("School")]
        public int? SchoolID { get; set; }
        public string Name { get; set; }

        // Virtuals
        public virtual School School { get; set; }
        public virtual List<Student> Students { get; set; }
        public virtual List<LearningDesign> LearningDesigns { get; set; }
        public virtual List<Assessment> Assessments { get; set; }
        public virtual List<Assignment> Assignments { get; set; }
    }
}