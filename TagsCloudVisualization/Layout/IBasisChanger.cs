namespace TagsCloudVisualization.Layout
{
    public interface IBasisChanger
    {
        (int X, int Y) TransformCoordinatesFromPolarToCartesian(double angle, double length);
    }
}