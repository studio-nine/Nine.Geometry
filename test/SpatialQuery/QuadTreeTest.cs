namespace Nine.Geometry.SpatialQuery
{
    using System.Collections.Generic;
    using System.Numerics;
    using Xunit;

    public class QuadTreeTest
    {
        public QuadTreeCollection quadtree;

        public QuadTreeTest()
        {
            this.quadtree = new QuadTreeCollection();
        }

        [Fact]
        public void query_boundingbox()
        {
            this.quadtree.Clear();

            for (int i = 0; i < 10; i++)
            {
                var min = new Vector3(10 * i, 10 * i, 0);
                var max = min + new Vector3(10, 10, 0);
                this.quadtree.Add(new SampleObject(new BoundingBox(min, max)));
            }

            var queryBox = new BoundingBox(new Vector3(0, 0, 0), new Vector3(50, 50, 0));

            var result = new List<ISpatialQueryable>();
            quadtree.FindAll(ref queryBox, result);

            // TODO: 
            //Assert.Equal(5, result.Count);
        }
    }
}
