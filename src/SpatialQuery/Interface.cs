namespace Nine.Geometry.SpatialQuery
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines Spatial relations between objects.
    /// </summary>
    public interface ISpatialQuery
    {
        /// <summary>
        /// Creates a spatial query of the specified target type.
        /// </summary>
        ISpatialQuery<T> CreateSpatialQuery<T>(Predicate<T> condition) where T : class;
    }

    /// <summary>
    /// Defines Spatial relations between objects.
    /// </summary>
    public interface ISpatialQuery<T>
    {
        /// <summary>
        /// Finds all the objects that intersects with the specified ray.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        void FindAll(ref Ray ray, ICollection<T> result);

        /// <summary>
        /// Finds all the objects resides within the specified bounding sphere.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        void FindAll(ref BoundingSphere boundingSphere, ICollection<T> result);

        /// <summary>
        /// Finds all the objects that intersects with the specified bounding box.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        void FindAll(ref BoundingBox boundingBox, ICollection<T> result);

        /// <summary>
        /// Finds all the objects resides within the specified bounding frustum.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        void FindAll(BoundingFrustum boundingFrustum, ICollection<T> result);
    }

    /// <summary>
    /// Interface for an object that can be queried by a scene manager.
    /// </summary>
    public interface ISpatialQueryable
    {
        /// <summary>
        /// Gets the axis aligned bounding box of this instance in world space.
        /// </summary>
        BoundingBox BoundingBox { get; }

        /// <summary>
        /// Occurs when the bounding box changed.
        /// </summary>
        event EventHandler<EventArgs> BoundingBoxChanged;

        /// <summary>
        /// Gets or sets the data used for spatial query.
        /// </summary>
        object SpatialData { get; set; }
    }

    /// <summary>
    /// Interface for a scene manager that manages the spatial relationships
    /// between objects.
    /// </summary>
    public interface ISceneManager<T> : ICollection<T>, ISpatialQuery<T>
    {
    }
}
