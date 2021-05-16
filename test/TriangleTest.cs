﻿namespace Nine.Geometry.Test
{
    using System.Diagnostics;
    using System.Numerics;
    using Xunit;

    public class TriangleTest
    {
        [Fact]
        public void Intersects_Ray()
        {
            Triangle target = new Triangle();

            target.V1 = Vector3.Zero;
            target.V2 = Vector3.UnitX * 2;
            target.V3 = Vector3.UnitY;

            Ray ray = new Ray(Vector3.One, -Vector3.Normalize(Vector3.One));

            Assert.True(target.Intersects(ray).HasValue);

            Assert.True(target.Intersects(Vector3.One, Vector3.UnitZ * -0.1f).HasValue);
            Assert.False(target.Intersects(Vector3.One, Vector3.UnitZ * 0.1f).HasValue);
        }

        // [Fact]
        // public void Contains_BoundingBox()
        // {
        //     Triangle target = new Triangle();
        // 
        //     target.V1 = Vector3.Zero;
        //     target.V2 = Vector3.UnitX * 2;
        //     target.V3 = Vector3.UnitY;
        // 
        //     BoundingBox box = new BoundingBox();
        // 
        //     box.Min = new Vector3(1.8f, 0.01f, -0.1f);
        //     box.Max = Vector3.One * 4;
        // 
        //     Assert.True(target.Intersects(box.Min, box.Min + Vector3.UnitZ * 4).HasValue);
        //     Assert.Equal(ContainmentType.Intersects, box.Contains(target));
        // }
    }
}