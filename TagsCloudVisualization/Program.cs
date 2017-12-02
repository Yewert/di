using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Autofac;
using CommandLine;
using TagsCloudVisualization.Layout;
using TagsCloudVisualization.StatsPreparation;
using TagsCloudVisualization.Visualisation;

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
            catch (TargetInvocationException e)
            {
                Console.WriteLine(e.InnerException?.Message);
                return;
            }
            var color = ColorTranslator.FromHtml(arguments.ColorCode);
            var brush = new SolidBrush(color);

            var container = new ContainerBuilder();
            container.RegisterType<CircularCloudLayouter>().As<ICloudLayouter>()
                .WithParameter("center", new Point(0, 0)).SingleInstance();
            container.Register(x => new FontNormalizer(arguments.MinFontSize, arguments.MaxFontSize)).As<IFontNormalizer>();
            container.RegisterType<ImageBounder>().As<IImageBounder>();
            container.Register(x => new WordValidator()).As<IWordValidator>();
            container.RegisterType<WordFrequencyAnalyzer>()
                .As<IWordFrequencyAnalyzer>()
                .WithParameter("minimalWordLength", arguments.MinWordLength)
                .WithParameter("amountOfWords", arguments.AmountOfWords)
                .WithParameter("lowerCase", arguments.LowerCase);
            
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
            var options = new ApplicationArguments();
            Parser.Default.ParseArgumentsStrict(    args, options);
            return options;
        }
    }
}