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
        public void Normal()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1, 0));
            var normal = segment.Normal;
            Assert.Equal(new Vector2(0, 1), normal);
        }

        [Fact]
        public void Center()
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
        public void Length()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1, 0));
            var length = segment.Length();
            Assert.Equal(1, length);
        }

        [Fact]
        public void LengthSquared()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1, 0));
            var length = segment.LengthSquared();
            Assert.Equal(1, length);
        }

        [Fact]
        public void Offset()
        {
            var segment = new LineSegment2D(new Vector2(0), new Vector2(1, 0));
            segment.Offset(1);

            Assert.Equal(0, segment.Start.X);
            Assert.Equal(1, segment.Start.Y);
            Assert.Equal(1, segment.End.X);
            Assert.Equal(1, segment.End.Y);
        }

        [Fact]
        public void Intersects()
        {
            var segment1 = new LineSegment2D(new Vector2(0, 0), new Vector2(2, 0));
            var segment2 = new LineSegment2D(new Vector2(1, 1), new Vector2(1, -1));

            var point = segment1.Intersect(segment2);

            Assert.Equal(new Vector2(1, 0), point);
        }
    }
}
