namespace Nine.Geometry.SpatialQuery
{
    public struct RaycastHit<T>
    {
        public readonly T Data;
        public readonly float Distance;

        public RaycastHit(T data, float distance)
        {
            this.Data = data;
            this.Distance = distance;
        }
    }
}
