namespace Nine.Geometry
{
    using System;
    using System.Numerics;

    /// <summary>
    /// 
    /// </summary>
    public struct Point : IEquatable<Point>
    {
        public int X;
        public int Y;

        /// <summary>
        /// Initialize a new <see cref="Point"/> struct.
        /// </summary>
        /// <param name="value"></param>
        public Point(int value)
            : this(value, value)
        {

        }

        /// <summary>
        /// Initialize a new <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Convert the <see cref="Point"/> to a <see cref="Vector2"/>.
        /// </summary>
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }

        public static Point operator +(Point value1, Point value2) => new Point(value1.X + value2.X, value1.Y + value2.Y);
        public static Point operator -(Point value1, Point value2) => new Point(value1.X - value2.X, value1.Y - value2.Y);
        public static Point operator *(Point value1, Point value2) => new Point(value1.X * value2.X, value1.Y * value2.Y);
        public static Point operator /(Point value1, Point value2) => new Point(value1.X / value2.X, value1.Y / value2.Y);

        public static bool operator ==(Point a, Point b) => a.Equals(b);
        public static bool operator !=(Point a, Point b) => !a.Equals(b);

        public bool Equals(Point other) => ((X == other.X) && (Y == other.Y));

        public override bool Equals(object obj)
        {
            return (obj is Point) && Equals((Point)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }
    }
}
