namespace Nine.Geometry.SpatialQuery
{
    using System;
    
    public class SampleObject : ISpatialQueryable
    {
        public BoundingBox BoundingBox { get; private set; }

        public object SpatialData { get; set; }
        public event EventHandler<EventArgs> BoundingBoxChanged;
        
        public SampleObject(BoundingBox boundingBox)
        {
            this.BoundingBox = boundingBox;
        }
    }
}
