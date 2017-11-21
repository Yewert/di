using System.Drawing;

namespace TagsCloudVisualization
{
    public interface IImageBounder
    {
        Point TransformRelativeToAbsoluteBounded(Point actualPoint, Point center, int boundX, int boundY);
    }
}