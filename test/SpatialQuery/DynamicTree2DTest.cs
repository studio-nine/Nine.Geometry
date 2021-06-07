namespace Nine.Geometry.Test.SpatialQuery
{
    using Nine.Geometry.SpatialQuery;
    using System;
    using Xunit;

    public class DynamicTree2DTest
    {
        private DynamicTree2D<BoundingRectangle> tree = new DynamicTree2D<BoundingRectangle>();
        
        [Fact]
        public void test_all()
        {
            // Add actors
            var random = new Random();
            for (int i = 0; i < 100; i++)
            {
                int x = i % 10;
                int y = i / 10;

                var size = random.Next(10, 100);
                var offsetX = random.Next(-100, 100);
                var offsetY = random.Next(-100, 100);

                var actor = new BoundingRectangle(200 * x + offsetX, 200 * y + offsetY, size, size);
                tree.Add(ref actor, actor);
            }

            // Query
            var queryRect = new BoundingRectangle(-200, -200, 200 * 10 + 200, 200 * 10 + 200);

            BoundingRectangle[] queryResult = null;
            Assert.Equal(100, tree.FindAll(ref queryRect, ref queryResult, tree.RootId));

            // Remove actors
            tree.Clear();
            Assert.Equal(0, tree.NodeCount);
        }
    }
}