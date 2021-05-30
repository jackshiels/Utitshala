using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    /// <summary>
    /// A student's record of sessions.
    /// </summary>
    public class StudentRecord
    {
        [Key]
        public int ID { get; set; }
        public List<Session> Session { get; set; }
    }
}