namespace Nine.Geometry
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public struct Rectangle : IEquatable<Rectangle>
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        /// <summary>
        /// Gets the left side of the rectangle.
        /// </summary>
        public int Left => X;

        /// <summary>
        /// Gets the right side of the rectangle.
        /// </summary>
        public int Right => X + Width;

        /// <summary>
        /// Gets the top side of the rectangle.
        /// </summary>
        public int Top => Y;

        /// <summary>
        /// Gets the bottom side of the rectangle.
        /// </summary>
        public int Bottom => Y + Height;

        /// <summary>
        /// Initialize a new <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        public Rectangle(Point position, Point size)
            : this(position.X, position.Y, size.X, size.Y)
        {

        }

        /// <summary>
        /// Initialize a new <see cref="Rectangle"/> struct.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rectangle(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool Contains(Point value) => Contains(value.X, value.Y);
        public bool Contains(int x, int y)
        {
            return (this.X <= x) && x < (this.X + this.Width) 
                && (this.Y <= y) && y < (this.Y + this.Height);
        }

        public bool Intersects(Rectangle value)
        {
            return value.Left < Right && Left < value.Right &&
                   value.Top < Bottom && Top < value.Bottom;
        }

        public static bool operator ==(Rectangle a, Rectangle b) => ((a.X == b.X) && (a.Y == b.Y) && (a.Width == b.Width) && (a.Height == b.Height));
        public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);

        public bool Equals(Rectangle other)
        {
            return this == other;
        }

        public override bool Equals(object obj)
        {
            return (obj is Rectangle) && this == ((Rectangle)obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"{X}, {Y}, {Width}, {Height}";
        }
    }
}
