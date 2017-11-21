using System.Collections.Generic;
using System.Drawing;

namespace TagsCloudVisualization
{
    public interface IWordCloudVisualisator
    {
        Bitmap DrawWorldCloud(IEnumerable<WordCloudElement> wordCloud);
    }
}