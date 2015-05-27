namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    // TODO: Rename ContainmentType to IntersectionType
    // TODO: Ray intersect Triangle

    /// <summary>
    /// Indicates the extent to which bounding volumes intersect or contain one another.
    /// </summary>
    public enum ContainmentType
    {
        /// <summary> Indicates that one bounding volume completely contains the other. </summary>
        Contains,

        /// <summary> Indicates there is no overlap between the bounding volumes. </summary>
        Disjoint,

        /// <summary> Indicates that the bounding volumes partially overlap. </summary>
        Intersects
    }

    /// <summary>
    /// Describes the intersection between a plane and a bounding volume.
    /// </summary>
    public enum PlaneIntersectionType
    {
        /// <summary> There is no intersection, and the bounding volume is in the negative half-space of the <see cref="Plane"/>. </summary>
        Front,

        /// <summary> There is no intersection, and the bounding volume is in the positive half-space of the <see cref="Plane"/>. </summary>
        Back,

        /// <summary> The <see cref="Plane"/> is intersected. </summary>
        Intersecting
    }

    /// <summary>
    /// Defines all the intersection methods. 
    /// </summary>
    public static class Intersection
    {
        /// <summary>
        /// Checks whether a <see cref="Ray"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public static void Intersect(ref Ray ray, ref BoundingBox boundingBox, out float? result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="Ray"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public static void Intersect(ref Ray ray, ref BoundingSphere boundingSphere, out float? result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="Ray"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public static void Intersect(ref Ray ray, ref BoundingFrustum boundingFrustum, out float? result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="Ray"/> intersects a <see cref="Plane"/>.
        /// </summary>
        public static void Intersect(ref Ray ray, ref Plane plane, out float? result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="BoundingBox"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public static void Intersect(ref BoundingBox boundingBox, ref BoundingFrustum boundingFrustum, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="BoundingBox"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public static void Intersect(ref BoundingBox boundingBox, ref BoundingSphere boundingFrustum, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="BoundingFrustum"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public static void Intersect(ref BoundingFrustum boundingFrustum, ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="Plane"/> intersects a <see cref="BoundingBox"/>.
        /// </summary>
        public static void Intersect(ref Plane plane, ref BoundingBox boundingBox, out ContainmentType result)
        {
            // http://zach.in.tu-clausthal.de/teaching/cg_literatur/lighthouse3d_view_frustum_culling/index.html
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="Plane"/> intersects a <see cref="BoundingFrustum"/>.
        /// </summary>
        public static void Intersect(ref Plane plane, ref BoundingFrustum boundingFrustum, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether a <see cref="Plane"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public static void Intersect(ref Plane plane, ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Checks whether a <see cref="BoundingSphere"/> intersects a <see cref="BoundingSphere"/>.
        /// </summary>
        public static void Intersect(ref BoundingSphere boundingSphere1, ref BoundingSphere boundingSphere2, out ContainmentType result)
        {
            throw new NotImplementedException();
        }
    }

    // Expand on this if you want the XNA syntax
    // The current structures only contan a custom Intersect method.
    // In XNA 'Contains(...)' is basically the same as 'Intersect(...)' 
    // and XNA's 'Intersect(...)' gives simpler return value.
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class XNASyntaxSugerExtensions
    {
        #region BoundingBox

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingBox boundingBox1, BoundingBox boundingBox2)
        {
            return boundingBox1.Intersects(boundingBox2);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingBox boundingBox, BoundingFrustum boundingfrustum)
        {
            return boundingBox.Intersects(boundingfrustum);
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingBox"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingBox boundingBox, BoundingSphere boundingSphere)
        {
            return boundingBox.Intersects(boundingSphere);
        }

        #endregion

        #region BoundingFrustum

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingFrustum boundingFrustum, BoundingBox boundingBox)
        {
            // TODO: BoundingFrustum contains BoundingBox
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingFrustum boundingFrustum, BoundingFrustum boundingfrustum)
        {
            // TODO: BoundingFrustum contains BoundingFrustum
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingFrustum"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingFrustum boundingFrustum, BoundingSphere boundingSphere)
        {
            // TODO: BoundingFrustum contains BoundingSphere
            throw new NotImplementedException();
        }

        #endregion

        #region BoundingSphere

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingSphere"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingSphere boundingSphere1, BoundingSphere boundingSphere2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingFrustum"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingSphere boundingSphere, BoundingFrustum boundingfrustum)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks whether the <see cref="BoundingSphere"/> contains a <see cref="BoundingBox"/>.
        /// </summary>
        public static ContainmentType Contains(this BoundingSphere boundingSphere, BoundingBox boundingBox)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
