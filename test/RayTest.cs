namespace Nine.Geometry.Test
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
            Assert.Equal(30.0f, new Ray(center - Vector3.UnitY * 40,  Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(30.0f, new Ray(center + Vector3.UnitY * 40, -Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(30.0f, new Ray(center - Vector3.UnitZ * 40,  Vector3.UnitZ).Intersects(boundingBox));
            Assert.Equal(30.0f, new Ray(center + Vector3.UnitZ * 40, -Vector3.UnitZ).Intersects(boundingBox));

            //// Test the corners along each axis.
            //Assert.Equal(10.0f, new Ray(boundingBox.Min - Vector3.UnitX * 10,  Vector3.UnitX).Intersects(boundingBox));
            //Assert.Equal(10.0f, new Ray(boundingBox.Min - Vector3.UnitY * 10,  Vector3.UnitY).Intersects(boundingBox));
            //Assert.Equal(10.0f, new Ray(boundingBox.Min - Vector3.UnitZ * 10,  Vector3.UnitZ).Intersects(boundingBox));
            //Assert.Equal(10.0f, new Ray(boundingBox.Max + Vector3.UnitX * 10, -Vector3.UnitX).Intersects(boundingBox));
            //Assert.Equal(10.0f, new Ray(boundingBox.Max + Vector3.UnitY * 10, -Vector3.UnitY).Intersects(boundingBox));
            //Assert.Equal(10.0f, new Ray(boundingBox.Max + Vector3.UnitZ * 10, -Vector3.UnitZ).Intersects(boundingBox));

            // Test inside out.
            Assert.Equal(0.0f, new Ray(center,  Vector3.UnitX).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center, -Vector3.UnitX).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center,  Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center, -Vector3.UnitY).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center,  Vector3.UnitZ).Intersects(boundingBox));
            Assert.Equal(0.0f, new Ray(center, -Vector3.UnitZ).Intersects(boundingBox));
        }

        // [Fact]
        // public void ray_boundingfrustum()
        // {
        // 
        // }
        // 
        // [Fact]
        // public void ray_boundingsphere()
        // {
        // 
        // }
        // 
        // [Fact]
        // public void ray_plane()
        // {
        // 
        // }
        // 
        // [Fact]
        // public void ray_triangle()
        // {
        // 
        // }
        // 
        // [Fact]
        // public void ray_custom_geometry()
        // {
        // 
        // }

        [Fact]
        public void Operator_Additive()
        {
            var value1 = new Ray(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            var value2 = new Ray(new Vector3(1, 0, 0), new Vector3(1, 0, 0));
            
            var value = value1 + value2;

            Assert.Equal(new Vector3(1, 0, 0), value.Position);
            Assert.Equal(new Vector3(1, 0, 0), value.Direction);
        }

        [Fact]
        public void Operator_Subtractive()
        {
            var value1 = new Ray(new Vector3(0, 0, 0), new Vector3(1, 0, 0));
            var value2 = new Ray(new Vector3(1, 0, 0), new Vector3(1, 0, 0));

            var value = value1 - value2;

            Assert.Equal(new Vector3(-1, 0, 0), value.Position);
            Assert.Equal(new Vector3(-1, 0, 0), value.Direction);
        }
    }
}
