using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TagsCloudVisualization.Layout;
using TagsCloudVisualization.StatsPreparation;
using TagsCloudVisualization.Visualisation;

namespace TagsCloudVisualization
{
    public class CloudMaker
    {
        private readonly IWordFrequencyAnalyzer statsMaker;
        private readonly ICloudLayouter layouter;
        private readonly string fontName;
        private readonly IFontNormalizer normalizer;
        private readonly CloudCenterer doCentering;
        private readonly IWordCloudVisualisator visualisator;

        public CloudMaker(
            IWordFrequencyAnalyzer statsMaker,
            ICloudLayouter layouter,
            string fontName,
            IFontNormalizer normalizer,
            CloudCenterer doCentering,
            IWordCloudVisualisator visualisator)
        {
            this.statsMaker = statsMaker;
            this.layouter = layouter;
            this.fontName = fontName;
            this.normalizer = normalizer;
            this.doCentering = doCentering;
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

            var temporaryGraphics = Graphics.FromImage(new Bitmap(1, 1));
            WordCloudElement GetFontAndPutRectangle(KeyValuePair<string, int> kvp)
            {
                var fontSize = normalizer.GetFontHeghit(kvp.Value, minWeight, maxWeight);
                var font = new Font(fontName, fontSize);
                var size = temporaryGraphics.MeasureString(kvp.Key, font);
                var rectangle = layouter.PutNextRectangle(new Size((int) Math.Round(size.Width),
                    (int) Math.Round(size.Height)));
                return new WordCloudElement(kvp.Key, rectangle, font);
            }

            var result = stats.Select(GetFontAndPutRectangle).ToArray();
            temporaryGraphics.Dispose();
            return result;
        }

        private IReadOnlyCollection<WordCloudElement> ShiftCloud(IReadOnlyCollection<WordCloudElement> cloud)
        {
            return cloud.Select(element =>
                new WordCloudElement(element.Name, 
                    new Rectangle(
                        doCentering(element.Rectangle.Location,
                            layouter.Center,
                            layouter.LeftBound, layouter.UpperBound),
                        element.Rectangle.Size),
                    element.Font)).ToArray();
        }
        
        private IReadOnlyCollection<WordCloudElement> MakeCloud(IReadOnlyCollection<KeyValuePair<string, int>> stats)
        {
            return ShiftCloud(MakeWordCloudFromStats(stats));
        }

        public Bitmap GetImage(IEnumerable<string> source)
        {
            return visualisator.DrawWordCloud(MakeCloud(MakeStats(source)));
        }
        
        public IReadOnlyCollection<WordCloudElement> GetCloud(IEnumerable<string> source)
        {
            return MakeCloud(MakeStats(source));
        }
    }
}