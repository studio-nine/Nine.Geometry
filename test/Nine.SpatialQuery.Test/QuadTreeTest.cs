namespace Nine.SpatialQuery
{
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
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
                this.quadtree.Add(new SampleObject(new Rectangle(10 * i, 10 * i, 10, 10), Color.Gray));
            }

            var queryBox = new BoundingBox(new Vector3(0, 0, 0), new Vector3(50, 50, 0));

            var result = new List<ISpatialQueryable>();
            quadtree.FindAll(ref queryBox, result);

            Assert.Equal(5, result.Count);
        }
    }
}
