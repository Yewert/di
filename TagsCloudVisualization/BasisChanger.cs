using System;

namespace TagsCloudVisualization
{
    public class BasisChanger : IBasisChanger
    {
        public (int X, int Y) TransformCoordinatesFromPolarToCartesian(double angle, double length)
        {
            var x = (int) (length * Math.Cos(angle));
            var y = (int) (length * Math.Sin(angle));
            return (x, y);
        }
    }
}