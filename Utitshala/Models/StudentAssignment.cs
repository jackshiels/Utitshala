using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// Acts as a pointer for an uploaded assignment, or a container
    /// for the string data of a text assignment.
    /// </summary>
    public class StudentAssignment
    {
        [Key]
        public int ID { get; set; }
        public DateTime UploadDate { get; set; }
        public string Value { get; set; }
        public string MetaData { get; set; }
        [ForeignKey("Assignment")]
        public int? AssignmentID { get; set; }
        public int? FileSize { get; set; }

        // Virtuals
        public virtual Assignment Assignment { get; set; }
    }
}