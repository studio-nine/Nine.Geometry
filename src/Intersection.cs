namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    // TODO: All the structures 'Contains(..)' methods should simpler than the methods in Intersection for optimization.
    // TODO: Move Ray intersect Triangle

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
        #region Ray

        public static void Intersect(ref BoundingBox boundingBox, ref Ray ray, out float? result) => Intersect(ref ray, ref boundingBox, out result);
        public static void Intersect(ref Ray ray, ref BoundingBox boundingBox, out float? result)
        {
            if (Math.Abs(ray.Direction.X) < Single.Epsilon && 
                (ray.Position.X < boundingBox.Min.X || ray.Position.X > boundingBox.Max.X))
            {
                result = null;
                return;
            }

            var min = 0.0f;
            var max = float.MaxValue;

            var inverseDirection = 1 / ray.Direction.X;
            var t1 = (boundingBox.Min.X - ray.Position.X) * inverseDirection;
            var t2 = (boundingBox.Max.X - ray.Position.X) * inverseDirection;

            if (t1 > t2)
            {
                var temp = t1;
                t1 = t2; t2 = temp;
            }

            min = Math.Max(min, t1);
            max = Math.Min(max, t2);

            if (min > max)
            {
                result = null;
                return;
            }

            if (Math.Abs(ray.Direction.Y) < Single.Epsilon && 
                (ray.Position.Y < boundingBox.Min.Y || ray.Position.Y > boundingBox.Max.Y))
            {
                result = null;
                return;
            }

            inverseDirection = 1 / ray.Direction.Y;
            t1 = (boundingBox.Min.Y - ray.Position.Y) * inverseDirection;
            t2 = (boundingBox.Max.Y - ray.Position.Y) * inverseDirection;

            if (t1 > t2)
            {
                var temp = t1;
                t1 = t2; t2 = temp;
            }

            min = Math.Max(min, t1);
            max = Math.Min(max, t2);

            if (min > max)
            {
                result = null;
                return;
            }

            if (Math.Abs(ray.Direction.Z) < Single.Epsilon && 
                (ray.Position.Z < boundingBox.Min.Z || ray.Position.Z > boundingBox.Max.Z))
            {
                result = null;
                return;
            }

            inverseDirection = 1 / ray.Direction.Z;
            t1 = (boundingBox.Min.Z - ray.Position.Z) * inverseDirection;
            t2 = (boundingBox.Max.Z - ray.Position.Z) * inverseDirection;

            if (t1 > t2)
            {
                float temp = t1;
                t1 = t2;
                t2 = temp;
            }

            min = Math.Max(min, t1);
            max = Math.Min(max, t2);

            if (min > max)
            {
                result = null;
                return;
            }

            result = min;
        }

        public static void Intersect(ref BoundingSphere boundingSphere, ref Ray ray, out float? result) => Intersect(ref ray, ref boundingSphere, out result);
        public static void Intersect(ref Ray ray, ref BoundingSphere boundingSphere, out float? result)
        {
            var difference = boundingSphere.Center - ray.Position;
            var differenceLengthSquared = difference.LengthSquared();
            var sphereRadiusSquared = boundingSphere.Radius * boundingSphere.Radius;

            if (differenceLengthSquared < sphereRadiusSquared)
            {
                result = 0.0f;
            }
            else
            {
                var distanceAlongRay = Vector3.Dot(ray.Direction, difference);
                if (distanceAlongRay < 0)
                {
                    result = null;
                }
                else
                {
                    var dist = sphereRadiusSquared + distanceAlongRay * distanceAlongRay - differenceLengthSquared;
                    result = (dist < 0) ? null : distanceAlongRay - (float?)Math.Sqrt(dist);
                }
            }
        }

        public static void Intersect(ref Plane plane, ref Ray ray, out float? result) => Intersect(ref ray, ref plane, out result);
        public static void Intersect(ref Ray ray, ref Plane plane, out float? result)
        {
            var velocity = Vector3.Dot(ray.Direction, plane.Normal);
            if (Math.Abs(velocity) < float.Epsilon)
            {
                result = null;
            }
            else
            {
                var distanceAlongNormal = Vector3.Dot(ray.Position, plane.Normal);
                distanceAlongNormal += plane.D;
                result = -distanceAlongNormal / velocity;
            }
        }

        public static void Intersect(ref BoundingFrustum boundingFrustum, ref Ray ray, out float? result) => Intersect(ref ray, ref boundingFrustum, out result);
        public static void Intersect(ref Ray ray, ref BoundingFrustum boundingFrustum, out float? result)
        {
            var containmentType = boundingFrustum.Contains(ray.Position);
            switch (containmentType)
            {
                case ContainmentType.Disjoint:
                    result = null;
                    return;

                case ContainmentType.Contains:
                    result = 0.0f;
                    return;

                case ContainmentType.Intersects:
                    // TODO: 
                    throw new NotImplementedException();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region BoundingBox & BoundingSphere

        public static void Contains(ref BoundingSphere boundingSphere, ref BoundingBox boundingBox, out ContainmentType result) => Contains(ref boundingBox, ref boundingSphere, out result);
        public static void Contains(ref BoundingBox boundingBox, ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        public static void Intersect(ref BoundingSphere boundingSphere, ref BoundingBox boundingBox, out bool result) => Intersect(ref boundingBox, ref boundingSphere, out result);
        public static void Intersect(ref BoundingBox boundingBox, ref BoundingSphere boundingSphere, out bool result)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region BoundingBox & Plane

        public static void Contains(ref BoundingBox boundingBox, ref Plane plane, out PlaneIntersectionType result) => Contains(ref plane, ref boundingBox, out result);
        public static void Contains(ref Plane plane, ref BoundingBox boundingBox, out PlaneIntersectionType result)
        {
            // http://zach.in.tu-clausthal.de/teaching/cg_literatur/lighthouse3d_view_frustum_culling/index.html
            throw new NotImplementedException();
        }
 
        public static void Intersect(ref BoundingBox boundingBox, ref Plane plane, out bool result) => Intersect(ref plane, ref boundingBox, out result);
        public static void Intersect(ref Plane plane, ref BoundingBox boundingBox, out bool result)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Plane & BoundingSphere

        public static void Contains(ref BoundingSphere boundingSphere, ref Plane plane, out ContainmentType result) => Contains(ref plane, ref boundingSphere, out result);
        public static void Contains(ref Plane plane, ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        public static void Intersect(ref BoundingSphere boundingSphere, ref Plane plane, out bool result) => Intersect(ref plane, ref boundingSphere, out result);
        public static void Intersect(ref Plane plane, ref BoundingSphere boundingSphere, out bool result)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region BoundingFrustum & BoundingBox

        public static void Contains(ref BoundingBox boundingBox, ref BoundingFrustum boundingFrustum, out ContainmentType result) => Contains(ref boundingFrustum, ref boundingBox, out result);
        public static void Contains(ref BoundingFrustum boundingFrustum, ref BoundingBox boundingBox, out ContainmentType result)
        {
            var planes = boundingFrustum.GetPlanes();
            var intersects = false;

            for (var i = 0; i < BoundingFrustum.PlaneCount; ++i)
            {
                var planeIntersectionType = boundingBox.Contains(planes[i]);
                switch (planeIntersectionType)
                {
                    case PlaneIntersectionType.Front:
                        result = ContainmentType.Disjoint;
                        return;

                    case PlaneIntersectionType.Intersecting:
                        intersects = true;
                        break;
                }
            }

            result = intersects ? ContainmentType.Intersects : ContainmentType.Contains;
        }

        public static void Intersect(ref BoundingBox boundingBox, ref BoundingFrustum boundingFrustum, out bool result) => Intersect(ref boundingFrustum, ref boundingBox, out result);
        public static void Intersect(ref BoundingFrustum boundingFrustum, ref BoundingBox boundingBox, out bool result)
        {
            var containmentType = ContainmentType.Disjoint;
            Contains(ref boundingFrustum, ref boundingBox, out containmentType);
            result = containmentType != ContainmentType.Disjoint;
        }

        #endregion

        #region BoundingFrustum & BoundingSphere

        public static void Contains(ref BoundingSphere boundingSphere, ref BoundingFrustum boundingFrustum, out ContainmentType result) => Contains(ref boundingFrustum, ref boundingSphere, out result);
        public static void Contains(ref BoundingFrustum boundingFrustum, ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        public static void Intersect(ref BoundingSphere boundingSphere, ref BoundingFrustum boundingFrustum, out bool result) => Intersect(ref boundingFrustum, ref boundingSphere, out result);
        public static void Intersect(ref BoundingFrustum boundingFrustum, ref BoundingSphere boundingSphere, out bool result)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region BoundingFrustum & Plane

        public static void Contains(ref Plane plane, ref BoundingFrustum boundingFrustum, out PlaneIntersectionType result) => Contains(ref boundingFrustum, ref plane, out result);
        public static void Contains(ref BoundingFrustum boundingFrustum, ref Plane plane, out PlaneIntersectionType result)
        {
            var corners = boundingFrustum.GetCorners();
            result = plane.Intersects(corners[0]);
            for (int i = 1; i < corners.Length; i++)
                if (plane.Intersects(corners[i]) != result)
                    result = PlaneIntersectionType.Intersecting;
        }

        public static void Intersect(ref Plane plane, ref BoundingFrustum boundingFrustum, out bool result) => Intersect(ref boundingFrustum, ref plane, out result);
        public static void Intersect(ref BoundingFrustum boundingFrustum, ref Plane plane, out bool result)
        {
            PlaneIntersectionType planeIntersectionType;
            Contains(ref boundingFrustum, ref plane, out planeIntersectionType);
            result = (planeIntersectionType == PlaneIntersectionType.Intersecting);
        }

        #endregion

        #region LineSegment & BoundingBox

        public static void Intersect(ref BoundingBox boundingBox, ref LineSegment lineSegment, out float? result)
        {
            Vector3 v1 = new Vector3(lineSegment.Start, 0), v2 = new Vector3(lineSegment.End, 0);
            Intersect(ref boundingBox, ref v1, ref v2, out result);
        }

        public static void Intersect(ref BoundingBox boundingBox, ref Vector3 v1, ref Vector3 v2, out float? result)
        {
            var dir = Vector3.Subtract(v2, v1);

            var length = dir.Length();
            if (length <= float.Epsilon)
            {
                result = null;
            }
            else
            {
                var inv = 1.0f / length;
                dir.X *= inv;
                dir.Y *= inv;
                dir.Z *= inv;

                var ray = new Ray(v1, dir);
                Intersect(ref ray, ref boundingBox, out result);

                if (result.HasValue && result.Value > length)
                    result = null;
            }
        }

        #endregion

        #region BoundingBox

        public static void Contains(ref BoundingBox boundingBox1, ref BoundingBox boundingBox2, out ContainmentType result)
        {
            if (boundingBox2.Max.X < boundingBox1.Min.X || boundingBox2.Min.X > boundingBox1.Max.X ||
                boundingBox2.Max.Y < boundingBox1.Min.Y || boundingBox2.Min.Y > boundingBox1.Max.Y ||
                boundingBox2.Max.Z < boundingBox1.Min.Z || boundingBox2.Min.Z > boundingBox1.Max.Z)
            {
                result = ContainmentType.Disjoint;
            }
            else if (
                boundingBox2.Min.X >= boundingBox1.Min.X && boundingBox2.Max.X <= boundingBox1.Max.X &&
                boundingBox2.Min.Y >= boundingBox1.Min.Y && boundingBox2.Max.Y <= boundingBox1.Max.Y &&
                boundingBox2.Min.Z >= boundingBox1.Min.Z && boundingBox2.Max.Z <= boundingBox1.Max.Z)
            {
                result = ContainmentType.Contains;
            }
            else
            {
                result = ContainmentType.Intersects;
            }
        }

        public static void Intersect(ref BoundingBox boundingBox1, ref BoundingBox boundingBox2, out bool result)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region BoundingSphere

        public static void Contains(ref BoundingSphere boundingSphere1, ref BoundingSphere boundingSphere2, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        public static void Intersect(ref BoundingSphere boundingSphere1, ref BoundingSphere boundingSphere2, out bool result)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region BoundingFrustum

        public static void Contains(ref BoundingFrustum boundingFrustum1, ref BoundingFrustum boundingFrustum2, out ContainmentType result)
        {
            throw new NotImplementedException();
        }

        public static void Intersect(ref BoundingFrustum boundingFrustum1, ref BoundingFrustum boundingFrustum2, out bool result)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
