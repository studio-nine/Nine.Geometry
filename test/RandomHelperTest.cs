namespace Nine.Geometry.Test
{
    using System;
    using Xunit;

    public class RandomHelperTest
    {
        [Theory]
        [InlineData(0, 1)]
        [InlineData(0, 100)]
        [InlineData(-1, 0)]
        [InlineData(-100, 0)]
        public void NextDouble_Unique(double min, double max)
        {
            var rd = new Random();

            var value1 = rd.NextDouble(min, max);
            var value2 = rd.NextDouble(min, max);

            Assert.NotEqual(value1, value2);
        }
    }
}
