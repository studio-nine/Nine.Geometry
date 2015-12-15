namespace Nine.Geometry
{
    static class Helper
    {
        public static float GetPerimeter(this BoundingBox boundingBox)
        {
            return 2.0f * (
                (boundingBox.Max.X - boundingBox.Min.X) + 
                (boundingBox.Max.Y - boundingBox.Min.Y) +
                (boundingBox.Max.Z - boundingBox.Min.Z));
        }
    }
}
