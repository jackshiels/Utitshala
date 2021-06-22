using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// A record of a student assessment being taken.
    /// </summary>
    public class Assessment
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Score { get; set; }
        public DateTime DateTimeAttempted { get; set; }
        [ForeignKey("Classroom")]
        public int? ClassroomID { get; set; }

        // Virtuals
        public virtual Classroom Classroom { get; set; }
    }
}