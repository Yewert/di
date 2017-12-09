using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization.Visualisation
{
    public interface IWordCloudVisualisator
    {
        Bitmap DrawWordCloud(IReadOnlyCollection<WordCloudElement> wordCloud);
    }
}