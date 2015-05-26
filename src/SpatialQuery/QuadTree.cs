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
        /// Creates a new QuadTree.
        /// </summary>
        public QuadTree()
            : this(new BoundingRectangle(-100f, -100f, 200f, 200f), 5)
        {

        }

        /// <summary>
        /// Creates a new QuadTree with the specified boundary.
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

    /*
    internal class QuadTreeReader<T> : ContentTypeReader<QuadTree<T>>
    {
        protected override QuadTree<T> Read(ContentReader input, QuadTree<T> existingInstance)
        {
            if (existingInstance == null)
                existingInstance = new QuadTree<T>();

            existingInstance.maxDepth = input.ReadInt32();
            existingInstance.root = input.ReadRawObject<QuadTreeNode<T>>(new QuadTreeNodeReader<T>());

            // Fix reference
            Stack<QuadTreeNode<T>> stack = new Stack<QuadTreeNode<T>>();

            stack.Push(existingInstance.root);

            while (stack.Count > 0)
            {
                QuadTreeNode<T> node = stack.Pop();

                node.Tree = existingInstance;

                if (node.hasChildren)
                {
                    foreach (QuadTreeNode<T> child in node.Children)
                    {
                        child.parent = node;
                        stack.Push(child);
                    }
                }
            }

            return existingInstance;
        }
    }

    internal class QuadTreeNodeReader<T> : ContentTypeReader<QuadTreeNode<T>>
    {
        protected override QuadTreeNode<T> Read(ContentReader input, QuadTreeNode<T> existingInstance)
        {
            if (existingInstance == null)
                existingInstance = new QuadTreeNode<T>();

            existingInstance.hasChildren = input.ReadBoolean();
            existingInstance.depth = input.ReadInt32();
            existingInstance.bounds = input.ReadObject<BoundingRectangle>();
            existingInstance.value = input.ReadObject<T>();

            if (existingInstance.hasChildren)
            {
                existingInstance.childNodes = input.ReadObject<QuadTreeNode<T>[]>();
                existingInstance.children = new ReadOnlyCollection<QuadTreeNode<T>>(existingInstance.childNodes);
            }
            return existingInstance;
        }
    }
    */
}