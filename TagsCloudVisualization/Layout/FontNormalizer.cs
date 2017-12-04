using System;

namespace TagsCloudVisualization.Layout
{
    public class FontNormalizer : IFontNormalizer
    {
        private readonly float minFont;
        private readonly float maxFont;

        public FontNormalizer(float minFont, float maxFont)
        {
            this.maxFont = maxFont;
            this.minFont = minFont;
        }

        public float GetFontHeghit(int frequency, int minOccurence, int maxOccurence)
        {
            return Math.Max(maxFont * (frequency - minOccurence) / (maxOccurence - minOccurence + 1), minFont);
        }
    }
}