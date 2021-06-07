namespace Nine.Geometry.Test
{
    using System.Collections.Generic;
    using System.Numerics;
    using Xunit;

    public class LineSegment3DTest
    {
        [Fact]
        public void Ctor()
        {
            var segment = new LineSegment3D(new Vector3(1, 2, 3), new Vector3(4, 5, 6));
            Assert.Equal(1, segment.Start.X);
            Assert.Equal(2, segment.Start.Y);
            Assert.Equal(3, segment.Start.Z);
            Assert.Equal(4, segment.End.X);
            Assert.Equal(5, segment.End.Y);
            Assert.Equal(6, segment.End.Z);
        }

        [Fact]
        public void Center_XYZ()
        {
            var segment = new LineSegment3D(new Vector3(0), new Vector3(1));
            var center = segment.Center;
            Assert.Equal(new Vector3(0.5f), center);
        }

        public static IEnumerable<object[]> ClosestPointOnLineData =>
            new List<object[]>
            {
                // ---
                //  o
                new object[] {
                    new Vector3(0, 0, 0),
                    new Vector3(2, 0, 0),
                    new Vector3(1, 1, 0),
                    new Vector3(1, 0, 0)
                },
                // ---
                //   o
                new object[] {
                    new Vector3(0, 0, 0),
                    new Vector3(2, 0, 0),
                    new Vector3(2, 1, 0),
                    new Vector3(2, 0, 0)
                },
            };

        [Theory]
        [MemberData(nameof(ClosestPointOnLineData))]
        public void ClosestPointOnLine(Vector3 start, Vector3 end, Vector3 point, Vector3 expected)
        {
            var segment = new LineSegment3D(start, end);
            var closestPoint = segment.ClosestPointOnLine(point);
            Assert.Equal(expected, closestPoint);
        }

        [Fact]
        public void Length_X()
        {
            var segment = new LineSegment3D(new Vector3(0), new Vector3(1, 0, 0));
            var length = segment.Length();
            Assert.Equal(1, length);
        }

        [Fact]
        public void Length_Y()
        {
            var segment = new LineSegment3D(new Vector3(0), new Vector3(0, 1, 0));
            var length = segment.Length();
            Assert.Equal(1, length);
        }

        [Fact]
        public void Length_Z()
        {
            var segment = new LineSegment3D(new Vector3(0), new Vector3(0, 0, 1));
            var length = segment.Length();
            Assert.Equal(1, length);
        }

        [Fact]
        public void LengthSquared_X()
        {
            var segment = new LineSegment3D(new Vector3(0), new Vector3(1, 0, 0));
            var length = segment.LengthSquared();
            Assert.Equal(1, length);
        }

        [Fact]
        public void LengthSquared_Y()
        {
            var segment = new LineSegment3D(new Vector3(0), new Vector3(0, 1, 0));
            var length = segment.LengthSquared();
            Assert.Equal(1, length);
        }

        [Fact]
        public void LengthSquared_Z()
        {
            var segment = new LineSegment3D(new Vector3(0), new Vector3(0, 0, 1));
            var length = segment.LengthSquared();
            Assert.Equal(1, length);
        }

        [Fact]
        public void Contains()
        {
            var segment = new LineSegment3D(new Vector3(0, 1, 1), new Vector3(2, 1, 1));
            var point = new Vector3(1, 1, 1);

            var hit = segment.Contains(point);

            Assert.True(hit);
        }

        [Fact]
        public void Contains_Invalid()
        {
            var segment = new LineSegment3D(new Vector3(0, 1, 1), new Vector3(2, 1, 1));
            var point = new Vector3(0, 0, 0);

            var hit = segment.Contains(point);

            Assert.False(hit);
        }

        [Fact]
        public void Intersects_XY()
        {
            var segment1 = new LineSegment3D(new Vector3(0, 1, 0), new Vector3(0, -1, 0));
            var segment2 = new LineSegment3D(new Vector3(-1, 0, 0), new Vector3(1, 0, 0));

            var point = segment1.Intersect(segment2);

            Assert.Equal(new Vector3(0, 0, 0), point);
        }

        [Fact]
        public void Intersects_Z()
        {
            var segment1 = new LineSegment3D(new Vector3(0, 1, 0), new Vector3(0, -1, 0));
            var segment2 = new LineSegment3D(new Vector3(0, 0, -1), new Vector3(0, 0, 1));

            var point = segment1.Intersect(segment2);

            Assert.Equal(new Vector3(0, 0, 0), point);
        }

        [Fact]
        public void Intersects_XYZ()
        {
            var segment1 = new LineSegment3D(new Vector3(0, 0, 0), new Vector3(2, 0, 0));
            var segment2 = new LineSegment3D(new Vector3(0, 1, 1), new Vector3(2, -1, -1));

            var point = segment1.Intersect(segment2);

            Assert.Equal(new Vector3(1, 0, 0), point);
        }

        [Fact]
        public void Intersects_XYZ_Invalid()
        {
            var segment1 = new LineSegment3D(new Vector3(0, 10, 0), new Vector3(5, 15, 5));
            var segment2 = new LineSegment3D(new Vector3(10, 20, 10), new Vector3(15, 25, 15));

            var point = segment1.Intersect(segment2);

            Assert.Null(point);
        }

        [Fact]
        public void ClosestConnection()
        {
            var segment1 = new LineSegment3D(new Vector3(1, 0, 0), new Vector3(2, 0, 0));
            var segment2 = new LineSegment3D(new Vector3(0, 1, 0), new Vector3(0, -1, 0));

            var segment = segment1.ClosestConnection(segment2);

            Assert.Equal(new Vector3(1, 0, 0), segment.Start);
            Assert.Equal(new Vector3(0, 0, 0), segment.End);
        }
    }
}
