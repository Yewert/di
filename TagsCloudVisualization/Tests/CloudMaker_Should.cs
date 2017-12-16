using System;
using System.Drawing;
using System.Linq;
using NUnit.Framework;
using TagsCloudVisualization.Layout;
using TagsCloudVisualization.StatsPreparation;
using TagsCloudVisualization.Visualisation;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class CloudMaker_Should
    {
        private CloudMaker defaultCloudMaker;

        [SetUp]
        public void SetUp()
        {
            BasisChanger changer = (angle, length) =>
            {
                var x = (int) (length * Math.Cos(angle));
                var y = (int) (length * Math.Sin(angle));
                return (X:x, Y:y);
            };
            defaultCloudMaker = new CloudMaker(
                new WordFrequencyAnalyzer(1, 100, new WordValidator(new string[]{})),
                new CircularCloudLayouter(new Point(0, 0), changer),
                "Arial",
                new FontNormalizer(10, 100),
                (actualPoint, center, boundX, boundY)
                    => new Point(actualPoint.X + center.X - boundX, actualPoint.Y + center.Y - boundY),
                new WordCloudVisualisator(0, false, Brushes.Black));
        }


        [Test]
        public void ReturnCloudOfOneWord_WhenOnlyOneWordPresentInText()
        {
            var cloud = defaultCloudMaker.GetCloud(Enumerable.Repeat("Lol", 100)).GetValueOrThrow();
            Assert.That(cloud.Count, Is.EqualTo(1));
        }
        
        [Test]
        public void ReturnRectanglePlacedAtTheCenterOfCoordinates_WhenWhenOnlyOneWordPresentInText()
        {
            var cloud = defaultCloudMaker.GetCloud(Enumerable.Repeat("Lol", 100)).GetValueOrThrow();
            Assert.That(cloud.ToArray()[0].Rectangle.Location,
                Is.EqualTo(new Point(0, 0)));
        }
        
        [Test]
        public void ReturnCorrectWords_WhenMultipleWordsPresented()
        {
            var cloud = defaultCloudMaker.GetCloud(Enumerable.Repeat("Lol KeK", 100)).GetValueOrThrow();
            Assert.That(cloud.Select(x => x.Name),
                Is.EquivalentTo(new []{"lol", "kek"}));
        }
        
        [Test]
        public void ReturnCorrectSizeProportions_WhenMultipleWordsPresented()
        {
            var cloud =
                defaultCloudMaker.GetCloud(
                    Enumerable.Repeat("Lol", 100)
                        .Concat(Enumerable.Repeat("cheBuRek", 50))
                        .Concat(new[] {"kek"}))
                    .GetValueOrThrow();
            var result = cloud
                .ToDictionary(x => x.Name, x => x.Rectangle.Height);
            Assert.Multiple(() =>
            {
                Assert.That(result["lol"], Is.GreaterThan(result["kek"]));
                Assert.That(result["lol"], Is.GreaterThan(result["cheburek"]));
                Assert.That(result["cheburek"], Is.GreaterThan(result["kek"]));
                
            });
        }

    }
}