
namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The audio element inherited class
    /// </summary>
    public class LearningDesignElementAudio : LearningDesignElement
    {
        public string AudioUrl { get; set; }

        public LearningDesignElementAudio()
        {
            LearningDesignElementType = LearningDesignElementType.Audio;
        }
    }
}