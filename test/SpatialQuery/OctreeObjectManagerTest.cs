namespace Nine.Geometry.Test.SpatialQuery
{
    using Nine.Geometry.SpatialQuery;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Numerics;
    using Xunit;

    public class OctreeObjectManagerTest
    {
        [Fact]
        public void AddUpdateRemoveTest()
        {
            OctreeSceneManager scene = new OctreeSceneManager();
            Assert.Single(scene.Tree);
            var boundable = new SpatialQueryableTest(new BoundingBox(Vector3.One * 0.1f, Vector3.One));
            scene.Add(boundable);

            //dynamic data1 = boundable.SpatialData;
            //var node1 = data1.Node;

            Assert.Single(scene);
            boundable.BoundingBox = new BoundingBox(-Vector3.One, -Vector3.One * 0.1f);

            //dynamic data2 = boundable.SpatialData;
            //var node2 = data2.Node;

            //Assert.AreNotEqual(node1, node2);

            Assert.Single(scene);

            scene.Remove(boundable);

            Assert.Empty(scene);
            Assert.Single(scene.Tree);
        }

        [Fact]
        public void AddUpdateToNewBoundsRemoveTest()
        {
            OctreeSceneManager scene = new OctreeSceneManager();

            var boundable = new SpatialQueryableTest(new BoundingBox(Vector3.One * 0.1f, Vector3.One));
            scene.Add(boundable);

            Assert.Single(scene);
            boundable.BoundingBox = new BoundingBox(-Vector3.One * 10000, Vector3.One * 10000);

            Assert.Single(scene);

            scene.Remove(boundable);

            Assert.Empty(scene);
            Assert.Single(scene.Tree);
        }

        [Fact(Skip = "Needs to be updated")]
        public void AddQueryUpdateRemoveTest()
        {
            // 1000 x 1000 x 1000
            var scene = new OctreeSceneManager();
            var bounds = new BoundingBox(Vector3.One * -500, Vector3.One * 1000);
            var objects = Enumerable.Range(0, 1000).Select(i => SpatialQueryableTest.CreateRandom(bounds, 50f)).ToArray();
            var updatedObjects = Enumerable.Range(0, 1000).Select(i => SpatialQueryableTest.CreateRandom(bounds, 50f)).ToArray();
            var queries = Enumerable.Range(0, 1000).Select(i => SpatialQueryableTest.CreateRandom(bounds, 200)).ToArray();

            // Add
            GC.Collect();
            Stopwatch watch = new Stopwatch();
            watch.Start();

            foreach (var o in objects)
            {
                scene.Add(o);
            }

            watch.Stop();
            Trace.WriteLine("Tree Nodes: " + scene.Tree.Count());
            Trace.WriteLine("Max num objects added per frame (60 FPS): " + objects.Length / watch.Elapsed.TotalSeconds / 60.0);

            Assert.Equal(objects.Length, scene.Count);
            Assert.Equal(objects.Length, scene.Count());

            List<ISpatialQueryable> queryResult = new List<ISpatialQueryable>();

            // Query
            watch.Restart();

            foreach (var q in queries)
            {
                var box = q.BoundingBox;
                scene.FindAll(ref box, queryResult);
            }

            watch.Stop();
            Trace.WriteLine("Tree Nodes: " + scene.Tree.Count());
            Trace.WriteLine(string.Format("Max queries per frame (60 FPS): {0} on {1} objects",
                                        queries.Length / watch.Elapsed.TotalSeconds / 60.0, objects.Length));

            foreach (var g in scene.Tree.GroupBy(n => n.Depth))
            {
                Trace.WriteLine(string.Format("Objects with depth {0}: {1}", g.Key, g.Sum(n => n.Value != null ? n.Value.Count : 0)));
                Trace.WriteLine(string.Format("Average objects per node with depth {0}: {1}", g.Key, g.Average(n => n.Value != null ? n.Value.Count : 0)));
            }

            // Update
            watch.Restart();

            for (int i = 0; i < updatedObjects.Length; ++i)
            {
                objects[i].BoundingBox = updatedObjects[i].BoundingBox;
            }

            watch.Stop();
            Trace.WriteLine("Tree Nodes: " + scene.Tree.Count());
            Trace.WriteLine("Max num objects updated per frame (60 FPS): " + objects.Length / watch.Elapsed.TotalSeconds / 60.0);

            Assert.Equal(objects.Length, scene.Count);
            Assert.Equal(objects.Length, scene.Count());

            //var all = scene.FindAll(scene.Bounds).ToArray();
            var all = objects;

            // Remove
            watch.Restart();

            for (int i = 0; i < all.Length; ++i)
            {
                scene.Remove(all[i]);
            }

            watch.Stop();
            Trace.WriteLine("Tree Nodes: " + scene.Tree.Count());
            Trace.WriteLine("Max num objects removed per frame (60 FPS): " + objects.Length / watch.Elapsed.TotalSeconds / 60.0);

            Assert.Empty(scene);
            Assert.True(objects[0].NoEventHandlerAttached);
            Assert.Single(scene.Tree);
        }
    }
}
