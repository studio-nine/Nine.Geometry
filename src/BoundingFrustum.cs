namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Defines a frustum and helps determine whether forms intersect with it.
    /// </summary>
    public class BoundingFrustum : IEquatable<BoundingFrustum>, IGeometryShape, IFormattable
    {
        /// <summary> Specifies the total number of planes (6) in the <see cref="BoundingFrustum"/>. </summary>
        public const int PlaneCount = 6;

        /// <summary> Specifies the total number of corners (8) in the <see cref="BoundingFrustum"/>. </summary>
        public const int CornerCount = 8;

        /// <summary> Gets the near plane. </summary>
        public Plane Near { get { return this.planes[0]; } }

        /// <summary> Gets the far plane. </summary>
        public Plane Far { get { return this.planes[1]; } }

        /// <summary> Gets the left plane. </summary>
        public Plane Left { get { return this.planes[2]; } }

        /// <summary> Gets the right plane. </summary>
        public Plane Right { get { return this.planes[3]; } }

        /// <summary> Gets the top plane. </summary>
        public Plane Top { get { return this.planes[4]; } }

        /// <summary> Gets the bottom plane. </summary>
        public Plane Bottom { get { return this.planes[5]; } }

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

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public void Contains(ref BoundingBox boundingBox, out ContainmentType result)
        {
            result = this.Contains(boundingBox);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public ContainmentType Contains(BoundingBox boundingBox)
        {
            // TODO: BoundingFrustum contains BoundingBox
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public void Contains(ref BoundingFrustum boundingfrustum, out ContainmentType result)
        {
            result = this.Contains(boundingfrustum);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public ContainmentType Contains(BoundingFrustum boundingfrustum)
        {
            // TODO: BoundingFrustum contains BoundingFrustum
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public void Contains(ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            result = this.Contains(boundingSphere);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public ContainmentType Contains(BoundingSphere boundingSphere)
        {
            // TODO: BoundingFrustum contains BoundingSphere
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="Vector3"/>.
        /// </summary>
        public void Contains(ref Vector3 vector, out ContainmentType result)
        {
            result = this.Contains(vector);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="Vector3"/>.
        /// </summary>
        public ContainmentType Contains(Vector3 vector)
        {
            // TODO: BoundingFrustum contains Vector3
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public void Intersects(ref BoundingBox boundingBox, out bool result)
        {
            result = this.Intersects(boundingBox);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public bool Intersects(BoundingBox boundingBox)
        {
            // TODO: BoundingFrustum intersect BoundingBox
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public void Intersects(ref BoundingFrustum boundingfrustum, out bool result)
        {
            result = this.Intersects(boundingfrustum);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public bool Intersects(BoundingFrustum boundingfrustum)
        {
            // TODO: BoundingFrustum intersect BoundingFrustum
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public void Intersects(ref BoundingSphere boundingSphere, out bool result)
        {
            result = this.Intersects(boundingSphere);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public bool Intersects(BoundingSphere boundingSphere)
        {
            // TODO: BoundingFrustum intersect BoundingSphere
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            // TODO: BoundingFrustum intersect Plane
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public bool Intersects(Plane plane)
        {
            PlaneIntersectionType result;
            this.Intersects(ref plane, out result);
            return result == PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public void Intersects(ref Ray ray, out float? result)
        {
            result = this.Intersects(ray);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public float? Intersects(Ray ray)
        {
            return ray.Intersects(this);
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

        /// <inheritdoc />
        public void GetTriangles(out Vector3[] vertices)
        {
            // TODO: GetTriangles
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given <see cref="BoundingFrustum"/>s are equal.
        /// </summary>
        public static bool operator ==(BoundingFrustum left, BoundingFrustum right)
        {
            return (left.Matrix == right.Matrix);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given <see cref="BoundingFrustum"/>s are not equal.
        /// </summary>
        public static bool operator !=(BoundingFrustum left, BoundingFrustum right)
        {
            return (left.Matrix != right.Matrix);
        }

        /// <inheritdoc />
        public bool Equals(BoundingFrustum other)
        {
            return (this.Matrix == other.Matrix);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return (obj is BoundingFrustum) && this.Equals((BoundingFrustum)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.matrix.GetHashCode();
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            // TODO: This should use the planes instead of the matrix.
            return string.Format(formatProvider, format ?? "Matrix: {0}", this.matrix);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToString(null, System.Globalization.CultureInfo.CurrentCulture);
        }

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
