﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TagsCloudVisualization.Layout;

namespace TagsCloudVisualization.Tests
{
    [TestFixture]
    public class CircularCloudLayouter_Should
    {
        private BasisChanger changer = (angle, length) =>
        {
            var x = (int) (length * Math.Cos(angle));
            var y = (int) (length * Math.Sin(angle));
            return (X:x, Y:y);
        };
        [TestCase(0, 0, 10, 10, -5, -5)]
        [TestCase(10, 10, 10, 10, 5, 5)]
        [TestCase(10, 10, 11, 11, 5, 5)]
        public void PutInCenter_WhenFirstRectangleGiven(
            int centerX, int centerY, int rectangleWidth, int rectangleHeight, int rectangleX, int rectangleY)
        {
            var center = new Point(centerX, centerY);
            var size = new Size(rectangleWidth, rectangleHeight);
            var rectangleCoords = new Point(rectangleX, rectangleY);
            var rectangle = new Rectangle(rectangleCoords, size);
            var layouter = new CircularCloudLayouter(center, changer);
            layouter.PutNextRectangle(size).ShouldBeEquivalentTo(rectangle);
        }

        [Test]
        public void HaveCorrectBounds_AfterPuttingFirstRectangle()
        {
            var center = new Point(0, 0);
            var size = new Size(10, 15);
            var layouter = new CircularCloudLayouter(center, changer);
            layouter.PutNextRectangle(size);
            (int left, int right, int top, int bottom, int width, int height) actualDimensions =
                (layouter.LeftBound, layouter.RightBound,
                layouter.UpperBound, layouter.LowerBound,
                layouter.Width, layouter.Height);
            actualDimensions.ShouldBeEquivalentTo((-5, 5, -7, 8, 10, 15));
        }

        [TestCase(2)]
        [TestCase(100)]
        [TestCase(1000)]
        public void HaveCorrectCount_WhenMultipleRectanglesAdded(int count)
        {
            var center = new Point(0, 0);
            var size = new Size(10, 10);
            var layouter = new CircularCloudLayouter(center, changer);
            for (var i = 0; i < count; i++)
            {
                layouter.PutNextRectangle(size);
            }
            Assert.AreEqual(count, layouter.Count);
        }

        [Test]
        public void NotReturnIntersectingRectangles_WhenPutMultiple()
        {
            var center = new Point(0, 0);
            var size = new Size(10, 10);
            var layouter = new CircularCloudLayouter(center, changer);
            var rectangles = new List<Rectangle>();
            for (var i = 0; i < 100; i++)
            {
                rectangles.Add(layouter.PutNextRectangle(size));
            }
            Assert.False(rectangles
                .AsParallel()
                .Select((rectangle, index) => rectangles
                    .Skip(index + 1)
                    .Any(rect => rect.IntersectsWith(rectangle)))
                .Any(intersects => intersects));
        }

        [Test, Timeout(2000)]
        public void WorkFast_When500RectanglesArePutIn()
        {
            var center = new Point(0, 0);
            var size = new Size(10, 10);
            var layouter = new CircularCloudLayouter(center, changer);
            for (var i = 0; i < 500; i++)
            {
                layouter.PutNextRectangle(size);
            }
        }

        [TestCase(-1, 1)]
        [TestCase(0, 1)]
        [TestCase(1, -1)]
        [TestCase(1, 0)]
        [TestCase(0, 0)]
        [TestCase(-1, -1)]
        public void ThrowArgumentException_WhenSizeHasAtLeastOneNonPositiveDimension(int width, int height)
        {
            var center = new Point(0, 0);
            var size = new Size(width, height);
            var layouter = new CircularCloudLayouter(center, changer);
            Assert.Throws<ArgumentException>(() => layouter.PutNextRectangle(size));
        }
    }
    
}