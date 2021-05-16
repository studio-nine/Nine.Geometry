namespace Nine.Geometry.Test.SpatialQuery
{
    using Nine.Geometry.SpatialQuery;
    using System.Numerics;
    using Xunit;

    public class QuadTreeTest
    {
        [Fact]
        public void AddRemoveTest()
        {
            Octree<bool> oct = new Octree<bool>(new BoundingBox(Vector3.Zero, Vector3.One * 4), 2);

            oct.ExpandAll((o) => { return o.Bounds.Contains(Vector3.One * 0.1f) == ContainmentType.Contains; });

            object a = new object();
            object b = new object();

            QuadTree<object> tree = new QuadTree<object>(new BoundingRectangle(4, 4), 2);
        }

        [Fact]
        public void ExpandTest()
        {
            Assert.Equal(ContainmentType.Contains,
                new BoundingBox(Vector3.Zero, Vector3.One).Contains(Vector3.One));

            BoundingRectangle bounds = new BoundingRectangle(4, 4);

            QuadTree<object> tree = new QuadTree<object>(bounds, 2);

            Assert.Equal(bounds, tree.Bounds);
            Assert.Equal(bounds, tree.Root.Bounds);
            Assert.Equal(0, tree.Root.Depth);
        }
    }
}
