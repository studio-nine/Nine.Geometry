namespace Nine.Geometry.SpatialQuery
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Numerics;

    /// <summary>
    /// Represents a space partition structure based on Octree.
    /// </summary>
    public class Octree<T> : SpacePartitionTree<T, OctreeNode<T>>
    {
        /// <summary>
        /// Specifies the total number of child nodes (8) in the Octree.
        /// </summary>
        const int ChildCount = 8;

        /// <summary>
        /// Gets the bounds of the Octree.
        /// </summary>
        public BoundingBox Bounds { get { return root.bounds; } }

        /// <summary>
        /// For serialization.
        /// </summary>
        internal Octree() { }

        /// <summary>
        /// Creates a new Octree with the specified boundary.
        /// </summary>
        public Octree(BoundingBox bounds, int maxDepth)
            : base(new OctreeNode<T>() { bounds = bounds }, maxDepth)
        {

        }

        protected override OctreeNode<T>[] ExpandNode(OctreeNode<T> node)
        {
            var childNodes = new OctreeNode<T>[ChildCount];
            OctreeNode<T> octreeNode = (OctreeNode<T>)node;

            Vector3 center;
            Vector3 min = octreeNode.bounds.Min;
            Vector3 max = octreeNode.bounds.Max;
            center = Vector3.Add(min, max);
            center = Vector3.Multiply(center, 0.5f);

            for (int i = 0; i < ChildCount; ++i)
            {
                var child = new OctreeNode<T>();
                child.bounds = new BoundingBox
                {
                    Min = new Vector3()
                    {
                        X = (i % 2 == 0 ? min.X : center.X),
                        Y = ((i < 2 || i == 4 || i == 5) ? min.Y : center.Y),
                        Z = (i < 4 ? min.Z : center.Z),
                    },
                    Max = new Vector3()
                    {
                        X = (i % 2 == 0 ? center.X : max.X),
                        Y = ((i < 2 || i == 4 || i == 5) ? center.Y : max.Y),
                        Z = (i < 4 ? center.Z : max.Z),
                    },
                };
                childNodes[i] = child;
            }
            return childNodes;
        }
    }

    /// <summary>
    /// Represents a node in Octree.
    /// </summary>
    public class OctreeNode<T> : SpacePartitionTreeNode<T, OctreeNode<T>>
    {
        /// <summary>
        /// Gets the bounds of the Octree node.
        /// </summary>
        public BoundingBox Bounds
        {
            get { return bounds; }
        }
        internal BoundingBox bounds;

        internal OctreeNode() { }
    }
}