namespace Nine.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Numerics;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class PlaneExtensions
    {
        // TODO:

        public static PlaneIntersectionType Intersects(this Plane plane, Vector3 vector)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        ///// Checks whether the <see cref="Plane"/> intersects a <see cref="BoundingBox"/>.
        ///// </summary>
        //public static void Intersects(this Plane plane, ref BoundingBox boundingBox, ref ContainmentType result)
        //{
        //    Intersection.Intersect(ref plane, ref boundingBox, out result);
        //}

        ///// <summary>
        ///// Checks whether the <see cref="Plane"/> intersects a <see cref="BoundingBox"/>.
        ///// </summary>
        //public static ContainmentType Intersects(this Plane plane, BoundingBox boundingBox)
        //{
        //    ContainmentType result;
        //    Intersection.Intersect(ref plane, ref boundingBox, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Checks whether the <see cref="Plane"/> intersects a <see cref="BoundingFrustum"/>.
        ///// </summary>
        //public static ContainmentType Intersects(this Plane plane, BoundingFrustum boundingFrustum)
        //{
        //    return Intersection.Intersect(boundingFrustum, plane);
        //}

        ///// <summary>
        ///// Checks whether the <see cref="Plane"/> intersects a <see cref="BoundingSphere"/>.
        ///// </summary>
        //public static ContainmentType Intersects(this Plane plane, BoundingSphere boundingSphere)
        //{
        //    ContainmentType result;
        //    Intersection.Intersect(ref plane, ref boundingSphere, out result);
        //    return result;
        //}

        ///// <summary>
        ///// Checks whether the <see cref="Plane"/> intersects a <see cref="Ray"/>.
        ///// </summary>
        //public static float? Intersects(this Plane plane, Ray ray)
        //{
        //    float? result;
        //    Intersection.Intersect(ref ray, ref plane, out result);
        //    return result;
        //}
    }
}
