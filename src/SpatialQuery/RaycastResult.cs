namespace Nine.Geometry.SpatialQuery
{
    public struct RaycastResult<T>
    {
        public readonly T Data;
        public readonly float Distance;

        public RaycastResult(T data, float distance)
        {
            this.Data = data;
            this.Distance = distance;
        }
    }
}
