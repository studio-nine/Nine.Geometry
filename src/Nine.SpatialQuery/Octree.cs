namespace Nine.SpatialQuery
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

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
            Vector3.Add(ref min, ref max, out center);
            Vector3.Multiply(ref center, 0.5f, out center);

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

    internal class OctreeReader<T> : ContentTypeReader<Octree<T>>
    {
        protected override Octree<T> Read(ContentReader input, Octree<T> existingInstance)
        {
            if (existingInstance == null)
                existingInstance = new Octree<T>();

            existingInstance.maxDepth = input.ReadInt32();
            existingInstance.root = input.ReadRawObject<OctreeNode<T>>(new OctreeNodeReader<T>());

            // Fix reference
            Stack<OctreeNode<T>> stack = new Stack<OctreeNode<T>>();

            stack.Push(existingInstance.root);

            while (stack.Count > 0)
            {
                OctreeNode<T> node = stack.Pop();

                node.Tree = existingInstance;

                if (node.hasChildren)
                {
                    foreach (OctreeNode<T> child in node.Children)
                    {
                        child.parent = node;
                        stack.Push(child);
                    }
                }
            }

            return existingInstance;
        }
    }

    internal class OctreeNodeReader<T> : ContentTypeReader<OctreeNode<T>>
    {
        protected override OctreeNode<T> Read(ContentReader input, OctreeNode<T> existingInstance)
        {
            if (existingInstance == null)
                existingInstance = new OctreeNode<T>();

            existingInstance.hasChildren = input.ReadBoolean();
            existingInstance.depth = input.ReadInt32();
            existingInstance.bounds = input.ReadObject<BoundingBox>();
            existingInstance.value = input.ReadObject<T>();

            if (existingInstance.hasChildren)
            {
                existingInstance.childNodes = input.ReadObject<OctreeNode<T>[]>();
                existingInstance.children = new ReadOnlyCollection<OctreeNode<T>>(existingInstance.childNodes);
            }
            return existingInstance;
        }
    }
}