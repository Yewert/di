using System.Collections.Generic;

namespace TagsCloudVisualization.StatsPreparation

{
    public interface IWordFrequencyAnalyzer
    {
        Dictionary<string, int> MakeStatisitcs(IEnumerable<string> lines);
    }
}