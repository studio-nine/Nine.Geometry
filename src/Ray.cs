namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Defines a ray.
    /// </summary>
    public struct Ray : IEquatable<Ray>, IFormattable
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
            result = this.Transform(transform);
        }

        /// <summary>
        /// Creates a new <see cref="Ray"/> that is the transformed by the input <see cref="Matrix4x4"/>.
        /// </summary>
        public Ray Transform(Matrix4x4 transform)
        {
            Ray result;
            result.Position = Vector3.Transform(this.Position, transform);
            result.Direction = Vector3.TransformNormal(this.Direction, transform);
            return result;
        }

        /// <summary>
        /// Checks whether the <see cref="Ray"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public void Intersects(ref BoundingBox boundingBox, out bool result)
        {
            result = this.Intersects(boundingBox);
        }

        /// <summary>
        /// Checks whether the <see cref="Ray"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public bool Intersects(BoundingBox boundingBox)
        {
            // TODO: Ray intersect with BoundingBox
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="Ray"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public void Intersects(ref BoundingFrustum boundingfrustum, out float? result)
        {
            result = this.Intersects(boundingfrustum);
        }

        /// <summary>
        /// Checks whether the <see cref="Ray"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public float? Intersects(BoundingFrustum boundingfrustum)
        {
            // TODO: Ray intersect with BoundingFrustum
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="Ray"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public void Intersects(ref BoundingSphere boundingSphere, out bool result)
        {
            result = this.Intersects(boundingSphere);
        }

        /// <summary>
        /// Checks whether the <see cref="Ray"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public bool Intersects(BoundingSphere boundingSphere)
        {
            // TODO: Ray intersect with BoundingSphere
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="Ray"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public void Intersects(ref Plane plane, out float? result)
        {
            result = this.Intersects(plane);
        }

        /// <summary>
        /// Checks whether the <see cref="Ray"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public float? Intersects(Plane plane)
        {
            // TODO: Ray intersect with Plane
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="Ray"/> intersects a <see cref="Triangle"/>.
        /// </summary>
        public void Intersects(ref Triangle triangle, out float? result)
        {
            this.Intersects(ref triangle.V1, ref triangle.V2, ref triangle.V3, out result);
        }

        /// <summary>
        /// Checks whether a <see cref="Ray"/> intersects a <see cref="Triangle"/>.
        /// </summary>
        public bool Intersects(Triangle triangle)
        {
            float? result;
            this.Intersects(ref triangle.V1, ref triangle.V2, ref triangle.V3, out result);
            return result.HasValue;
        }

        /// <summary>
        /// Tests to see if a geometry intersects with the specified ray.
        /// If a bounding sphere is provided, the algorithm will perform bounding sphere
        /// intersection test before per triangle test.
        /// 
        /// The geometry and bounding sphere will be transformed by the specified
        /// transformation matrix before and intersection tests.
        /// </summary>
        public float? Intersects(IGeometry geometry)
        {
            float? result = null;
            float? point = null;
            Ray ray;

            var transform = geometry.Transform;
            Matrix4x4.Invert(transform, out transform);
            this.Transform(ref transform, out ray);

            // Test each triangle
            Vector3[] positions;
            ushort[] indices;
            geometry.TryGetTriangles(out positions, out indices);
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

        /// <summary>
        /// Checks whether a <see cref="Ray"/> intersects a <see cref="Triangle"/>.
        /// </summary>
        /// <remarks> 
        /// This uses the algorithm
        /// developed by Tomas Moller and Ben Trumbore, which was published in the
        /// Journal of Graphics Tools, volume 2, "Fast, Minimum Storage Ray-Triangle
        /// Intersection".
        /// 
        /// This method is implemented using the pass-by-reference versions. 
        /// Using these overloads is generally not recommended, because they make the 
        /// code less readable than the normal pass-by-value versions. This method can 
        /// be called very frequently in a tight inner loop, however, so in this 
        /// particular case the performance benefits from passing
        /// everything by reference outweigh the loss of readability.
        /// </remarks>
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

        /// <summary>
        /// Returns a boolean indicating whether the two given <see cref="Ray"/>s are equal.
        /// </summary>
        public static bool operator ==(Ray left, Ray right)
        {
            return (left.Position == right.Position) && (left.Direction == right.Direction);
        }

        /// <summary>
        /// Returns a boolean indicating whether the two given <see cref="Ray"/>s are not equal.
        /// </summary>
        public static bool operator !=(Ray left, Ray right)
        {
            return (left.Position != right.Position) && (left.Direction != right.Direction);
        }

        /// <inheritdoc />
        public bool Equals(Ray other)
        {
            return (this.Position == other.Position) && (this.Direction == other.Direction);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return (obj is Ray) && this.Equals((Ray)obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return this.Direction.GetHashCode() ^ this.Position.GetHashCode();
        }

        /// <inheritdoc />
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, format ?? "Position: {0}, Direction: {1}", this.Position, this.Direction);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.ToString(null, System.Globalization.CultureInfo.CurrentCulture);
        }
    }
}
