using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Utitshala.Models
{
    public enum Language { English, Afrikaans, Xhosa, Zulu }
    /// <summary>
    /// The database table for student information.
    /// </summary>
    public class Student
    {
        [Key]
        public int ID { get; set; }
        public int TelegramUserID { get; set; }
        public string Name { get; set; }
        public Language Language { get; set; }
    }
}