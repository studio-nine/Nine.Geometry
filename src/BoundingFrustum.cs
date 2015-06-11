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
        public Plane Near => this.planes[0];

        /// <summary> Gets the far plane. </summary>
        public Plane Far => this.planes[1];

        /// <summary> Gets the left plane. </summary>
        public Plane Left => this.planes[2];

        /// <summary> Gets the right plane. </summary>
        public Plane Right => this.planes[3];

        /// <summary> Gets the top plane. </summary>
        public Plane Top => this.planes[4];

        /// <summary> Gets the bottom plane. </summary>
        public Plane Bottom => this.planes[5];

        /// <summary>
        /// Gets or sets the <see cref="Matrix4x4"/> that describes this <see cref="BoundingFrustum"/>.
        /// </summary>
        public Matrix4x4 Matrix
        {
            get { return matrix; }
            set
            {
                this.matrix = value;
                this.OnMatrixChanged();
            }
        }
        private Matrix4x4 matrix;

        private readonly Plane[] planes;
        private readonly Vector3[] corners;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingFrustum"/> class.
        /// </summary>
        public BoundingFrustum(Matrix4x4 matrix)
        {
            this.matrix = matrix;

            this.planes = new Plane[PlaneCount];
            this.corners = new Vector3[CornerCount];
        }

        private void OnMatrixChanged()
        {
            // TODO: Redesign this

            // Create the planes
            this.planes[0] = new Plane(-this.matrix.M13, -this.matrix.M23, -this.matrix.M33, -this.matrix.M43);
            this.planes[1] = new Plane(this.matrix.M13 - this.matrix.M14, this.matrix.M23 - this.matrix.M24, this.matrix.M33 - this.matrix.M34, this.matrix.M43 - this.matrix.M44);
            this.planes[2] = new Plane(-this.matrix.M14 - this.matrix.M11, -this.matrix.M24 - this.matrix.M21, -this.matrix.M34 - this.matrix.M31, -this.matrix.M44 - this.matrix.M41);
            this.planes[3] = new Plane(this.matrix.M11 - this.matrix.M14, this.matrix.M21 - this.matrix.M24, this.matrix.M31 - this.matrix.M34, this.matrix.M41 - this.matrix.M44);
            this.planes[4] = new Plane(this.matrix.M12 - this.matrix.M14, this.matrix.M22 - this.matrix.M24, this.matrix.M32 - this.matrix.M34, this.matrix.M42 - this.matrix.M44);
            this.planes[5] = new Plane(-this.matrix.M14 - this.matrix.M12, -this.matrix.M24 - this.matrix.M22, -this.matrix.M34 - this.matrix.M32, -this.matrix.M44 - this.matrix.M42);

            // Normalize all the planes
            for (int i = 0; i < this.planes.Length; i++)
                this.planes[i] = Plane.Normalize(this.planes[i]);

            // Create the corners
            IntersectionPoint(ref this.planes[0], ref this.planes[2], ref this.planes[4], out this.corners[0]);
            IntersectionPoint(ref this.planes[0], ref this.planes[3], ref this.planes[4], out this.corners[1]);
            IntersectionPoint(ref this.planes[0], ref this.planes[3], ref this.planes[5], out this.corners[2]);
            IntersectionPoint(ref this.planes[0], ref this.planes[2], ref this.planes[5], out this.corners[3]);
            IntersectionPoint(ref this.planes[1], ref this.planes[2], ref this.planes[4], out this.corners[4]);
            IntersectionPoint(ref this.planes[1], ref this.planes[3], ref this.planes[4], out this.corners[5]);
            IntersectionPoint(ref this.planes[1], ref this.planes[3], ref this.planes[5], out this.corners[6]);
            IntersectionPoint(ref this.planes[1], ref this.planes[2], ref this.planes[5], out this.corners[7]);
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

        public ContainmentType Contains(Plane plane)
        {
            var me = this;
            ContainmentType result;
            Intersection.Contains(ref me, ref plane, out result);
            return result;
        }

        public ContainmentType Contains(Vector3 vector)
        {
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
            Intersection.Intersect(ref me, ref boundingfrustum, out result);
            return result;
        }

        public bool Intersects(BoundingBox boundingBox)
        {
            var me = this;
            bool result;
            Intersection.Intersect(ref me, ref boundingBox, out result);
            return result;
        }

        public bool Intersects(BoundingSphere boundingSphere)
        {
            var me = this;
            bool result;
            Intersection.Intersect(ref me, ref boundingSphere, out result);
            return result;
        }

        public bool Intersects(Plane plane)
        {
            var me = this;
            bool result;
            Intersection.Intersect(ref me, ref plane, out result);
            return result;
        }

        public float? Intersects(Ray ray)
        {
            var me = this;
            float? result;
            Intersection.Intersect(ref ray, ref me, out result);
            return result;
        }
        
        public void GetCorners(ref Vector3[] result) => result = this.GetCorners();
        public Vector3[] GetCorners() => corners;

        public static bool operator ==(BoundingFrustum left, BoundingFrustum right) => (left.Matrix == right.Matrix);
        public static bool operator !=(BoundingFrustum left, BoundingFrustum right) => (left.Matrix != right.Matrix);

        /// <inheritdoc />
        public bool Equals(BoundingFrustum other) => (this.Matrix == other.Matrix);

        /// <inheritdoc />
        public override bool Equals(object obj) => (obj is BoundingFrustum) && this.Equals((BoundingFrustum)obj);

        /// <inheritdoc />
        public override int GetHashCode() => this.matrix.GetHashCode();
        
        /// <inheritdoc />
        public override string ToString() => 
            string.Format("Near: {0}, Far: {1}, Left: {2}, Right: {3}, Top: {4}, Bottom: {5}",
                this.planes[0], this.planes[1], this.planes[2],
                this.planes[3], this.planes[4], this.planes[5]);

        private static void IntersectionPoint(ref Plane a, ref Plane b, ref Plane c, out Vector3 result)
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

            result.X = (v1.X + v2.X + v3.X) / f;
            result.Y = (v1.Y + v2.Y + v3.Y) / f;
            result.Z = (v1.Z + v2.Z + v3.Z) / f;
        }
    }
}
