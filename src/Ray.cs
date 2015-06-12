namespace Nine.Geometry
{
    using System;
    using System.Numerics;
    
    /// <summary>
    /// Defines a ray.
    /// </summary>
    public struct Ray : IEquatable<Ray>
    {
        /// <summary> Gets or sets the position. </summary>
        public Vector3 Position;

        /// <summary> Gets or sets the direction. </summary>
        public Vector3 Direction;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ray"/> class.
        /// </summary>
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
            Intersection.Intersect(ref this, ref boundingBox, out result);
            return result;
        }
        
        public float? Intersects(BoundingFrustum boundingfrustum)
        {
            float? result;
            Intersection.Intersect(ref this, ref boundingfrustum, out result);
            return result;
        }
        
        public float? Intersects(BoundingSphere boundingSphere)
        {
            float? result;
            Intersection.Intersect(ref this, ref boundingSphere, out result);
            return result;
        }
        
        public float? Intersects(Plane plane)
        {
            float? result;
            Intersection.Intersect(ref this, ref plane, out result);
            return result;
        }

        public void Intersects(ref Triangle triangle, out float? result)
        {
            this.Intersects(ref triangle.V1, ref triangle.V2, ref triangle.V3, out result);
        }

        public bool Intersects(Triangle triangle)
        {
            float? result;
            this.Intersects(ref triangle.V1, ref triangle.V2, ref triangle.V3, out result);
            return result.HasValue;
        }

        public float? Intersects(Vector3[] positions, ushort[] indices, Matrix4x4 world)
        {
            float? result = null;
            float? point = null;
            Ray ray;
            
            Matrix4x4.Invert(world, out world);
            this.Transform(ref world, out ray);

            // Test each triangle
            for (int i = 0; i < indices.Length; i += 3)
            {
                this.Intersects(ref positions[indices[i]],
                                ref positions[indices[i + 1]],
                                ref positions[indices[i + 2]], out point);

                if (point.HasValue)
                {
                    if (!result.HasValue || point.Value < result.Value)
                        result = point.Value;
                }
            }

            return result;
        }

        public void Intersects(ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, out float? result)
        {
            const float Epsilon = 1E-10F;

            // Compute vectors along two edges of the triangle.
            var edge1 = Vector3.Subtract(vertex2, vertex1);
            var edge2 = Vector3.Subtract(vertex3, vertex1);

            // Compute the determinant.
            var directionCrossEdge2 = Vector3.Cross(this.Direction, edge2);
            var determinant = Vector3.Dot(edge1, directionCrossEdge2);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -Epsilon && determinant < Epsilon)
            {
                result = null;
                return;
            }

            var inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            var distanceVector = Vector3.Subtract(this.Position, vertex1);

            var triangleU = Vector3.Dot(distanceVector, directionCrossEdge2);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            var distanceCrossEdge1 = Vector3.Cross(distanceVector, edge1);

            var triangleV = Vector3.Dot(this.Direction, distanceCrossEdge1);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            var rayDistance = Vector3.Dot(edge2, distanceCrossEdge1);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }
        
        public static bool operator ==(Ray left, Ray right) => (left.Position == right.Position) && (left.Direction == right.Direction);
        public static bool operator !=(Ray left, Ray right) => (left.Position != right.Position) && (left.Direction != right.Direction);

        /// <inheritdoc />
        public bool Equals(Ray other) => (this.Position == other.Position) && (this.Direction == other.Direction);

        /// <inheritdoc />
        public override bool Equals(object obj) => (obj is Ray) && this.Equals((Ray)obj);

        /// <inheritdoc />
        public override int GetHashCode() => this.Direction.GetHashCode() ^ this.Position.GetHashCode();
        
        /// <inheritdoc />
        public override string ToString() => $"<Position: {this.Position}, Direction: {this.Direction}>";
    }
}
