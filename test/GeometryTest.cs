namespace Nine.Geometry
{
    using System.Numerics;
    using Xunit;

    /// <remarks>
    /// This is just call test to see so there are no exeptions thrown.
    /// Visual representation should be tested in Nine.Graphics.
    /// </remarks>
    public class GeometryTest
    {
        [Fact]
        public void call_test()
        {
            Vector3[] points;
            ushort[] indices;

            // TODO: I should probably vary the arguments

            Geometry.CreateBox(new BoundingBox(new Vector3(0, 0, 0), new Vector3(100, 100, 100)), out points, out indices);
            Geometry.CreateCone(new Vector3(0, 0, 0), 8, 8, 16, out points, out indices);
            Geometry.CreateCylinder(new Vector3(0, 0, 0), 8, 8, 16, out points, out indices);
            
            Geometry.CreateFrustum(new BoundingFrustum(Matrix4x4.Identity), out points, out indices);
            Geometry.CreateSphere(new BoundingSphere(new Vector3(0, 0, 0), 8), 16, out points, out indices);

            Geometry.CreateSolidBox(new BoundingBox(new Vector3(0, 0, 0), new Vector3(100, 100, 100)), out points, out indices);
            Geometry.CreateSolidCone(new Vector3(0, 0, 0), 8, 8, 16, out points, out indices);
            Geometry.CreateSolidCylinder(new Vector3(0, 0, 0), 8, 8, 16, out points, out indices);
            Geometry.CreateSolidFrustum(new BoundingFrustum(Matrix4x4.Identity), out points, out indices);
            Geometry.CreateSolidSphere(new BoundingSphere(new Vector3(0, 0, 0), 8), 16, out points, out indices);
        }
    }
}
