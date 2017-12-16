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

        private Result<WordCloudElement[]> MakeWordCloudFromStats(
                IReadOnlyCollection<KeyValuePair<string, int>> stats)
        {
            var maxWeight = stats.Max(kvp => kvp.Value);
            var minWeight = stats.Min(kvp => kvp.Value);

            var temporaryGraphics = Graphics.FromImage(new Bitmap(1, 1));
            Result<WordCloudElement> GetFontAndPutRectangle(KeyValuePair<string, int> kvp)
            {
                var fontSize = normalizer.GetFontHeghit(kvp.Value, minWeight, maxWeight);
                var font = Result.Of(() => new Font(fontName, fontSize), "Incorrect font or size");
                var size = font.Then(f => temporaryGraphics.MeasureString(kvp.Key, f));
                return size.Then(s => layouter.PutNextRectangle(new Size((int) Math.Round(s.Width),
                    (int) Math.Round(s.Height)))).Then(r => Result.Ok(new WordCloudElement(kvp.Key, r, font.Value)));
            }

            return stats.Select(GetFontAndPutRectangle).ToArray().CheckForErrors();
        }

        private Result<WordCloudElement[]> ShiftCloud(Result<WordCloudElement[]> cloud)
        {
            return cloud.Then(x => x.Select(element =>
                new WordCloudElement(element.Name, 
                    new Rectangle(
                        doCentering(element.Rectangle.Location,
                            layouter.Center,
                            layouter.LeftBound, layouter.UpperBound),
                        element.Rectangle.Size),
                    element.Font)).ToArray());
        }
        
        private Result<WordCloudElement[]> MakeCloud(IReadOnlyCollection<KeyValuePair<string, int>> stats)
        {
            return ShiftCloud(MakeWordCloudFromStats(stats));
        }

        public Result<Bitmap> GetImage(IEnumerable<string> source)
        {
            return MakeCloud(MakeStats(source)).Then(x => visualisator.DrawWordCloud(x));
        }

        public Result<WordCloudElement[]> GetCloud(IEnumerable<string> source)
        {
            return MakeCloud(MakeStats(source));
        }
    }
}