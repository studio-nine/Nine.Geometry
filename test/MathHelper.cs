namespace Nine.Geometry.Test
{
    static class MathHelper
    {
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }
    }
}
