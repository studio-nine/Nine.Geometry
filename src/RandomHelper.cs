namespace Nine.Geometry
{
    using System;
    using System.ComponentModel;
    using System.Numerics;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class RandomHelper
    {
        public static double NextDouble(this Random rd, double min, double max)
        {
            return rd.NextDouble() * (max - min) + min;
        }

        public static Vector2 NextVector2(this Random rd, Vector2 min, Vector2 max)
        {
            var valueX = (float)rd.NextDouble(min.X, max.X);
            var valueY = (float)rd.NextDouble(min.Y, max.Y);
            return new Vector2(valueX, valueY);
        }

        public static Vector3 NextVector3(this Random rd, Vector3 min, Vector3 max)
        {
            var valueX = (float)rd.NextDouble(min.X, max.X);
            var valueY = (float)rd.NextDouble(min.Y, max.Y);
            var valueZ = (float)rd.NextDouble(min.Z, max.Z);
            return new Vector3(valueX, valueY, valueZ);
        }
    }
}
