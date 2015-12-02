namespace Nine.Geometry
{
    using System;
    using System.Numerics;
    
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

        public static void Intersects(ref BoundingBox boundingBox, ref Ray ray, out float? result) => Intersects(ref ray, ref boundingBox, out result);
        public static void Intersects(ref Ray ray, ref BoundingBox boundingBox, out float? result)
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

        public static void Intersects(ref BoundingSphere boundingSphere, ref Ray ray, out float? result) => Intersects(ref ray, ref boundingSphere, out result);
        public static void Intersects(ref Ray ray, ref BoundingSphere boundingSphere, out float? result)
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

        public static void Intersects(ref Plane plane, ref Ray ray, out float? result) => Intersects(ref ray, ref plane, out result);
        public static void Intersects(ref Ray ray, ref Plane plane, out float? result)
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

        public static void Intersects(ref BoundingFrustum boundingFrustum, ref Ray ray, out float? result) => Intersects(ref ray, ref boundingFrustum, out result);
        public static void Intersects(ref Ray ray, ref BoundingFrustum boundingFrustum, out float? result)
        {
            // TODO: Make test case for this
            if (boundingFrustum.Contains(ray.Position) == ContainmentType.Contains)
            {
                // the ray is inside the frustum
                result = 0.0f;
            }
            else
            {
                result = null;

                var corners = boundingFrustum.GetCorners();
                for (int i = 0; i < (BoundingFrustum.PlaneCount * 2); i++)
                {
                    var v1 = corners[Geometry.TriangleIndices[i + 0]];
                    var v2 = corners[Geometry.TriangleIndices[i + 1]];
                    var v3 = corners[Geometry.TriangleIndices[i + 2]];

                    Intersects(ref ray, ref v1, ref v2, ref v3, out result);

                    if (result.HasValue)
                        break;
                }
            }
        }
        
        public static void Intersects(ref Ray ray, ref Triangle triangle, out float? result)
            => Intersection.Intersects(ref ray, ref triangle.V1, ref triangle.V2, ref triangle.V3, out result);

        public static void Intersects(ref Ray ray, ref Vector3[] positions, ref ushort[] indices, ref Matrix4x4 world, out float? result)
        {
            result = null;

            float? point = null;
            Ray tray;

            Matrix4x4.Invert(world, out world);
            ray.Transform(ref world, out tray);

            // Test each triangle
            for (int i = 0; i < indices.Length; i += 3)
            {
                Intersects(ref ray, ref positions[indices[i]], ref positions[indices[i + 1]], ref positions[indices[i + 2]], out point);

                if (point.HasValue)
                {
                    if (!result.HasValue || point.Value < result.Value)
                        result = point.Value;
                }
            }
        }

        public static void Intersects(ref Ray ray, ref Vector3 vertex1, ref Vector3 vertex2, ref Vector3 vertex3, out float? result)
        {
            const float Epsilon = 1E-10F;

            // Compute vectors along two edges of the triangle.
            var edge1 = Vector3.Subtract(vertex2, vertex1);
            var edge2 = Vector3.Subtract(vertex3, vertex1);

            // Compute the determinant.
            var directionCrossEdge2 = Vector3.Cross(ray.Direction, edge2);
            var determinant = Vector3.Dot(edge1, directionCrossEdge2);

            // If the ray is parallel to the triangle plane, there is no collision.
            if (determinant > -Epsilon && determinant < Epsilon)
            {
                result = null;
                return;
            }

            var inverseDeterminant = 1.0f / determinant;

            // Calculate the U parameter of the intersection point.
            var distanceVector = Vector3.Subtract(ray.Position, vertex1);

            var triangleU = Vector3.Dot(distanceVector, directionCrossEdge2);
            triangleU *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleU < 0 || triangleU > 1)
            {
                result = null;
                return;
            }

            // Calculate the V parameter of the intersection point.
            var distanceCrossEdge1 = Vector3.Cross(distanceVector, edge1);

            var triangleV = Vector3.Dot(ray.Direction, distanceCrossEdge1);
            triangleV *= inverseDeterminant;

            // Make sure it is inside the triangle.
            if (triangleV < 0 || triangleU + triangleV > 1)
            {
                result = null;
                return;
            }

            // Compute the distance along the ray to the triangle.
            var rayDistance = Vector3.Dot(edge2, distanceCrossEdge1);
            rayDistance *= inverseDeterminant;

            // Is the triangle behind the ray origin?
            if (rayDistance < 0)
            {
                result = null;
                return;
            }

            result = rayDistance;
        }
        
        #endregion

        #region Plane

        public static void Intersects(ref BoundingBox boundingBox, ref Plane plane, out PlaneIntersectionType result) => Intersects(ref plane, ref boundingBox, out result);
        public static void Intersects(ref Plane plane, ref BoundingBox boundingBox, out PlaneIntersectionType result)
        {
            Vector3 positiveVertex;
            Vector3 negativeVertex;

            // X
            if (plane.Normal.X >= 0)
            {
                positiveVertex.X = boundingBox.Max.X;
                negativeVertex.X = boundingBox.Min.X;
            }
            else
            {
                positiveVertex.X = boundingBox.Min.X;
                negativeVertex.X = boundingBox.Max.X;
            }

            // Y
            if (plane.Normal.Y >= 0)
            {
                positiveVertex.Y = boundingBox.Max.Y;
                negativeVertex.Y = boundingBox.Min.Y;
            }
            else
            {
                positiveVertex.Y = boundingBox.Min.Y;
                negativeVertex.Y = boundingBox.Max.Y;
            }

            // Z
            if (plane.Normal.Z >= 0)
            {
                positiveVertex.Z = boundingBox.Max.Z;
                negativeVertex.Z = boundingBox.Min.Z;
            }
            else
            {
                positiveVertex.Z = boundingBox.Min.Z;
                negativeVertex.Z = boundingBox.Max.Z;
            }

            var distance = Vector3.Dot(plane.Normal, negativeVertex) + plane.D;
            if (distance > 0)
            {
                result = PlaneIntersectionType.Front;
            }
            else
            {
                distance = Vector3.Dot(plane.Normal, positiveVertex) + plane.D;
                result = (distance < 0) ? PlaneIntersectionType.Back : PlaneIntersectionType.Intersecting;
            }
        }

        public static void Intersects(ref BoundingSphere boundingSphere, ref Plane plane, out PlaneIntersectionType result) => Intersects(ref plane, ref boundingSphere, out result);
        public static void Intersects(ref Plane plane, ref BoundingSphere boundingSphere, out PlaneIntersectionType result)
        {
            var distance = Vector3.Dot(plane.Normal, boundingSphere.Center);
            distance += plane.D;

            if (distance > boundingSphere.Radius)
            {
                result = PlaneIntersectionType.Front;
            }
            else if (distance < boundingSphere.Radius)
            {
                result = PlaneIntersectionType.Back;
            }
            else
            {
                result = PlaneIntersectionType.Intersecting;
            }
        }
        
        public static void Intersects(ref Plane plane, ref BoundingFrustum boundingFrustum, out PlaneIntersectionType result) => Intersects(ref boundingFrustum, ref plane, out result);
        public static void Intersects(ref BoundingFrustum boundingFrustum, ref Plane plane, out PlaneIntersectionType result)
        {
            var corners = boundingFrustum.GetCorners();
            result = plane.Intersects(corners[0]);
            for (int i = 1; i < corners.Length; i++)
                if (plane.Intersects(corners[i]) != result)
                    result = PlaneIntersectionType.Intersecting;
        }

        #endregion

        #region BoundingBox & BoundingSphere

        public static void Contains(ref BoundingSphere boundingSphere, ref BoundingBox boundingBox, out ContainmentType result) => Contains(ref boundingBox, ref boundingSphere, out result);
        public static void Contains(ref BoundingBox boundingBox, ref BoundingSphere boundingSphere, out ContainmentType result)
        {
            var min = boundingBox.Min;
            var max = boundingBox.Max;

            if ((boundingSphere.Center.X - min.X) >= boundingSphere.Radius &&
                (boundingSphere.Center.Y - min.Y) >= boundingSphere.Radius &&
                (boundingSphere.Center.Z - min.Z) >= boundingSphere.Radius &&
                (max.X - boundingSphere.Center.X) >= boundingSphere.Radius &&
                (max.Y - boundingSphere.Center.Y) >= boundingSphere.Radius &&
                (max.Z - boundingSphere.Center.Z) >= boundingSphere.Radius)
            {
                result = ContainmentType.Contains;
            }
            else
            {
                double dmin = 0.0;
                double e = boundingSphere.Center.X - min.X;

                if (e < 0)
                {
                    if (e < -boundingSphere.Radius)
                    {
                        result = ContainmentType.Disjoint;
                        return;
                    }

                    dmin += e * e;
                }
                else
                {
                    e = boundingSphere.Center.X - max.X;
                    if (e > 0)
                    {
                        if (e > boundingSphere.Radius)
                        {
                            result = ContainmentType.Disjoint;
                            return;
                        }

                        dmin += e * e;
                    }
                }

                e = boundingSphere.Center.Y - min.Y;
                if (e < 0)
                {
                    if (e < -boundingSphere.Radius)
                    {
                        result = ContainmentType.Disjoint;
                        return;
                    }

                    dmin += e * e;
                }
                else
                {
                    e = boundingSphere.Center.Y - max.Y;
                    if (e > 0)
                    {
                        if (e > boundingSphere.Radius)
                        {
                            result = ContainmentType.Disjoint;
                            return;
                        }

                        dmin += e * e;
                    }
                }

                e = boundingSphere.Center.Z - min.Z;
                if (e < 0)
                {
                    if (e < -boundingSphere.Radius)
                    {
                        result = ContainmentType.Disjoint;
                        return;
                    }

                    dmin += e * e;
                }
                else
                {
                    e = boundingSphere.Center.Z - max.Z;
                    if (e > 0)
                    {
                        if (e > boundingSphere.Radius)
                        {
                            result = ContainmentType.Disjoint;
                            return;
                        }

                        dmin += e * e;
                    }
                }

                result = (dmin <= boundingSphere.Radius * boundingSphere.Radius) ? 
                    ContainmentType.Intersects : ContainmentType.Disjoint;
            }
        }

        public static void Intersects(ref BoundingSphere boundingSphere, ref BoundingBox boundingBox, out bool result) => Intersects(ref boundingBox, ref boundingSphere, out result);
        public static void Intersects(ref BoundingBox boundingBox, ref BoundingSphere boundingSphere, out bool result)
        {
            result = false;

            var min = boundingBox.Min;
            var max = boundingBox.Max;

            if ((boundingSphere.Center.X - min.X > boundingSphere.Radius) && 
                (boundingSphere.Center.Y - min.Y > boundingSphere.Radius) && 
                (boundingSphere.Center.Z - min.Z > boundingSphere.Radius) && 
                (max.X - boundingSphere.Center.X > boundingSphere.Radius) && 
                (max.Y - boundingSphere.Center.Y > boundingSphere.Radius) &&
                (max.Z - boundingSphere.Center.Z > boundingSphere.Radius))
            {
                result = true;
            }
            else
            {
                var dmin = 0.0;

                // X
                if (boundingSphere.Center.X - min.X <= boundingSphere.Radius)
                {
                    dmin += (boundingSphere.Center.X - min.X) * (boundingSphere.Center.X - min.X);
                }
                else if (max.X - boundingSphere.Center.X <= boundingSphere.Radius)
                {
                    dmin += (boundingSphere.Center.X - max.X) * (boundingSphere.Center.X - max.X);
                }

                // Y
                if (boundingSphere.Center.Y - min.Y <= boundingSphere.Radius)
                {
                    dmin += (boundingSphere.Center.Y - min.Y) * (boundingSphere.Center.Y - min.Y);
                }
                else if (max.Y - boundingSphere.Center.Y <= boundingSphere.Radius)
                {
                    dmin += (boundingSphere.Center.Y - max.Y) * (boundingSphere.Center.Y - max.Y);
                }

                // Z
                if (boundingSphere.Center.Z - min.Z <= boundingSphere.Radius)
                {
                    dmin += (boundingSphere.Center.Z - min.Z) * (boundingSphere.Center.Z - min.Z);
                }
                else if (max.Z - boundingSphere.Center.Z <= boundingSphere.Radius)
                {
                    dmin += (boundingSphere.Center.Z - max.Z) * (boundingSphere.Center.Z - max.Z);
                }

                if (dmin <= (boundingSphere.Radius * boundingSphere.Radius))
                {
                    result = true;
                }
            }
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

        public static void Intersects(ref BoundingBox boundingBox, ref BoundingFrustum boundingFrustum, out bool result) => Intersects(ref boundingFrustum, ref boundingBox, out result);
        public static void Intersects(ref BoundingFrustum boundingFrustum, ref BoundingBox boundingBox, out bool result)
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
            result = ContainmentType.Contains;

            var planes = boundingFrustum.GetPlanes();
            for (var i = 0; i < BoundingFrustum.PlaneCount; ++i)
            {
                var planeIntersectionType = default(PlaneIntersectionType);
                Intersects(ref boundingSphere, ref planes[i], out planeIntersectionType);

                if (planeIntersectionType == PlaneIntersectionType.Front)
                {
                    result = ContainmentType.Disjoint;
                    break;
                }
                else if (planeIntersectionType == PlaneIntersectionType.Intersecting)
                {
                    result = ContainmentType.Intersects;
                    break;
                }
            }
        }

        public static void Intersects(ref BoundingSphere boundingSphere, ref BoundingFrustum boundingFrustum, out bool result) => Intersects(ref boundingFrustum, ref boundingSphere, out result);
        public static void Intersects(ref BoundingFrustum boundingFrustum, ref BoundingSphere boundingSphere, out bool result)
        {
            ContainmentType contains;
            Contains(ref boundingFrustum, ref boundingSphere, out contains);
            result = (contains == ContainmentType.Contains) | (contains == ContainmentType.Intersects);
        }

        #endregion
        
        #region LineSegment & BoundingBox

        public static void Intersects(ref BoundingBox boundingBox, ref LineSegment2D lineSegment, out float? result)
        {
            Vector3 v1 = new Vector3(lineSegment.Start, 0), v2 = new Vector3(lineSegment.End, 0);
            Intersects(ref boundingBox, ref v1, ref v2, out result);
        }

        public static void Intersects(ref BoundingBox boundingBox, ref Vector3 v1, ref Vector3 v2, out float? result)
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
                Intersects(ref ray, ref boundingBox, out result);

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

        public static void Intersects(ref BoundingBox boundingBox1, ref BoundingBox boundingBox2, out bool result)
        {
            ContainmentType contains;
            Contains(ref boundingBox1, ref boundingBox2, out contains);
            result = (contains == ContainmentType.Contains) | (contains == ContainmentType.Intersects);
        }

        #endregion

        #region BoundingSphere

        public static void Contains(ref BoundingSphere boundingSphere1, ref BoundingSphere boundingSphere2, out ContainmentType result)
        {
            var distance = Vector3.Distance(boundingSphere1.Center, boundingSphere2.Center);
            if (distance > boundingSphere1.Radius + boundingSphere2.Radius)
            {
                result = ContainmentType.Disjoint;
            }
            else
            {
                result = (distance <= boundingSphere1.Radius - boundingSphere2.Radius) ? ContainmentType.Contains : ContainmentType.Intersects;
            }
        }

        public static void Intersects(ref BoundingSphere boundingSphere1, ref BoundingSphere boundingSphere2, out bool result)
        {
            var distance = Vector3.Distance(boundingSphere1.Center, boundingSphere2.Center);
            result = (distance < (boundingSphere1.Radius + boundingSphere2.Radius));
        }

        #endregion

        #region BoundingFrustum

        public static void Contains(ref BoundingFrustum boundingFrustum1, ref BoundingFrustum boundingFrustum2, out ContainmentType result)
        {
            // TODO: Contains, BoundingFrustum.
            throw new NotImplementedException();
        }

        public static void Intersects(ref BoundingFrustum boundingFrustum1, ref BoundingFrustum boundingFrustum2, out bool result)
        {
            // TODO: Intersects, BoundingFrustum.
            throw new NotImplementedException();
        }

        #endregion

        #region BoundingRectangle

        public static void Contains(ref BoundingRectangle boundingRectangle1, ref BoundingRectangle boundingRectangle2, out ContainmentType result)
        {
            if (boundingRectangle1.X > boundingRectangle2.Right ||
                boundingRectangle1.Y > boundingRectangle2.Bottom ||
                boundingRectangle1.Right < boundingRectangle2.X ||
                boundingRectangle1.Bottom < boundingRectangle2.Y)
            {
                result = ContainmentType.Disjoint;
            }
            else if (
                boundingRectangle1.X <= boundingRectangle2.X &&
                boundingRectangle1.Y <= boundingRectangle2.Y &&
                boundingRectangle1.Right >= boundingRectangle2.Right &&
                boundingRectangle1.Bottom >= boundingRectangle2.Bottom)
            {
                result = ContainmentType.Contains;
            }
            else
            {
                result = ContainmentType.Intersects;
            }
        }

        public static void Intersects(ref BoundingRectangle boundingRectangle1, ref BoundingRectangle boundingRectangle2, out bool result)
        {
            if (boundingRectangle1.X > boundingRectangle2.Right ||
                boundingRectangle1.Y > boundingRectangle2.Bottom ||
                boundingRectangle1.Right < boundingRectangle2.X ||
                boundingRectangle1.Bottom < boundingRectangle2.Y)
            {
                result = false;
            }
            else
            {
                result = true;
            }
        }

        #endregion
    }
}
