using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TagsCloudVisualization
{
    public class CircularCloudLayouter : ICloudLayouter
    {   
        private readonly Point center;
        private readonly IBasisChanger transformer;

        public int LeftBound
        {
            get => leftBound;
            private set => leftBound = Math.Min(leftBound, value);
        }

        public int RightBound
        {
            get => rightBound;
            private set => rightBound = Math.Max(rightBound, value);
        }

        public int UpperBound
        {
            get => upperBound;
            private set => upperBound = Math.Min(upperBound, value);
        }

        public int LowerBound
        {
            get => lowerBound;
            private set => lowerBound = Math.Max(lowerBound, value);
        }

        private int leftBound;
        private int rightBound;
        private int upperBound;
        private int lowerBound;
        
        private readonly List<Rectangle> rectangles;

        public int Count => rectangles.Count;

        public int Width => RightBound - LeftBound;
        public int Height => LowerBound - UpperBound;

        public Point Center => center;
        
        public CircularCloudLayouter(Point center, IBasisChanger transformer)
        {
            this.center = center;
            this.transformer = transformer;
            rectangles = new List<Rectangle>();
        }

        public Rectangle PutNextRectangle(Size rectangleSize)
        {
            if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
                throw new ArgumentException();
            var success = TryPutNextRectangle(rectangleSize, out var rectangle);
            if (!success)
            {
                throw new NotSupportedException();
            }
            UpdateBounds(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            rectangles.Add(rectangle);
            return rectangle;
        }

        private void UpdateBounds(int x, int y, int width, int height)
        {
            LeftBound = x;
            RightBound = x + width;
            UpperBound = y;
            LowerBound = y + height;
        }

        private bool TryPutNextRectangle(Size rectangleSize, out Rectangle rectangle)
        {
            var square = (long)rectangleSize.Height * rectangleSize.Width;
            var stretchCoefficient = (1.0 / square);
            var angle = 0.0;
            for(var i = 0; i < int.MaxValue; i++)
            {
                var shiftFromCenter = transformer.TransformCoordinatesFromPolarToCartesian(angle, angle);
                var upperLeftCorner = new Point(Center.X + shiftFromCenter.X - rectangleSize.Width / 2,
                    Center.Y + shiftFromCenter.Y - rectangleSize.Height / 2);
                var temporaryRectangle = new Rectangle(upperLeftCorner, rectangleSize);
                if (!IntersectsWithAnyOtherRectangle(temporaryRectangle))
                {
                    rectangle = temporaryRectangle;
                    return true;
                }
                angle += GetNextShift(i, stretchCoefficient);
            }
            rectangle = Rectangle.Empty;
            return false;
        }


        private const double LimitOfMultipliers = 0.5;
        
        private const double BaseDeltaAngle = Math.PI / 15;

        private const double StretchYCoefficient = 0.75;
        
        private double GetNextShift(int iteration, double stretchXCoefficient)
        {
            return BaseDeltaAngle * (StretchYCoefficient / (iteration * stretchXCoefficient + 1) + LimitOfMultipliers);
        }

        private bool IntersectsWithAnyOtherRectangle(Rectangle rect)
        {
            return rectangles.Any(r => r.IntersectsWith(rect));
        }

        
    }
}