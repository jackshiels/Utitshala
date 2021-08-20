using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Services
{
    /// <summary>
    /// A VueJS property parsing attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class VueData : Attribute
    {
        public VueData(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}