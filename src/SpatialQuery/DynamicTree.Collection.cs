namespace Nine.Geometry.SpatialQuery
{
    using System;
    using System.Diagnostics;

    partial class DynamicTree<T>
    {
        private void InsertLeaf(int leaf)
        {
            if (root == NullNode)
            {
                this.root = leaf;
                //nodes[root].IndexId = leaf;
                nodes[root].ParentOrNext = NullNode;
                return;
            }

            // Find the best sibling for this node
            var leafBounds = nodes[leaf].Bounds;
            var index = root;
            while (nodes[index].IsLeaf() == false)
            {
                var child1 = nodes[index].Child1;
                var child2 = nodes[index].Child2;

                var area = nodes[index].Bounds.Perimeter;

                var newBounds = BoundingRectangle.CreateMerged(nodes[index].Bounds, leafBounds);
                var combinedArea = newBounds.Perimeter;

                // Cost of creating a new parent for this node and the new leaf
                var cost = 2.0f * combinedArea;

                // Minimum cost of pushing the leaf further down the tree
                var inheritanceCost = 2.0f * (combinedArea - area);

                // Cost of descending into child1
                float cost1;
                if (nodes[child1].IsLeaf())
                {
                    var newBounds2 = BoundingRectangle.CreateMerged(nodes[child1].Bounds, leafBounds);
                    cost1 = newBounds2.Perimeter + inheritanceCost;
                }
                else
                {
                    var newBounds2 = BoundingRectangle.CreateMerged(nodes[child1].Bounds, leafBounds);
                    cost1 = newBounds2.Perimeter + inheritanceCost;

                    var oldArea = nodes[child1].Bounds.Perimeter;
                    var newArea = newBounds2.Perimeter;
                    cost1 = (newArea - oldArea) + inheritanceCost;
                }

                // Cost of descending into child2
                float cost2;
                if (nodes[child2].IsLeaf())
                {
                    var newBounds2 = BoundingRectangle.CreateMerged(nodes[child2].Bounds, leafBounds);
                    cost2 = newBounds2.Perimeter + inheritanceCost;
                }
                else
                {
                    var newBounds2 = BoundingRectangle.CreateMerged(nodes[child2].Bounds, leafBounds);
                    cost2 = newBounds2.Perimeter + inheritanceCost;
                    
                    var oldArea = nodes[child2].Bounds.Perimeter;
                    var newArea = newBounds2.Perimeter;
                    cost2 = newArea - oldArea + inheritanceCost;
                }

                // Descend according to the minimum cost.
                if (cost < cost1 && cost1 < cost2)
                    break;

                // Descend
                index = (cost1 < cost2) ? child1 : child2;
            }

            int sibling = index;

            // Create a new parent.
            int oldParent = nodes[sibling].ParentOrNext;
            int newParent = Allocate();

            //nodes[newParent].IndexId = newParent;
            nodes[newParent].ParentOrNext = oldParent;
            nodes[newParent].Value = default(T);
            nodes[newParent].Bounds = BoundingRectangle.CreateMerged(nodes[sibling].Bounds, leafBounds);
            nodes[newParent].Height = nodes[sibling].Height + 1;

            if (oldParent != NullNode)
            {
                // The sibling was not the root.
                if (nodes[oldParent].Child1 == sibling)
                {
                    nodes[oldParent].Child1 = newParent;
                }
                else
                {
                    nodes[oldParent].Child2 = newParent;
                }

                nodes[newParent].Child1 = sibling;
                nodes[newParent].Child2 = leaf;
                nodes[sibling].ParentOrNext = newParent;
                nodes[leaf].ParentOrNext = newParent;
            }
            else
            {
                // The sibling was the root.
                nodes[newParent].Child1 = sibling;
                nodes[newParent].Child2 = leaf;
                nodes[sibling].ParentOrNext = newParent;
                nodes[leaf].ParentOrNext = newParent;
                root = newParent;
            }

            // Walk back up the tree fixing heights and AABBs
            index = nodes[leaf].ParentOrNext;
            while (index != NullNode)
            {
                index = Balance(index);

                int child1 = nodes[index].Child1;
                int child2 = nodes[index].Child2;

                Debug.Assert(child1 != NullNode);
                Debug.Assert(child2 != NullNode);

                nodes[index].Height = 1 + Math.Max(nodes[child1].Height, nodes[child2].Height);
                nodes[index].Bounds = BoundingRectangle.CreateMerged(nodes[child1].Bounds, nodes[child2].Bounds);

                index = nodes[index].ParentOrNext;
            }

            Validate();
        }

        private bool RemoveLeaf(int leaf)
        {
            if (leaf == root)
            {
                root = NullNode;
                return true;
            }

            var parent = nodes[leaf].ParentOrNext;
            var grandParent = nodes[parent].ParentOrNext;
            var sibling = (nodes[parent].Child1 == leaf) ? nodes[parent].Child2 : nodes[parent].Child1;

            if (grandParent != NullNode)
            {
                // Destroy parent and connect sibling to grandParent.
                if (nodes[grandParent].Child1 == parent)
                {
                    nodes[grandParent].Child1 = sibling;
                }
                else
                {
                    nodes[grandParent].Child2 = sibling;
                }

                nodes[sibling].ParentOrNext = grandParent;
                FreeNode(parent);

                // Adjust ancestor bounds.
                var index = grandParent;
                while (index != NullNode)
                {
                    index = Balance(index);

                    int child1 = nodes[index].Child1;
                    int child2 = nodes[index].Child2;

                    nodes[index].Bounds = BoundingRectangle.CreateMerged(nodes[child1].Bounds, nodes[child2].Bounds);
                    nodes[index].Height = 1 + Math.Max(nodes[child1].Height, nodes[child2].Height);

                    index = nodes[index].ParentOrNext;
                }
            }
            else
            {
                root = sibling;
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

            int iB = A.Child1;
            int iC = A.Child2;

            Debug.Assert(0 <= iB && iB < nodeCapacity);
            Debug.Assert(0 <= iC && iC < nodeCapacity);

            var B = nodes[iB];
            var C = nodes[iC];

            int balance = C.Height - B.Height;

            // Rotate C up
            if (balance > 1)
            {
                int iF = C.Child1;
                int iG = C.Child2;

                var F = nodes[iF];
                var G = nodes[iG];

                Debug.Assert(0 <= iF && iF < nodeCapacity);
                Debug.Assert(0 <= iG && iG < nodeCapacity);

                // Swap A and C
                C.Child1 = iA;
                C.ParentOrNext = A.ParentOrNext;
                A.ParentOrNext = iC;

                // A's old parent should point to C
                if (C.ParentOrNext != NullNode)
                {
                    if (nodes[C.ParentOrNext].Child1 == iA)
                    {
                        nodes[C.ParentOrNext].Child1 = iC;
                    }
                    else
                    {
                        Debug.Assert(nodes[C.ParentOrNext].Child2 == iA);
                        nodes[C.ParentOrNext].Child2 = iC;
                    }
                }
                else
                {
                    root = iC;
                }

                // Rotate
                if (F.Height > G.Height)
                {
                    C.Child2 = iF;
                    A.Child2 = iG;
                    G.ParentOrNext = iA;
                    A.Bounds = BoundingRectangle.CreateMerged(B.Bounds, G.Bounds);
                    C.Bounds = BoundingRectangle.CreateMerged(A.Bounds, F.Bounds);

                    A.Height = 1 + Math.Max(B.Height, G.Height);
                    C.Height = 1 + Math.Max(A.Height, F.Height);
                }
                else
                {
                    C.Child2 = iG;
                    A.Child2 = iF;
                    F.ParentOrNext = iA;
                    A.Bounds = BoundingRectangle.CreateMerged(B.Bounds, F.Bounds);
                    C.Bounds = BoundingRectangle.CreateMerged(A.Bounds, G.Bounds);

                    A.Height = 1 + Math.Max(B.Height, F.Height);
                    C.Height = 1 + Math.Max(A.Height, G.Height);
                }

                return iC;
            }

            // Rotate B up
            if (balance < -1)
            {
                int iD = B.Child1;
                int iE = B.Child2;

                var D = nodes[iD];
                var E = nodes[iE];

                Debug.Assert(0 <= iD && iD < nodeCapacity);
                Debug.Assert(0 <= iE && iE < nodeCapacity);

                // Swap A and B
                B.Child1 = iA;
                B.ParentOrNext = A.ParentOrNext;
                A.ParentOrNext = iB;

                // A's old parent should point to B
                if (B.ParentOrNext != NullNode)
                {
                    if (nodes[B.ParentOrNext].Child1 == iA)
                    {
                        nodes[B.ParentOrNext].Child1 = iB;
                    }
                    else
                    {
                        Debug.Assert(nodes[B.ParentOrNext].Child2 == iA);
                        nodes[B.ParentOrNext].Child2 = iB;
                    }
                }
                else
                {
                    root = iB;
                }

                // Rotate
                if (D.Height > E.Height)
                {
                    B.Child2 = iD;
                    A.Child1 = iE;
                    E.ParentOrNext = iA;
                    A.Bounds = BoundingRectangle.CreateMerged(C.Bounds, E.Bounds);
                    B.Bounds = BoundingRectangle.CreateMerged(A.Bounds, D.Bounds);

                    A.Height = 1 + Math.Max(C.Height, E.Height);
                    B.Height = 1 + Math.Max(A.Height, D.Height);
                }
                else
                {
                    B.Child2 = iE;
                    A.Child1 = iD;
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

        private int Allocate()
        {
            // Expand the node pool as needed.
            if (this.freeList == NullNode)
            {
                Debug.Assert(nodeCount == nodeCapacity);

                // The free list is empty. Rebuild a bigger pool.
                DynamicTreeNode<T>[] oldNodes = nodes;
                nodeCapacity *= 2;
                nodes = new DynamicTreeNode<T>[nodeCapacity];
                Array.Copy(oldNodes, nodes, nodeCount);

                // Build a linked list for the free list. The parent
                // pointer becomes the "next" pointer.
                for (var i = nodeCount; i < nodeCapacity - 1; ++i)
                {
                    nodes[i] = new DynamicTreeNode<T>();
                    nodes[i].ParentOrNext = i + 1;
                    nodes[i].Height = -1;
                }

                nodes[nodeCapacity - 1] = new DynamicTreeNode<T>();
                nodes[nodeCapacity - 1].ParentOrNext = NullNode;
                nodes[nodeCapacity - 1].Height = -1;
                freeList = nodeCount;
            }

            // Peel a node off the free list.
            var nodeId = freeList;
            freeList = nodes[nodeId].ParentOrNext;
            nodes[nodeId].ParentOrNext = NullNode;
            nodes[nodeId].Child1 = NullNode;
            nodes[nodeId].Child2 = NullNode;
            nodes[nodeId].Height = 0;
            nodes[nodeId].Value = default(T);

            ++nodeCount;

            return nodeId;
        }
    }
}
