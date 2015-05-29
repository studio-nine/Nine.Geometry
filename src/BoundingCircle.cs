namespace Nine.Geometry
{
    using System.Numerics;

    /// <summary>
    /// 
    /// </summary>
    public struct BoundingCircle
    {
        /// <summary> GEts or sets the center point. </summary>
        public Vector2 Center;

        /// <summary> Gets or sets the radius. </summary>
        public float Radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingCircle"/> class.
        /// </summary>
        public BoundingCircle(Vector2 center, float radius)
        {
            this.Center = center;
            this.Radius = radius;
        }

        // TODO: Add struct
    }
}
