namespace Nine.Geometry
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    
    /// <summary>
    /// Defines an axis-aligned box-shaped 3D volume.
    /// </summary>
    public struct BoundingBox : IEquatable<BoundingBox>
    {
        public const int Corners = 8;

        /// <summary> Gets or sets the minimum position. </summary>
        public Vector3 Min;

        /// <summary> Gets or sets the maximum position. </summary>
        public Vector3 Max;

        /// <summary>
        /// Gets the center of the <see cref="BoundingBox"/>.
        /// </summary>
        public Vector3 Center
        {
            get
            {
                return new Vector3(
                    (this.Min.X + this.Max.X) / 2.0f,
                    (this.Min.Y + this.Max.Y) / 2.0f,
                    (this.Min.Z + this.Max.Z) / 2.0f);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundingBox"/> class.
        /// </summary>
        public BoundingBox(Vector3 min, Vector3 max)
        {
            this.Min = min;
            this.Max = max;
        }
        
        public void Contains(ref BoundingBox boundingBox, out ContainmentType result) => result = this.Contains(boundingBox);
        
        public ContainmentType Contains(BoundingBox boundingBox)
        {
            ContainmentType result;
            Intersection.Intersect(ref boundingBox, ref this, out result);
            return result;
        }

        public ContainmentType Contains(BoundingFrustum boundingfrustum) => Intersection.Intersect(boundingfrustum, this);

        public void Contains(ref BoundingSphere boundingSphere, out ContainmentType result) => result = this.Contains(boundingSphere);

        public ContainmentType Contains(BoundingSphere boundingSphere)
        {
            ContainmentType result;
            Intersection.Intersect(ref this, ref boundingSphere, out result);
            return result;
        }

        public void Contains(ref Plane plane, out ContainmentType result) => result = this.Contains(plane);

        public ContainmentType Contains(Plane plane)
        {
            ContainmentType result;
            Intersection.Intersect(ref plane, ref this, out result);
            return result;
        }

        public void Contains(ref Vector3 vector, out ContainmentType result) => result = this.Contains(vector);

        public ContainmentType Contains(Vector3 vector)
        {
            if (vector.X < this.Min.X || vector.X > this.Max.X ||
                vector.Y < this.Min.Y || vector.Y > this.Max.Y ||
                vector.Z < this.Min.Z || vector.Z > this.Max.Z)
            {
                return ContainmentType.Disjoint;
            }
            else if (
                vector.X == this.Min.X || vector.X == this.Max.X ||
                vector.Y == this.Min.Y || vector.Y == this.Max.Y ||
                vector.Z == this.Min.Z || vector.Z == this.Max.Z)
            {
                return ContainmentType.Intersects;
            }
            else
            {
                return ContainmentType.Contains;
            }
        }

        /// <summary>
        /// Tests whether the BoundingBox intersects with a line segment.
        /// </summary>
        public void Intersects(ref Vector3 v1, ref Vector3 v2, out float? result)
        {
            const float Epsilon = 1E-10F;

            var dir = Vector3.Subtract(v2, v1);

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

            Ray ray = new Ray(v1, dir);
            this.Intersects(ref ray, out result);
            if (result.HasValue && result.Value > length)
            {
                result = null;
            }
        }

        public float? Intersects(Vector3 v1, Vector3 v2)
        {
            float? distance;
            Intersects(ref v1, ref v2, out distance);
            return distance;
        }

        public void Intersects(ref BoundingBox boundingBox, out bool result)
        {
            result = this.Intersects(boundingBox);
        }

        public bool Intersects(BoundingBox boundingBox)
        {
            ContainmentType result;
            Intersection.Intersect(ref this, ref boundingBox, out result);
            return this.DoesIntersect(result);
        }

        public void Intersects(ref BoundingSphere boundingSphere, out bool result)
        {
            result = this.Intersects(boundingSphere);
        }

        public bool Intersects(BoundingSphere boundingSphere)
        {
            ContainmentType result;
            Intersection.Intersect(ref this, ref boundingSphere, out result);
            return this.DoesIntersect(result);
        }

        public void Intersects(ref Plane plane, out bool result)
        {
            result = this.Intersects(plane);
        }

        public bool Intersects(Plane plane)
        {
            ContainmentType result;
            Intersection.Intersect(ref plane, ref this, out result);
            return this.DoesIntersect(result);
        }

        public void Intersects(ref Ray ray, out float? result)
        {
            result = this.Intersects(ray);
        }

        public float? Intersects(Ray ray)
        {
            float? result;
            Intersection.Intersect(ref ray, ref this, out result);
            return result;
        }
        

        /// <summary>
        /// Clips a box against a frustum and split the input triangle when they interests.
        /// </summary>
        public void Intersects(BoundingFrustum frustum, out Vector3[] intersections, out int length)
        {
            if (Intersections == null)
                Intersections = new Vector3[32 * 12];

            intersections = Intersections;
            length = this.Intersects(frustum, intersections, 0);
        }
        static Vector3[] Intersections;

        /// <summary>
        /// Clips a box against a frustum and split the input triangle when they interests.
        /// </summary>
        public int Intersects(BoundingFrustum frustum, Vector3[] intersections, int startIndex)
        {
            var FrustumCorners = new Vector3[BoundingFrustum.CornerCount];

            var count = 0;
            frustum.GetCorners(ref FrustumCorners);
            for (int i = 0; i < TriangleIndices.Length; i += 3)
            {
                count += Triangle.Intersects(ref FrustumCorners[TriangleIndices[i]],
                                             ref FrustumCorners[TriangleIndices[i + 1]],
                                             ref FrustumCorners[TriangleIndices[i + 2]],
                                             ref this, intersections, startIndex + count);
            }

            return startIndex + count;
        }

        // TODO: Make this public (?)
        internal static readonly ushort[] TriangleIndices = new ushort[]
        {
            0,1,2,  3,0,2,
            4,6,5,  4,7,6,
            0,3,4,  4,3,7,
            5,1,0,  5,0,4,
            5,6,2,  5,2,1,
            3,2,6,  3,6,7,
        };


        private bool DoesIntersect(ContainmentType containmentType)
        {
            // TODO: Optimize
            return containmentType == ContainmentType.Contains || containmentType == ContainmentType.Intersects;
        }

        /// <summary>
        /// Gets an array of points that make up the corners of the <see cref="BoundingBox"/>.
        /// </summary>
        public void GetCorners(ref Vector3[] vertices)
        {
            vertices = this.GetCorners();
        }

        /// <summary>
        /// Gets an array of points that make up the corners of the <see cref="BoundingBox"/>.
        /// </summary>
        public Vector3[] GetCorners()
        {
            return new Vector3[]
            {
                new Vector3(this.Min.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Max.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Max.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Max.Z),
                new Vector3(this.Min.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Max.Y, this.Min.Z),
                new Vector3(this.Max.X, this.Min.Y, this.Min.Z),
                new Vector3(this.Min.X, this.Min.Y, this.Min.Z),
            };
        }

        /// <summary>
        /// Creates a <see cref="BoundingBox"/> that can contain a list of <see cref="Vector3"/>.
        /// </summary>
        public static BoundingBox CreateFromPoints(IEnumerable<Vector3> vectors)
        {
            if (vectors == null)
                throw new ArgumentNullException(nameof(vectors));

            var min = Vector3.One * float.MaxValue;
            var max = Vector3.One * float.MinValue;

            foreach (var vector in vectors)
            {
                if (vector.X < min.X) min.X = vector.X;
                if (vector.Y < min.Y) min.Y = vector.Y;
                if (vector.Z < min.Z) min.Z = vector.Z;

                if (vector.X < max.X) max.X = vector.X;
                if (vector.Y < max.Y) max.Y = vector.Y;
                if (vector.Z < max.Z) max.Z = vector.Z;
            }

            return new BoundingBox(min, max);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingBox"/> that can contain a <see cref="BoundingSphere"/>.
        /// </summary>
        public static void CreateFromSphere(ref BoundingSphere boundingSphere, out BoundingBox boundingBox)
        {
            var radius = new Vector3(boundingSphere.Radius);
            boundingBox.Min = boundingSphere.Center - radius;
            boundingBox.Max = boundingSphere.Center + radius;
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingBox"/> that can contain a <see cref="BoundingSphere"/>.
        /// </summary>
        public static BoundingBox CreateFromSphere(BoundingSphere sphere)
        {
            BoundingBox result;
            BoundingBox.CreateFromSphere(ref sphere, out result);
            return result;
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingBox"/> that contains the two <see cref="BoundingBox"/>es.
        /// </summary>
        public static void CreateMerged(BoundingBox original, BoundingBox additional, out BoundingBox result)
        {
            result = BoundingBox.CreateMerged(original, additional);
        }

        /// <summary>
        /// Creates the smallest <see cref="BoundingBox"/> that contains the two <see cref="BoundingBox"/>es.
        /// </summary>
        public static BoundingBox CreateMerged(BoundingBox original, BoundingBox additional)
        {
            BoundingBox result = new BoundingBox();

            result.Min.X = Math.Min(original.Min.X, additional.Min.X);
            result.Min.Y = Math.Min(original.Min.Y, additional.Min.Y);
            result.Min.Z = Math.Min(original.Min.Z, additional.Min.Z);

            result.Max.X = Math.Max(original.Max.X, additional.Max.X);
            result.Max.Y = Math.Max(original.Max.Y, additional.Max.Y);
            result.Max.Z = Math.Max(original.Max.Z, additional.Max.Z);

            return result;
        }

        /// <summary>
        /// Creates a merged bounding box from a list of existing bounding boxes.
        /// </summary>
        public static BoundingBox CreateMerged(IEnumerable<BoundingBox> boxes)
        {
            var result = new BoundingBox();

            var iBox = 0;
            foreach (var box in boxes)
            {
                result = (iBox == 0) ? box : BoundingBox.CreateMerged(result, box);
                iBox++;
            }

            return result;
        }

        /// <summary>
        /// Compute the axis aligned bounding box from an oriented bounding box.
        /// </summary>
        public static BoundingBox CreateAxisAligned(BoundingBox box, Matrix4x4 transform)
        {
            BoundingBox result;
            CreateAxisAligned(box, ref transform, out result);
            return result;
        }

        /// <summary>
        /// Compute the axis aligned bounding box from an oriented bounding box.
        /// </summary>
        public static void CreateAxisAligned(BoundingBox box, ref Matrix4x4 transform, out BoundingBox result)
        {
            // Find the 8 corners
            var corners = new Vector3[Corners];
            box.GetCorners(ref corners);

            // Compute bounding box
            result.Max = Vector3.Transform(corners[0], transform);
            result.Min = result.Max;

            var v = new Vector3();
            for (int i = 1; i < corners.Length; ++i)
            {
                v = Vector3.Transform(corners[i], transform);

                if (v.X < result.Min.X)
                    result.Min.X = v.X;
                else if (v.X > result.Max.X)
                    result.Max.X = v.X;

                if (v.Y < result.Min.Y)
                    result.Min.Y = v.Y;
                else if (v.Y > result.Max.Y)
                    result.Max.Y = v.Y;

                if (v.Z < result.Min.Z)
                    result.Min.Z = v.Z;
                else if (v.Z > result.Max.Z)
                    result.Max.Z = v.Z;
            }
        }

        public static bool operator ==(BoundingBox left, BoundingBox right) => (left.Min == right.Min) && (left.Max == right.Max);
        public static bool operator !=(BoundingBox left, BoundingBox right) => (left.Min != right.Min) && (left.Max != right.Max);

        /// <inheritdoc />
        public bool Equals(BoundingBox other) => (this.Min == other.Min) && (this.Max == other.Max);

        /// <inheritdoc />
        public override bool Equals(object obj) => (obj is BoundingBox) && this.Equals((BoundingBox)obj);

        /// <inheritdoc />
        public override int GetHashCode() => this.Min.GetHashCode() ^ this.Max.GetHashCode();
        
        /// <inheritdoc />
        public override string ToString() => "Min: " + this.Min + ", Max: " + this.Max;
    }
}
