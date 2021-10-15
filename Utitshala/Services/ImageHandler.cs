using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static Utitshala.Services.Interfaces;
using ImageMagick;

namespace Utitshala.Services
{
    public class ImageHandler : IImageHandler
    {
        /// <summary>
        /// Compresses a file by resizing it and modifying its compression level.
        /// </summary>
        /// <param name="image">The filestream of the image to compress.</param>
        /// <param name="filePath">The file path to save it to.</param>
        /// <returns>A compressed image filestream.</returns>
        public FileStream CompressImage(FileStream image, string filePath)
        {
            var resizedImage = new MagickImage(image);
            // Close the existing filestream
            image.Close();
            // Delete the stored image
            File.Delete(filePath);
            // Resize to 50%
            var resize = new MagickGeometry(new Percentage(50), new Percentage(50));
            resizedImage.Resize(resize);
            // Write to path with .jpg
            resizedImage.Write(filePath + ".jpg");
            // Compress, starting with opening the file again
            image = File.Open(filePath + ".jpg", FileMode.Open);
            // Create a new compressor and act
            new ImageOptimizer().Compress(image);
            // Convert to an image type stream
            var compressedImage = new MagickImage(image);
            // Close the old stream and write the new, compressed image stream
            image.Close();
            compressedImage.Write(filePath + ".jpg");
            // Open the image to return it
            image = File.Open(filePath + ".jpg", FileMode.Open);
            return image;
        }
    }
}