namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    public static class PlaneExtensions
    {
        /// <summary>
        /// Returns whether the plane is valid.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static bool IsValid(this Plane plane)
            => plane.Normal.LengthSquared() > 0.0f;

        /// <summary>
        /// Returns the <see cref="Plane"/> with a flipped normal.
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static Plane Flip(this Plane plane)
            => new Plane(plane.Normal * -1.0f, plane.D * -1.0f);

        public static PlaneIntersectionType Intersects(this Plane plane, Vector3 vector)
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        public static PlaneIntersectionType Intersects(this Plane plane, BoundingBox boundingBox)
        {
            PlaneIntersectionType result;
            Intersection.Intersects(ref plane, ref boundingBox, out result);
            return result;
        }

        public static PlaneIntersectionType Intersects(this Plane plane, BoundingFrustum boundingFrustum)
        {
            PlaneIntersectionType result;
            Intersection.Intersects(ref plane, ref boundingFrustum, out result);
            return result;
        }

        public static PlaneIntersectionType Intersects(this Plane plane, BoundingSphere boundingSphere)
        {
            PlaneIntersectionType result;
            Intersection.Intersects(ref plane, ref boundingSphere, out result);
            return result;
        }

        public static float? Intersects(this Plane plane, Ray ray)
        {
            float? result;
            Intersection.Intersects(ref ray, ref plane, out result);
            return result;
        }
    }
}
