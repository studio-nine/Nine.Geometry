namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    /// <summary>
    /// Defines a line segment in 3D space.
    /// </summary>
    public struct LineSegment3D : IEquatable<LineSegment3D>
    {
        /// <summary> 
        /// Gets or sets the start point of this <see cref="LineSegment3D"/>. 
        /// </summary>
        public Vector3 Start;

        /// <summary> 
        /// Gets or sets the end point of this <see cref="LineSegment3D"/>. 
        /// </summary>
        public Vector3 End;

        /// <summary> 
        /// Gets the center of this <see cref="LineSegment3D"/>. 
        /// </summary>
        public Vector3 Center
        {
            get
            {
                var result = new Vector3();
                result.X = 0.5f * (Start.X + End.X);
                result.Y = 0.5f * (Start.Y + End.Y);
                result.Z = 0.5f * (Start.Z + End.Z);
                return result;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSegment3D"/> struct.
        /// </summary>
        public LineSegment3D(Vector3 start, Vector3 end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Find the closest point between <see cref="Start"/> and <see cref="End"/>.
        /// </summary>
        public Vector3 ClosestPointOnLine(Vector3 point)
        {
            var lineLength = Vector3.Distance(Start, End);
            var lineDir = (End - Start) / lineLength;
            var distance = Vector3.Dot(point - Start, lineDir);

            if (distance <= 0)
                return Start;

            if (distance >= lineLength)
                return End;

            return Start + lineDir * distance;
        }

        /// <summary>
        /// Gets the length of this <see cref="LineSegment3D"/>.
        /// </summary>
        public float Length()
        {
            var xx = Start.X - End.X;
            var yy = Start.Y - End.Y;
            var zz = Start.Z - End.Z;
            return (float)Math.Sqrt(xx * xx + yy * yy + zz * zz);
        }

        /// <summary>
        /// Gets the squared length of this <see cref="LineSegment3D"/>.
        /// </summary>
        public float LengthSquared()
        {
            var xx = Start.X - End.X;
            var yy = Start.Y - End.Y;
            var zz = Start.Z - End.Z;
            return xx * xx + yy * yy + zz * zz;
        }
        
        /// <summary>
        /// Returns whether this <see cref="LineSegment3D"/> contains the point.
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector3 point)
        {
            float m = (this.End.Y - this.Start.Y) / (this.End.X - this.Start.X);
            float b = this.Start.Y - m * this.Start.X;

            if (Math.Abs(point.Y - (m * point.X + b)) < float.Epsilon)
                return true;
            
            return false;
        }

        /// <summary>
        /// Returns whether the <see cref="LineSegment3D"/> intersects each others.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Intersects(LineSegment3D value)
            => this.Intersect(value).HasValue;

        /// <summary>
        /// Returns the intersection point between two <see cref="LineSegment3D"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Vector3? Intersect(LineSegment3D value)
            => this.Intersect(value.Start, value.End);

        /// <summary>
        /// Returns whether the <see cref="LineSegment3D"/> intersects each others.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool Intersects(Vector3 start, Vector3 end)
            => this.Intersect(start, end).HasValue;

        /// <summary>
        /// Returns the intersection point between two <see cref="LineSegment3D"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Vector3? Intersect(Vector3 start, Vector3 end)
        {
            var connection = ClosestConnection(start, end);
            if (connection.Start == connection.End)
                return connection.Start;
            
            return null;
        }

        /// <summary>
        /// Returns a <see cref="LineSegment3D"/> of the closest connection point between the two <see cref="LineSegment3D"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public LineSegment3D ClosestConnection(LineSegment3D value)
            => this.ClosestConnection(value.Start, value.End);

        /// <summary>
        /// Returns a <see cref="LineSegment3D"/> of the closest connection point between the two <see cref="LineSegment3D"/>.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public LineSegment3D ClosestConnection(Vector3 start, Vector3 end)
        {
            Vector3 u = this.End - this.Start;
            Vector3 v = end - start;
            Vector3 w = this.Start - start;

            float a = Vector3.Dot(u, u);    // always >= 0
            float b = Vector3.Dot(u, v);
            float c = Vector3.Dot(v, v);    // always >= 0
            float d = Vector3.Dot(u, w);
            float e = Vector3.Dot(v, w);

            float D = a * c - b * b;        // always >= 0
            float sN, sD = D;               // s = sN / sD, default sD = D >= 0
            float tN, tD = D;               // t = tN / tD, default tD = D >= 0

            // compute the line parameters of the two closest points
            if (D < float.Epsilon)
            {
                // the lines are almost parallel
                sN = 0.0f;         // force using point P0 on segment S1
                sD = 1.0f;         // to prevent possible division by 0.0 later
                tN = e;
                tD = c;
            }
            else
            {
                // get the closest points on the infinite lines
                sN = (b * e - c * d);
                tN = (a * e - b * d);
                if (sN < 0.0f)
                {
                    // sc < 0 => the s=0 edge is visible
                    sN = 0.0f;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {
                    // sc > 1  => the s=1 edge is visible
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0.0)
            {
                // tc < 0 => the t=0 edge is visible
                tN = 0.0f;
                // recompute sc for this edge
                if (-d < 0.0f)
                {
                    sN = 0.0f;
                }
                else if (-d > a)
                {
                    sN = sD;
                }
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {
                // tc > 1  => the t=1 edge is visible
                tN = tD;
                // recompute sc for this edge
                if ((-d + b) < 0.0f)
                {
                    sN = 0.0f;
                }
                else if ((-d + b) > a)
                {
                    sN = sD;
                }
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }

            // finally do the division to get sc and tc
            var s = Math.Abs(sN) < float.Epsilon ? 0.0f : sN / sD;
            var t = Math.Abs(tN) < float.Epsilon ? 0.0f : tN / tD;

            // get the difference of the two closest points
            var c1 = (1.0f - s) * this.Start + s * this.End;
            var c2 = (1.0f - t) * start + t * end;

            return new LineSegment3D(c1, c2);
        }

        public static bool operator ==(LineSegment3D value1, LineSegment3D value2)
            => (value1.Start == value2.Start && value1.End == value2.End);
        
        public static bool operator !=(LineSegment3D value1, LineSegment3D value2)
            => (value1.Start != value2.Start && value1.End != value2.End);

        /// <inheritdoc />
        public bool Equals(LineSegment3D other)
            => this.Start == other.Start && this.End == other.End;

        /// <inheritdoc />
        public override bool Equals(object obj)
            => (obj is LineSegment3D) && this.Equals((LineSegment3D)obj);

        /// <inheritdoc />
        public override int GetHashCode()
            => this.Start.GetHashCode() ^ this.End.GetHashCode();

        /// <inheritdoc />
        public override string ToString()
            => $"<{Start} - {End}>";
    }
}
