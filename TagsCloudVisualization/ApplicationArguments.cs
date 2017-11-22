using System;

namespace TagsCloudVisualization
{
    public class ApplicationArguments
    {
        public string SavePath { get; }
        public string StatsSource { get; }
        public int AmountOfWords { get; }
        public int MinWordLength { get; }
        public float MinFontSize { get; }
        public float MaxFontSize { get; }
        public bool Debug { get; }
        public bool LowerCase { get; }
        public string ColorCode { get; }
        public string FontName { get; }

        public ApplicationArguments(string savePath, string statsSource, int amountOfWords, int minWordLength,
            float minFontSize, float maxFontSize, bool debug, bool lowerCase, string colorCode, string fontName)
        {
            // ReSharper disable once NotResolvedInText
            if (amountOfWords <= 0)
                throw new ArgumentOutOfRangeException("Amount of words should be a positive integer");
            // ReSharper disable once NotResolvedInText
            if (minWordLength <= 0)
                throw new ArgumentOutOfRangeException("Minimal word length should be a positive integer");
            // ReSharper disable once NotResolvedInText
            if (minFontSize <= 0)
                throw new ArgumentOutOfRangeException("Minimal font size should be a positive integer");
            // ReSharper disable once NotResolvedInText
            if (maxFontSize <= 0)
                throw new ArgumentOutOfRangeException("Maximal font size should be a positive integer");
            if (minFontSize >= maxFontSize)
                throw new ArgumentException("Minimal font size should be less than maximal");
            // ReSharper disable once NotResolvedInText
            SavePath = savePath
                       ?? throw new ArgumentNullException("Save path cannot be null");
            // ReSharper disable once NotResolvedInText
            StatsSource = statsSource
                          ?? throw new ArgumentNullException("Stat source path cannot be null");
            AmountOfWords = amountOfWords;
            MinWordLength = minWordLength;
            MinFontSize = minFontSize;
            MaxFontSize = maxFontSize;
            Debug = debug;
            LowerCase = lowerCase;
            ColorCode = colorCode;
            FontName = fontName;
        }
    }
}