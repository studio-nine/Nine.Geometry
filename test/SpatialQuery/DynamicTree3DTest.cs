namespace Nine.Geometry.Test.SpatialQuery
{
    using Nine.Geometry.SpatialQuery;
    using System;
    using System.Numerics;
    using Xunit;

    public class DynamicTree3DTest
    {
        private DynamicTree3D<BoundingBox> tree = new DynamicTree3D<BoundingBox>();
        
        [Fact]
        public void test_all()
        {
            // Add actors
            var random = new Random();
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    for (int z = 0; z < 10; z++)
                    {
                        var size = random.Next(10, 100);
                        var offsetX = random.Next(-100, 100);
                        var offsetY = random.Next(-100, 100);
                        var offsetZ = random.Next(-100, 100);

                        var min = new Vector3(200 * x + offsetX, 200 * y + offsetY, 200 * z + offsetZ);
                        var actor = new BoundingBox(min, min + new Vector3(size));
                        tree.Add(ref actor, actor);
                    }
                }
            }

            // Query
            var queryRect = new BoundingBox(
                new Vector3(-200, -200, -200), 
                new Vector3(200 * 10 + 200, 200 * 10 + 200, 200 * 10 + 200));

            BoundingBox[] queryResult = null;
            Assert.Equal(1000, tree.FindAll(ref queryRect, ref queryResult, tree.RootId));

            // Remove actors
            tree.Clear();
            Assert.Equal(0, tree.NodeCount);
        }
    }
}