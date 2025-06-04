using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Versioning;

namespace FractalCore.Orchestrator.Services
{
    public static class ImageSegmenter
    {
        /// <summary>
        /// Splits the given image into vertical segments.
        /// </summary>
        [SupportedOSPlatform("windows")]
        public static List<byte[]> SplitImage(string imagePath, int numberOfSegments)
        {
            var segments = new List<byte[]>();

            using (var originalImage = Image.FromFile(imagePath))
            {
                int segmentWidth = originalImage.Width / numberOfSegments;
                int height = originalImage.Height;

                for (int i = 0; i < numberOfSegments; i++)
                {
                    int x = i * segmentWidth;
                    Rectangle cropArea = new Rectangle(x, 0, segmentWidth, height);

                    using (var bmpImage = new Bitmap(originalImage))
                    using (var bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat))
                    using (var ms = new MemoryStream())
                    {
                        bmpCrop.Save(ms, ImageFormat.Jpeg);
                        segments.Add(ms.ToArray());
                    }
                }
            }

            return segments;
        }
    }
}
