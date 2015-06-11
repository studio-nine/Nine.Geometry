namespace Nine.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    /// <summary>
    /// Defines a sphere.
    /// </summary>
    public struct BoundingSphere : IEquatable<BoundingSphere>
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

        public void Contains(ref BoundingBox boundingBox, out ContainmentType result)
        {
            result = this.Contains(boundingBox);
        }

        public ContainmentType Contains(BoundingBox boundingBox)
        {
            ContainmentType result;
            Intersection.Intersect(ref boundingBox, ref this, out result);
            return result;
        }

        public void Contains(ref BoundingFrustum boundingfrustum, out ContainmentType result)
        {
            result = this.Contains(boundingfrustum);
        }

        public ContainmentType Contains(BoundingFrustum boundingfrustum)
        {
            return Intersection.Intersect(boundingfrustum, this);
        }

        public void Contains(ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            result = this.Contains(boundingSphere);
        }

        public ContainmentType Contains(BoundingSphere boundingSphere)
        {
            ContainmentType result;
            Intersection.Intersect(ref this, ref boundingSphere, out result);
            return result;
        }

        public void Contains(ref Vector3 vector, out ContainmentType result)
        {
            result = this.Contains(vector);
        }

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

        public void Intersects(ref BoundingBox boundingBox, out bool result)
        {
            result = this.Intersects(boundingBox);
        }

        public bool Intersects(BoundingBox boundingBox)
        {
            ContainmentType result;
            Intersection.Intersect(ref boundingBox, ref this, out result);
            return Geometry.DoesIntersect(result);
        }

        public void Intersects(ref BoundingFrustum boundingfrustum, out bool result)
        {
            result = this.Intersects(boundingfrustum);
        }

        public bool Intersects(BoundingFrustum boundingfrustum)
        {
            ContainmentType result = Intersection.Intersect(boundingfrustum, this);
            return Geometry.DoesIntersect(result);
        }

        public void Intersects(ref BoundingSphere boundingSphere, out bool result)
        {
            result = this.Intersects(boundingSphere);
        }

        public bool Intersects(BoundingSphere boundingSphere)
        {
            ContainmentType result;
            Intersection.Intersect(ref boundingSphere, ref this, out result);
            return Geometry.DoesIntersect(result);
        }

        public void Intersects(ref Plane plane, out bool result)
        {
            result = this.Intersects(plane);
        }

        public bool Intersects(Plane plane)
        {
            ContainmentType result;
            Intersection.Intersect(ref plane, ref this, out result);
            return Geometry.DoesIntersect(result);
        }

        public void Intersects(ref Ray ray, out float? result)
        {
            result = this.Intersects(ray);
        }

        public float? Intersects(Ray ray)
        {
            float? result;
            Intersection.Intersect(ref ray, ref this, out result);
            return result;
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
            ushort[] indices;

            Geometry.CreateFrustum(boundingFrustum, out triangles, out indices);

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
        public static void CreateMerged(BoundingSphere original, BoundingSphere additional, out BoundingSphere result)
            => result = BoundingSphere.CreateMerged(original, additional);

        /// <summary>
        /// Creates the smallest <see cref="BoundingSphere"/> that contains the two <see cref="BoundingSphere"/>s.
        /// </summary>
        public static BoundingSphere CreateMerged(BoundingSphere original, BoundingSphere additional)
        {
            var difference = Vector3.Subtract(additional.Center, original.Center);
            var distance = difference.Length();
            if (distance <= (original.Radius + additional.Radius))
            {
                if (distance <= (original.Radius - additional.Radius))
                    return original;
                
                if (distance <= additional.Radius - original.Radius)
                    return additional;
            }

            var radius1 = Math.Max(original.Radius - distance, additional.Radius);
            var radius2 = Math.Max(original.Radius + distance, additional.Radius);

            difference += (((radius1 - radius2) / (2 * difference.Length())) * difference);

            var result = new BoundingSphere();
            result.Center = original.Center + difference;
            result.Radius = (radius1 + radius2) / 2;
            return result;
        }

        public static bool operator ==(BoundingSphere left, BoundingSphere right) => (left.Center == right.Center) && (left.Radius == right.Radius);
        public static bool operator !=(BoundingSphere left, BoundingSphere right) => (left.Center != right.Center) && (left.Radius != right.Radius);

        /// <inheritdoc />
        public bool Equals(BoundingSphere other) => (this.Center == other.Center) && (this.Radius == other.Radius);

        /// <inheritdoc />
        public override bool Equals(object obj) => (obj is BoundingSphere) && this.Equals((BoundingSphere)obj);

        /// <inheritdoc />
        public override int GetHashCode() => this.Center.GetHashCode() ^ this.Radius.GetHashCode();
        
        /// <inheritdoc />
        public override string ToString() => "Center: " + this.Center + ", Radius: " + this.Radius;
    }
}
