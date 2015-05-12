namespace Nine.SpatialQuery
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Defines an axis-aligned rectangle-shaped 2D volume.
    /// </summary>
    public struct BoundingRectangle : IEquatable<BoundingRectangle>
    {
        /// <summary>
        /// Gets or sets the x value.
        /// </summary>
        public float X;

        /// <summary>
        /// Gets or sets the y value.
        /// </summary>
        public float Y;

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public float Width;

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public float Height;

        /// <summary>
        /// Returns the x-coordinate of the left side of the rectangle.
        /// </summary>
        public float Left { get { return X; } }

        /// <summary>
        /// Returns the x-coordinate of the right side of the rectangle.
        /// </summary>
        public float Right { get { return X + Width; } }

        /// <summary>
        /// Returns the y-coordinate of the top of the rectangle.
        /// </summary>
        public float Top { get { return Y; } }

        /// <summary>
        /// Returns the y-coordinate of the bottom of the rectangle.
        /// </summary>
        public float Bottom { get { return Y + Height; } }

        /// <summary>
        /// Returns the center point of the bottom of the rectangle.
        /// </summary>
        public Vector2 Center
        {
            get { return new Vector2(X + Width * 0.5f, Y + Height * 0.5f); }
        }

        /// <summary>
        /// Returns the top left corner of the rectangle.
        /// </summary>
        public Vector2 Location
        {
            get { return new Vector2(X, Y); }
        }

        /// <summary>
        /// Returns the size of the rectangle.
        /// </summary>
        public Vector2 Size
        {
            get { return new Vector2(Width, Height); }
        }

        /// <summary>
        /// Returns a Rectangle with all of its values set to zero.
        /// </summary>
        public static BoundingRectangle Empty { get; private set; }

        /// <summary>
        /// Create a new instance of BoundingRectangle object.
        /// </summary>
        public BoundingRectangle(float width, float height)
        {
            this.X = 0;
            this.Y = 0;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Create a new instance of BoundingRectangle object.
        /// </summary>
        public BoundingRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }
        
        /// <summary>
        /// Tests whether the BoundingRectangle contains a point.
        /// </summary>
        public ContainmentType Contains(float x, float y)
        {
            return (x >= this.X && x <= this.X + this.Width && y >= this.Y && y <= this.Y + this.Height) ? ContainmentType.Contains : ContainmentType.Disjoint;
        }

        /// <summary>
        /// Tests whether the BoundingRectangle contains a point.
        /// </summary>
        public ContainmentType Contains(Vector2 point)
        {
            return (point.X >= this.X && point.X <= this.X + this.Width && point.Y >= this.Y && point.Y <= this.Y + this.Height) ? ContainmentType.Contains : ContainmentType.Disjoint;
        }

        /// <summary>
        /// Tests whether the BoundingRectangle contains another rectangle.
        /// </summary>
        public ContainmentType Contains(BoundingRectangle rectangle)
        {
            if (this.X > rectangle.X + rectangle.Width ||
                this.Y > rectangle.Y + rectangle.Height ||
                this.X + this.Width < rectangle.X ||
                this.Y + this.Height < rectangle.Y)
            {
                return ContainmentType.Disjoint;
            }
            else if (
                this.X <= rectangle.X &&
                this.Y <= rectangle.Y &&
                this.X + this.Width >= rectangle.X + rectangle.Width &&
                this.Y + this.Height >= rectangle.Y + rectangle.Height)
            {
                return ContainmentType.Contains;
            }
            else
            {
                return ContainmentType.Intersects;
            }
        }

        /// <summary>
        /// Tests whether the BoundingRectangle contains another rectangle.
        /// </summary>
        public void Contains(ref BoundingRectangle rectangle, out ContainmentType containmentType)
        {
            if (this.X > rectangle.X + rectangle.Width ||
                this.Y > rectangle.Y + rectangle.Height ||
                this.X + this.Width < rectangle.X ||
                this.Y + this.Height < rectangle.Y)
            {
                containmentType = ContainmentType.Disjoint;
                return;
            }

            if (this.X <= rectangle.X &&
                this.Y <= rectangle.Y &&
                this.X + this.Width >= rectangle.X + rectangle.Width &&
                this.Y + this.Height >= rectangle.Y + rectangle.Height)
            {
                containmentType = ContainmentType.Contains;
                return;
            }
            containmentType = ContainmentType.Intersects;
        }

        public bool Equals(BoundingRectangle other)
        {
            return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (obj is BoundingRectangle)
                return Equals((BoundingRectangle)obj);

            return false;
        }

        public static bool operator ==(BoundingRectangle value1, BoundingRectangle value2)
        {
            return (value1.X == value2.X) && (value1.Y == value2.Y) &&
                   (value1.Width == value2.Width) && (value1.Height == value2.Height);
        }

        public static bool operator !=(BoundingRectangle value1, BoundingRectangle value2)
        {
            return !(value1 == value2);
        }
        
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Width.GetHashCode() ^ Height.GetHashCode();
        }

        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString() + ", " +
                   Width.ToString() + ", " + Height.ToString();
        }

        /// <summary>
        /// Creates the smallest BoundingBox that will contain a group of points.
        /// </summary>
        public static BoundingRectangle CreateFromPoints(IEnumerable<Vector2> points)
        {
            var min = Vector2.One * float.MaxValue;
            var max = Vector2.One * float.MinValue;

            foreach (Vector2 pt in points)
            {
                if (pt.X < min.X)
                    min.X = pt.X;
                if (pt.X > max.X)
                    max.X = pt.X;
                if (pt.Y < min.Y)
                    min.Y = pt.Y;
                if (pt.Y > max.Y)
                    max.Y = pt.Y;
            }

            return new BoundingRectangle(min.X, min.Y, max.X - min.X, max.Y - min.Y);
        }

        /// <summary>
        /// Creates the merged bounding rectangle.
        /// </summary>        
        public static void CreateMerged(ref BoundingRectangle original, ref BoundingRectangle additional, out BoundingRectangle result)
        {
            result = new BoundingRectangle();
            result.X = (original.X > additional.X) ? additional.X : original.X;
            result.Y = (original.Y > additional.Y) ? additional.Y : original.Y;
            result.Width = (original.X + original.Width < additional.X + additional.Width) ? additional.X + additional.Width : original.X + original.Width;
            result.Height = (original.Y + original.Height < additional.Y + additional.Height) ? additional.Y + additional.Height : original.Y + original.Height;
            result.Width -= result.X;
            result.Height -= result.Y;
        }

        /// <summary>
        /// Creates the merged bounding rectangle.
        /// </summary>
        public static BoundingRectangle CreateMerged(BoundingRectangle original, BoundingRectangle additional)
        {
            BoundingRectangle result = new BoundingRectangle();
            CreateMerged(ref original, ref additional, out result);
            return result;
        }
    }
}
