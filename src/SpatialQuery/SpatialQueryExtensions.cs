namespace Nine.Geometry.SpatialQuery
{
    using System.Collections.Generic;

    public static class SpatialQueryExtensions
    {
        /// <summary>
        /// Finds all the objects that intersects with the specified ray.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        public static ICollection<ISpatialQueryable> FindAll(this ISceneManager<ISpatialQueryable> scene, Ray ray)
            => FindAll(scene, ref ray);

        /// <summary>
        /// Finds all the objects that intersects with the specified ray.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        public static ICollection<ISpatialQueryable> FindAll(this ISceneManager<ISpatialQueryable> scene, ref Ray ray)
        {
            var result = new List<ISpatialQueryable>();
            scene.FindAll(ref ray, result);
            return result;
        }

        /// <summary>
        /// Finds all the objects resides within the specified bounding sphere.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        public static ICollection<ISpatialQueryable> FindAll(this ISceneManager<ISpatialQueryable> scene, BoundingSphere boundingSphere)
            => FindAll(scene, ref boundingSphere);

        /// <summary>
        /// Finds all the objects resides within the specified bounding sphere.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        public static ICollection<ISpatialQueryable> FindAll(this ISceneManager<ISpatialQueryable> scene, ref BoundingSphere boundingSphere)
        {
            var result = new List<ISpatialQueryable>();
            scene.FindAll(ref boundingSphere, result);
            return result;
        }

        /// <summary>
        /// Finds all the objects that intersects with the specified bounding box.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        public static ICollection<ISpatialQueryable> FindAll(this ISceneManager<ISpatialQueryable> scene, BoundingBox boundingBox)
            => FindAll(scene, ref boundingBox);

        /// <summary>
        /// Finds all the objects that intersects with the specified bounding box.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        public static ICollection<ISpatialQueryable> FindAll(this ISceneManager<ISpatialQueryable> scene, ref BoundingBox boundingBox)
        {
            var result = new List<ISpatialQueryable>();
            scene.FindAll(ref boundingBox, result);
            return result;
        }

        /// <summary>
        /// Finds all the objects that intersects with the specified bounding box.
        /// </summary>
        /// <param name="result">The caller is responsible for clearing the result collection</param>
        public static ICollection<ISpatialQueryable> FindAll(this ISceneManager<ISpatialQueryable> scene, BoundingFrustum boundingFrustum)
        {
            var result = new List<ISpatialQueryable>();
            scene.FindAll(boundingFrustum, result);
            return result;
        }
    }
}
