namespace Nine.Geometry
{
    using System;
    using System.Numerics;
    
    /// <summary>
    /// 
    /// </summary>
    public static class Geometry
    {
        public static void CreateBox(this BoundingBox boundingBox, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateSolidBox(this BoundingBox boundingBox, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateSphere(this BoundingSphere boundingSphere, int tessellation, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateSolidSphere(this BoundingSphere boundingSphere, int tessellation, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateFrustum(this BoundingFrustum boundingFrustum, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateSolidFrustum(this BoundingFrustum boundingFrustum, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateCone(Vector3 position, float height, float radius, int tessellation, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateSolidCone(Vector3 position, float height, float radius, int tessellation, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateCylinder(Vector3 position, float height, float radius, int tessellation, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }

        public static void CreateSolidCylinder(Vector3 position, float height, float radius, int tessellation, out Vector3[] triangles, out ushort[] indices)
        {
            throw new NotImplementedException();
        }
    }
}
