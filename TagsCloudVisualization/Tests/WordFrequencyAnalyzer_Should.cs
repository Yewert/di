using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using TagsCloudVisualization.StatsPreparation;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class WordFrequencyAnalyzer_Should
    {
        private Mock<IWordValidator> mock; 
       
        [SetUp]
        public void SetUp()
        {
            mock = new Mock<IWordValidator>();
        }

        [Test]
        public void ReturnAllWords_WhenAllCriteriasMetWithSomeMargin()
        {
            var analyzer = new WordFrequencyAnalyzer(1, 100, mock.Object);
            var expected = new Dictionary<string, int>
            {
                ["gay"] = 2,
                ["hi"] = 1,
                ["i"] = 1,
                ["am"] = 1,
                ["don't"] = 1,
                ["judge"] = 1,
                ["me"] = 1,
                ["for"] = 1,
                ["being"] = 1,
            };
            Assert.That(analyzer.MakeStatisitcs(
                new []{"Hi, I am gay", "Don't judge me for being gay!"}),
                Is.EquivalentTo(expected));
        }
        
        [Test]
        public void ReturnSomeWordsExceptBanned_WhenSingleIsBanned()
        {
            mock.Setup(x => x.IsExcluded("gay")).Returns(true);
            var analyzer = new WordFrequencyAnalyzer(1, 100, mock.Object);
            var expected = new Dictionary<string, int>
            {
                ["hi"] = 1,
                ["i"] = 1,
                ["am"] = 1,
                ["don't"] = 1,
                ["judge"] = 1,
                ["me"] = 1,
                ["for"] = 1,
                ["being"] = 1,
            };
            Assert.That(
                analyzer.MakeStatisitcs(new []{"Hi, I am gay", "Don't judge me for being gay!"}),
                Is.EquivalentTo(expected));
        }
        
        [Test]
        public void ReturnMostFrequentWords_WhenMaxAmountIsLessThanActual()
        {
            mock.Setup(x => x.IsExcluded(It.IsAny<string>())).Returns(false);
            var analyzer = new WordFrequencyAnalyzer(1, 3, mock.Object);
            var expected = new Dictionary<string, int>
            {
                ["gay"] = 2,
                ["i"] = 3,
                ["am"] = 3
            };
            Assert.That(
                analyzer.MakeStatisitcs(new []{"Hi, I am gay", "Don't judge me for being gay!", "I am who i am"}),
                Is.EquivalentTo(expected));
        }
        
        [Test]
        public void ReturnEmpty_WhenAllWordsAreBanned()
        {
            mock.Setup(x => x.IsExcluded(It.IsAny<string>())).Returns(true);
            var analyzer = new WordFrequencyAnalyzer(1, 3, mock.Object);
            Assert.That(
                analyzer.MakeStatisitcs(new []{"Hi, I am gay", "Don't judge me for being gay!", "I am who i am"}),
                Is.Empty);
        }

        [Test]
        public void ReturnOnlyLongEnoughWords_WhenMinimalLengthIsGreaterThanOne()
        {
            mock.Setup(x => x.IsExcluded(It.IsAny<string>())).Returns(false);
            var analyzer = new WordFrequencyAnalyzer(5, 100, mock.Object);
            var expected = new Dictionary<string, int>    
            {
                ["don't"] = 1,
                ["judge"] = 1,
                ["being"] = 1,
            };
            Assert.That(analyzer.MakeStatisitcs(
                new []{"Hi, I am gay", "Don't judge me for being gay!", "I am who i am"}),
                Is.EquivalentTo(expected));
        }
        
    }
}