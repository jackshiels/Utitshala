using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// An object representing a forum's student messages.
    /// </summary>
    public class ForumMessage
    {
        [Key]
        public int ID { get; set; }
        public string MessageContents { get; set; }
        public DateTime MessageDate { get; set; }
        [ForeignKey("Forum")]
        public int ForumID { get; set; }
        [ForeignKey("Student")]
        public int StudentID { get; set; }

        // Virtuals
        public virtual Forum Forum { get; set; }
        public virtual Student Student { get; set; }

        // Functions
        /// <summary>
        /// Returns the message with the date and time formatted as a suffix.
        /// </summary>
        /// <returns>A string of the formatted message.</returns>
        public string GetMessage()
        {
            return "Sender: " + Student.Name + "\nSent: " + MessageDate.ToShortDateString() + ", " + MessageDate.ToShortTimeString() + "\n\n" + MessageContents;
        }
    }
}