namespace Nine.Geometry
{
    using System.Numerics;

    /// <summary>
    /// Indicates the extent to which bounding volumes intersect or contain one another.
    /// </summary>
    public enum ContainmentType
    {
        /// <summary> Indicates that one bounding volume completely contains the other. </summary>
        Contains,

        /// <summary> Indicates there is no overlap between the bounding volumes. </summary>
        Disjoint,

        /// <summary> Indicates that the bounding volumes partially overlap. </summary>
        Intersects
    }

    /// <summary>
    /// Describes the intersection between a plane and a bounding volume.
    /// </summary>
    public enum PlaneIntersectionType
    {
        /// <summary> There is no intersection, and the bounding volume is in the negative half-space of the <see cref="Plane"/>. </summary>
        Front,

        /// <summary> There is no intersection, and the bounding volume is in the positive half-space of the <see cref="Plane"/>. </summary>
        Back,
        
        /// <summary> The <see cref="Plane"/> is intersected. </summary>
        Intersecting
    }

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
    /// Interface for a 3D geometry made up of triangles
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
}
