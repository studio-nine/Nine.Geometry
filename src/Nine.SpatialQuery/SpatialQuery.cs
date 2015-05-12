namespace Nine.SpatialQuery
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Represents an adapter class that filters and converts the result of
    /// an existing <c>SpatialQuery</c>.
    /// </summary>
    class SpatialQuery<TInput, TOutput> : ISpatialQuery<TOutput>
    {
        /// <summary>
        /// Gets or sets the inner query.
        /// </summary>
        public IList<ISpatialQuery<TInput>> InnerQueries { get; set; }

        /// <summary>
        /// Gets or sets a predicate that filters the result of the inner query.
        /// Objects passed the predicated will be included in the query.
        /// </summary>
        public Predicate<TInput> Condition { get; set; }

        /// <summary>
        /// Gets or sets a predicate that converts the result of the inner query.
        /// </summary>
        public Func<TInput, TOutput> Converter { get; set; }

        private CollectionAdapter adapter;

        public SpatialQuery(params ISpatialQuery<TInput>[] queries)
        {
            if (queries == null)
                throw new ArgumentNullException("query");

            this.Condition = d => d is TOutput;
            this.Converter = d => (TOutput)(object)d;
            this.InnerQueries = new List<ISpatialQuery<TInput>>(queries);
            this.adapter = new CollectionAdapter() { Parent = this };
        }

        private bool Convert(TInput input, out TOutput output)
        {
            if (Condition != null && !Condition(input))
            {
                output = default(TOutput);
                return false;
            }

            if (Converter != null)
            {
                output = Converter(input);
                return true;
            }

            if (input is TOutput)
            {
                output = (TOutput)(object)input;
                return true;
            }

            output = default(TOutput);
            return false;
        }

        public void FindAll(ref BoundingSphere boundingSphere, ICollection<TOutput> result)
        {
            adapter.Result = result;
            if (InnerQueries != null)
                for (int i = 0; i < InnerQueries.Count; ++i)
                    InnerQueries[i].FindAll(ref boundingSphere, adapter);
            adapter.Result = null;
        }

        public void FindAll(ref Ray ray, ICollection<TOutput> result)
        {
            adapter.Result = result;
            if (InnerQueries != null)
                for (int i = 0; i < InnerQueries.Count; ++i)
                    InnerQueries[i].FindAll(ref ray, adapter);
            adapter.Result = null;
        }

        public void FindAll(ref BoundingBox boundingBox, ICollection<TOutput> result)
        {
            adapter.Result = result;
            if (InnerQueries != null)
                for (int i = 0; i < InnerQueries.Count; ++i)
                    InnerQueries[i].FindAll(ref boundingBox, adapter);
            adapter.Result = null;
        }

        public void FindAll(BoundingFrustum boundingFrustum, ICollection<TOutput> result)
        {
            adapter.Result = result;
            if (InnerQueries != null)
                for (int i = 0; i < InnerQueries.Count; ++i)
                    InnerQueries[i].FindAll(boundingFrustum, adapter);
            adapter.Result = null;
        }

        class CollectionAdapter : SpatialQueryCollectionAdapter<TInput>
        {
            public SpatialQuery<TInput, TOutput> Parent;
            public ICollection<TOutput> Result;

            public override void Add(TInput item)
            {
                TOutput output;
                if (Parent.Convert(item, out output))
                    Result.Add(output);
            }
        }
    }

    /// <summary>
    /// Represents a basic query from fixed list.
    /// </summary>
    class SpatialQuery<T> : ISpatialQuery<T> where T : class
    {
        private IList<object> objects;
        private Predicate<T> condition;

        public SpatialQuery(IList<object> objects, Predicate<T> condition)
        {
            this.objects = objects;
            this.condition = condition;
        }

        private void Find(ICollection<T> result)
        {
            if (objects != null)
            {
                var count = objects.Count;
                for (int i = 0; i < count; ++i)
                {
                    var t = objects[i] as T;
                    if (t != null && (condition == null || condition(t)))
                        result.Add(t);
                }
            }
        }

        public void FindAll(ref BoundingSphere boundingSphere, ICollection<T> result)
        {
            Find(result);
        }

        public void FindAll(ref BoundingBox boundingBox, ICollection<T> result)
        {
            Find(result);
        }

        public void FindAll(BoundingFrustum boundingFrustum, ICollection<T> result)
        {
            Find(result);
        }

        public void FindAll(ref Ray ray, ICollection<T> result)
        {
            Find(result);
        }
    }

    /// <summary>
    /// Defines a dummy spatial query.
    /// </summary>
    class SpatialQuery : ISpatialQuery
    {
        private List<object> objects;

        public SpatialQuery() { }
        public SpatialQuery(IEnumerable objects)
        {
            if (objects == null)
                throw new ArgumentNullException("objects");
            this.objects = new List<object>();
            foreach (var obj in objects)
                this.objects.Add(obj);
        }

        public ISpatialQuery<T> CreateSpatialQuery<T>(Predicate<T> condition) where T : class
        {
            return new SpatialQuery<T>(objects, condition);
        }
    }
}