namespace Nine.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    // TODO: Optimize all Lists (?)

    /// <summary>
    /// Creates Geometry Shapes.
    /// </summary>
    public static class Geometry
    {
        /// <remarks>
        /// Primitive Type: Line List
        /// </remarks>
        public static void CreateBox(this BoundingBox boundingBox, out Vector3[] points, out ushort[] indices)
        {
            points = boundingBox.GetCorners();
            indices = new ushort[]
            {
                0,1,  1,2,  2,3,  3,0,
                4,5,  5,6,  6,7,  7,4,
                0,4,  1,5,  2,6,  3,7,
            };
        }

        /// <remarks>
        /// Primitive Type: Triangle List
        /// </remarks>
        public static void CreateSolidBox(this BoundingBox boundingBox, out Vector3[] points, out ushort[] indices)
        {
            points = boundingBox.GetCorners();
            indices = BoundingBox.TriangleIndices;
        }

        /// <remarks>
        /// Primitive Type: Line List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateSphere(this BoundingSphere boundingSphere, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            int currentVertex = 2;
            int verticalSegments = tessellation;
            int horizontalSegments = tessellation;

            var vertices = new List<Vector3>();
            var indices2 = new List<ushort>();

            // Start with a single vertex at the bottom of the sphere.
            vertices.Add(-Vector3.UnitY * boundingSphere.Radius + boundingSphere.Center);

            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; ++i)
            {
                float latitude = ((i + 1) * (float)Math.PI / verticalSegments) - (float)(Math.PI / 2.0);

                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * (float)(Math.PI * 2.0) / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);

                    vertices.Add(normal * boundingSphere.Radius + boundingSphere.Center);
                    currentVertex++;
                }
            }

            // Finish with a single vertex at the top of the sphere.
            vertices.Add(Vector3.UnitY * boundingSphere.Radius + boundingSphere.Center);

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; ++i)
            {
                indices2.Add(0);
                indices2.Add((ushort)(1 + (i + 1) % horizontalSegments));
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; ++i)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    indices2.Add((ushort)(1 + i * horizontalSegments + j));
                    indices2.Add((ushort)(1 + i * horizontalSegments + nextJ));

                    indices2.Add((ushort)(1 + i * horizontalSegments + nextJ));
                    indices2.Add((ushort)(1 + nextI * horizontalSegments + nextJ));

                    indices2.Add((ushort)(1 + nextI * horizontalSegments + j));
                    indices2.Add((ushort)(1 + nextI * horizontalSegments + j));
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; ++i)
            {
                indices2.Add((ushort)(currentVertex - 1));
                indices2.Add((ushort)(currentVertex - 2 - (i + 1) % horizontalSegments));
            }

            points = vertices.ToArray();
            indices = indices2.ToArray();
        }

        /// <remarks>
        /// Primitive Type: Triangle List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateSolidSphere(this BoundingSphere boundingSphere, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            var vertices = new List<Vector3>();
            var indices2 = new List<ushort>();

            int currentVertex = 2;
            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            // Start with a single vertex at the bottom of the sphere.
            vertices.Add(-Vector3.UnitY * boundingSphere.Radius + boundingSphere.Center);

            // Create rings of vertices at progressively higher latitudes.
            for (int i = 0; i < verticalSegments - 1; ++i)
            {
                float latitude = ((i + 1) * (float)Math.PI / verticalSegments) - (float)(Math.PI / 2.0);

                float dy = (float)Math.Sin(latitude);
                float dxz = (float)Math.Cos(latitude);

                // Create a single ring of vertices at this latitude.
                for (int j = 0; j < horizontalSegments; j++)
                {
                    float longitude = j * (float)(Math.PI * 2.0) / horizontalSegments;

                    float dx = (float)Math.Cos(longitude) * dxz;
                    float dz = (float)Math.Sin(longitude) * dxz;

                    Vector3 normal = new Vector3(dx, dy, dz);

                    vertices.Add(normal * boundingSphere.Radius + boundingSphere.Center);
                    currentVertex++;
                }
            }

            // Finish with a single vertex at the top of the sphere.
            vertices.Add(Vector3.UnitY * boundingSphere.Radius + boundingSphere.Center);

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; ++i)
            {
                indices2.Add(0);
                indices2.Add((ushort)(1 + (i + 1) % horizontalSegments));
                indices2.Add((ushort)(1 + i));
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; ++i)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    indices2.Add((ushort)(1 + i * horizontalSegments + j));
                    indices2.Add((ushort)(1 + i * horizontalSegments + nextJ));
                    indices2.Add((ushort)(1 + nextI * horizontalSegments + j));

                    indices2.Add((ushort)(1 + i * horizontalSegments + nextJ));
                    indices2.Add((ushort)(1 + nextI * horizontalSegments + nextJ));
                    indices2.Add((ushort)(1 + nextI * horizontalSegments + j));
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; ++i)
            {
                indices2.Add((ushort)(currentVertex - 1));
                indices2.Add((ushort)(currentVertex - 2 - (i + 1) % horizontalSegments));
                indices2.Add((ushort)(currentVertex - 2 - i));
            }

            points = vertices.ToArray();
            indices = indices2.ToArray();
        }

        /// <remarks>
        /// Primitive Type: Line List
        /// </remarks>
        public static void CreateFrustum(this BoundingFrustum boundingFrustum, out Vector3[] points, out ushort[] indices)
        {
            points = boundingFrustum.GetCorners();
            indices = new ushort[]
            {
                0,1,  1,2,  2,3,  3,0, // near plane
                0,4,  1,5,  2,6,  3,7, // connections
                4,5,  5,6,  6,7,  7,4, // far plane
            };
        }

        /// <remarks>
        /// Primitive Type: Triangle List
        /// </remarks>
        public static void CreateSolidFrustum(this BoundingFrustum boundingFrustum, out Vector3[] points, out ushort[] indices)
        {
            points = boundingFrustum.GetCorners();
            indices = BoundingBox.TriangleIndices;
        }

        /// <remarks>
        /// Primitive Type: Line List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateCone(Vector3 position, float height, float radius, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            var vertices = new List<Vector3>();
            var indices2 = new List<ushort>();

            vertices.Add(position + Vector3.UnitY * height);

            for (int i = 0; i < tessellation; ++i)
            {
                float angle = i * (float)(Math.PI * 2) / tessellation;

                float dx = (float)Math.Cos(angle);
                float dz = (float)Math.Sin(angle);

                Vector3 normal = new Vector3(dx, 0, dz);

                vertices.Add(position + normal * radius);

                indices2.Add(0);
                indices2.Add((ushort)(1 + i));

                indices2.Add((ushort)(1 + i));
                indices2.Add((ushort)(1 + (i + 1) % tessellation));
            }

            points = vertices.ToArray();
            indices = indices2.ToArray();
        }

        /// <remarks>
        /// Primitive Type: Triangle List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateSolidCone(Vector3 position, float height, float radius, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            var vertices = new List<Vector3>();
            var indices2 = new List<ushort>();

            vertices.Add(position + Vector3.UnitY * height);

            for (int i = 0; i < tessellation; ++i)
            {
                float angle = i * (float)(Math.PI * 2.0) / tessellation;

                float dx = (float)Math.Cos(angle);
                float dz = (float)Math.Sin(angle);

                Vector3 normal = new Vector3(dx, 0, dz);

                vertices.Add(position + normal * radius);

                indices2.Add(0);
                indices2.Add((ushort)(1 + i));
                indices2.Add((ushort)(2 + (i + 1) % tessellation));

                indices2.Add((ushort)(1 + i));
                indices2.Add((ushort)(1 + (i + 1) % tessellation));
                indices2.Add((ushort)(2 + i));
            }

            points = vertices.ToArray();
            indices = indices2.ToArray();
        }

        /// <remarks>
        /// Primitive Type: Line List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateCylinder(Vector3 position, float height, float radius, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            var vertices = new List<Vector3>();
            var indices2 = new List<ushort>();

            vertices.Add(position + Vector3.UnitY * height);
            vertices.Add(position);

            for (int i = 0; i < tessellation; ++i)
            {
                var angle = i * (float)(Math.PI * 2.0) / tessellation;

                var dx = (float)Math.Cos(angle);
                var dz = (float)Math.Sin(angle);

                var normal = new Vector3(dx, 0, dz);

                vertices.Add(position + normal * radius + Vector3.UnitY * height);
                vertices.Add(position + normal * radius);

                indices2.Add((ushort)(2 + i * 2));
                indices2.Add((ushort)(3 + i * 2));

                indices2.Add((ushort)(2 + i * 2));
                indices2.Add((ushort)(2 + ((i + 1) % tessellation) * 2));

                indices2.Add((ushort)(3 + i * 2));
                indices2.Add((ushort)(3 + ((i + 1) % tessellation) * 2));
            }

            points = vertices.ToArray();
            indices = indices2.ToArray();
        }

        /// <remarks>
        /// Primitive Type: Triangle List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateSolidCylinder(Vector3 position, float height, float radius, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            var vertices = new List<Vector3>();
            var indices2 = new List<ushort>();

            vertices.Add(position + Vector3.UnitY * height);
            vertices.Add(position);

            for (int i = 0; i < tessellation; ++i)
            {
                var angle = i * (float)(Math.PI * 2.0) / tessellation;

                var dx = (float)Math.Cos(angle);
                var dz = (float)Math.Sin(angle);

                var normal = new Vector3(dx, 0, dz);

                vertices.Add(position + normal * radius + Vector3.UnitY * height);
                vertices.Add(position + normal * radius);
                
                indices2.Add((ushort)(2 + i * 2));
                indices2.Add((ushort)(2 + (i * 2 + 2) % (tessellation * 2)));
                indices2.Add((ushort)(2 + i * 2 + 1));

                indices2.Add((ushort)(2 + i * 2 + 1));
                indices2.Add((ushort)(2 + (i * 2 + 2) % (tessellation * 2)));
                indices2.Add((ushort)(2 + (i * 2 + 3) % (tessellation * 2)));

                indices2.Add(0);
                indices2.Add((ushort)(2 + (i * 2 + 2) % (tessellation * 2)));
                indices2.Add((ushort)(2 + i * 2));

                indices2.Add(1);
                indices2.Add((ushort)(2 + i * 2 + 1));
                indices2.Add((ushort)(2 + (i * 2 + 3) % (tessellation * 2)));
            }

            points = vertices.ToArray();
            indices = indices2.ToArray();
        }
    }
}
