namespace Nine.Geometry.SpatialQuery
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;

    public enum TraverseOptions
    {
        /// <summary>
        /// The traverse operation should continue to visit the next node.
        /// </summary>
        Continue,

        /// <summary>
        /// The traverse operation should skip the current node and its child nodes.
        /// </summary>
        Skip,

        /// <summary>
        /// The traverse operation should stop visiting nodes.
        /// </summary>
        Stop,
    }

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

        public int Find(BoundingRectangle rectangle, T[] output, int startIndex)
        {
            throw new NotImplementedException();
        }


        static DynamicTreeNode<T>[] Stack = new DynamicTreeNode<T>[64];
        static int StackCount = 0;

        public void Traverse(Func<DynamicTreeNode<T>, TraverseOptions> result)
        {
            if (root == NullNode)
                throw new ArgumentNullException();

            this.Traverse(nodes[root], result);
        }

        public void Traverse(DynamicTreeNode<T> target, Func<DynamicTreeNode<T>, TraverseOptions> result)
        {
            StackCount = 0;
            Stack[StackCount++] = target;

            while (StackCount > 0)
            {
                DynamicTreeNode<T> node = Stack[--StackCount];
                var traverseOptions = result(node);
                if (traverseOptions == TraverseOptions.Stop)
                    break;

                if (traverseOptions == TraverseOptions.Continue && node.Height > 0)
                {
                    var count = node.Height;
                    var requiredCpacity = count + StackCount;
                    if (requiredCpacity > Stack.Length)
                        Array.Resize(ref Stack, Math.Max(Stack.Length * 2, requiredCpacity));
                    
                    if (count >= 1) Stack[StackCount++] = nodes[node.Child1];
                    if (count >= 2) Stack[StackCount++] = nodes[node.Child2];
                }
            }
        }

        #region ISpatialQuery2D

        public int Raycast(ref Vector2 origin, ref Vector2 direction, ref RaycastHit<T>[] result, int startIndex, Func<T, float> callback = null)
        {
            throw new NotImplementedException();
        }

        public int FindAll(ref BoundingRectangle bounds, ref T[] result, int startIndex)
        {
            throw new NotImplementedException();

            //if (this.RootId == NullNode)
            //{
            //    result = new T[0];
            //    startIndex = 0;
            //    return 0;
            //}

            //this.Traverse(e =>
            //{
            //    var root = this.Root;

            //    var containsResult = ContainmentType.Disjoint;
            //    Intersection.Contains(ref root.Bounds, ref bounds, out containsResult);

            //    switch (containsResult)
            //    {
            //        case ContainmentType.Contains:
            //            // TODO: Add all children
            //            return TraverseOptions.Skip;

            //        case ContainmentType.Intersects: return TraverseOptions.Continue;
            //        case ContainmentType.Disjoint: return TraverseOptions.Stop;
            //    }

            //    throw new ArgumentException();
            //});

            //return 0;
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
