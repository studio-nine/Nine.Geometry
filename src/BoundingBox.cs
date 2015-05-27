namespace Nine.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    // TODO: Missing Methods

    /// <summary>
    /// Defines an axis-aligned box-shaped 3D volume.
    /// </summary>
    public struct BoundingBox : IEquatable<BoundingBox>, IGeometryShape, IFormattable
    {
        /// <summary> Gets or sets the minimum position. </summary>
        public Vector3 Min;

        /// <summary> Gets or sets the maximum position. </summary>
        public Vector3 Max;

        /// <summary>
        /// Gets the center.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                return new Vector3(
                    (this.Min.X + this.Max.X) / 2.0f,
                    (this.Min.Y + this.Max.Y) / 2.0f,
                    (this.Min.Z + this.Max.Z) / 2.0f);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="Vector3"/>.
        /// </summary>
        public ContainmentType Contains(Vector3 vector)
        {
            if (vector.X < this.Min.X || vector.X > this.Max.X || 
                vector.Y < this.Min.Y || vector.Y > this.Max.Y ||
                vector.Z < this.Min.Z || vector.Z > this.Max.Z)
            {
                return ContainmentType.Disjoint;
            }
            else if (
                vector.X == this.Min.X || vector.X == this.Max.X ||
                vector.Y == this.Min.Y || vector.Y == this.Max.Y ||
                vector.Z == this.Min.Z || vector.Z == this.Max.Z)
            {
                return ContainmentType.Intersects;
            }
            else
            {
                return ContainmentType.Contains;
            }
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public ContainmentType Intersects(BoundingBox boundingBox)
        {
            if (boundingBox.Max.X < this.Min.X || boundingBox.Min.X > this.Max.X ||
                boundingBox.Max.Y < this.Min.Y || boundingBox.Min.Y > this.Max.Y ||
                boundingBox.Max.Z < this.Min.Z || boundingBox.Min.Z > this.Max.Z)
            {
                return ContainmentType.Disjoint;
            }

            if (boundingBox.Min.X >= this.Min.X && boundingBox.Max.X <= this.Max.X &&
                boundingBox.Min.Y >= this.Min.Y && boundingBox.Max.Y <= this.Max.Y &&
                boundingBox.Min.Z >= this.Min.Z && boundingBox.Max.Z <= this.Max.Z)
            {
                return ContainmentType.Contains;
            }

            return ContainmentType.Intersects;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public ContainmentType Intersects(BoundingFrustum boundingfrustum)
        {
            ContainmentType result;
            Intersection.Intersect(ref this, ref boundingfrustum, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public ContainmentType Intersects(BoundingSphere boundingSphere)
        {
            ContainmentType result;
            Intersection.Intersect(ref this, ref boundingSphere, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public ContainmentType Intersects(Plane plane)
        {
            ContainmentType result;
            Intersection.Intersect(ref plane, ref this, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public float? Intersects(Ray ray)
        {
            float? result;
            Intersection.Intersect(ref ray, ref this, out result);
            return result;
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingBox"/> that can contain a <see cref="BoundingSphere"/>.
        /// </summary>
        public static void CreateFromSphere(ref BoundingSphere sphere, out BoundingBox result)
        {
            result = BoundingBox.CreateFromSphere(sphere);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingBox"/> that can contain a <see cref="BoundingSphere"/>.
        /// </summary>
        public static BoundingBox CreateFromSphere(BoundingSphere sphere)
        {
            BoundingBox result;

            var radius = new Vector3(sphere.Radius);
            result.Min = sphere.Center - radius;
            result.Max = sphere.Center + radius;

            return result;
        }

        /// <summary>
        /// Creates a <see cref="BoundingBox"/> that can contain a list of <see cref="Vector3"/>.
        /// </summary>
        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> vectors)
        {
            if (vectors == null)
                throw new ArgumentNullException(nameof(vectors));

            var min = Vector3.One * float.MaxValue;
            var max = Vector3.One * float.MinValue;

            foreach (var vector in vectors)
            {
                if (vector.X < min.X) min.X = vector.X;
                if (vector.Y < min.Y) min.Y = vector.Y;
                if (vector.Z < min.Z) min.Z = vector.Z;

                if (vector.X < max.X) max.X = vector.X;
                if (vector.Y < max.Y) max.Y = vector.Y;
                if (vector.Z < max.Z) max.Z = vector.Z;
            }

            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingBox"/> that contains the two <see cref="BoundingBox"/>es.
        /// </summary>
        public static void CreateMerged(BoundingBox left, BoundingBox right, out BoundingBox result)
        {
            result = BoundingBox.CreateMerged(left, right);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingBox"/> that contains the two <see cref="BoundingBox"/>es.
        /// </summary>
        public static BoundingBox CreateMerged(BoundingBox left, BoundingBox right)
        {
            BoundingBox result;

            result.Min.X = Math.Min(left.Min.X, right.Min.X);
            result.Min.Y = Math.Min(left.Min.Y, right.Min.Y);
            result.Min.Z = Math.Min(left.Min.Z, right.Min.Z);

            result.Max.X = Math.Max(left.Max.X, right.Max.X);
            result.Max.Y = Math.Max(left.Max.Y, right.Max.Y);
            result.Max.Z = Math.Max(left.Max.Z, right.Max.Z);

            return result;
        }

        /// <inheritdoc />
        public void GetTriangles(out Vector3[] vertices)
        {
            // TODO: GetTriangles
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given <see cref="BoundingBox"/>es are equal.
        /// </summary>
        public static bool operator ==(BoundingBox left, BoundingBox right)
        {
            return (left.Min == right.Min) && (left.Max == right.Max);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given <see cref="BoundingBox"/>es are not equal.
        /// </summary>
        public static bool operator !=(BoundingBox left, BoundingBox right)
        {
            return (left.Min != right.Min) && (left.Max != right.Max);
        }

        /// <inheritdoc />
        public bool Equals(BoundingBox other)
        {
            return (this.Min == other.Min) && (this.Max == other.Max);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return (obj is BoundingBox) && this.Equals((BoundingBox)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Min.GetHashCode() ^ this.Max.GetHashCode();
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, format ?? "Min: {0}, Max: {1}", this.Min, this.Max);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToString(null, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
