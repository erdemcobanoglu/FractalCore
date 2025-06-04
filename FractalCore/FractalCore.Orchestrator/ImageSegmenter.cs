using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace FractalCore.Orchestrator
{
    public static class ImageSegmenter
    {
        /// <summary>
        /// Verilen görüntüyü dikey olarak parçalara ayırır (örneğin 10 dikey şerit).
        /// </summary>
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
