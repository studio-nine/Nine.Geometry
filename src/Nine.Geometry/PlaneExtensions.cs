namespace Nine.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Numerics;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PlaneExtensions
    {
        public static PlaneIntersectionType Intersects(this Plane plane, Vector3 vector)
        {
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
