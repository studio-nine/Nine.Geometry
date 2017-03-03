namespace Nine.Geometry
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Numerics;

    /// <summary>
    /// Defines a sphere.
    /// </summary>
    public struct BoundingSphere : IEquatable<BoundingSphere>
    {
        /// <summary> 
        /// Gets or sets the center point. 
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// Gets or sets the radius. 
        /// </summary>
        public float Radius;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingSphere"/> class.
        /// </summary>
        /// <param name="center">sphere center position.</param>
        /// <param name="radius">sphere radius.</param>
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
        
        public ContainmentType Contains(BoundingBox boundingBox)
        {
            ContainmentType result;
            Intersection.Contains(ref boundingBox, ref this, out result);
            return result;
        }
        
        public ContainmentType Contains(BoundingFrustum boundingfrustum)
        {
            ContainmentType result;
            Intersection.Contains(ref boundingfrustum, ref this, out result);
            return result;
        }
        
        public ContainmentType Contains(BoundingSphere boundingSphere)
        {
            ContainmentType result;
            Intersection.Contains(ref this, ref boundingSphere, out result);
            return result;
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
        
        public bool Intersects(BoundingBox boundingBox)
        {
            bool result;
            Intersection.Intersects(ref this, ref boundingBox, out result);
            return result;
        }
        
        public bool Intersects(BoundingFrustum boundingfrustum)
        {
            bool result;
            Intersection.Intersects(ref this, ref boundingfrustum, out result);
            return result;
        }
        
        public bool Intersects(BoundingSphere boundingSphere)
        {
            bool result;
            Intersection.Intersects(ref this, ref boundingSphere, out result);
            return result;
        }
        
        public PlaneIntersectionType Intersects(Plane plane)
        {
            PlaneIntersectionType result;
            Intersection.Intersects(ref plane, ref this, out result);
            return result;
        }
        
        public float? Intersects(Ray ray)
        {
            float? result;
            Intersection.Intersects(ref ray, ref this, out result);
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
            if (points == null) throw new ArgumentNullException(nameof(points));
            if (points.Count() == 0) throw new ArgumentException("You must have at least one point in points.");
            
            var minx =  new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxx = -new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            var miny =  new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxy = -new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            var minz =  new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            var maxz = -new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

            foreach (var point in points)
            {
                if (point.X < minx.X) minx = point;
                if (point.X > maxx.X) maxx = point;
                if (point.Y < miny.Y) miny = point;
                if (point.Y > maxy.Y) maxy = point;
                if (point.Z < minz.Z) minz = point;
                if (point.Z > maxz.Z) maxz = point;
            }
            
            var distX = Vector3.DistanceSquared(maxx, minx);
            var distY = Vector3.DistanceSquared(maxy, miny);
            var distZ = Vector3.DistanceSquared(maxz, minz);

            if (distY > distX && distY > distZ)
            {
                maxx = maxy;
                minx = miny;
            }

            if (distZ > distX && distZ > distY)
            {
                maxx = maxz;
                minx = minz;
            }

            var center = (minx + maxx) * 0.5f;
            var radius = Vector3.Distance(maxx, center);

            return new BoundingSphere(center, radius);
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
        public override string ToString() => $"<Center: { Center }, Radius: { Radius }>";
    }
}
