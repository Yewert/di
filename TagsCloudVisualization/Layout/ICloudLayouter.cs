using System.Drawing;

namespace TagsCloudVisualization.Layout
{
    public interface ICloudLayouter
    {
        int LeftBound { get; }
        int RightBound { get; }
        int UpperBound { get; }
        int LowerBound { get; }
        int Count { get; }
        int Width { get; }
        int Height { get; }
        Point Center { get; }
        Result<Rectangle> PutNextRectangle(Size rectangleSize);
    }
}