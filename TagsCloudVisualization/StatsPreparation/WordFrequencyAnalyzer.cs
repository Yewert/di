using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TagsCloudVisualization.StatsPreparation
{
    public class WordFrequencyAnalyzer : IWordFrequencyAnalyzer
    {
        private readonly int amountOfWords;
        private readonly IWordValidator validator;
        private readonly bool lowerCase;
        private readonly Regex wordPattern;

        public WordFrequencyAnalyzer(int minimalWordLength, int amountOfWords, IWordValidator validator, bool lowerCase = true)
        {
            if(minimalWordLength < 1)
                throw new ArgumentException();
            this.amountOfWords = amountOfWords;
            this.validator = validator;
            this.lowerCase = lowerCase;
            var pattern = $@"[a-zа-я][a-zа-я-']{{{minimalWordLength - 1},}}";
            wordPattern = new Regex(pattern,
                RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        }

        public Dictionary<string, int> MakeStatisitcs(IEnumerable<string> lines)
        {
            return lines.SelectMany(line => wordPattern.Matches(line).Cast<Match>()).Select(m => lowerCase
                        ? m.Value.ToLowerInvariant()
                        : m.Value.ToUpperInvariant())
                    .Where(w => !validator.IsExcluded(w))
                    .GroupBy(w => w)
                    .OrderByDescending(x => x.Count())
                    .Take(amountOfWords)
                    .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}