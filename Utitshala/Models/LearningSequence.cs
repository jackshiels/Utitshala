using Spin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utitshala.Services;

namespace Utitshala.Models
{
    /// <summary>
    /// A class for the customised extention of a Spin sequence for learning.
    /// </summary>
    public class LearningSequence : Sequence
    {
        /// <summary>
        /// Creates a learning sequence with the appropriate extensions.
        /// </summary>
        /// <param name="backend">The dictionary backed.</param>
        /// <param name="file">The file loader backend</param>
        public LearningSequence(DictionaryBackend backend, FileDocumentLoader file) : base(backend, file)
        {
            AddCommand("opt", SequenceExtensions.OptionTraversal);
            AddCommand("image", SequenceExtensions.ImageMessageHandler);
            AddCommand("sticker", SequenceExtensions.StickerMessageHandler);
            AddCommand("wait", SequenceExtensions.WaitTimer);
            AddCommand("input", SequenceExtensions.ReceiveInput);
            AddCommand("execute", SequenceExtensions.ExecuteFunction);
        }
    }
}