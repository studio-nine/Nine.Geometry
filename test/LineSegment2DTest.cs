namespace Nine.Geometry.Test
{
    using System.Collections.Generic;
    using System.Numerics;
    using Xunit;

    public class LineSegment2DTest
    {
        [Fact]
        public void Ctor()
        {
            var segment = new LineSegment2D(new Vector2(1, 2), new Vector2(3, 4));
            Assert.Equal(1, segment.Start.X);
            Assert.Equal(2, segment.Start.Y);
            Assert.Equal(3, segment.End.X);
            Assert.Equal(4, segment.End.Y);
        }

        [Fact]
        public void Normal_X()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1, 0));
            var normal = segment.Normal;
            Assert.Equal(new Vector2(0, 1), normal);
        }

        [Fact]
        public void Normal_Y()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(0, 1));
            var normal = segment.Normal;
            Assert.Equal(new Vector2(-1, 0), normal);
        }

        [Fact]
        public void Center_XY()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1));
            var center = segment.Center;
            Assert.Equal(new Vector2(0.5f), center);
        }

        public static IEnumerable<object[]> ClosestPointOnLineData =>
            new List<object[]>
            {
                // ---
                //  o
                new object[] {
                    new Vector2(0, 0),
                    new Vector2(2, 0),
                    new Vector2(1, 1),
                    new Vector2(1, 0)
                },
                // ---
                //   o
                new object[] {
                    new Vector2(0, 0),
                    new Vector2(2, 0),
                    new Vector2(2, 1),
                    new Vector2(2, 0)
                },
            };

        [Theory]
        [MemberData(nameof(ClosestPointOnLineData))]
        public void ClosestPointOnLine(Vector2 start, Vector2 end, Vector2 point, Vector2 expected)
        {
            var segment = new LineSegment2D(start, end);
            var closestPoint = segment.ClosestPointOnLine(point);
            Assert.Equal(expected, closestPoint);
        }

        [Fact]
        public void Length_X()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1, 0));
            var length = segment.Length();
            Assert.Equal(1, length);
        }

        [Fact]
        public void Length_Y()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(0, 1));
            var length = segment.Length();
            Assert.Equal(1, length);
        }

        [Fact]
        public void LengthSquared_X()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1, 0));
            var length = segment.LengthSquared();
            Assert.Equal(1, length);
        }

        [Fact]
        public void LengthSquared_Y()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(0, 1));
            var length = segment.LengthSquared();
            Assert.Equal(1, length);
        }

        [Fact]
        public void Offset_X()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1, 0));
            segment.Offset(1);

            Assert.Equal(0, segment.Start.X);
            Assert.Equal(1, segment.Start.Y);
            Assert.Equal(1, segment.End.X);
            Assert.Equal(1, segment.End.Y);
        }

        [Fact]
        public void Intersects_XY()
        {
            var segment1 = new LineSegment2D(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new LineSegment2D(new Vector2(1, 1), new Vector2(1, -1));

            var point = segment1.Intersect(segment2);

            Assert.Equal(new Vector2(1, 0), point);
        }

        [Fact]
        public void Intersects_X_Invalid()
        {
            var segment1 = new LineSegment2D(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new LineSegment2D(new Vector2(0, 1), new Vector2(2, 1));

            var point = segment1.Intersect(segment2);

            Assert.Null(point);
        }

        [Fact]
        public void Intersects_Y_Invalid()
        {
            var segment1 = new LineSegment2D(new Vector2(0, 0), new Vector2(5, 0));
            var segment2 = new LineSegment2D(new Vector2(10, 0), new Vector2(15, 0));

            var point = segment1.Intersect(segment2);

            Assert.Null(point);
        }

        [Fact]
        public void Intersects_XY_Invalid()
        {
            var segment1 = new LineSegment2D(new Vector2(0, 10), new Vector2(5, 15));
            var segment2 = new LineSegment2D(new Vector2(5, 15), new Vector2(10, 20));

            var point = segment1.Intersect(segment2);

            Assert.Null(point);
        }
    }
}
