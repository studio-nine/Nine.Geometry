namespace Nine.Geometry
{
    using System;
    using System.Numerics;
    
    /// <summary>
    /// Defines a ray.
    /// </summary>
    public struct Ray : IEquatable<Ray>
    {
        /// <summary> 
        /// Gets or sets the position. 
        /// </summary>
        public Vector3 Position;

        /// <summary> 
        /// Gets or sets the direction. 
        /// </summary>
        public Vector3 Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ray"/> class.
        /// </summary>
        /// <param name="position">Ray start position.</param>
        /// <param name="direction">Ray direction.</param>
        public Ray(Vector3 position, Vector3 direction)
        {
            this.Position = position;
            this.Direction = direction;
        }

        /// <summary>
        /// Creates a new <see cref="Ray"/> that is the transformed by the input <see cref="Matrix4x4"/>.
        /// </summary>
        public void Transform(ref Matrix4x4 transform, out Ray result)
        {
            result.Position = Vector3.Transform(this.Position, transform);
            result.Direction = Vector3.TransformNormal(this.Direction, transform);
        }

        /// <summary>
        /// Creates a new <see cref="Ray"/> that is the transformed by the input <see cref="Matrix4x4"/>.
        /// </summary>
        public Ray Transform(Matrix4x4 transform)
        {
            Ray result;
            this.Transform(ref transform, out result);
            return result;
        }
        
        public float? Intersects(BoundingBox boundingBox)
        {
            float? result;
            Intersection.Intersects(ref this, ref boundingBox, out result);
            return result;
        }
        
        public float? Intersects(BoundingFrustum boundingfrustum)
        {
            float? result;
            Intersection.Intersects(ref this, ref boundingfrustum, out result);
            return result;
        }
        
        public float? Intersects(BoundingSphere boundingSphere)
        {
            float? result;
            Intersection.Intersects(ref this, ref boundingSphere, out result);
            return result;
        }
        
        public float? Intersects(Plane plane)
        {
            float? result;
            Intersection.Intersects(ref this, ref plane, out result);
            return result;
        }

        public float? Intersects(Triangle triangle)
        {
            float? result;
            Intersection.Intersects(ref this, ref triangle.V1, ref triangle.V2, ref triangle.V3, out result);
            return result;
        }

        public float? Intersects(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            float? result;
            Intersection.Intersects(ref this, ref v1, ref v2, ref v3, out result);
            return result;
        }

        public static Ray operator +(Ray left, Ray right)
            => new Ray(left.Position + right.Position, Vector3.Normalize(left.Direction + right.Direction));

        public static Ray operator -(Ray left, Ray right)
            => new Ray(left.Position - right.Position, -Vector3.Normalize(left.Direction + right.Direction));

        public static bool operator ==(Ray left, Ray right)
            => (left.Position == right.Position) && (left.Direction == right.Direction);

        public static bool operator !=(Ray left, Ray right)
            => (left.Position != right.Position) && (left.Direction != right.Direction);

        /// <inheritdoc />
        public bool Equals(Ray other)
            => (this.Position == other.Position) && (this.Direction == other.Direction);

        /// <inheritdoc />
        public override bool Equals(object obj)
            => (obj is Ray) && this.Equals((Ray)obj);

        /// <inheritdoc />
        public override int GetHashCode()
            => this.Direction.GetHashCode() ^ this.Position.GetHashCode();
        
        /// <inheritdoc />
        public override string ToString()
            => $"<Position: {Position}, Direction: {Direction}>";
    }
}
