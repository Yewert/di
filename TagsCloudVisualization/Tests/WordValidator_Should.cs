using System.Linq;
using NUnit.Framework;
using TagsCloudVisualization.StatsPreparation;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class WordValidator_Should
    {
        [Test]
        public void AcceptEveryWord_WhenNoWordsAreBanned()
        {
            var validator = new WordValidator(new string[]{});
            var source = new[] {"a", "BB", "cC"};
            var result = source.Where(x => !validator.IsExcluded(x)).ToArray();
            Assert.That(result, Is.EquivalentTo(source));
        }

        [Test]
        public void DropBannedWords_WhenTheyAreExactlyTheSame()
        {
            var validator = new WordValidator(new []{"BB"});
            var source = new[] {"a", "BB", "cC"};
            var result = source.Where(x => !validator.IsExcluded(x)).ToArray();
            Assert.That(result, Is.EquivalentTo(new [] {"a", "cC"}));
        }

        [Test]
        public void DropBannedWords_WhenTheyAreInDifferentCase()
        {
            var validator = new WordValidator(new []{"cC"});
            var source = new[] {"a", "BB", "Cc", "cc", "CC"};
            var result = source.Where(x => !validator.IsExcluded(x)).ToArray();
            Assert.That(result, Is.EquivalentTo(new [] {"a", "BB"}));
        }
    }
}