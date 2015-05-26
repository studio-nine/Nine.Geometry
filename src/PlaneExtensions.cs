namespace Nine.Geometry
{
    using System.ComponentModel;
    using System.Numerics;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PlaneExtensions
    {
        /// <summary>
        /// Checks whether the <see cref="Plane"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public static void Intersects(this Plane plane, ref Ray ray, out float? result)
        {
            result = ray.Intersects(plane);
        }

        /// <summary>
        /// Checks whether the <see cref="Plane"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public static float? Intersects(this Plane plane, Ray ray)
        {
            return ray.Intersects(plane);
        }
    }
}
