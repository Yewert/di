using System;
using System.Drawing;
using System.IO;
using Autofac;
using Fclp;

namespace TagsCloudVisualization
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            ApplicationArguments arguments;
            try
            {
                arguments = ParseArgs(args);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            var color = ColorTranslator.FromHtml(arguments.ColorCode);
            var brush = new SolidBrush(color);

            var container = new ContainerBuilder();
            container.RegisterType<CircularCloudLayouter>().As<ICloudLayouter>()
                .WithParameter("center", new Point(0, 0)).InstancePerDependency();
            container.Register(x => new FontNormalizer(arguments.MinFontSize, arguments.MaxFontSize)).As<IFontNormalizer>();
            container.RegisterType<ImageBounder>().As<IImageBounder>();
            container.Register(x => new WordFrequencyAnalyzer(arguments.MinWordLength,
                    arguments.AmountOfWords, arguments.LowerCase))
                .As<IWordFrequencyAnalyzer>();
            
            container.RegisterType<WordCloudVisualisator>()
                .As<IWordCloudVisualisator>().WithParameter("brush", brush).WithParameter("debug", arguments.Debug);
            container.RegisterType<CloudMaker>().AsSelf().WithParameter("fontName", arguments.FontName);
            container.RegisterType<BasisChanger>().As<IBasisChanger>();
            
            var build = container.Build();
            var maker = build.Resolve<CloudMaker>();
            
            maker.GetImage(File.ReadLines(arguments.StatsSource)).Save(arguments.SavePath + ".png");
        }

        private static ApplicationArguments ParseArgs(string[] args)
        {
            string savePath = null;
            string statsSource = null;
            var amountOfWords = 250;
            var minWordLength = 3;
            var minFontSize = 10.0f;
            var maxFontSize = 150.0f;
            var debug = false;
            var lowerCase = false;
            
            //Здесь даже если сделать белый дефолтным, все то же самое
            var colorCode = "#000000";
            var fontName = "Arial";
            var parser = new FluentCommandLineParser();
            parser.Setup<string>('S', "source").Callback(source => statsSource = source).Required();
            parser.Setup<string>('s', "save").Callback(save => savePath = save).Required();
            parser.Setup<int>('a', "amount").Callback(amount => amountOfWords = amount);
            parser.Setup<int>('L', "Length").Callback(length => minWordLength = length);
            parser.Setup<int>('m', "minfont").Callback(min => minFontSize = min);
            parser.Setup<int>('M', "maxfont").Callback(max => maxFontSize = max);
            parser.Setup<string>('c', "color").Callback(code => colorCode = code);
            parser.Setup<string>('f', "font").Callback(font => fontName = font);
            parser.Setup<bool>('d', "debug").Callback(d => debug = d);
            parser.Setup<bool>('l', "lower").Callback(l => lowerCase = l);
            parser.SetupHelp("h", "help", "?").Callback(x => Console.WriteLine(x)).UseForEmptyArgs();
            parser.Parse(args);
            return new ApplicationArguments(savePath, statsSource, amountOfWords, minWordLength, minFontSize,
                maxFontSize, debug, lowerCase, colorCode, fontName);
        }
    }
}