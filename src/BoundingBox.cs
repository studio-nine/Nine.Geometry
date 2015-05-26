namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Defines an axis-aligned box-shaped 3D volume.
    /// </summary>
    public struct BoundingBox : IEquatable<BoundingBox>, IFormattable
    {
        /// <summary> Gets or sets the minimum position. </summary>
        public Vector3 Min;

        /// <summary> Gets or sets the maximum position. </summary>
        public Vector3 Max;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public void Contains(ref BoundingBox boundingBox, out ContainmentType result)
        {
            result = this.Contains(boundingBox);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public ContainmentType Contains(BoundingBox boundingBox)
        {
            // TODO: BoundingBox contains BoundingBox
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public void Contains(ref BoundingFrustum boundingfrustum, out ContainmentType result)
        {
            result = this.Contains(boundingfrustum);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public ContainmentType Contains(BoundingFrustum boundingfrustum)
        {
            // TODO: BoundingBox contains BoundingFrustum
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public void Contains(ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            result = this.Contains(boundingSphere);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public ContainmentType Contains(BoundingSphere boundingSphere)
        {
            // TODO: BoundingBox contains BoundingSphere
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="Vector3"/>.
        /// </summary>
        public void Contains(ref Vector3 vector, out ContainmentType result)
        {
            result = this.Contains(vector);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="Vector3"/>.
        /// </summary>
        public ContainmentType Contains(Vector3 vector)
        {
            // TODO: BoundingBox contains Vector3
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public void Intersects(ref BoundingBox boundingBox, out bool result)
        {
            result = this.Intersects(boundingBox);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public bool Intersects(BoundingBox boundingBox)
        {
            // TODO: BoundingBox intersect with BoundingBox
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public void Intersects(ref BoundingFrustum boundingfrustum, out bool result)
        {
            result = this.Intersects(boundingfrustum);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public bool Intersects(BoundingFrustum boundingfrustum)
        {
            // TODO: BoundingBox intersect with BoundingFrustum
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public void Intersects(ref BoundingSphere boundingSphere, out bool result)
        {
            result = this.Intersects(boundingSphere);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public bool Intersects(BoundingSphere boundingSphere)
        {
            // TODO: BoundingBox intersect with BoundingSphere
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public void Intersects(ref Plane plane, out float? result)
        {
            result = this.Intersects(plane);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public float? Intersects(Plane plane)
        {
            // TODO: BoundingBox intersect with Plane
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public void Intersects(ref Ray ray, out float? result)
        {
            result = this.Intersects(ray);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public float? Intersects(Ray ray)
        {
            // TODO: BoundingBox intersect with Ray
            throw new NotImplementedException();
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
            // TODO: BoundingBox Create Merged
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
