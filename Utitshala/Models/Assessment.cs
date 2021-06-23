using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// An assessment that can be taken by a student.
    /// </summary>
    public class Assessment
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public string StorageURL { get; set; }
        public int QuestionsCount { get; set; }
        public bool Public { get; set; }
        [ForeignKey("Classroom")]
        public int? ClassroomID { get; set; }

        // Virtuals
        public virtual Classroom Classroom { get; set; }
    }
}