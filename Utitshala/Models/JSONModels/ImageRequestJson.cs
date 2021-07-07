using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utitshala.Models.JSONModels
{
    /// <summary>
    /// Used to receive JSON for an image location request in Telegram.
    /// </summary>
    public class ImageRequestJson
    {
        public bool ok { get; set; }
        public ImageResult result { get; set; }
    }

    public class ImageResult
    {
        public string file_id { get; set; }
        public string file_unique_id { get; set; }
        public int file_size { get; set; }
        public string file_path { get; set; }
    }
}