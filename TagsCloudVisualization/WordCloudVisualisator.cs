using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace TagsCloudVisualization
{
    public class WordCloudVisualisator : IWordCloudVisualisator
    {
        private readonly int margin;
        private readonly bool debug;
        private readonly Brush brush;

        public WordCloudVisualisator(bool debug, Brush brush) : this(20, debug, brush)
        {
        }

        public WordCloudVisualisator(int margin, bool debug, Brush brush)
        {
            this.margin = margin;
            this.debug = debug;
            this.brush = brush;
        }
        public Bitmap DrawWordCloud(IReadOnlyCollection<WordCloudElement> wordCloud)
        {
            if(wordCloud is null)
                throw new ArgumentNullException();
            wordCloud = wordCloud.ToArray();

            var size = CalculateWidthAndHeight(wordCloud);
            
            var image = new Bitmap(size.Width + 2 * margin, size.Height + 2 * margin);
            using (var graphics = Graphics.FromImage(image))
            {
                graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
                foreach (var cloudElement in wordCloud)
                {
                    graphics.DrawString(cloudElement.Name, cloudElement.Font, brush,
                        new PointF(cloudElement.Rectangle.X + margin,
                        cloudElement.Rectangle.Y + margin));
                    if (debug)
                    {
                        graphics.DrawRectangle(Pens.Blue, new Rectangle(
                            new Point(cloudElement.Rectangle.X + margin,
                                cloudElement.Rectangle.Y +  margin),
                            cloudElement.Rectangle.Size));
                    }
                }
            }
            return image;
        }

        private static Size CalculateWidthAndHeight(IEnumerable<WordCloudElement> wordCloud)
        {
            var leftBound = 0;
            var rightBound = 0;
            var upperBound = 0;
            var lowerBound = 0;
            foreach (var cloudElement in wordCloud)
            {
                leftBound = Math.Min(leftBound, cloudElement.Rectangle.X);
                upperBound = Math.Min(upperBound, cloudElement.Rectangle.Y);
                rightBound = Math.Max(rightBound, cloudElement.Rectangle.X + cloudElement.Rectangle.Width);
                lowerBound = Math.Max(lowerBound, cloudElement.Rectangle.Y + cloudElement.Rectangle.Height);
            }
            return new Size(rightBound - leftBound, lowerBound - upperBound);
        }
    }
}