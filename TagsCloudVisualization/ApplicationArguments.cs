using System;
using System.Linq;
using CommandLine;
using CommandLine.Text;

namespace TagsCloudVisualization
{
    public class ApplicationArguments
    {
        [Option('s', "save-path", Required = true, HelpText="Save path for image (excluding extension)")]
        public string SavePath
        {
            get => savePath;
            set => savePath = value ?? throw new ArgumentException("Save path cannot be null");
        }
        
        [Option('t', "text-path", Required = true, HelpText = "Path to source text file(plain text only)")]
        public string StatsSource
        {
            get => statsSource;
            set => statsSource = value ?? throw new ArgumentException("Stat source cannot be null");
        }

        [Option('n', "number-of-words", HelpText = "Max amount of words in cloud", DefaultValue = 200)]
        public int AmountOfWords
        {
            get => amountOfWords;
            set
            {
                if (value < 1)
                    throw new ArgumentOutOfRangeException($"Amount of words should be a positive integer");
                amountOfWords = value;
            }
        }

        [Option('w', "minimal-word-length", HelpText = "Minimal word length", DefaultValue = 2)]
        public int MinWordLength
        {
            get => minWordLength;
            set
            {
                if (minWordLength < 1)
                    throw new ArgumentOutOfRangeException($"Minimal word length should be a positive integer");
                minWordLength = value;
            }
        }

        private (int min, int max) fontSizePair = (10, 150);
        private string savePath;
        private string statsSource;
        private int amountOfWords = 100;
        private int minWordLength = 2;

        public float MinFontSize => fontSizePair.min;
        public float MaxFontSize => fontSizePair.max;
        
        [Option('d', "debug", HelpText = "Enable blue rectangles around words", DefaultValue = false)]
        public bool Debug { get; set; } = false;
        [Option('l', "lower-case", HelpText = "Set all words to lower case, otherwise to upper", DefaultValue = false)]
        public bool LowerCase { get; set; } = false;
        [Option('c', "color-code", HelpText = "Set color code in html hex format", DefaultValue = "#000000")]
        public string ColorCode { get; set; } = "#000000";
        [Option('F', "font", HelpText = "Set font", DefaultValue = "Arial")]
        public string FontName { get; set; } = "Arial";

        [Option('f', "font-size-pair", HelpText = "Font size pair in format \"10,150\"", DefaultValue = "10,150")]
        public string FontSizePair
        {
            get => fontSizePair.ToString();
            set
            {
                try
                {
                    var tokens = value.Split(',').Select(int.Parse).ToArray();
                    if (tokens[0] <= 0)
                        throw new ArgumentOutOfRangeException($"Minimal font size should be a positive integer");
                    if (tokens[1] <= 0)
                        throw new ArgumentOutOfRangeException($"Maximal font size should be a positive integer");
                    if (tokens[0] >= tokens[1])
                        throw new ArgumentException("Minimal font size should be less than maximal");
                    fontSizePair = (tokens[0], tokens[1]);
                }
                catch (FormatException e)
                {
                    throw new ArgumentException(e.Message);
                }
                
            }
        }
        
        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("Tag Cloud Generator App", "1.0"),
                Copyright = new CopyrightInfo("github.com/yewert", 3017),
                AddDashesToOption = true,
                AdditionalNewLineAfterOption = true
            };
            help.AddOptions(this);
            return help;
        }
    }
}