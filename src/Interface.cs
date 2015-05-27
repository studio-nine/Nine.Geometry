namespace Nine.Geometry
{
    using System.Numerics;

    /// <summary>
    /// Interface for an object that can be picked by a given ray.
    /// </summary>
    public interface IPickable
    {
        /// <summary>
        /// Gets whether the object contains the given point.
        /// </summary>
        bool Contains(Vector3 point);

        /// <summary>
        /// Gets the nearest intersection point from the specified picking ray.
        /// </summary>
        /// <returns>Distance to the start of the ray.</returns>
        float? Intersects(Ray ray);
    }

    /// <summary>
    /// Interface for a 3D geometry made up of triangles.
    /// </summary>
    public interface IGeometry
    {
        /// <summary>
        /// Gets the world transform matrix of the target geometry.
        /// </summary>
        Matrix4x4 Transform { get; }

        /// <summary>
        /// Gets the triangle vertices of the target geometry.
        /// </summary>
        /// <param name="vertices">Output vertex buffer</param>
        /// <param name="indices">Output index buffer</param>
        /// <returns>
        /// Returns whether the result contains any triangles.
        /// </returns>
        bool TryGetTriangles(out Vector3[] vertices, out ushort[] indices);
    }

    /// <summary>
    /// Interface for a 3D geometry shape made up of triangles.
    /// </summary>
    public interface IGeometryShape
    {
        /// <summary>
        /// Gets the triangle vertices of the target geometry.
        /// </summary>
        /// <param name="vertices">Output vertex buffer</param>
        void GetTriangles(out Vector3[] vertices);
    }
}
