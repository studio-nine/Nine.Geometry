namespace Nine.Geometry.SpatialQuery
{
    using System;
    using System.Diagnostics;

    partial class DynamicTree3D<T>
    {
        public int ComputeHeight() => this.ComputeHeight(this.root);
        public int ComputeHeight(int nodeId)
        {
            Debug.Assert(0 <= nodeId && nodeId < nodeCapacity);

            var node = nodes[nodeId];
            if (node.IsLeaf())
                return 0;

            var height1 = ComputeHeight(node.Child1);
            var height2 = ComputeHeight(node.Child2);
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

            var child1 = node.Child1;
            var child2 = node.Child2;

            if (node.IsLeaf())
            {
                Debug.Assert(child1 == NullNode);
                Debug.Assert(child2 == NullNode);
                Debug.Assert(node.Height == 0);
                return;
            }

            Debug.Assert(0 <= child1 && child1 < nodeCapacity);
            Debug.Assert(0 <= child2 && child2 < nodeCapacity);

            Debug.Assert(nodes[child1].ParentOrNext == index);
            Debug.Assert(nodes[child2].ParentOrNext == index);

            ValidateStructure(child1);
            ValidateStructure(child2);
        }

        protected void ValidateMetrics(int index)
        {
            if (index == NullNode)
                return;

            var node = nodes[index];

            var child1 = node.Child1;
            var child2 = node.Child2;

            if (node.IsLeaf())
            {
                Debug.Assert(child1 == NullNode);
                Debug.Assert(child2 == NullNode);
                Debug.Assert(node.Height == 0);
                return;
            }

            Debug.Assert(0 <= child1 && child1 < nodeCapacity);
            Debug.Assert(0 <= child2 && child2 < nodeCapacity);

            var height1 = nodes[child1].Height;
            var height2 = nodes[child2].Height;
            var height = 1 + Math.Max(height1, height2);
            Debug.Assert(node.Height == height);

            var bounds = BoundingBox.CreateMerged(nodes[child1].Bounds, nodes[child2].Bounds);

            Debug.Assert(bounds.Min == node.Bounds.Min);
            Debug.Assert(bounds.Max == node.Bounds.Max);

            ValidateMetrics(child1);
            ValidateMetrics(child2);
        }
    }
}
