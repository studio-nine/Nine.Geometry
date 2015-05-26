namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Defines a 3D triangle made up of three vertices.
    /// </summary>
    public struct Triangle : IEquatable<Triangle>
    {
        /// <summary> Gets or sets the first vertex. </summary>
        public Vector3 V1;

        /// <summary> Gets or sets the second vertex. </summary>
        public Vector3 V2;

        /// <summary> Gets or sets the third vertex. </summary>
        public Vector3 V3;

        /// <summary>
        /// Initializes a new instance of the <see cref="Triangle"/> class.
        /// </summary>
        public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            this.V1 = v1;
            this.V2 = v2;
            this.V3 = v3;
        }

        /// <summary>
        /// Checks whether the current triangle intersects with a ray.
        /// </summary>
        public float? Intersects(Ray ray)
        {
            float? result;
            ray.Intersects(ref V1, ref V2, ref V3, out result);
            return result;
        }

        /// <summary>
        /// Checks whether the current triangle intersects with a ray.
        /// </summary>
        public void Intersects(ref Ray ray, out float? result)
        {
            ray.Intersects(ref V1, ref V2, ref V3, out result);
        }

        /// <summary>
        /// Checks whether the current triangle intersects with a line segment.
        /// </summary>
        /// <returns>
        /// The distance between the intersection point and v1.
        /// </returns>
        public float? Intersects(Vector3 v1, Vector3 v2)
        {
            float? result;
            Intersects(ref v1, ref v2, out result);
            return result;
        }

        /// <summary>
        /// Checks whether the current triangle intersects with a line segment.
        /// </summary>
        /// <returns>
        /// The distance between the intersection point and v1.
        /// </returns>
        public void Intersects(ref Vector3 v1, ref Vector3 v2, out float? result)
        {
            const float Epsilon = 1E-10F;

            Vector3 dir = Vector3.Subtract(v2, v1);

            float length = dir.Length();
            if (length <= Epsilon)
            {
                result = null;
                return;
            }

            float inv = 1.0f / length;
            dir.X *= inv;
            dir.Y *= inv;
            dir.Z *= inv;

            new Ray(v1, dir).Intersects(ref V1, ref V2, ref V3, out result);
            if (result.HasValue && result.Value > length)
                result = null;
        }

        /// <summary>
        /// Clips a triangle against a box and split the input triangle when they intersects.
        /// </summary>
        /// <returns>
        /// The count of intersection points.
        /// </returns>
        /// <remarks>
        /// The vertices of the output vertices will be copied to the intersections array
        /// starting from the startIndex parameter.
        /// </remarks>
        public int Intersects(ref BoundingBox box, Vector3[] intersections, int startIndex)
        {
            return Intersects(ref V1, ref V2, ref V3, ref box, intersections, startIndex);
        }

        /// <remarks>
        /// This algorithm is inspired by a blog post from wolfire games:
        /// http://blog.wolfire.com/2009/06/how-to-project-decals/.
        /// By the way, Overgrowth is AWEOSOME!!!
        /// </remarks>
        internal static int Intersects(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref BoundingBox box, Vector3[] intersections, int startIndex)
        {
            var mid = new Vector3();
            var source = intersections;
            var target = IntersectionPoints;
            var sourceCount = 3;
            var targetCount = 0;

            source[startIndex + 0] = v1;
            source[startIndex + 1] = v2;
            source[startIndex + 2] = v3;

            #region Clip against -X plane
            for (var i = 0; i < sourceCount; ++i)
            {
                var start = source[startIndex + i];
                var end = source[startIndex + (i + 1) % sourceCount];

                var insideStart = start.X >= box.Min.X;
                var insideEnd = end.X >= box.Min.X;

                if (!insideStart && !insideEnd)
                    continue;

                if (insideStart && insideEnd)
                {
                    target[targetCount++] = start;
                    continue;
                }

                var delta = (box.Min.X - start.X) / (end.X - start.X);
                if (delta <= float.Epsilon || delta >= 1 - float.Epsilon)
                    continue;

                mid.X = start.X + (end.X - start.X) * delta;
                mid.Y = start.Y + (end.Y - start.Y) * delta;
                mid.Z = start.Z + (end.Z - start.Z) * delta;

                if (insideStart)
                {
                    target[targetCount++] = start;
                    target[targetCount++] = mid;
                }
                else
                {
                    target[targetCount++] = mid;
                }
            }
            #endregion

            #region Clip against +X plane
            sourceCount = 0;
            for (var i = 0; i < targetCount; ++i)
            {
                var start = target[i];
                var end = target[(i + 1) % targetCount];

                var insideStart = start.X <= box.Max.X;
                var insideEnd = end.X <= box.Max.X;

                if (!insideStart && !insideEnd)
                    continue;

                if (insideStart && insideEnd)
                {
                    source[startIndex + sourceCount++] = start;
                    continue;
                }

                var delta = (box.Max.X - start.X) / (end.X - start.X);
                if (delta <= float.Epsilon || delta >= 1 - float.Epsilon)
                    continue;

                mid.X = start.X + (end.X - start.X) * delta;
                mid.Y = start.Y + (end.Y - start.Y) * delta;
                mid.Z = start.Z + (end.Z - start.Z) * delta;

                if (insideStart)
                {
                    source[startIndex + sourceCount++] = start;
                    source[startIndex + sourceCount++] = mid;
                }
                else
                {
                    source[startIndex + sourceCount++] = mid;
                }
            }
            #endregion

            #region Clip against -Y plane
            targetCount = 0;
            for (var i = 0; i < sourceCount; ++i)
            {
                var start = source[startIndex + i];
                var end = source[startIndex + (i + 1) % sourceCount];

                var insideStart = start.Y >= box.Min.Y;
                var insideEnd = end.Y >= box.Min.Y;

                if (!insideStart && !insideEnd)
                    continue;

                if (insideStart && insideEnd)
                {
                    target[targetCount++] = start;
                    continue;
                }

                var delta = (box.Min.Y - start.Y) / (end.Y - start.Y);
                if (delta <= float.Epsilon || delta >= 1 - float.Epsilon)
                    continue;

                mid.X = start.X + (end.X - start.X) * delta;
                mid.Y = start.Y + (end.Y - start.Y) * delta;
                mid.Z = start.Z + (end.Z - start.Z) * delta;

                if (insideStart)
                {
                    target[targetCount++] = start;
                    target[targetCount++] = mid;
                }
                else
                {
                    target[targetCount++] = mid;
                }
            }
            #endregion

            #region Clip against +Y plane
            sourceCount = 0;
            for (var i = 0; i < targetCount; ++i)
            {
                var start = target[i];
                var end = target[(i + 1) % targetCount];

                var insideStart = start.Y <= box.Max.Y;
                var insideEnd = end.Y <= box.Max.Y;

                if (!insideStart && !insideEnd)
                    continue;

                if (insideStart && insideEnd)
                {
                    source[startIndex + sourceCount++] = start;
                    continue;
                }

                var delta = (box.Max.Y - start.Y) / (end.Y - start.Y);
                if (delta <= float.Epsilon || delta >= 1 - float.Epsilon)
                    continue;

                mid.X = start.X + (end.X - start.X) * delta;
                mid.Y = start.Y + (end.Y - start.Y) * delta;
                mid.Z = start.Z + (end.Z - start.Z) * delta;

                if (insideStart)
                {
                    source[startIndex + sourceCount++] = start;
                    source[startIndex + sourceCount++] = mid;
                }
                else
                {
                    source[startIndex + sourceCount++] = mid;
                }
            }
            #endregion

            #region Clip against -Z plane
            targetCount = 0;
            for (var i = 0; i < sourceCount; ++i)
            {
                var start = source[startIndex + i];
                var end = source[startIndex + (i + 1) % sourceCount];

                var insideStart = start.Z >= box.Min.Z;
                var insideEnd = end.Z >= box.Min.Z;

                if (!insideStart && !insideEnd)
                    continue;

                if (insideStart && insideEnd)
                {
                    target[targetCount++] = start;
                    continue;
                }

                var delta = (box.Min.Z - start.Z) / (end.Z - start.Z);
                if (delta <= float.Epsilon || delta >= 1 - float.Epsilon)
                    continue;

                mid.X = start.X + (end.X - start.X) * delta;
                mid.Y = start.Y + (end.Y - start.Y) * delta;
                mid.Z = start.Z + (end.Z - start.Z) * delta;

                if (insideStart)
                {
                    target[targetCount++] = start;
                    target[targetCount++] = mid;
                }
                else
                {
                    target[targetCount++] = mid;
                }
            }
            #endregion

            #region Clip against +Z plane
            sourceCount = 0;
            for (var i = 0; i < targetCount; ++i)
            {
                var start = target[i];
                var end = target[(i + 1) % targetCount];

                var insideStart = start.Z <= box.Max.Z;
                var insideEnd = end.Z <= box.Max.Z;

                if (!insideStart && !insideEnd)
                    continue;

                if (insideStart && insideEnd)
                {
                    source[startIndex + sourceCount++] = start;
                    continue;
                }

                var delta = (box.Max.Z - start.Z) / (end.Z - start.Z);
                if (delta <= float.Epsilon || delta >= 1 - float.Epsilon)
                    continue;

                mid.X = start.X + (end.X - start.X) * delta;
                mid.Y = start.Y + (end.Y - start.Y) * delta;
                mid.Z = start.Z + (end.Z - start.Z) * delta;

                if (insideStart)
                {
                    source[startIndex + sourceCount++] = start;
                    source[startIndex + sourceCount++] = mid;
                }
                else
                {
                    source[startIndex + sourceCount++] = mid;
                }
            }
            #endregion

            return sourceCount;
        }

        /// <summary>
        /// Box triangle have at most 32 intersection points.
        /// </summary>
        static Vector3[] IntersectionPoints = new Vector3[32];

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Triangle other)
        {
            return V1 == other.V1 && V2 == other.V2 && V3 == other.V3;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is Triangle)
                return Equals((Triangle)obj);

            return false;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Triangle value1, Triangle value2)
        {
            return ((value1.V1 == value2.V1) && (value1.V2 == value2.V2) && (value1.V3 == value2.V3));
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Triangle value1, Triangle value2)
        {
            return !(value1.V1 == value2.V1 && value1.V2 == value2.V2 && value1.V3 == value2.V3);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return V1.GetHashCode() ^ V2.GetHashCode() ^ V3.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return V1.ToString() + "|" + V2.ToString() + "|" + V3.ToString();
        }
    }
}
