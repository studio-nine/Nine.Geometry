namespace Nine.Geometry.SpatialQuery
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;

    public partial class DynamicTree<T>
    {
        internal const int NullNode = -1;

        /// <summary> 
        /// Compute the height of the binary tree in O(N) time. 
        /// </summary>
        public int Height => (this.root == NullNode) ? 0 : this.nodes[this.root].Height;

        //public float AreaRatio
        //{
        //    get
        //    {
        //        if (_root == NullNode)
        //        {
        //            return 0.0f;
        //        }

        //        TreeNode<T> root = _nodes[_root];
        //        float rootArea = root.AABB.Perimeter;

        //        float totalArea = 0.0f;
        //        for (int i = 0; i < _nodeCapacity; ++i)
        //        {
        //            TreeNode<T> node = _nodes[i];
        //            if (node.Height < 0)
        //            {
        //                // Free node in pool
        //                continue;
        //            }

        //            totalArea += node.AABB.Perimeter;
        //        }

        //        return totalArea / rootArea;
        //    }
        //}

        //public int MaxBalance
        //{
        //    get
        //    {
        //        int maxBalance = 0;
        //        for (int i = 0; i < _nodeCapacity; ++i)
        //        {
        //            TreeNode<T> node = _nodes[i];
        //            if (node.Height <= 1)
        //            {
        //                continue;
        //            }

        //            Debug.Assert(node.IsLeaf() == false);

        //            int child1 = node.Child1;
        //            int child2 = node.Child2;
        //            int balance = Math.Abs(_nodes[child2].Height - _nodes[child1].Height);
        //            maxBalance = Math.Max(maxBalance, balance);
        //        }

        //        return maxBalance;
        //    }
        //}

        private readonly Stack<int> raycastStack;
        private readonly Stack<int> queryStack;

        private DynamicTreeNode<T>[] nodes;

        private int freeList;
        private int nodeCapacity;
        private int nodeCount;
        private int root;

        public DynamicTree()
        {
            this.raycastStack = new Stack<int>(256);
            this.queryStack = new Stack<int>(256);

            this.root = NullNode;
            this.nodeCapacity = 16;
            this.nodeCount = 0;
            this.nodes = new DynamicTreeNode<T>[nodeCapacity];

            // Build a linked list for the free list.
            for (int i = 0; i < nodeCapacity - 1; ++i)
            {
                this.nodes[i] = new DynamicTreeNode<T>();
                this.nodes[i].ParentOrNext = i + 1;
                this.nodes[i].Height = 1;
            }

            this.nodes[nodeCapacity - 1] = new DynamicTreeNode<T>();
            this.nodes[nodeCapacity - 1].ParentOrNext = NullNode;
            this.nodes[nodeCapacity - 1].Height = 1;
            this.freeList = 0;
        }
        
        public DynamicTreeNode<T> Add(ref BoundingRectangle bounds, T value)
        {
            var proxyId = Allocate();
            
            var r = new Vector2(0.1f);

            nodes[proxyId].Bounds.Lower = bounds.Lower - r;
            nodes[proxyId].Bounds.Upper = bounds.Upper + r;
            nodes[proxyId].Value = value;
            nodes[proxyId].Height = 0;
            nodes[proxyId].ProxyId = proxyId;

            InsertLeaf(proxyId);

            return nodes[proxyId];
        }

        public bool Remove(DynamicTreeNode<T> node)
        {
            return RemoveAt(node.ProxyId);
        }
        
        public bool RemoveAt(int index)
        {
            Debug.Assert(0 <= index && index < this.nodeCapacity);
            Debug.Assert(this.nodes[index].IsLeaf());

            RemoveLeaf(index);
            FreeNode(index);

            return true;
        }

        public bool Move(DynamicTreeNode<T> node, BoundingRectangle bounds)
        {
            var proxyId = node.ProxyId;
            var displacement = node.Bounds.Center - bounds.Center;

            Debug.Assert(0 <= proxyId && proxyId < nodeCapacity);
            Debug.Assert(nodes[proxyId].IsLeaf());

            if (nodes[proxyId].Bounds.Contains(bounds) == ContainmentType.Contains)
                return false;

            RemoveLeaf(proxyId);

            // Extend AABB.
            var r = new Vector2(0.1f);

            var b = bounds;
            b.Lower = b.Lower - r;
            b.Upper = b.Upper + r;

            // Predict AABB displacement.
            var d = 2.0f * displacement;

            if (d.X < 0.0f)
            {
                b.Width += d.X;
            }
            else
            {
                b.X += d.X;
            }

            if (d.Y < 0.0f)
            {
                b.Height += d.Y;
            }
            else
            {
                b.Y += d.Y;
            }

            nodes[proxyId].Bounds = b;
            InsertLeaf(proxyId);

            return true;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int Find(BoundingRectangle rectangle, T[] output, int startIndex)
        {
            throw new NotImplementedException();
        }
    }

    public struct DynamicTreeNode<T>
    {
        internal int Child1;
        internal int Child2;
        internal int ProxyId;

        internal int Height;
        internal int ParentOrNext;

        public T Value;
        public BoundingRectangle Bounds;

        public bool IsLeaf() => this.Child1 == DynamicTree<T>.NullNode;
    }
}
