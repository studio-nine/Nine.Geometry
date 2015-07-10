namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Defines a frustum and helps determine whether forms intersect with it.
    /// </summary>
    public class BoundingFrustum : IEquatable<BoundingFrustum>
    {
        /// <summary> Specifies the total number of planes (6) in the <see cref="BoundingFrustum"/>. </summary>
        public const int PlaneCount = 6;

        /// <summary> Specifies the total number of corners (8) in the <see cref="BoundingFrustum"/>. </summary>
        public const int CornerCount = 8;

        /// <summary> Gets the near plane. </summary>
        public Plane Near => planes.Value[0];

        /// <summary> Gets the far plane. </summary>
        public Plane Far => planes.Value[1];

        /// <summary> Gets the left plane. </summary>
        public Plane Left => planes.Value[2];

        /// <summary> Gets the right plane. </summary>
        public Plane Right => planes.Value[3];

        /// <summary> Gets the top plane. </summary>
        public Plane Top => planes.Value[4];

        /// <summary> Gets the bottom plane. </summary>
        public Plane Bottom => planes.Value[5];

        /// <summary>
        /// Gets or sets the <see cref="Matrix4x4"/> that describes this <see cref="BoundingFrustum"/>.
        /// </summary>
        public readonly Matrix4x4 Matrix;

        private readonly Lazy<Plane[]> planes;
        private readonly Lazy<Vector3[]> corners;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingFrustum"/> class.
        /// </summary>
        public BoundingFrustum(Matrix4x4 matrix)
        {
            this.Matrix = matrix;
            this.planes = new Lazy<Plane[]>(CreatePlanes);
            this.corners = new Lazy<Vector3[]>(CreateCorners);
        }

        private Plane[] CreatePlanes()
        {
            return new[]
            {
                Plane.Normalize(new Plane(-Matrix.M13, -Matrix.M23, -Matrix.M33, -Matrix.M43)),
                Plane.Normalize(new Plane(Matrix.M13 - Matrix.M14, Matrix.M23 - Matrix.M24, Matrix.M33 - Matrix.M34, Matrix.M43 - Matrix.M44)),
                Plane.Normalize(new Plane(-Matrix.M14 - Matrix.M11, -Matrix.M24 - Matrix.M21, -Matrix.M34 - Matrix.M31, -Matrix.M44 - Matrix.M41)),
                Plane.Normalize(new Plane(Matrix.M11 - Matrix.M14, Matrix.M21 - Matrix.M24, Matrix.M31 - Matrix.M34, Matrix.M41 - Matrix.M44)),
                Plane.Normalize(new Plane(Matrix.M12 - Matrix.M14, Matrix.M22 - Matrix.M24, Matrix.M32 - Matrix.M34, Matrix.M42 - Matrix.M44)),
                Plane.Normalize(new Plane(-Matrix.M14 - Matrix.M12, -Matrix.M24 - Matrix.M22, -Matrix.M34 - Matrix.M32, -Matrix.M44 - Matrix.M42)),
            };
        }

        private Vector3[] CreateCorners()
        {
            var planes = this.planes.Value;

            return new[]
            {
                IntersectionPoint(ref planes[0], ref planes[2], ref planes[4]),
                IntersectionPoint(ref planes[0], ref planes[3], ref planes[4]),
                IntersectionPoint(ref planes[0], ref planes[3], ref planes[5]),
                IntersectionPoint(ref planes[0], ref planes[2], ref planes[5]),
                IntersectionPoint(ref planes[1], ref planes[2], ref planes[4]),
                IntersectionPoint(ref planes[1], ref planes[3], ref planes[4]),
                IntersectionPoint(ref planes[1], ref planes[3], ref planes[5]),
                IntersectionPoint(ref planes[1], ref planes[2], ref planes[5]),
            };
        }

        public ContainmentType Contains(BoundingFrustum boundingfrustum)
        {
            var me = this;
            ContainmentType result;
            Intersection.Contains(ref me, ref boundingfrustum, out result);
            return result;
        }

        public ContainmentType Contains(BoundingBox boundingBox)
        {
            var me = this;
            ContainmentType result;
            Intersection.Contains(ref me, ref boundingBox, out result);
            return result;
        }

        public ContainmentType Contains(BoundingSphere boundingSphere)
        {
            var me = this;
            ContainmentType result;
            Intersection.Contains(ref me, ref boundingSphere, out result);
            return result;
        }

        public PlaneIntersectionType Intersects(Plane plane)
        {
            var me = this;
            PlaneIntersectionType result;
            Intersection.Intersects(ref me, ref plane, out result);
            return result;
        }

        public ContainmentType Contains(Vector3 vector)
        {
            var planes = this.planes.Value;
            for (var i = 0; i < PlaneCount; ++i)
            {
                var value = vector * planes[i].Normal;
                if ((value.X + value.Y + value.Z + planes[i].D) > 0)
                {
                    return ContainmentType.Disjoint;
                }
            }

            return ContainmentType.Contains;
        }

        public bool Intersects(BoundingFrustum boundingfrustum)
        {
            var me = this;
            bool result;
            Intersection.Intersects(ref me, ref boundingfrustum, out result);
            return result;
        }

        public bool Intersects(BoundingBox boundingBox)
        {
            var me = this;
            bool result;
            Intersection.Intersects(ref me, ref boundingBox, out result);
            return result;
        }

        public bool Intersects(BoundingSphere boundingSphere)
        {
            var me = this;
            bool result;
            Intersection.Intersects(ref me, ref boundingSphere, out result);
            return result;
        }

        public float? Intersects(Ray ray)
        {
            var me = this;
            float? result;
            Intersection.Intersects(ref ray, ref me, out result);
            return result;
        }

        public void GetPlanes(ref Plane[] result) => result = GetPlanes();
        public Plane[] GetPlanes() => planes.Value;

        public void GetCorners(ref Vector3[] result) => result = GetCorners();
        public Vector3[] GetCorners() => corners.Value;

        public static bool operator ==(BoundingFrustum left, BoundingFrustum right) => (left.Matrix == right.Matrix);
        public static bool operator !=(BoundingFrustum left, BoundingFrustum right) => (left.Matrix != right.Matrix);

        /// <inheritdoc />
        public bool Equals(BoundingFrustum other) => (this.Matrix == other.Matrix);

        /// <inheritdoc />
        public override bool Equals(object obj) => (obj is BoundingFrustum) && this.Equals((BoundingFrustum)obj);

        /// <inheritdoc />
        public override int GetHashCode() => this.Matrix.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
        {
            var planes = this.planes.Value;
            return string.Format("<Near: {0}, Far: {1}, Left: {2}, Right: {3}, Top: {4}, Bottom: {5}>",
                planes[0], planes[1], planes[2],
                planes[3], planes[4], planes[5]);
        }

        private static Vector3 IntersectionPoint(ref Plane a, ref Plane b, ref Plane c)
        {
            // Formula used
            //                d1 ( N2 * N3 ) + d2 ( N3 * N1 ) + d3 ( N1 * N2 )
            //P =   -------------------------------------------------------------------------
            //                             N1 . ( N2 * N3 )
            //
            // Note: N refers to the normal, d refers to the displacement. '.' means dot product. '*' means cross product

            Vector3 v1, v2, v3;
            Vector3 cross = Vector3.Cross(b.Normal, c.Normal);

            var f = Vector3.Dot(a.Normal, cross);
            f *= -1.0f;

            cross = Vector3.Cross(b.Normal, c.Normal);
            v1 = Vector3.Multiply(cross, a.D);
            //v1 = (a.D * (Vector3.Cross(b.Normal, c.Normal)));

            cross = Vector3.Cross(c.Normal, a.Normal);
            v2 = Vector3.Multiply(cross, b.D);
            //v2 = (b.D * (Vector3.Cross(c.Normal, a.Normal)));

            cross = Vector3.Cross(a.Normal, b.Normal);
            v3 = Vector3.Multiply(cross, c.D);
            //v3 = (c.D * (Vector3.Cross(a.Normal, b.Normal)));

            return new Vector3(
                (v1.X + v2.X + v3.X) / f,
                (v1.Y + v2.Y + v3.Y) / f,
                (v1.Z + v2.Z + v3.Z) / f);
        }
    }
}
