using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    // The language enumerator. Will probably have to be extended.
    public enum Language { English, Afrikaans, Xhosa, Zulu }

    /// <summary>
    /// The object for student information and representation.
    /// </summary>
    public class Student
    {
        [Key]
        public int ID { get; set; }
        public string ServiceUserID { get; set; }
        [ForeignKey("StudentRecord")]
        public int? StudentRecordID { get; set; }
        [ForeignKey("Classroom")]
        public int? ClassroomID { get; set; }
        public string Name { get; set; }
        public Language Language { get; set; }
        public DateTime DateJoined { get; set; }

        // Virtuals
        public virtual StudentRecord StudentRecord { get; set; }
        public virtual Classroom Classroom { get; set; }
    }
}