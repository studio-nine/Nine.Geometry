namespace Nine.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;

    /// <summary>
    /// 
    /// </summary>
    public struct BoundingCircle : IEquatable<BoundingCircle>
    {
        /// <summary> Gets or sets the center point. </summary>
        public Vector2 Center;

        /// <summary> Gets or sets the radius. </summary>
        public float Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingCircle"/> class.
        /// </summary>
        public BoundingCircle(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }
        
        public ContainmentType Contains(Vector2 point)
        {
            throw new NotImplementedException();
        }

        public void Contains(ref BoundingCircle boundingCircle, out ContainmentType result)
        {
            result = this.Contains(boundingCircle);
        }

        public ContainmentType Contains(BoundingCircle boundingCircle)
        {
            throw new NotImplementedException();
        }

        public void Contains(ref BoundingRectangle boundingRectangle, out ContainmentType result)
        {
            result = this.Contains(boundingRectangle);
        }

        public ContainmentType Contains(BoundingRectangle boundingRectangle)
        {
            throw new NotImplementedException();
        }

        public bool Intersects(BoundingCircle boundingCircle)
        {
            throw new NotImplementedException();
        }

        public void Intersects(ref BoundingCircle boundingCircle, out bool result)
        {
            result = this.Intersects(boundingCircle);
        }
        
        public bool Intersects(BoundingRectangle boundingRectangle)
        {
            throw new NotImplementedException();
        }

        public void Intersects(ref BoundingRectangle boundingRectangle, out bool result)
        {
            result = this.Intersects(boundingRectangle);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingRectangle"/> that will contain a group of points.
        /// </summary>
        public static BoundingCircle CreateFromPoints(IEnumerable<Vector2> points)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the merged <see cref="BoundingCircle"/>.
        /// </summary>        
        public static void CreateMerged(ref BoundingCircle original, ref BoundingCircle additional, out BoundingCircle result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates the merged <see cref="BoundingCircle"/>.
        /// </summary>
        public static BoundingCircle CreateMerged(BoundingCircle original, BoundingCircle additional)
        {
            BoundingCircle result;
            BoundingCircle.CreateMerged(ref original, ref additional, out result);
            return result;
        }

        /// <inheritdoc />
        public bool Equals(BoundingCircle other) => (this.Center == other.Center) && (this.Radius == other.Radius);

        /// <inheritdoc />
        public override bool Equals(object obj) => (obj is BoundingCircle) && this.Equals((BoundingCircle)obj);

        /// <inheritdoc />
        public override int GetHashCode() => this.Center.GetHashCode() ^ this.Radius.GetHashCode();
        
        /// <inheritdoc />
        public override string ToString() => "Center: " + this.Center + ", Radius: " + this.Radius;
    }
}
