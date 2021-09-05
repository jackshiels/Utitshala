
namespace Utitshala.Models.LearningDesignElement
{
    /// <summary>
    /// The text element inherited class
    /// </summary>
    public class LearningDesignElementText : LearningDesignElement
    {
        public string TextContent { get; set; }

        public LearningDesignElementText()
        {
            LearningDesignElementType = LearningDesignElementType.Text;
        }
    }
}