namespace Nine.Geometry
{
    using System.Numerics;
    
    public static class GeometryHelper
    {
        /// <summary>
        /// Returns whether the points are in counter clockwise order.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static bool PointsAreCounterClockwiseOrder(Vector2[] points)
        {
            float signedArea = 0;
            for (int i = 0; i < points.Length; i++)
            {
                int nextIndex = (i + 1) % points.Length;
                signedArea += (points[nextIndex].X - points[i].X)
                            * (points[nextIndex].Y + points[i].Y);
            }

            return signedArea < 0;
        }

        /// <summary>
        /// Returns whether the points are in counter clockwise order.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static bool PointsAreCounterClockwiseOrder(Vector3[] points)
        {
            float signedArea = 0;
            for (int i = 0; i < points.Length; i++)
            {
                int nextIndex = (i + 1) % points.Length;
                signedArea += (points[nextIndex].X - points[i].X)
                            * (points[nextIndex].Y + points[i].Y)
                            * (points[nextIndex].Z + points[i].Z);
            }

            return signedArea < 0;
        }
    }
}
