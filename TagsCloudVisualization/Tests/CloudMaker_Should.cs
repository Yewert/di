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
            defaultCloudMaker = new CloudMaker(
                new WordFrequencyAnalyzer(1, 100, new WordValidator(new string[]{})),
                new CircularCloudLayouter(new Point(0, 0), new BasisChanger()),
                "Arial",
                new FontNormalizer(10, 100),
                new ImageBounder(),
                new WordCloudVisualisator(0, false, Brushes.Black));
        }


        [Test]
        public void ReturnCloudOfOneWord_WhenOnlyOneWordPresentInText()
        {
            Assert.That(defaultCloudMaker.GetCloud(Enumerable.Repeat("Lol", 100)).Count, Is.EqualTo(1));
        }
        
        [Test]
        public void ReturnRectanglePlacedAtTheCenterOfCoordinates_WhenWhenOnlyOneWordPresentInText()
        {
            Assert.That(defaultCloudMaker.GetCloud(Enumerable.Repeat("Lol", 100)).ToArray()[0].Rectangle.Location,
                Is.EqualTo(new Point(0, 0)));
        }
        
        [Test]
        public void ReturnCorrectWords_WhenMultipleWordsPresented()
        {
            Assert.That(defaultCloudMaker.GetCloud(Enumerable.Repeat("Lol KeK", 100)).Select(x => x.Name),
                Is.EquivalentTo(new []{"lol", "kek"}));
        }
        
        [Test]
        public void ReturnCorrectSizeProportions_WhenMultipleWordsPresented()
        {
            var result = defaultCloudMaker.GetCloud(Enumerable.Repeat("Lol", 100).Concat(Enumerable.Repeat("cheBuRek", 50)).Concat(new[] {"kek"}))
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