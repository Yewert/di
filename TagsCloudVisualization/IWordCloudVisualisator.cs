using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization
{
    public interface IWordCloudVisualisator
    {
        Bitmap DrawWordCloud(IReadOnlyCollection<WordCloudElement> wordCloud);
    }
}