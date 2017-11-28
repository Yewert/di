using System.Drawing;

namespace TagsCloudVisualization.Visualisation
{
    public class ImageBounder : IImageBounder
    {
        public Point TransformRelativeToAbsoluteBounded(Point actualPoint, Point center, int boundX, int boundY)
        {
            return new Point(actualPoint.X + center.X - boundX, actualPoint.Y + center.Y - boundY);
        }
    }
}