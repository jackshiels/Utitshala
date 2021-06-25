using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// A student's record of sessions and assignments.
    /// </summary>
    public class StudentRecord
    {
        [Key]
        public int ID { get; set; }

        // Virtuals
        public virtual List<Session> Sessions { get; set; }
        public virtual List<StudentAssignment> StudentAssignments { get; set; }
    }
}