namespace Nine.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Numerics;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PlaneExtensions
    {
        /// <summary>
        /// Checks whether the <see cref="Plane"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public static ContainmentType Intersects(this Plane plane, BoundingBox boundingBox)
        {
            ContainmentType result;
            Intersection.Intersect(ref plane, ref boundingBox, out result);
            return result;
        }
        
        /// <summary>
        /// Checks whether the <see cref="Plane"/> intersects a <see cref="Ray"/>.
        /// </summary>
        public static float? Intersects(this Plane plane, Ray ray)
        {
            float? result;
            Intersection.Intersect(ref ray, ref plane, out result);
            return result;
        }
    }
}
