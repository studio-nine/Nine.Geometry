namespace Nine.Geometry
{
    using System.Numerics;
    using Xunit;

    public class RayTest
    {
        [Fact]
        public void ray_boundingbox()
        {
            var boundingBox = new BoundingBox(new Vector3(-10), new Vector3(10));
            var center = boundingBox.Center;
            
            // Test misses.
            Assert.Null(new Ray(center - Vector3.UnitX * 40, -Vector3.UnitX).Intersects(boundingBox));
            Assert.Null(new Ray(center + Vector3.UnitX * 40,  Vector3.UnitX).Intersects(boundingBox));
            Assert.Null(new Ray(center - Vector3.UnitY * 40, -Vector3.UnitY).Intersects(boundingBox));
            Assert.Null(new Ray(center + Vector3.UnitY * 40,  Vector3.UnitY).Intersects(boundingBox));
            Assert.Null(new Ray(center - Vector3.UnitZ * 40, -Vector3.UnitZ).Intersects(boundingBox));
            Assert.Null(new Ray(center + Vector3.UnitZ * 40,  Vector3.UnitZ).Intersects(boundingBox));

            // Test middle of each face.
            Assert.Equal(30.0f, new Ray(center - Vector3.UnitX * 40,  Vector3.UnitX).Intersects(boundingBox));
            Assert.Equal(30.0f, new Ray(center + Vector3.UnitX * 40, -Vector3.UnitX).Intersects(boundingBox));
            Assert.Equal(20.0f, new Ray(center - Vector3.UnitY * 40,  Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(20.0f, new Ray(center + Vector3.UnitY * 40, -Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(10.0f, new Ray(center - Vector3.UnitZ * 40,  Vector3.UnitZ).Intersects(boundingBox));
            Assert.Equal(10.0f, new Ray(center + Vector3.UnitZ * 40, -Vector3.UnitZ).Intersects(boundingBox));

            // Test the corners along each axis.
            Assert.Equal(10.0f, new Ray(boundingBox.Min - Vector3.UnitX * 10,  Vector3.UnitX).Intersects(boundingBox));
            Assert.Equal(10.0f, new Ray(boundingBox.Min - Vector3.UnitY * 10,  Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(10.0f, new Ray(boundingBox.Min - Vector3.UnitZ * 10,  Vector3.UnitZ).Intersects(boundingBox));
            Assert.Equal(10.0f, new Ray(boundingBox.Max + Vector3.UnitX * 10, -Vector3.UnitX).Intersects(boundingBox));
            Assert.Equal(10.0f, new Ray(boundingBox.Max + Vector3.UnitY * 10, -Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(10.0f, new Ray(boundingBox.Max + Vector3.UnitZ * 10, -Vector3.UnitZ).Intersects(boundingBox));

            // Test inside out.
            Assert.Equal(0.0f, new Ray(center,  Vector3.UnitX).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center, -Vector3.UnitX).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center,  Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center, -Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center,  Vector3.UnitZ).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center, -Vector3.UnitZ).Intersects(boundingBox));
        }
    }
}
