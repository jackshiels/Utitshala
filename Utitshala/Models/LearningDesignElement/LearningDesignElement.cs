
namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// Represents individual learning design elements, editable in the front end.
    /// </summary>
    public enum LearningDesignElementType { Text, Sticker, Image, Option, Audio, Video }
    public abstract class LearningDesignElement
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public LearningDesignElementType LearningDesignElementType { get; protected set; }
    }
}