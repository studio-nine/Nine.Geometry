namespace Nine.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    /// <summary>
    /// Defines a sphere.
    /// </summary>
    public struct BoundingSphere : IEquatable<BoundingSphere>, IGeometryShape, IFormattable
    {
        /// <summary> Gets or sets the center point. </summary>
        public Vector3 Center;

        /// <summary> Gets or sets the radius. </summary>
        public float Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingSphere"/> class.
        /// </summary>
        public BoundingSphere(Vector3 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        /// <summary>
        /// Creates a new <see cref="BoundingSphere"/> that is the transformed by the input <see cref="Matrix4x4"/>.
        /// </summary>
        public void Transform(ref Matrix4x4 transform, out BoundingSphere result)
        {
            result.Center = Vector3.Transform(this.Center, transform);
            result.Radius = this.Radius;
        }

        /// <summary>
        /// Creates a new <see cref="BoundingSphere"/> that is the transformed by the input <see cref="Matrix4x4"/>.
        /// </summary>
        public BoundingSphere Transform(Matrix4x4 transform)
        {
            BoundingSphere result;
            this.Transform(ref transform, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="Vector3"/>.
        /// </summary>
        public ContainmentType Contains(Vector3 vector)
        {
            var radius2 = Radius * Radius;
            var distance = Vector3.DistanceSquared(vector, this.Center);

            if (distance > radius2)
            {
                return ContainmentType.Disjoint;
            }
            else if (distance < radius2)
            {
                return ContainmentType.Contains;
            }
            else
            {
                return ContainmentType.Intersects;
            }
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public ContainmentType Intersects(BoundingBox boundingBox)
        {
            ContainmentType result;
            Intersection.Intersect(ref boundingBox, ref this, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public ContainmentType Intersects(BoundingFrustum boundingfrustum)
        {
            ContainmentType result;
            Intersection.Intersect(ref boundingfrustum, ref this, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public ContainmentType Intersects(BoundingSphere boundingSphere)
        {
            ContainmentType result;
            Intersection.Intersect(ref boundingSphere, ref this, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public ContainmentType Intersects(Plane plane)
        {
            ContainmentType result;
            Intersection.Intersect(ref plane, ref this, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public float? Intersects(Ray ray)
        {
            float? result;
            Intersection.Intersect(ref ray, ref this, out result);
            return result;
        }

        /// <inheritdoc />
        public void GetTriangles(out Vector3[] vertices)
        {
            // TODO: How should I handle indices?
            ushort[] indices;
            Geometry.CreateSphere(this, 16, out vertices, out indices);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingSphere"/> that can contain a <see cref="BoundingBox"/>.
        /// </summary>
        public static void CreateFromBoundingBox(ref BoundingBox boundingBox, out BoundingSphere result)
        {
            result = BoundingSphere.CreateFromBoundingBox(boundingBox);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingSphere"/> that can contain a <see cref="BoundingBox"/>.
        /// </summary>
        public static BoundingSphere CreateFromBoundingBox(BoundingBox boundingBox)
        {
            var center = boundingBox.Center;
            var radius = Vector3.Distance(center, boundingBox.Max);
            return new BoundingSphere(center, radius);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingSphere"/> that can contain a <see cref="BoundingFrustum"/>.
        /// </summary>
        public static void CreateFromFrustum(ref BoundingFrustum boundingFrustum, out BoundingSphere result)
        {
            result = BoundingSphere.CreateFromFrustum(boundingFrustum);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingSphere"/> that can contain a <see cref="BoundingFrustum"/>.
        /// </summary>
        public static BoundingSphere CreateFromFrustum(BoundingFrustum boundingFrustum)
        {
            Vector3[] triangles;
            boundingFrustum.GetTriangles(out triangles);
            return BoundingSphere.CreateFromPoints(triangles);
        }

        /// <summary>
        /// Creates a <see cref="BoundingSphere"/> that can contain a list of <see cref="Vector3"/>.
        /// </summary>
        public static BoundingSphere CreateFromPoints(IEnumerable<Vector3> points)
        {
            // TODO: BoundingSphere CreateFromPoints
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingSphere"/> that contains the two <see cref="BoundingSphere"/>s.
        /// </summary>
        public static void CreateMerged(BoundingSphere left, BoundingSphere right, out BoundingSphere result)
        {
            result = BoundingSphere.CreateMerged(left, right);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingSphere"/> that contains the two <see cref="BoundingSphere"/>s.
        /// </summary>
        public static BoundingSphere CreateMerged(BoundingSphere left, BoundingSphere right)
        {
            // TODO: BoundingSphere CreateMerged
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given <see cref="BoundingSphere"/>s are equal.
        /// </summary>
        public static bool operator ==(BoundingSphere left, BoundingSphere right)
        {
            return (left.Center == right.Center) && (left.Radius == right.Radius);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given <see cref="BoundingSphere"/>s are not equal.
        /// </summary>
        public static bool operator !=(BoundingSphere left, BoundingSphere right)
        {
            return (left.Center != right.Center) && (left.Radius != right.Radius);
        }

        /// <inheritdoc />
        public bool Equals(BoundingSphere other)
        {
            return (this.Center == other.Center) && (this.Radius == other.Radius);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return (obj is BoundingSphere) && this.Equals((BoundingSphere)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Center.GetHashCode() ^ this.Radius.GetHashCode();
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, format ?? "Center: {0}, Radius: {1}", this.Center, this.Radius);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToString(null, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
