using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utitshala.Models.LearningDesignElement;
using static Utitshala.Services.Interfaces;

namespace Utitshala.Services
{
    /// <summary>
    /// An implementation of the learning design translator class, used to
    /// translate file data to learning design element model data, and vice
    /// versa.
    /// </summary>
    public class LearningDesignTranslator : ILearningDesignTranslator
    {
        /// <summary>
        /// Translates a .spd file into learning design element model data.
        /// </summary>
        /// <param name="learningDesignText"></param>
        /// <returns></returns>
        public List<LearningDesignElement> TranslateFileToElements(string learningDesignText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Translates a list of learning design element model data into an
        /// .spd file.
        /// </summary>
        /// <param name="elements">The list of learning design elements.</param>
        /// <returns>A .spd file string.</returns>
        public string TranslateElementsToFile(List<LearningDesignElement> elements)
        {
            throw new NotImplementedException();
        }
    }
}