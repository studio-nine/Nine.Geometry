namespace Nine.Geometry.Test.SpatialQuery
{
    using Nine.Geometry.SpatialQuery;
    using System;
    using System.Numerics;

    class SpatialQueryableTest : ISpatialQueryable
    {
        private static readonly Random random = new();
        public static SpatialQueryableTest CreateRandom(BoundingBox bounds, float size, object spatialData = null)
        {
            BoundingBox box;
            do
            {
                var point = new Vector3(MathHelper.Lerp(bounds.Min.X, bounds.Max.X, (float)random.NextDouble()),
                                        MathHelper.Lerp(bounds.Min.Y, bounds.Max.Y, (float)random.NextDouble()),
                                        MathHelper.Lerp(bounds.Min.Z, bounds.Max.Z, (float)random.NextDouble()));

                box = BoundingBox.CreateFromPoints(new Vector3[] { point, point + Vector3.One * size });

            } while (bounds.Contains(box) != ContainmentType.Contains);

            return new SpatialQueryableTest(box, spatialData);
        }

        public bool NoEventHandlerAttached => BoundingBoxChanged == null;

        public BoundingBox BoundingBox
        {
            get => boundingBox;
            set
            {
                if (this.boundingBox != value)
                {
                    this.boundingBox = value;
                    this.BoundingBoxChanged?.Invoke(this, new EventArgs());
                }
            }
        }
        private BoundingBox boundingBox;

        public object SpatialData { get; set; }

        public event EventHandler<EventArgs> BoundingBoxChanged;

        public SpatialQueryableTest(BoundingBox boundingBox, object spatialData = null)
        {
            this.boundingBox = boundingBox;
            this.SpatialData = spatialData;
        }
    }
}
