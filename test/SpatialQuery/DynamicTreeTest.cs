namespace Nine.Geometry.SpatialQuery
{
    using System.Collections.Generic;
    using Xunit;

    public struct Actor
    {
        public BoundingRectangle Bounds;
    }

    public class DynamicTreeTest
    {
        private DynamicTree<Actor> tree = new DynamicTree<Actor>();

        [Fact]
        public void test_all()
        {
            const int count = 4;

            var actors = new List<DynamicTreeNode<Actor>>();

            // Add actors
            for (int i = 0; i < count; i++)
            {
                var actor = new Actor();
                actor.Bounds = new BoundingRectangle(0, 10 * i, 10, 10);

                var result = tree.Add(ref actor.Bounds, actor);
                actors.Add(result);
            }

            Assert.Equal(2, tree.Height); // fix? 

            // TODO: Query

            //// Move actors
            //for (int i = 0; i < actors.Count; i++)
            //{
            //    // Make bounds ref?
            //    tree.Move(actors[i], new BoundingRectangle(10, 10 * i, 10, 10));
            //}

            // TODO: Query

            // Remove actors
            // TODO: Fix clear
            foreach (var actor in actors)
            {
                tree.Remove(actor);
            }

            Assert.Equal(0, tree.Height);
        }
    }
}
