namespace Nine.Geometry
{
    using System;
    using System.Numerics;
    
    /// <summary>
    /// Creates Geometry Shapes.
    /// </summary>
    public static class Geometry
    {
        /// <summary> Triangle Indices used by BoundingBox and BoundingFrustum. </summary>
        internal static readonly ushort[] TriangleIndices = new ushort[]
        {
            0,1,2,  3,0,2,
            4,6,5,  4,7,6,
            0,3,4,  4,3,7,
            5,1,0,  5,0,4,
            5,6,2,  5,2,1,
            3,2,6,  3,6,7,
        };
        
        public static Vector3 Up => Vector3.UnitY;
        public static Vector3 Down => -Vector3.UnitY;

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
            indices = TriangleIndices;
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

            points = new Vector3[(verticalSegments - 1) * horizontalSegments + 2];
            indices = new ushort[(horizontalSegments * 4) + ( ((verticalSegments - 2) * horizontalSegments) * 6 )];

            int pointsCurrentIndex = 0, indicesCurrentIndex = 0;

            // Start with a single vertex at the bottom of the sphere.
            points[pointsCurrentIndex++] = Down * boundingSphere.Radius + boundingSphere.Center;

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

                    points[pointsCurrentIndex++] = normal * boundingSphere.Radius + boundingSphere.Center;
                    currentVertex++;
                }
            }

            // Finish with a single vertex at the top of the sphere.
            points[pointsCurrentIndex++] = Up * boundingSphere.Radius + boundingSphere.Center;

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; ++i)
            {
                indices[indicesCurrentIndex++] = 0;
                indices[indicesCurrentIndex++] = (ushort)(1 + (i + 1) % horizontalSegments);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; ++i)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    indices[indicesCurrentIndex++] = (ushort)(1 + i * horizontalSegments + j);
                    indices[indicesCurrentIndex++] = (ushort)(1 + i * horizontalSegments + nextJ);

                    indices[indicesCurrentIndex++] = (ushort)(1 + i * horizontalSegments + nextJ);
                    indices[indicesCurrentIndex++] = (ushort)(1 + nextI * horizontalSegments + nextJ);

                    indices[indicesCurrentIndex++] = (ushort)(1 + nextI * horizontalSegments + j);
                    indices[indicesCurrentIndex++] = (ushort)(1 + nextI * horizontalSegments + j);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; ++i)
            {
                indices[indicesCurrentIndex++] = (ushort)(currentVertex - 1);
                indices[indicesCurrentIndex++] = (ushort)(currentVertex - 2 - (i + 1) % horizontalSegments);
            }
        }

        /// <remarks>
        /// Primitive Type: Triangle List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateSolidSphere(this BoundingSphere boundingSphere, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));
            
            int currentVertex = 2;
            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            points = new Vector3[(verticalSegments - 1) * horizontalSegments + 2];
            indices = new ushort[(horizontalSegments * 6) + (((verticalSegments - 2) * horizontalSegments) * 6)];

            int pointsCurrentIndex = 0, indicesCurrentIndex = 0;

            // Start with a single vertex at the bottom of the sphere.
            points[pointsCurrentIndex++] = Down * boundingSphere.Radius + boundingSphere.Center;

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

                    points[pointsCurrentIndex++] = normal * boundingSphere.Radius + boundingSphere.Center;
                    currentVertex++;
                }
            }

            // Finish with a single vertex at the top of the sphere.
            points[pointsCurrentIndex++] = Up * boundingSphere.Radius + boundingSphere.Center;

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for (int i = 0; i < horizontalSegments; ++i)
            {
                indices[indicesCurrentIndex++] = 0;
                indices[indicesCurrentIndex++] = (ushort)(1 + (i + 1) % horizontalSegments);
                indices[indicesCurrentIndex++] = (ushort)(1 + i);
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for (int i = 0; i < verticalSegments - 2; ++i)
            {
                for (int j = 0; j < horizontalSegments; j++)
                {
                    int nextI = i + 1;
                    int nextJ = (j + 1) % horizontalSegments;

                    indices[indicesCurrentIndex++] = (ushort)(1 + i * horizontalSegments + j);
                    indices[indicesCurrentIndex++] = (ushort)(1 + i * horizontalSegments + nextJ);
                    indices[indicesCurrentIndex++] = (ushort)(1 + nextI * horizontalSegments + j);

                    indices[indicesCurrentIndex++] = (ushort)(1 + i * horizontalSegments + nextJ);
                    indices[indicesCurrentIndex++] = (ushort)(1 + nextI * horizontalSegments + nextJ);
                    indices[indicesCurrentIndex++] = (ushort)(1 + nextI * horizontalSegments + j);
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for (int i = 0; i < horizontalSegments; ++i)
            {
                indices[indicesCurrentIndex++] = (ushort)(currentVertex - 1);
                indices[indicesCurrentIndex++] = (ushort)(currentVertex - 2 - (i + 1) % horizontalSegments);
                indices[indicesCurrentIndex++] = (ushort)(currentVertex - 2 - i);
            }
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
            indices = TriangleIndices;
        }

        /// <remarks>
        /// Primitive Type: Line List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateCone(Vector3 position, float height, float radius, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));
            
            points = new Vector3[tessellation + 1];
            indices = new ushort[tessellation * 4];

            int pointsCurrentIndex = 0, indicesCurrentIndex = 0;

            points[pointsCurrentIndex++] = position + Up * height;

            for (int i = 0; i < tessellation; ++i)
            {
                float angle = i * (float)(Math.PI * 2) / tessellation;

                float dx = (float)Math.Cos(angle);
                float dz = (float)Math.Sin(angle);

                Vector3 normal = new Vector3(dx, 0, dz);

                points[pointsCurrentIndex++] = position + normal * radius;

                indices[indicesCurrentIndex++] = 0;
                indices[indicesCurrentIndex++] = (ushort)(1 + i);

                indices[indicesCurrentIndex++] = (ushort)(1 + i);
                indices[indicesCurrentIndex++] = (ushort)(1 + (i + 1) % tessellation);
            }
        }

        /// <remarks>
        /// Primitive Type: Triangle List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateSolidCone(Vector3 position, float height, float radius, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));
            
            points = new Vector3[tessellation + 1];
            indices = new ushort[tessellation * 6];

            int pointsCurrentIndex = 0, indicesCurrentIndex = 0;

            points[pointsCurrentIndex++] = position + Up * height;

            for (int i = 0; i < tessellation; ++i)
            {
                float angle = i * (float)(Math.PI * 2.0) / tessellation;

                float dx = (float)Math.Cos(angle);
                float dz = (float)Math.Sin(angle);

                Vector3 normal = new Vector3(dx, 0, dz);

                points[pointsCurrentIndex++] = position + normal * radius;

                indices[indicesCurrentIndex++] = 0;
                indices[indicesCurrentIndex++] = (ushort)(1 + i);
                indices[indicesCurrentIndex++] = (ushort)(2 + (i + 1) % tessellation);

                indices[indicesCurrentIndex++] = (ushort)(1 + i);
                indices[indicesCurrentIndex++] = (ushort)(1 + (i + 1) % tessellation);
                indices[indicesCurrentIndex++] = (ushort)(2 + i);
            }
        }

        /// <remarks>
        /// Primitive Type: Line List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateCylinder(Vector3 position, float height, float radius, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            points = new Vector3[tessellation * 2 + 2];
            indices = new ushort[tessellation * 6];

            int pointsCurrentIndex = 0, indicesCurrentIndex = 0;

            points[pointsCurrentIndex++] = position + Up * height;
            points[pointsCurrentIndex++] = position;

            for (int i = 0; i < tessellation; ++i)
            {
                var angle = i * (float)(Math.PI * 2.0) / tessellation;

                var dx = (float)Math.Cos(angle);
                var dz = (float)Math.Sin(angle);

                var normal = new Vector3(dx, 0, dz);

                points[pointsCurrentIndex++] = position + normal * radius + Up * height;
                points[pointsCurrentIndex++] = position + normal * radius;

                indices[indicesCurrentIndex++] = (ushort)(2 + i * 2);
                indices[indicesCurrentIndex++] = (ushort)(3 + i * 2);

                indices[indicesCurrentIndex++] = (ushort)(2 + i * 2);
                indices[indicesCurrentIndex++] = (ushort)(2 + ((i + 1) % tessellation) * 2);

                indices[indicesCurrentIndex++] = (ushort)(3 + i * 2);
                indices[indicesCurrentIndex++] = (ushort)(3 + ((i + 1) % tessellation) * 2);
            }
        }

        /// <remarks>
        /// Primitive Type: Triangle List
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException" />
        public static void CreateSolidCylinder(Vector3 position, float height, float radius, int tessellation, out Vector3[] points, out ushort[] indices)
        {
            if (tessellation < 3)
                throw new ArgumentOutOfRangeException(nameof(tessellation));

            points = new Vector3[tessellation * 2 + 2];
            indices = new ushort[tessellation * 12];

            int pointsCurrentIndex = 0, indicesCurrentIndex = 0;

            points[pointsCurrentIndex++] = position + Up * height;
            points[pointsCurrentIndex++] = position;

            for (int i = 0; i < tessellation; ++i)
            {
                var angle = i * (float)(Math.PI * 2.0) / tessellation;

                var dx = (float)Math.Cos(angle);
                var dz = (float)Math.Sin(angle);

                var normal = new Vector3(dx, 0, dz);

                points[pointsCurrentIndex++] = position + normal * radius + Up * height;
                points[pointsCurrentIndex++] = position + normal * radius;
                
                indices[indicesCurrentIndex++] = (ushort)(2 + i * 2);
                indices[indicesCurrentIndex++] = (ushort)(2 + (i * 2 + 2) % (tessellation * 2));
                indices[indicesCurrentIndex++] = (ushort)(2 + i * 2 + 1);

                indices[indicesCurrentIndex++] = (ushort)(2 + i * 2 + 1);
                indices[indicesCurrentIndex++] = (ushort)(2 + (i * 2 + 2) % (tessellation * 2));
                indices[indicesCurrentIndex++] = (ushort)(2 + (i * 2 + 3) % (tessellation * 2));

                indices[indicesCurrentIndex++] = 0;
                indices[indicesCurrentIndex++] = (ushort)(2 + (i * 2 + 2) % (tessellation * 2));
                indices[indicesCurrentIndex++] = (ushort)(2 + i * 2);

                indices[indicesCurrentIndex++] = 1;
                indices[indicesCurrentIndex++] = (ushort)(2 + i * 2 + 1);
                indices[indicesCurrentIndex++] = (ushort)(2 + (i * 2 + 3) % (tessellation * 2));
            }
        }
    }
}
