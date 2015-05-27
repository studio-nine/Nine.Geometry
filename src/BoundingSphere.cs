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
            result = this.Transform(transform);
        }

        /// <summary>
        /// Creates a new <see cref="BoundingSphere"/> that is the transformed by the input <see cref="Matrix4x4"/>.
        /// </summary>
        public BoundingSphere Transform(Matrix4x4 transform)
        {
            BoundingSphere result;
            result.Center = Vector3.Transform(this.Center, transform);
            result.Radius = this.Radius;
            return result;
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public void Contains(ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            result = this.Contains(boundingSphere);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public ContainmentType Contains(BoundingSphere boundingSphere)
        {
            var distance = Vector3.DistanceSquared(boundingSphere.Center, this.Center);
            if (distance > (boundingSphere.Radius + Radius) * (boundingSphere.Radius + Radius))
            {
                return ContainmentType.Disjoint;
            }
            else if (distance <= (Radius - boundingSphere.Radius) * (Radius - boundingSphere.Radius))
            {
                return ContainmentType.Contains;
            }
            else
            {
                return ContainmentType.Intersects;
            }
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="Vector3"/>.
        /// </summary>
        public void Contains(ref Vector3 vector, out ContainmentType result)
        {
            result = this.Contains(vector);
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
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public void Contains(ref BoundingFrustum boundingfrustum, out ContainmentType result)
        {
            result = this.Contains(boundingfrustum);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public ContainmentType Contains(BoundingFrustum boundingfrustum)
        {
            // TODO: BoundingSphere contains BoundingFrustum
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public void Contains(ref BoundingBox boundingBox, out ContainmentType result)
        {
            result = this.Contains(boundingBox);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public ContainmentType Contains(BoundingBox boundingBox)
        {
            Vector3[] triangles;
            boundingBox.GetTriangles(out triangles);
            foreach (var corner in triangles)
            {
                if (this.Contains(corner) == ContainmentType.Disjoint)
                {
                    return ContainmentType.Contains;
                }
            }

            var min = 0.0;

            if (this.Center.X < boundingBox.Min.X)      min += (this.Center.X - boundingBox.Min.X) * (this.Center.X - boundingBox.Min.X);
            else if (this.Center.X > boundingBox.Max.X) min += (this.Center.X - boundingBox.Max.X) * (this.Center.X - boundingBox.Max.X);

            if (this.Center.Y < boundingBox.Min.Y)      min += (this.Center.Y - boundingBox.Min.Y) * (this.Center.Y - boundingBox.Min.Y);
            else if (this.Center.Y > boundingBox.Max.Y) min += (this.Center.Y - boundingBox.Max.Y) * (this.Center.Y - boundingBox.Max.Y);

            if (this.Center.Z < boundingBox.Min.Z)      min += (this.Center.Z - boundingBox.Min.Z) * (this.Center.Z - boundingBox.Min.Z);
            else if (this.Center.Z > boundingBox.Max.Z) min += (this.Center.Z - boundingBox.Max.Z) * (this.Center.Z - boundingBox.Max.Z);

            if (min <= Radius * Radius)
            {
                return ContainmentType.Intersects;
            }
            
            return ContainmentType.Disjoint;
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
            // TODO: BoundingSphere intersect BoundingBox
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
            // TODO: BoundingSphere intersect BoundingFrustum
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
            return this.Contains(boundingSphere) == ContainmentType.Intersects;
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public void Intersects(ref Plane plane, out PlaneIntersectionType result)
        {
            // TODO: BoundingSphere intersect Plane
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

        /// <inheritdoc />
        public void GetTriangles(out Vector3[] vertices)
        {
            // TODO: GetTriangles
            throw new NotImplementedException();
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
