using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        public decimal Score { get; set; }
        public DateTime DateTimeAttempted { get; set; }
    }
}