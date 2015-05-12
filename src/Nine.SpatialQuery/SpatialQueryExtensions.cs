namespace Nine.SpatialQuery
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Contains extension methods for spatial queries.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class SpatialQueryExtensions
    {
        public static ISpatialQuery<T> CreateSpatialQuery<T>(this ISpatialQuery spatialQuery) where T : class
        {
            return spatialQuery.CreateSpatialQuery<T>(null);
        }

        public static void FindAll<T>(this ISpatialQuery<T> spatialQuery, ref Ray ray, Action<T> result)
        {
            spatialQuery.FindAll(ref ray, new SpatialQueryDelegateAdapter<T>(result));
        }

        public static void FindAll<T>(this ISpatialQuery<T> spatialQuery, ref BoundingSphere boundingSphere, Action<T> result)
        {
            spatialQuery.FindAll(ref boundingSphere, new SpatialQueryDelegateAdapter<T>(result));
        }

        public static void FindAll<T>(this ISpatialQuery<T> spatialQuery, ref BoundingBox boundingBox, Action<T> result)
        {
            spatialQuery.FindAll(ref boundingBox, new SpatialQueryDelegateAdapter<T>(result));
        }

        public static void FindAll<T>(this ISpatialQuery<T> spatialQuery, BoundingFrustum boundingFrustum, Action<T> result)
        {
            spatialQuery.FindAll(boundingFrustum, new SpatialQueryDelegateAdapter<T>(result));
        }
    }

    abstract class SpatialQueryCollectionAdapter<T> : ICollection<T>
    {
        public abstract void Add(T item);
        public void Clear() { throw new InvalidOperationException(); }
        public bool Contains(T item) { throw new InvalidOperationException(); }
        public void CopyTo(T[] array, int arrayIndex) { throw new InvalidOperationException(); }
        public int Count { get { throw new InvalidOperationException(); } }
        public bool IsReadOnly { get { throw new InvalidOperationException(); } }
        public bool Remove(T item) { throw new InvalidOperationException(); }
        public IEnumerator<T> GetEnumerator() { throw new InvalidOperationException(); }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { throw new InvalidOperationException(); }
    }

    class SpatialQueryDelegateAdapter<T> : SpatialQueryCollectionAdapter<T>
    {
        private Action<T> result;

        public SpatialQueryDelegateAdapter(Action<T> result)
        {
            if (result == null)
                throw new ArgumentNullException("result");
            this.result = result;
        }

        public override void Add(T item)
        {
            result(item);
        }
    }
}