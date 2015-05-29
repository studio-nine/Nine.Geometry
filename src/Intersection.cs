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
        public static void Intersect(ref Ray ray, ref BoundingBox boundingBox, out float? result)
        {
            // TODO: Beak this down into two methods (?)

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
        
        public static void Intersect(ref Ray ray, ref BoundingSphere boundingSphere, out float? result)
        {
            throw new NotImplementedException();
        }
        
        public static void Intersect(ref Ray ray, ref Plane plane, out float? result)
        {
            var velocity = Vector3.Dot(ray.Direction, plane.Normal);
            if (Math.Abs(velocity) < Single.Epsilon)
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
        
        public static void Intersect(ref BoundingBox boundingBox, ref BoundingSphere boundingFrustum, out ContainmentType result)
        {
            throw new NotImplementedException();
        }
        
        public static void Intersect(ref Plane plane, ref BoundingBox boundingBox, out ContainmentType result)
        {
            // http://zach.in.tu-clausthal.de/teaching/cg_literatur/lighthouse3d_view_frustum_culling/index.html
            throw new NotImplementedException();
        }
        
        public static void Intersect(ref Plane plane, ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            throw new NotImplementedException();
        }


        public static void Intersect(ref BoundingBox boundingBox1, ref BoundingBox boundingBox2, out ContainmentType result)
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
        
        public static void Intersect(ref BoundingSphere boundingSphere1, ref BoundingSphere boundingSphere2, out ContainmentType result)
        {
            throw new NotImplementedException();
        }


        public static ContainmentType Intersect(BoundingFrustum boundingFrustum1, BoundingFrustum boundingFrustum2)
        {
            throw new NotImplementedException();
        }

        public static ContainmentType Intersect(BoundingFrustum boundingFrustum, BoundingBox boundingBox)
        {
            throw new NotImplementedException();
        }

        public static ContainmentType Intersect(BoundingFrustum boundingFrustum, BoundingSphere boundingSphere)
        {
            throw new NotImplementedException();
        }

        public static ContainmentType Intersect(BoundingFrustum boundingFrustum, Plane plane)
        {
            throw new NotImplementedException();
        }

        public static float? Intersect(BoundingFrustum boundingFrustum, Ray ray)
        {
            throw new NotImplementedException();
        }
    }
}
