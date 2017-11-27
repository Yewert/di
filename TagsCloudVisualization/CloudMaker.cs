using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CloudMaker
    {
        private readonly IWordFrequencyAnalyzer statsMaker;
        private readonly ICloudLayouter layouter;
        private readonly string fontName;
        private readonly IFontNormalizer normalizer;
        private readonly IImageBounder bounder;
        private readonly IWordCloudVisualisator visualisator;

        public CloudMaker(
            IWordFrequencyAnalyzer statsMaker,
            ICloudLayouter layouter,
            string fontName,
            IFontNormalizer normalizer,
            IImageBounder bounder,
            IWordCloudVisualisator visualisator)
        {
            this.statsMaker = statsMaker;
            this.layouter = layouter;
            this.fontName = fontName;
            this.normalizer = normalizer;
            this.bounder = bounder;
            this.visualisator = visualisator;
            
        }

        private IReadOnlyCollection<KeyValuePair<string, int>> MakeStats(IEnumerable<string> sourceData)
        {
            var stats = statsMaker.MakeStatisitcs(sourceData);
            return stats.ToArray();
        }

        private IReadOnlyCollection<WordCloudElement> MakeWordCloudFromStats(
                IReadOnlyCollection<KeyValuePair<string, int>> stats)
        {
            var maxWeight = stats.Max(kvp => kvp.Value);
            var minWeight = stats.Min(kvp => kvp.Value);

            WordCloudElement GetFontAndPutRectangle(KeyValuePair<string, int> kvp)
            {
                var fontSize = normalizer.GetFontHeghit(kvp.Value, minWeight, maxWeight);
                var font = new Font(fontName, fontSize);
                Rectangle rectangle;
                using (var temporaryGraphics = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    var size = temporaryGraphics.MeasureString(kvp.Key, font);
                    rectangle = layouter.PutNextRectangle(new Size((int)Math.Round(size.Width),
                        (int)Math.Round(size.Height)));
                }
                return new WordCloudElement(kvp.Key, rectangle, font);
            }

            return stats.Select(GetFontAndPutRectangle).ToArray();
        }



        public Bitmap GetImage(IEnumerable<string> source)
        {
            var stats = MakeStats(source);
            var wordCloud = MakeWordCloudFromStats(stats)
                .Select(element =>
                    new WordCloudElement(element.Name, 
                        new Rectangle(
                            bounder.TransformRelativeToAbsoluteBounded(element.Rectangle.Location,
                                layouter.Center,
                                layouter.LeftBound, layouter.UpperBound),
                            element.Rectangle.Size),
                        element.Font)).ToArray();
            return visualisator.DrawWordCloud(wordCloud);
        }
    }
}