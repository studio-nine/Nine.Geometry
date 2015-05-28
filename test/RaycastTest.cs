namespace Nine.Geometry
{
    using System.Numerics;
    using Xunit;

    public class RaycastTest
    {
        [Fact]
        public void ray_intersect_boundingbox()
        {
            var boundingBox = new BoundingBox(new Vector3(-10), new Vector3(10));
            var ray = new Ray(new Vector3(-20, 0, 0), Vector3.UnitX);
            Assert.Equal(10, ray.Intersects(boundingBox));
        }
    }
}
