
namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The sticker element inherited class
    /// </summary>
    public class LearningDesignElementSticker : LearningDesignElement
    {
        public string StickerUrl { get; set; }

        public LearningDesignElementSticker()
        {
            LearningDesignElementType = LearningDesignElementType.Sticker;
        }
    }
}