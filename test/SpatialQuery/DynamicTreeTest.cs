namespace Nine.Geometry.SpatialQuery
{
    using System.Diagnostics;
    using Xunit;
    using Xunit.Abstractions;

    public struct Actor
    {
        public BoundingRectangle Bounds;

        public Actor(BoundingRectangle bounds)
        {
            this.Bounds = bounds;
        }

        public override string ToString()
        {
            return Bounds.ToString();
        }
    }

    public class DynamicTreeTest
    {
        public static readonly Actor[] actors =
        {
            new Actor(new BoundingRectangle(100, 100)),
            new Actor(new BoundingRectangle(10, 0, 40, 80)),
            new Actor(new BoundingRectangle(60, 0, 40, 80)),
            new Actor(new BoundingRectangle(20, 10, 20, 20)),
            new Actor(new BoundingRectangle(20, 10, 20, 20)),
        };

        private DynamicTree<Actor> tree = new DynamicTree<Actor>();
        
        [Fact] // xunit Timeout?
        public void test_all()
        {
            // Add actors
            for (int i = 0; i < actors.Length - 1; i++)
            {
                var actor = actors[i];
                var result = tree.Add(ref actor.Bounds, actor);
            }

            PrintChildren(0, tree, tree.GetNodeAt(tree.RootId));
            tree.Traverse(e =>
            {
                Debug.WriteLine(e);
                return TraverseOptions.Continue;
            });
            
            // Remove actors
            tree.Clear();

            Assert.Equal(0, tree.NodeCount);
        }

        void PrintChildren(int depth, DynamicTree<Actor> tree, DynamicTreeNode<Actor> node)
        {
            Debug.WriteLine($"{new string('\t', depth)} -> {node.Index} [ {node.Child1Id} . {node.Child2Id} ]");

            var newDepth = depth + 1;
            if (node.Child1Id != -1) PrintChildren(newDepth, tree, tree.GetNodeAt(node.Child1Id));
            if (node.Child2Id != -1) PrintChildren(newDepth, tree, tree.GetNodeAt(node.Child2Id));
        }
    }
}
