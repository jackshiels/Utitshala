using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// A class that acts as a pointer to learning design files on the server.
    /// </summary>
    public class LearningDesign
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string StorageURL { get; set; }
        public bool Public { get; set; }
        [ForeignKey("Classroom")]
        public int? ClassroomID { get; set; }

        // Virtual
        public virtual Classroom Classroom { get; set; }
    }
}