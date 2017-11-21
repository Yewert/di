using System.Collections.Generic;

namespace TagsCloudVisualization

{
    public interface IWordFrequencyAnalyzer
    {
        Dictionary<string, int> MakeStatisitcs(IEnumerable<string> lines);
    }
}