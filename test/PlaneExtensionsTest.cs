namespace Nine.Geometry.Test
{
    using System.Numerics;
    using Xunit;

    public class PlaneExtensionsTest
    {
        [Fact]
        public void IsValid()
        {
            var plane = Plane.CreateFromVertices(
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0)
            );

            var valid = plane.IsValid();

            Assert.True(valid);
        }

        [Fact]
        public void IsValid_False()
        {
            var plane = Plane.CreateFromVertices(
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0)
            );

            var valid = plane.IsValid();

            Assert.False(valid);
        }

        [Fact]
        public void Flip()
        {
            var plane = Plane.CreateFromVertices(
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0)
            );

            var org_normal = plane.Normal;
            var new_normal = plane.Flip().Normal;

            Assert.Equal(new Vector3(0, 0, -1), org_normal);
            Assert.Equal(new Vector3(0, 0, 1), new_normal);
        }
    }
}
