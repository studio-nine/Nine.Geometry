namespace Nine.Geometry.SpatialQuery
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;

    public partial class DynamicTree<T> : ISpatialQuery2D<T>
    {
        internal const int NullNode = -1;

        /// <summary> 
        /// Compute the height of the binary tree in O(N) time. 
        /// </summary>
        public int Height => (this.root == NullNode) ? 0 : this.nodes[this.root].Height;

        public float AreaRatio => GetAreaRatio();
        public int MaxBalance => GetMaxBalance();

        public int NodeCount => nodeCount;

        public int RootId => root;
        public DynamicTreeNode<T> Root => nodes[root];

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

            this.Clear();
        }
        
        public DynamicTreeNode<T> GetNodeAt(int index) => nodes[index];

        public int Add(ref BoundingRectangle bounds, T value)
        {
            var newIndex = Allocate();
            
            var r = new Vector2();

            nodes[newIndex].Bounds.Lower = bounds.Lower - r;
            nodes[newIndex].Bounds.Upper = bounds.Upper + r;
            nodes[newIndex].Value = value;
            nodes[newIndex].Height = 0;
            //nodes[newIndex].IndexId = newIndex;

            InsertLeaf(newIndex);

            return newIndex;
        }
        
        public bool RemoveAt(int index)
        {
            Debug.Assert(0 <= index && index < this.nodeCapacity);
            Debug.Assert(this.nodes[index].IsLeaf());

            RemoveLeaf(index);
            FreeNode(index);

            return true;
        }

        public bool Move(int index, BoundingRectangle bounds)
        {
            Debug.Assert(0 <= index && index < nodeCapacity);
            Debug.Assert(nodes[index].IsLeaf());

            var node = nodes[index];
            var displacement = node.Bounds.Center - bounds.Center;

            if (nodes[index].Bounds.Contains(bounds) == ContainmentType.Contains)
                return false;

            RemoveLeaf(index);

            // Extend AABB.
            var r = new Vector2();

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

            nodes[index].Bounds = b;
            InsertLeaf(index);

            return true;
        }

        public void Clear()
        {
            this.nodeCapacity = 16;
            this.nodeCount = 0;
            this.root = NullNode;

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

        public float GetAreaRatio()
        {
            if (this.root == NullNode)
                return 0.0f;

            var root = this.nodes[this.root];
            var rootArea = root.Bounds.Perimeter;

            float totalArea = 0.0f;
            for (int i = 0; i < nodeCapacity; ++i)
            {
                var node = nodes[i];
                if (node.Height < 0)
                    continue; // Free node in pool

                totalArea += node.Bounds.Perimeter;
            }

            return totalArea / rootArea;
        }

        public int GetMaxBalance()
        {
            var maxBalance = 0;
            for (int i = 0; i < nodeCapacity; ++i)
            {
                var node = nodes[i];
                if (node.Height <= 1)
                    continue;

                Debug.Assert(node.IsLeaf() == false);

                int child1 = node.Child1;
                int child2 = node.Child2;
                int balance = Math.Abs(nodes[child2].Height - nodes[child1].Height);
                maxBalance = Math.Max(maxBalance, balance);
            }

            return maxBalance;
        }
        
        #region ISpatialQuery2D

        public int Raycast(ref Vector2 origin, ref Vector2 direction, ref RaycastHit<T>[] result, int startIndex, Func<T, float> callback = null, Stack<int> traverseStack = null)
        {
            throw new NotImplementedException();
        }

        public int FindAll(ref BoundingRectangle bounds, ref T[] result, int startIndex, Stack<int> traverseStack = null)
        {
            if (this.root == NullNode)
            {
                result = new T[0];
                return 0;
            }

            var resultIds = new List<int>();

            var nodeStack = new Queue<int>();
            nodeStack.Enqueue(this.root);

            while (nodeStack.Count > 0)
            {
                var nodeId = nodeStack.Dequeue();
                if (nodeId == NullNode) continue;

                var node = nodes[nodeId];

                resultIds.Add(nodeId);

                nodeStack.Enqueue(node.Child1);
                nodeStack.Enqueue(node.Child2);
            }

            result = new T[resultIds.Count];
            for (int i = 0; i < resultIds.Count; i++)
            {
                result[i] = nodes[resultIds[i]].Value;
            }

            return resultIds.Count;
        }
        
        #endregion
    }

    public struct DynamicTreeNode<T>
    {
        internal int Child1;
        internal int Child2;

        internal int Height;
        internal int ParentOrNext;

        public int Child1Id => Child1;
        public int Child2Id => Child2;

        public T Value;
        public BoundingRectangle Bounds;

        public bool IsLeaf() => this.Child1 == DynamicTree<T>.NullNode;

        public override string ToString()
        {
            return $"Child1: {Child1}, Child2: {Child2}, Value: {Value}";
        }
    }
}
