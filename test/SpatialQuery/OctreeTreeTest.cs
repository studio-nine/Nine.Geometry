namespace Nine.Geometry.SpatialQuery
{
    using System.Collections.Generic;
    using System.Numerics;
    using Xunit;

    public class OctreeTreeTest
    {
        public OctreeCollection octree;

        public OctreeTreeTest()
        {
            this.octree = new OctreeCollection();
        }

        [Fact]
        public void query_boundingbox()
        {
            this.octree.Clear();

            for (int i = 0; i < 10; i++)
            {
                //this.octree.Add(new SampleObject(
                //    new Vector3(10 * i, 10 * i, 10 * i),
                //    new Vector3(10, 10, 10)));
            }

            var queryBox = new BoundingBox(new Vector3(0, 0, 0), new Vector3(49, 49, 49));

            var result = new List<ISpatialQueryable>();
            octree.FindAll(ref queryBox, result);

            Assert.Equal(5, result.Count);
        }
    }
}
