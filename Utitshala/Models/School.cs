using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// A class representing the school that a teacher administrates.
    /// </summary>
    public class School
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Virtuals
        public virtual List<Classroom> Classrooms { get; set; }
    }
}