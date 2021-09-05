using System.Collections.Generic;

namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The option element inherited class
    /// </summary>
    public class LearningDesignElementOption : LearningDesignElement
    {
        public List<string> Options { get; set; }

        public LearningDesignElementOption()
        {
            LearningDesignElementType = LearningDesignElementType.Option;
        }
    }
}