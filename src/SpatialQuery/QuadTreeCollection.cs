namespace Nine.Geometry.SpatialQuery
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;

    /// <summary>
    /// Manages a collection of objects using quad tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("Count = {Count}")]
    public class QuadTreeCollection : ISpatialCollection<ISpatialQueryable>
    {
        #region Properties

        internal QuadTree<QuadTreeCollectionNodeData<ISpatialQueryable>> Tree;

        /// <summary>
        /// Gets the bounds of this QuadTreeSceneManager.
        /// </summary>
        public BoundingBox Bounds
        {
            get
            {
                var bounds = Tree.Bounds;
                return new BoundingBox(new Vector3(bounds.X, bounds.X + bounds.Width, 0),
                                       new Vector3(bounds.Y, bounds.Y + bounds.Height, 0));
            }
        }

        /// <summary>
        /// Gets the max depth of this QuadTreeSceneManager.
        /// </summary>
        public int MaxDepth
        {
            get { return Tree.maxDepth; }
        }
        
        #endregion

        #region Fields

        private bool needResize;
        private bool addedToNode;
        private ISpatialQueryable item;
        private Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions> add;

        private ICollection<ISpatialQueryable> result;
        private Ray ray;
        private BoundingBox boundingBox;
        private BoundingFrustum boundingFrustum;
        private BoundingSphere boundingSphere;
        private Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions> findAllRay;
        private Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions> findAllBoundingBox;
        private Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions> findAllBoundingSphere;
        private Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions> findAllBoundingFrustum;
        private EventHandler<EventArgs> boundingBoxChanged;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of QuadTreeSceneManager.
        /// </summary>
        public QuadTreeCollection() 
            : this(new BoundingRectangle(-100f, -100f, 200f, 200f), 5)
        {

        }

        /// <summary>
        /// Creates a new instance of QuadTreeSceneManager.
        /// </summary>
        public QuadTreeCollection(BoundingRectangle bounds, int maxDepth)
        {
            Tree = new QuadTree<QuadTreeCollectionNodeData<ISpatialQueryable>>(bounds, maxDepth);

            add = new Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions>(Add);
            findAllRay = new Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions>(FindAllRay);
            findAllBoundingBox = new Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions>(FindAllBoundingBox);
            findAllBoundingSphere = new Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions>(FindAllBoundingSphere);
            findAllBoundingFrustum = new Func<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>, TraverseOptions>(FindAllBoundingFrustum);
            boundingBoxChanged = new EventHandler<EventArgs>(BoundingBoxChanged);
        }

        #endregion
        
        #region ICollection

        public void Add(ISpatialQueryable item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            if (item.SpatialData != null)
                throw new InvalidOperationException("The input has already been added to a scene manager.");

            item.SpatialData = new QuadTreeCollectionSpatialData<ISpatialQueryable>();

            AddWithResize(Tree.root, item);

            item.BoundingBoxChanged += boundingBoxChanged;

            Count++;
        }

        private void AddWithResize(QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> treeNode, ISpatialQueryable item)
        {
            needResize = false;
            Add(treeNode, item);

            if (needResize)
            {
                needResize = false;

                BoundingBox itemBounds = item.BoundingBox;
                BoundingRectangle newBounds = new BoundingRectangle();
                newBounds.X = itemBounds.Min.X;
                newBounds.Y = itemBounds.Min.Z;
                newBounds.Width = itemBounds.Max.X - itemBounds.Min.X;
                newBounds.Height = itemBounds.Max.Z - itemBounds.Min.Z;

                BoundingRectangle.CreateMerged(ref Tree.root.bounds, ref newBounds, out newBounds);

                float extendX = 0.5f * (newBounds.Width - Tree.root.bounds.Width);
                float extendY = 0.5f * (newBounds.Height - Tree.root.bounds.Height);

                newBounds.X -= extendX;
                newBounds.Width += extendX * 2;
                newBounds.Y -= extendY;
                newBounds.Height += extendY * 2;

                Resize(ref newBounds);

                Add(Tree.root, item);

                if (needResize)
                {
                    // Should be able to add when the tree is resized.
                    throw new InvalidOperationException();
                }
            }
        }

        private void Add(QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> treeNode, ISpatialQueryable item)
        {
            addedToNode = false;

            this.item = item;
            Tree.Traverse(treeNode, add);
            this.item = default(ISpatialQueryable);

            if (!addedToNode && !needResize)
            {
                // Something must be wrong if the node is not added.
                throw new InvalidOperationException();
            }
        }

        private TraverseOptions Add(QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> node)
        {
            ContainmentType containment;
            BoundingRectangle itemRectangle = new BoundingRectangle();
            BoundingBox itemBounds = item.BoundingBox;
            itemRectangle.X = itemBounds.Min.X;
            itemRectangle.Y = itemBounds.Min.Z;
            itemRectangle.Width = itemBounds.Max.X - itemBounds.Min.X;
            itemRectangle.Height = itemBounds.Max.Z - itemBounds.Min.Z;
            node.bounds.Contains(ref itemRectangle, out containment);

            // Expand the tree to root if the object is too large
            if (node == Tree.root && containment != ContainmentType.Contains)
            {
                needResize = true;
                return TraverseOptions.Stop;
            }

            if (containment == ContainmentType.Disjoint)
                return TraverseOptions.Skip;

            if (containment == ContainmentType.Intersects)
            {
                AddToNode(item, node.parent);
                return TraverseOptions.Stop;
            }

            if (containment == ContainmentType.Contains && node.depth == Tree.maxDepth - 1)
            {
                AddToNode(item, node);
                return TraverseOptions.Stop;
            }
            return Tree.Expand(node) ? TraverseOptions.Continue : TraverseOptions.Skip;
        }

        private void AddToNode(ISpatialQueryable item, QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> node)
        {
            if (node.value.List == null)
            {
                node.value.List = new List<ISpatialQueryable>();
            }

            var data = (QuadTreeCollectionSpatialData<ISpatialQueryable>)item.SpatialData;
            data.Tree = Tree;
            data.Node = node;
            node.value.List.Add(item);
            addedToNode = true;

            // Bubble up the tree to adjust the node bounds accordingly.
            //
            // TODO: Adjust node bounds when objects removed from nodes.
            while (node != null)
            {
                if (node.value.Initialized)
                {
                    if (item.BoundingBox.Min.Y < node.value.MinHeight)
                        node.value.MinHeight = item.BoundingBox.Min.Y;
                    if (item.BoundingBox.Max.Y > node.value.MaxHeight)
                        node.value.MaxHeight = item.BoundingBox.Max.Y;
                }
                else
                {
                    node.value.MinHeight = item.BoundingBox.Min.Y;
                    node.value.MaxHeight = item.BoundingBox.Max.Y;
                    node.value.Initialized = true;
                }
                node = node.parent;
            }
        }

        public bool Remove(ISpatialQueryable item)
        {
            QuadTreeCollectionSpatialData<ISpatialQueryable> data = item.SpatialData as QuadTreeCollectionSpatialData<ISpatialQueryable>;
            if (data == null || data.Tree != Tree)
                return false;

            var node = data.Node;
            if (!node.value.List.Remove(item))
            {
                // Something must be wrong if we cannot remove it.
                throw new InvalidOperationException();
            }

            while (node.value.List == null || node.value.List.Count <= 0)
            {
                if (node.parent == null)
                    break;
                node = node.parent;
            }

            Tree.Collapse(node, n => n.value.List == null || n.value.List.Count <= 0);

            item.SpatialData = null;
            item.BoundingBoxChanged -= boundingBoxChanged;
            Count--;

            return true;
        }

        private void BoundingBoxChanged(object sender, EventArgs e)
        {
            var item = (ISpatialQueryable)sender;
            var data = (QuadTreeCollectionSpatialData<ISpatialQueryable>)item.SpatialData;

            // Bubble up the tree to find the node that fit the size of the object
            var node = data.Node;
            while (true)
            {
                ContainmentType containment;
                BoundingRectangle itemRectangle = new BoundingRectangle();
                BoundingBox itemBounds = item.BoundingBox;
                itemRectangle.X = itemBounds.Min.X;
                itemRectangle.Y = itemBounds.Min.Z;
                itemRectangle.Width = itemBounds.Max.X - itemBounds.Min.X;
                itemRectangle.Height = itemBounds.Max.Z - itemBounds.Min.Z;
                node.bounds.Contains(ref itemRectangle, out containment);

                if (containment == ContainmentType.Contains || node.parent == null)
                    break;
                node = node.parent;
            }

            if (node != data.Node)
            {
                if (!data.Node.value.List.Remove(item))
                {
                    // Something must be wrong if we cannot remove it.
                    throw new InvalidOperationException();
                }

                Count--;
                AddWithResize(node, item);
                Count++;
            }
        }

        public void Clear()
        {
            // Clear event handlers
            foreach (var item in this)
            {
                item.SpatialData = null;
                item.BoundingBoxChanged -= boundingBoxChanged;
            }

            Tree.Collapse();
            Count = 0;
        }

        private void Resize(ref BoundingRectangle boundingRectangle)
        {
            var items = new ISpatialQueryable[Count];

            CopyTo(items, 0);
            Clear();

            Tree = new QuadTree<QuadTreeCollectionNodeData<ISpatialQueryable>>(boundingRectangle, MaxDepth);
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public bool Contains(ISpatialQueryable item)
        {
            foreach (var val in this)
            {
                if (val.Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(ISpatialQueryable[] array, int arrayIndex)
        {
            foreach (var val in this)
            {
                array[arrayIndex++] = val;
            }
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get { return false; } }
        
        public IEnumerator<ISpatialQueryable> GetEnumerator()
        {
            foreach (var node in Tree)
            {
                if (node.value.List != null)
                {
                    foreach (var val in node.value.List)
                        yield return val;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ISpatialQuery

        public void FindAll(ref Ray ray, ICollection<ISpatialQueryable> result)
        {
            this.result = result;
            this.ray = ray;
            Tree.Traverse(Tree.root, findAllRay);
            this.result = null;
        }

        private TraverseOptions FindAllRay(QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> node)
        {
            if (!node.value.Initialized)
                return TraverseOptions.Skip;

            float? intersection;
            bool skip = (node != Tree.root);
            if (skip)
            {
                var nodeBounds = new BoundingBox(new Vector3(node.bounds.X, node.bounds.Y, node.value.MinHeight),
                                                 new Vector3(node.bounds.X + node.bounds.Width, node.bounds.Y + node.bounds.Height, node.value.MaxHeight));

                nodeBounds.Intersects(ref ray, out intersection);
                if (intersection.HasValue)
                {
                    skip = false;
                }
            }

            if (skip)
                return TraverseOptions.Skip;

            if (node.value.List != null)
            {
                var count = node.value.List.Count;
                for (int i = 0; i < count; ++i)
                {
                    var val = node.value.List[i];
                    val.BoundingBox.Intersects(ref ray, out intersection);
                    if (intersection.HasValue)
                    {
                        result.Add(val);
                    }
                }
            }
            return TraverseOptions.Continue;
        }

        public void FindAll(ref BoundingBox boundingBox, ICollection<ISpatialQueryable> result)
        {
            this.result = result;
            this.boundingBox = boundingBox;
            Tree.Traverse(Tree.root, findAllBoundingBox);
            this.result = null;
        }

        private TraverseOptions FindAllBoundingBox(QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> node)
        {
            if (!node.value.Initialized)
                return TraverseOptions.Skip;

            var nodeContainment = ContainmentType.Intersects;
            var nodeBounds = new BoundingBox(new Vector3(node.bounds.X, node.bounds.Y, node.value.MinHeight),
                                             new Vector3(node.bounds.X + node.bounds.Width, node.bounds.Y + node.bounds.Height, node.value.MaxHeight));

            boundingBox.Contains(ref nodeBounds, out nodeContainment);

            if (nodeContainment == ContainmentType.Disjoint)
                return TraverseOptions.Skip;

            if (nodeContainment == ContainmentType.Contains)
            {
                AddAllDesedents(node);
                return TraverseOptions.Skip;
            }

            if (node.value.List != null)
            {
                var count = node.value.List.Count;
                for (int i = 0; i < count; ++i)
                {
                    var val = node.value.List[i];
                    ContainmentType objectContainment;
                    val.BoundingBox.Contains(ref boundingBox, out objectContainment);
                    if (objectContainment != ContainmentType.Disjoint)
                        result.Add(val);
                }
            }
            return TraverseOptions.Continue;
        }

        public void FindAll(ref BoundingSphere boundingSphere, ICollection<ISpatialQueryable> result)
        {
            this.result = result;
            this.boundingSphere = boundingSphere;
            Tree.Traverse(Tree.root, findAllBoundingBox);
            this.result = null;
        }

        private TraverseOptions FindAllBoundingSphere(QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> node)
        {
            if (!node.value.Initialized)
                return TraverseOptions.Skip;

            var nodeContainment = ContainmentType.Intersects;
            var nodeBounds = new BoundingBox(new Vector3(node.bounds.X, node.bounds.Y, node.value.MinHeight),
                                             new Vector3(node.bounds.X + node.bounds.Width, node.bounds.Y + node.bounds.Height, node.value.MaxHeight));

            boundingSphere.Contains(ref boundingBox, out nodeContainment);

            if (nodeContainment == ContainmentType.Disjoint)
                return TraverseOptions.Skip;

            if (nodeContainment == ContainmentType.Contains)
            {
                AddAllDesedents(node);
                return TraverseOptions.Skip;
            }

            if (node.value.List != null)
            {
                var count = node.value.List.Count;
                for (int i = 0; i < count; ++i)
                {
                    var val = node.value.List[i];
                    ContainmentType objectContainment;
                    val.BoundingBox.Contains(ref boundingSphere, out objectContainment);
                    if (objectContainment != ContainmentType.Disjoint)
                        result.Add(val);
                }
            }
            return TraverseOptions.Continue;
        }

        public void FindAll(BoundingFrustum boundingFrustum, ICollection<ISpatialQueryable> result)
        {
            this.result = result;
            this.boundingFrustum = boundingFrustum;
            Tree.Traverse(Tree.root, findAllBoundingFrustum);
            this.result = null;
        }

        private TraverseOptions FindAllBoundingFrustum(QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> node)
        {
            if (!node.value.Initialized)
                return TraverseOptions.Skip;

            var nodeContainment = ContainmentType.Intersects;
            var nodeBounds = new BoundingBox(new Vector3(node.bounds.X, node.bounds.Y, node.value.MinHeight),
                                             new Vector3(node.bounds.X + node.bounds.Width, node.bounds.Y + node.bounds.Height, node.value.MaxHeight));

            boundingFrustum.Contains(ref boundingBox, out nodeContainment);

            if (nodeContainment == ContainmentType.Disjoint)
                return TraverseOptions.Skip;

            if (nodeContainment == ContainmentType.Contains)
            {
                AddAllDesedents(node);
                return TraverseOptions.Skip;
            }

            if (node.value.List != null)
            {
                var count = node.value.List.Count;
                for (int i = 0; i < count; ++i)
                {
                    var val = node.value.List[i];
                    if (boundingFrustum.Contains(val.BoundingBox) != ContainmentType.Disjoint)
                    {
                        result.Add(val);
                    }
                }
            }
            return TraverseOptions.Continue;
        }

        private void AddAllDesedents(QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> node)
        {
            DesedentsStack.Push(node);

            while (DesedentsStack.Count > 0)
            {
                node = DesedentsStack.Pop();
                if (node.value.List != null)
                {
                    var count = node.value.List.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        result.Add(node.value.List[i]);
                    }
                }

                var children = node.Children;
                for (int i = 0; i < children.Count; ++i)
                    DesedentsStack.Push(children[i]);
            }
        }
        
        static Stack<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>> DesedentsStack = new Stack<QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>>>();

        #endregion
    }

    struct QuadTreeCollectionNodeData<ISpatialQueryable>
    {
        public List<ISpatialQueryable> List;
        public float MinHeight;
        public float MaxHeight;

        // Flag for whether the bounding box of the node has initialized.
        public bool Initialized;
    }

    class QuadTreeCollectionSpatialData<ISpatialQueryable>
    {
        public QuadTree<QuadTreeCollectionNodeData<ISpatialQueryable>> Tree;
        public QuadTreeNode<QuadTreeCollectionNodeData<ISpatialQueryable>> Node;
    }
}
