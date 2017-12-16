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

            CloudCenterer doCenterig = (actualPoint, center, boundX, boundY)
                => new Point(actualPoint.X + center.X - boundX, actualPoint.Y + center.Y - boundY);
            
            BasisChanger changer = (angle, length) =>
            {
                var x = (int) (length * Math.Cos(angle));
                var y = (int) (length * Math.Sin(angle));
                return (X:x, Y:y);
            };
            
            
            var container = new ContainerBuilder();
            container.RegisterType<CircularCloudLayouter>().As<ICloudLayouter>()
                .WithParameter("center", new Point(0, 0))
                .WithParameter("transform", changer);
            container.Register(x => new FontNormalizer(arguments.MinFontSize, arguments.MaxFontSize)).As<IFontNormalizer>();
            container.Register(x => new WordValidator()).As<IWordValidator>();
            container.RegisterType<WordFrequencyAnalyzer>()
                .As<IWordFrequencyAnalyzer>()
                .WithParameter("minimalWordLength", arguments.MinWordLength)
                .WithParameter("amountOfWords", arguments.AmountOfWords)
                .WithParameter("lowerCase", arguments.LowerCase);
            container.RegisterType<WordCloudVisualisator>()
                .As<IWordCloudVisualisator>().WithParameter("brush", brush).WithParameter("debug", arguments.Debug);
            container.RegisterType<CloudMaker>().AsSelf()
                .WithParameter("fontName", arguments.FontName)
                .WithParameter("doCentering", doCenterig);
            
            var build = container.Build();
            var maker = build.Resolve<CloudMaker>();

            var image = maker.GetImage(File.ReadLines(arguments.StatsSource));
            image.Then(x => Result.OfAction(() => x.Save(arguments.SavePath + ".png"))).OnFail(Console.WriteLine);
        }

        private static ApplicationArguments ParseArgs(string[] args)
        {
            var options = new ApplicationArguments();
            Parser.Default.ParseArgumentsStrict(    args, options);
            return options;
        }
    }
}