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
            string savePath = null;
            string statsSource = null;
            var amountOfWords = 250;
            var minWordLength = 3;
            var minFontSize = 10.0f;
            var maxFontSize = 150.0f;
            var debug = false;
            var lowerCase = false;
            var parser = new FluentCommandLineParser();
            parser.Setup<string>('S', "source").Callback(source => statsSource = source).Required();
            parser.Setup<string>('s', "save").Callback(save => savePath = save).Required();
            parser.Setup<int>('a', "amount").Callback(amount => amountOfWords = amount);
            parser.Setup<int>('L', "Length").Callback(length => minWordLength = length);
            parser.Setup<int>('m', "minfont").Callback(min => minFontSize = min);
            parser.Setup<int>('M', "maxfont").Callback(max => maxFontSize = max);
            parser.Setup<bool>('d', "debug").Callback(d => debug = d);
            parser.Setup<bool>('l', "lower").Callback(l => lowerCase = l);
            parser.SetupHelp("h", "help", "?").Callback(x => Console.WriteLine(x)).UseForEmptyArgs();
            parser.Parse(args);
            
            if (savePath is null || statsSource is null)
            {
                //Почему этот с(т)ран(н)ый парсер такое допускает?
                Console.WriteLine("save and source are required");
                return;
            }

            if (minFontSize >= maxFontSize)
            {
                Console.WriteLine("minimal font size should be less than maximal");
                return;
            }

            var container = new ContainerBuilder();
            container.RegisterType<CircularCloudLayouter>().As<ICircularCloudLayouter>();
            container.Register(c => new Point(0, 0)).AsSelf().Named<Point>("CloudCenter");
            container.Register(x => new FontNormalizer(minFontSize, maxFontSize)).As<IFontNormalizer>();
            container.RegisterType<ImageBounder>().As<IImageBounder>();
            container.Register(x => new WordFrequencyAnalyzer(minWordLength, amountOfWords, lowerCase))
                .As<IWordFrequencyAnalyzer>();
            
            //Как через консоль выбирать цвет?)
            container.Register(c => Brushes.Black).AsSelf().Named<Brush>("WordBrush");
            
            //Почему он подсовывает этот bool всем, а не только тем, кто помечен аттрибутом?
            container.Register(c => debug).AsSelf().Named<bool>("DrawingDebug");
            container.RegisterType<WordCloudVisualisator>()
                .As<IWordCloudVisualisator>();
            container.RegisterType<CloudMaker>().AsSelf();
            container.RegisterType<BasisChanger>().As<IBasisChanger>();
            
            var build = container.Build();
            var maker = build.Resolve<CloudMaker>();
            
            maker.GetImage(File.ReadLines(statsSource)).Save(savePath + ".png");
        }
    }
}