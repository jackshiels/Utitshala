using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// An object to represent a forum and its chat messages.
    /// </summary>
    public class Forum
    {
        [Key]
        public int ID { get; set; }
        [ForeignKey("LearningDesign")]
        public int LearningDesignID { get; set; }

        // Virtuals
        public virtual LearningDesign LearningDesign { get; set; }
        public virtual List<ForumMessage> ForumMessages { get; set; }
    }
}