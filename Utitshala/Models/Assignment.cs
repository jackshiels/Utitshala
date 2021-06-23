using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// An enum to represent the type of assignment. 
    /// Text: message input from chat.
    /// Image: photo image input from chat.
    /// Audio: recorded input from chat client.
    /// </summary>
    public enum AssignmentType { Text, Image, Audio }
    /// <summary>
    /// A class to represent assignments to be conducted by students.
    /// </summary>
    public class Assignment
    {
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
        public DateTime DateOpened { get; set; }
        public DateTime? DateDue { get; set; }
        public AssignmentType Type { get; set; }
        public bool Public { get; set; }
        [ForeignKey("Classroom")]
        public int? ClassroomID { get; set; }
        // Virtuals
        public virtual Classroom Classroom { get; set; }
    }
}