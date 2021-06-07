namespace Nine.Geometry.Test
{
    using System.Numerics;
    using Xunit;
    
    public class GeometryHelperTest
    {
        [Fact]
        public void PointsAreCounterClockwiseOrder_Vector2()
        {
            var points = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
            };

            var counterClockwise = GeometryHelper.PointsAreCounterClockwiseOrder(points);

            Assert.True(counterClockwise);
        }

        [Fact]
        public void PointsAreCounterClockwiseOrder_Vector2_Clockwise()
        {
            var points = new Vector2[]
            {
                new Vector2(1, 1),
                new Vector2(1, 0),
                new Vector2(0, 0),
            };

            var counterClockwise = GeometryHelper.PointsAreCounterClockwiseOrder(points);

            Assert.False(counterClockwise);
        }

        [Fact]
        public void PointsAreCounterClockwiseOrder_Vector3()
        {
            var points = new Vector3[]
            {
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1),
            };

            var counterClockwise = GeometryHelper.PointsAreCounterClockwiseOrder(points);

            Assert.False(counterClockwise);
        }

        [Fact]
        public void PointsAreCounterClockwiseOrder_Vector3_Clockwise()
        {
            var points = new Vector3[]
            {
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1),
            };

            var counterClockwise = GeometryHelper.PointsAreCounterClockwiseOrder(points);

            Assert.False(counterClockwise);
        }
    }
}
