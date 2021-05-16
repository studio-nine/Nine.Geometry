namespace Nine.Geometry.SpatialQuery
{
    using System.Numerics;

    /// <summary>
    /// Represents a space partition structure based on QuadTree.
    /// </summary>
    public class QuadTree<T> : SpacePartitionTree<T, QuadTreeNode<T>>
    {
        /// <summary>
        /// Specifies the total number of child nodes (4) in the QuadTree.
        /// </summary>
        const int ChildCount = 4;

        /// <summary>
        /// Gets the bounds of the QuadTree node.
        /// </summary>
        public BoundingRectangle Bounds { get { return root.bounds; } }

        /// <summary>
        /// For serialization.
        /// </summary>
        internal QuadTree() { }

        /// <summary>
        /// Creates a new Octree with the specified boundary.
        /// </summary>
        public QuadTree(BoundingRectangle bounds, int maxDepth)
            : base(new QuadTreeNode<T>() { bounds = bounds }, maxDepth)
        {

        }

        protected override QuadTreeNode<T>[] ExpandNode(QuadTreeNode<T> node)
        {
            var childNodes = new QuadTreeNode<T>[ChildCount];
            var quadTreeNode = (QuadTreeNode<T>)node;

            var halfBounds = quadTreeNode.bounds;
            halfBounds.Width *= 0.5f;
            halfBounds.Height *= 0.5f;

            var center = new Vector2(halfBounds.X + halfBounds.Width, halfBounds.Y + halfBounds.Height);

            for (int i = 0; i < ChildCount; ++i)
            {
                var child = new QuadTreeNode<T>();
                child.bounds = new BoundingRectangle(
                    (i % 2 == 0 ? halfBounds.X : center.X),
                    (i < 2 ? halfBounds.Y : center.Y),
                    halfBounds.Width,
                    halfBounds.Height);

                childNodes[i] = child;
            }
            return childNodes;
        }
    }

    /// <summary>
    /// Represents a node in QuadTree.
    /// </summary>
    public sealed class QuadTreeNode<T> : SpacePartitionTreeNode<T, QuadTreeNode<T>>
    {
        /// <summary>
        /// Gets the bounds of the QuadTree node.
        /// </summary>
        public BoundingRectangle Bounds
        {
            get { return bounds; }
        }
        internal BoundingRectangle bounds;

        internal QuadTreeNode() { }
    }
}
