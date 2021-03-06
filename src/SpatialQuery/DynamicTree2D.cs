﻿namespace Nine.Geometry.SpatialQuery
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Numerics;

    public class DynamicTree2D<T> : ISpatialQuery2D<T>
    {
        internal const int NullNode = -1;

        /// <summary> Compute the height of the binary tree in O(N) time. </summary>
        public int Height => (this.root == NullNode) ? 0 : this.nodes[this.root].Height;

        /// <summary> </summary>
        public float AreaRatio => GetAreaRatio();

        /// <summary> </summary>
        public int MaxBalance => GetMaxBalance();

        /// <summary> Gets the number of nodes. </summary>
        public int NodeCount => nodeCount;

        /// <summary> Gets the root id. </summary>
        public int RootId => root;

        /// <summary> 
        /// Gets the root node. 
        /// </summary>
        public DynamicTreeNode Root
        {
            get
            {
                if (root == NullNode) throw new ArgumentException("There is no root node.");
                return nodes[root];
            }
        }

        public readonly float Margin;

        private readonly Stack<int> raycastStack;
        private readonly Stack<int> queryStack;

        private DynamicTreeNode[] nodes;

        private int freeList;
        private int nodeCapacity;
        private int nodeCount;
        private int root;

        /// <summary>
        /// 
        /// </summary>
        public DynamicTree2D(float margin = 1)
        {
            if (margin <= 0) throw new ArgumentOutOfRangeException(nameof(margin));

            this.Margin = margin;
            this.raycastStack = new Stack<int>(256);
            this.queryStack = new Stack<int>(256);

            this.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public DynamicTreeNode GetNodeAt(int index) => nodes[index];

        /// <summary>
        /// 
        /// </summary>
        public int Add(ref BoundingRectangle bounds, T value)
        {
            var newIndex = AllocateNode();

            var r = new Vector2(Margin);

            nodes[newIndex].Bounds = new BoundingRectangle(bounds.Upper - r, bounds.Lower + r);
            nodes[newIndex].Value = value;
            nodes[newIndex].Height = 0;

            InsertLeaf(newIndex);

            return newIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RemoveAt(int index)
        {
            Debug.Assert(0 <= index && index < this.nodeCapacity);
            Debug.Assert(this.nodes[index].IsLeaf());

            RemoveLeaf(index);
            FreeNode(index);

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Move(int index, BoundingRectangle bounds)
        {
            Debug.Assert(0 <= index && index < nodeCapacity);
            Debug.Assert(nodes[index].IsLeaf());

            var node = nodes[index];
            var displacement = node.Bounds.Center - bounds.Center;

            if (nodes[index].Bounds.Contains(bounds) == ContainmentType.Intersects)
                return false;

            RemoveLeaf(index);

            // Extend AABB.
            var r = new Vector2(Margin);
            var b = new BoundingRectangle(bounds.Lower - r, bounds.Upper + r);

            // Predict AABB displacement.
            var d = displacement * 2;

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

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.nodeCapacity = 16;
            this.nodeCount = 0;
            this.root = NullNode;

            this.nodes = new DynamicTreeNode[nodeCapacity];

            // Build a linked list for the free list.
            for (int i = 0; i < nodeCapacity - 1; ++i)
            {
                this.nodes[i] = new DynamicTreeNode();
                this.nodes[i].ParentOrNext = i + 1;
                this.nodes[i].Height = 1;
            }

            this.nodes[nodeCapacity - 1] = new DynamicTreeNode();
            this.nodes[nodeCapacity - 1].ParentOrNext = NullNode;
            this.nodes[nodeCapacity - 1].Height = 1;
            this.freeList = 0;
        }

        private float GetAreaRatio()
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

        private int GetMaxBalance()
        {
            var maxBalance = 0;
            for (int i = 0; i < nodeCapacity; ++i)
            {
                var node = nodes[i];
                if (node.Height <= 1)
                    continue;

                Debug.Assert(node.IsLeaf() == false);

                int Child1Id = node.Child1Id;
                int Child2Id = node.Child2Id;
                int balance = Math.Abs(nodes[Child2Id].Height - nodes[Child1Id].Height);
                maxBalance = Math.Max(maxBalance, balance);
            }

            return maxBalance;
        }

        #region ISpatialQuery2D
        public int Raycast(ref Vector2 origin, ref Vector2 direction, ref RaycastHit<T>[] result, int startIndex, Func<T, float> callback = null, Stack<int> traverseStack = null)
        {
            var p1 = origin;
            var p2 = origin + direction;
            return RaycastBetween(ref p1, ref p2, ref result, startIndex, callback, traverseStack);
        }

        public int RaycastBetween(ref Vector2 p1, ref Vector2 p2, ref RaycastHit<T>[] result, int startIndex, Func<T, float> callback = null, Stack<int> traverseStack = null)
        {
            result = new RaycastHit<T>[8];

            int resultCount = 0;

            var maxFraction = float.MaxValue / 2;

            var r = p2 - p1;
            Debug.Assert(r.LengthSquared() > 0.0f);
            r = Vector2.Normalize(r);

            // v is perpendicular to the segment.
            var v = new Vector2(-r.Y, r.X);
            var absV = new Vector2(Math.Abs(v.Y), Math.Abs(v.X));

            // Build a bounding box for the segment.
            var aabbt = p1 + maxFraction * (p2 - p1);
            var segmentAABB = new BoundingRectangle(Vector2.Min(p1, aabbt), Vector2.Max(p1, aabbt));

            var nodeStack = new Stack<int>();
            nodeStack.Push(startIndex);

            while (nodeStack.Count > 0)
            {
                var nodeId = nodeStack.Pop();
                if (nodeId == NullNode)
                    continue;

                var node = nodes[nodeId];
                if (segmentAABB.Contains(node.Bounds) == ContainmentType.Disjoint)
                    continue;

                // Separating axis for segment (Gino, p80).
                // |dot(v, p1 - c)| > dot(|v|, h)
                var c = node.Bounds.Center;
                var h = node.Bounds.Size;
                var separation = Math.Abs(Vector2.Dot(v, p1 - c)) - Vector2.Dot(absV, h);
                if (separation > 0.0f)
                {
                    continue;
                }

                if (node.IsLeaf())
                {
                    var nodeValue = nodes[nodeId].Value;
                    if (resultCount >= result.Length)
                        Array.Resize(ref result, result.Length * 2);

                    result[resultCount] = new RaycastHit<T>(nodeValue, separation);
                    resultCount++;

                    if (callback != null)
                    {
                        var value = callback(nodeValue);
                        if (value == float.NaN)
                        {
                            // The client has terminated the ray cast.

                            if (result.Length > resultCount)
                                Array.Resize(ref result, resultCount);

                            return 0;
                        }
                        else
                        {
                            // Update segment bounding box.
                            maxFraction = value;
                            var t = p1 + maxFraction * (p2 - p1);
                            segmentAABB = new BoundingRectangle(Vector2.Min(p1, t), Vector2.Max(p1, t));
                        }
                    }
                }
                else
                {
                    nodeStack.Push(node.Child1Id);
                    nodeStack.Push(node.Child2Id);
                }
            }

            if (result.Length > resultCount)
                Array.Resize(ref result, resultCount);

            return resultCount;
        }

        public int FindAll(ref BoundingRectangle bounds, ref T[] result, int? startIndex = null, Stack<int> traverseStack = null)
        {
            // TODO: traverseStack

            if (startIndex == NullNode)
            {
                result = new T[0];
                return 0;
            }

            var results = new List<int>();

            var nodeStack = new Stack<int>();
            nodeStack.Push(startIndex ?? RootId);

            while (nodeStack.Count > 0)
            {
                var nodeId = nodeStack.Pop();
                if (nodeId == NullNode)
                    continue;

                var node = nodes[nodeId];
                if (bounds.Contains(node.Bounds) == ContainmentType.Disjoint)
                    continue;

                if (node.IsLeaf())
                    results.Add(nodeId);

                nodeStack.Push(node.Child1Id);
                nodeStack.Push(node.Child2Id);
            }

            result = new T[results.Count];
            for (int i = 0; i < results.Count; i++)
            {
                result[i] = nodes[results[i]].Value;
            }

            return results.Count;
        }
        #endregion

        #region Collection
        private void InsertLeaf(int leaf)
        {
            if (root == NullNode)
            {
                root = leaf;
                nodes[root].ParentOrNext = NullNode;
                return;
            }

            // Find the best sibling for this node
            var leafAABB = nodes[leaf].Bounds;
            int sibling = root;
            while (nodes[sibling].IsLeaf() == false)
            {
                int Child1Id = nodes[sibling].Child1Id;
                int Child2Id = nodes[sibling].Child2Id;

                // Expand the node's AABB.
                //nodes[sibling].Bounds = BoundingRectangle.CreateMerged(nodes[sibling].Bounds, leafAABB);
                nodes[sibling].Height += 1;

                var siblingArea = nodes[sibling].Bounds.Perimeter;

                var parentAABB = BoundingRectangle.CreateMerged(nodes[sibling].Bounds, leafAABB);
                var parentArea = parentAABB.Perimeter;

                // Cost of creating a new parent for this node and the new leaf
                var cost = 2.0f * parentArea;

                // Minimum cost of pushing the leaf further down the tree
                var inheritanceCost = 2.0f * (parentArea - siblingArea);

                // Cost of descending into Child1Id
                var leafAABBChild1 = BoundingRectangle.CreateMerged(leafAABB, nodes[Child1Id].Bounds);
                var cost1 = (nodes[Child1Id].IsLeaf() ? leafAABBChild1.Perimeter : (leafAABBChild1.Perimeter - nodes[Child1Id].Bounds.Perimeter)) + inheritanceCost;

                // Cost of descending into Child2Id
                var leafAABBChild2 = BoundingRectangle.CreateMerged(leafAABB, nodes[Child2Id].Bounds);
                var cost2 = (nodes[Child2Id].IsLeaf() ? leafAABBChild2.Perimeter : (leafAABBChild2.Perimeter - nodes[Child2Id].Bounds.Perimeter)) + inheritanceCost;
                
                // Descend according to the minimum cost.
                if (cost < cost1 && cost < cost2)
                {
                    break;
                }

                // Expand the node's AABB to account for the new leaf.
                nodes[sibling].Bounds = BoundingRectangle.CreateMerged(nodes[sibling].Bounds, leafAABB);

                // Descend
                if (cost1 < cost2)
                {
                    sibling = Child1Id;
                }
                else
                {
                    sibling = Child2Id;
                }
            }

            // Create a new parent for the siblings.
            int oldParent = nodes[sibling].ParentOrNext;
            int newParent = AllocateNode();
            nodes[newParent].ParentOrNext = oldParent;
            nodes[newParent].Value = default(T);
            nodes[newParent].Bounds = BoundingRectangle.CreateMerged(leafAABB, nodes[sibling].Bounds);
            nodes[newParent].Height = nodes[sibling].Height + 1;

            if (oldParent != NullNode)
            {
                // The sibling was not the root.
                if (nodes[oldParent].Child1Id == sibling)
                {
                    nodes[oldParent].Child1Id = newParent;
                }
                else
                {
                    nodes[oldParent].Child2Id = newParent;
                }

                nodes[newParent].Child1Id = sibling;
                nodes[newParent].Child2Id = leaf;
                nodes[sibling].ParentOrNext = newParent;
                nodes[leaf].ParentOrNext = newParent;
            }
            else
            {
                // The sibling was the root.
                nodes[newParent].Child1Id = sibling;
                nodes[newParent].Child2Id = leaf;
                nodes[sibling].ParentOrNext = newParent;
                nodes[leaf].ParentOrNext = newParent;
                root = newParent;
            }

            // Walk back up the tree fixing heights and AABBs
            int index = sibling;
            index = nodes[leaf].ParentOrNext;
            while (index != NullNode)
            {
                index = Balance(index);

                int Child1Id = nodes[index].Child1Id;
                int Child2Id = nodes[index].Child2Id;

                Debug.Assert(Child1Id != NullNode);
                Debug.Assert(Child2Id != NullNode);

                nodes[index].Height = 1 + Math.Max(nodes[Child1Id].Height, nodes[Child2Id].Height);
                nodes[index].Bounds = BoundingRectangle.CreateMerged(nodes[Child1Id].Bounds, nodes[Child2Id].Bounds);

                index = nodes[index].ParentOrNext;
            }

            //Validate();
        }

        private bool RemoveLeaf(int leaf)
        {
            if (leaf == root)
            {
                this.root = NullNode;
                return true;
            }

            var parent = nodes[leaf].ParentOrNext;
            var grandParent = nodes[parent].ParentOrNext;
            var sibling = (nodes[parent].Child1Id == leaf) ? nodes[parent].Child2Id : nodes[parent].Child1Id;

            if (grandParent != NullNode)
            {
                // Destroy parent and connect sibling to grandParent.
                if (nodes[grandParent].Child1Id == parent)
                {
                    nodes[grandParent].Child1Id = sibling;
                }
                else
                {
                    nodes[grandParent].Child2Id = sibling;
                }

                nodes[sibling].ParentOrNext = grandParent;
                FreeNode(parent);

                // Adjust ancestor bounds.
                var index = grandParent;
                while (index != NullNode)
                {
                    index = Balance(index);

                    int Child1Id = nodes[index].Child1Id;
                    int Child2Id = nodes[index].Child2Id;

                    nodes[index].Bounds = BoundingRectangle.CreateMerged(nodes[Child1Id].Bounds, nodes[Child2Id].Bounds);
                    nodes[index].Height = 1 + Math.Max(nodes[Child1Id].Height, nodes[Child2Id].Height);

                    index = nodes[index].ParentOrNext;
                }
            }
            else
            {
                this.root = sibling;
                nodes[sibling].ParentOrNext = NullNode;
                FreeNode(parent);
            }

            Validate();

            // TODO:
            return false;
        }

        private int Balance(int iA)
        {
            Debug.Assert(iA != NullNode);

            var A = nodes[iA];
            if (A.IsLeaf() || A.Height < 2)
                return iA;

            int iB = A.Child1Id;
            int iC = A.Child2Id;

            Debug.Assert(0 <= iB && iB < nodeCapacity);
            Debug.Assert(0 <= iC && iC < nodeCapacity);

            var B = nodes[iB];
            var C = nodes[iC];

            int balance = C.Height - B.Height;

            if (balance > 1)
            {
                int iF = C.Child1Id;
                int iG = C.Child2Id;
                var F = nodes[iF];
                var G = nodes[iG];
                Debug.Assert(0 <= iF && iF < nodeCapacity);
                Debug.Assert(0 <= iG && iG < nodeCapacity);

                // Swap A and C
                C.Child1Id = iA;
                C.ParentOrNext = A.ParentOrNext;
                A.ParentOrNext = iC;

                // A's old parent should point to C
                if (C.ParentOrNext != NullNode)
                {
                    if (nodes[C.ParentOrNext].Child1Id == iA)
                    {
                        nodes[C.ParentOrNext].Child1Id = iC;
                    }
                    else
                    {
                        Debug.Assert(nodes[C.ParentOrNext].Child2Id == iA);
                        nodes[C.ParentOrNext].Child2Id = iC;
                    }
                }
                else
                {
                    this.root = iC;
                }

                // Rotate
                if (F.Height > G.Height)
                {
                    C.Child2Id = iF;
                    A.Child2Id = iG;
                    G.ParentOrNext = iA;
                    A.Bounds = BoundingRectangle.CreateMerged(B.Bounds, G.Bounds);
                    C.Bounds = BoundingRectangle.CreateMerged(A.Bounds, F.Bounds);

                    A.Height = 1 + Math.Max(B.Height, G.Height);
                    C.Height = 1 + Math.Max(A.Height, F.Height);
                }
                else
                {
                    C.Child2Id = iG;
                    A.Child2Id = iF;
                    F.ParentOrNext = iA;
                    A.Bounds = BoundingRectangle.CreateMerged(B.Bounds, F.Bounds);
                    C.Bounds = BoundingRectangle.CreateMerged(A.Bounds, G.Bounds);

                    A.Height = 1 + Math.Max(B.Height, F.Height);
                    C.Height = 1 + Math.Max(A.Height, G.Height);
                }

                return iC;
            }
            else if (balance < -1)
            {
                int iD = B.Child1Id;
                int iE = B.Child2Id;

                var D = nodes[iD];
                var E = nodes[iE];

                Debug.Assert(0 <= iD && iD < nodeCapacity);
                Debug.Assert(0 <= iE && iE < nodeCapacity);

                // Swap A and B
                B.Child1Id = iA;
                B.ParentOrNext = A.ParentOrNext;
                A.ParentOrNext = iB;

                // A's old parent should point to B
                if (B.ParentOrNext != NullNode)
                {
                    if (nodes[B.ParentOrNext].Child1Id == iA)
                    {
                        nodes[B.ParentOrNext].Child1Id = iB;
                    }
                    else
                    {
                        Debug.Assert(nodes[B.ParentOrNext].Child2Id == iA);
                        nodes[B.ParentOrNext].Child2Id = iB;
                    }
                }
                else
                {
                    this.root = iB;
                }

                // Rotate
                if (D.Height > E.Height)
                {
                    B.Child2Id = iD;
                    A.Child1Id = iE;
                    E.ParentOrNext = iA;
                    A.Bounds = BoundingRectangle.CreateMerged(C.Bounds, E.Bounds);
                    B.Bounds = BoundingRectangle.CreateMerged(A.Bounds, D.Bounds);

                    A.Height = 1 + Math.Max(C.Height, E.Height);
                    B.Height = 1 + Math.Max(A.Height, D.Height);
                }
                else
                {
                    B.Child2Id = iE;
                    A.Child1Id = iD;
                    D.ParentOrNext = iA;
                    A.Bounds = BoundingRectangle.CreateMerged(C.Bounds, D.Bounds);
                    B.Bounds = BoundingRectangle.CreateMerged(A.Bounds, E.Bounds);

                    A.Height = 1 + Math.Max(C.Height, D.Height);
                    B.Height = 1 + Math.Max(A.Height, E.Height);
                }

                return iB;
            }

            return iA;
        }

        private void FreeNode(int nodeId)
        {
            Debug.Assert(0 <= nodeId && nodeId < nodeCapacity);
            Debug.Assert(0 < nodeCount);

            nodes[nodeId].ParentOrNext = freeList;
            nodes[nodeId].Height = -1;
            freeList = nodeId;
            --nodeCount;
        }

        private int AllocateNode()
        {
            // Expand the node pool as needed.
            if (this.freeList == NullNode)
            {
                Debug.Assert(nodeCount == nodeCapacity);

                // The free list is empty. Rebuild a bigger pool.
                DynamicTreeNode[] oldNodes = nodes;
                nodeCapacity *= 2;
                nodes = new DynamicTreeNode[nodeCapacity];
                Array.Copy(oldNodes, nodes, nodeCount);

                // Build a linked list for the free list. The parent
                // pointer becomes the "next" pointer.
                for (var i = nodeCount; i < nodeCapacity - 1; ++i)
                {
                    nodes[i] = new DynamicTreeNode();
                    nodes[i].ParentOrNext = i + 1;
                    nodes[i].Height = -1;
                }

                nodes[nodeCapacity - 1] = new DynamicTreeNode();
                nodes[nodeCapacity - 1].ParentOrNext = NullNode;
                nodes[nodeCapacity - 1].Height = -1;
                freeList = nodeCount;
            }

            // Peel a node off the free list.
            var nodeId = freeList;
            freeList = nodes[nodeId].ParentOrNext;
            nodes[nodeId].ParentOrNext = NullNode;
            nodes[nodeId].Child1Id = NullNode;
            nodes[nodeId].Child2Id = NullNode;
            nodes[nodeId].Height = 0;
            nodes[nodeId].Value = default(T);

            ++nodeCount;

            return nodeId;
        }
        #endregion

        #region Validation
        public int ComputeHeight() => this.ComputeHeight(this.root);
        public int ComputeHeight(int nodeId)
        {
            Debug.Assert(0 <= nodeId && nodeId < nodeCapacity);

            var node = nodes[nodeId];
            if (node.IsLeaf())
                return 0;

            var height1 = ComputeHeight(node.Child1Id);
            var height2 = ComputeHeight(node.Child2Id);
            return 1 + Math.Max(height1, height2);
        }

        [Conditional("DEBUG")]
        protected void Validate()
        {
            ValidateStructure(root);
            ValidateMetrics(root);

            int freeCount = 0;
            int freeIndex = freeList;
            while (freeIndex != NullNode)
            {
                Debug.Assert(0 <= freeIndex && freeIndex < nodeCapacity);
                freeIndex = nodes[freeIndex].ParentOrNext;
                ++freeCount;
            }

            Debug.Assert(Height == ComputeHeight());
            Debug.Assert((nodeCount + freeCount) == nodeCapacity);
        }

        protected void ValidateStructure(int index)
        {
            if (index == NullNode)
                return;

            if (index == root)
                Debug.Assert(nodes[index].ParentOrNext == NullNode);

            var node = nodes[index];

            var Child1Id = node.Child1Id;
            var Child2Id = node.Child2Id;

            if (node.IsLeaf())
            {
                Debug.Assert(Child1Id == NullNode);
                Debug.Assert(Child2Id == NullNode);
                Debug.Assert(node.Height == 0);
                return;
            }

            Debug.Assert(0 <= Child1Id && Child1Id < nodeCapacity);
            Debug.Assert(0 <= Child2Id && Child2Id < nodeCapacity);

            Debug.Assert(nodes[Child1Id].ParentOrNext == index);
            Debug.Assert(nodes[Child2Id].ParentOrNext == index);

            ValidateStructure(Child1Id);
            ValidateStructure(Child2Id);
        }

        protected void ValidateMetrics(int index)
        {
            if (index == NullNode)
                return;

            var node = nodes[index];

            var Child1Id = node.Child1Id;
            var Child2Id = node.Child2Id;

            if (node.IsLeaf())
            {
                Debug.Assert(Child1Id == NullNode);
                Debug.Assert(Child2Id == NullNode);
                Debug.Assert(node.Height == 0);
                return;
            }

            Debug.Assert(0 <= Child1Id && Child1Id < nodeCapacity);
            Debug.Assert(0 <= Child2Id && Child2Id < nodeCapacity);

            var height1 = nodes[Child1Id].Height;
            var height2 = nodes[Child2Id].Height;
            var height = 1 + Math.Max(height1, height2);
            Debug.Assert(node.Height == height);

            var bounds = BoundingRectangle.CreateMerged(nodes[Child1Id].Bounds, nodes[Child2Id].Bounds);

            Debug.Assert(bounds.Lower == node.Bounds.Lower);
            Debug.Assert(bounds.Upper == node.Bounds.Upper);

            ValidateMetrics(Child1Id);
            ValidateMetrics(Child2Id);
        }
        #endregion

        public struct DynamicTreeNode
        {
            internal int Height;
            internal int ParentOrNext;

            public int Child1Id { get; internal set; }
            public int Child2Id { get; internal set; }

            public T Value;
            public BoundingRectangle Bounds;

            public bool IsLeaf() => this.Child1Id == NullNode;

            public override string ToString()
            {
                return $"Child1Id: { Child1Id }, Child2Id: { Child2Id }, Value: { Value }";
            }
        }
    }
}
