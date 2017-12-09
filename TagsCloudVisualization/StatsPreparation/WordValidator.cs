using System;
using System.Collections.Generic;

namespace TagsCloudVisualization.StatsPreparation
{
    public class WordValidator : IWordValidator
    {
        private HashSet<string> excludedWords;

        public WordValidator() : this(new []
        {
            "а",
            "но",
            "в",
            "по",
            "и",
            "без",
            "безо",
            "о",
            "об",
            "обо",
            "не",
            "с",
            "со",
            "от"
        })
        {
        }
           
        public WordValidator(IEnumerable<string> excludedWords)
        {
            this.excludedWords = new HashSet<string>(excludedWords, StringComparer.OrdinalIgnoreCase);
        }
        
        public bool IsExcluded(string word)
        {
            return excludedWords.Contains(word);
        }
    }
}