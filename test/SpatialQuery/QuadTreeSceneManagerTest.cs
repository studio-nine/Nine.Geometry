namespace Nine.Geometry.Test.SpatialQuery
{
    using Nine.Geometry.SpatialQuery;
    using System.Numerics;
    using Xunit;

    public class QuadTreeSceneManagerTest
    {
        [Fact]
        public void FindAll_BoundingBox()
        {
            var scene = new QuadTreeSceneManager();
            scene.Add(new SpatialQueryableTest(new BoundingBox(new Vector3(10), new Vector3(20))));
            scene.Add(new SpatialQueryableTest(new BoundingBox(new Vector3(30), new Vector3(40))));

            var allBox = new BoundingBox(new Vector3(0), new Vector3(50));
            var allBoxResult = scene.FindAll(ref allBox);

            var partialBox = new BoundingBox(new Vector3(0), new Vector3(15));
            var partialBoxResult = scene.FindAll(ref partialBox);

            Assert.Equal(2, allBoxResult.Count);
            Assert.Equal(1, partialBoxResult.Count);
        }
    }
}
