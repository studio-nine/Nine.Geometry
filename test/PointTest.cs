namespace Nine.Geometry.Test
{
    using Xunit;

    public class PointTest
    {
        [Fact]
        public void Ctor_value()
        {
            var input = 123456789;

            var value = new Point(input);

            Assert.Equal(input, value.X);
            Assert.Equal(input, value.Y);
        }

        [Fact]
        public void Ctor_x_y()
        {
            var inputX = 1234;
            var inputY = 5678;

            var value = new Point(inputX, inputY);

            Assert.Equal(inputX, value.X);
            Assert.Equal(inputY, value.Y);
        }

        [Fact]
        public void ToVector2()
        {
            var inputX = 1234;
            var inputY = 5678;

            var value = new Point(inputX, inputY).ToVector2();

            Assert.Equal(inputX, value.X);
            Assert.Equal(inputY, value.Y);
        }

        [Theory]
        [InlineData(100, 200, 300, 400)]
        public void Operator_Additive(int input1X, int input1Y, int input2X, int input2Y)
        {
            var value1 = new Point(input1X, input1Y);
            var value2 = new Point(input2X, input2Y);
            var value = value1 + value2;

            Assert.Equal(input1X + input2X, value.X);
            Assert.Equal(input1Y + input2Y, value.Y);
        }

        [Theory]
        [InlineData(100, 200, 300, 400)]
        public void Operator_Subtractive(int input1X, int input1Y, int input2X, int input2Y)
        {
            var value1 = new Point(input1X, input1Y);
            var value2 = new Point(input2X, input2Y);
            var value = value1 - value2;

            Assert.Equal(input1X - input2X, value.X);
            Assert.Equal(input1Y - input2Y, value.Y);
        }

        [Theory]
        [InlineData(100, 200, 300, 400)]
        public void Operator_Multiplicative(int input1X, int input1Y, int input2X, int input2Y)
        {
            var value1 = new Point(input1X, input1Y);
            var value2 = new Point(input2X, input2Y);
            var value = value1 * value2;

            Assert.Equal(input1X * input2X, value.X);
            Assert.Equal(input1Y * input2Y, value.Y);
        }

        [Theory]
        [InlineData(100, 200, 300, 400)]
        public void Operator_Division(int input1X, int input1Y, int input2X, int input2Y)
        {
            var value1 = new Point(input1X, input1Y);
            var value2 = new Point(input2X, input2Y);
            var value = value1 / value2;

            Assert.Equal(input1X / input2X, value.X);
            Assert.Equal(input1Y / input2Y, value.Y);
        }

        [Theory]
        [InlineData(100, 200, 100, 200, true)]
        [InlineData(200, 100, 100, 200, false)]
        public void Operator_Equal(int input1X, int input1Y, int input2X, int input2Y, bool expected)
        {
            var value1 = new Point(input1X, input1Y);
            var value2 = new Point(input2X, input2Y);
            var value = value1 == value2;

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(100, 200, 100, 200, false)]
        [InlineData(200, 100, 100, 200, true)]
        public void Operator_NotEqual(int input1X, int input1Y, int input2X, int input2Y, bool expected)
        {
            var value1 = new Point(input1X, input1Y);
            var value2 = new Point(input2X, input2Y);
            var value = value1 != value2;

            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData(100, 200, 100, 200, true)]
        [InlineData(200, 100, 100, 200, false)]
        public void Operator_EqualObject(int input1X, int input1Y, int input2X, int input2Y, bool expected)
        {
            var value1 = new Point(input1X, input1Y);
            var value2 = new Point(input2X, input2Y);
            var value = value1.Equals((object)value2);

            Assert.Equal(expected, value);
        }

        [Fact]
        public void Operator_EqualObject_Null()
        {
            var value1 = new Point();
            var value = value1.Equals(null);

            Assert.False(value);
        }

        [Theory]
        [InlineData(100, 200, 100, 200, true)]
        [InlineData(200, 200, 200, 200, true)]
        [InlineData(200, 200, 300, 300, false)]
        [InlineData(200, 200, 100, 100, false)]
        public void GetHashCode_Unique(int input1X, int input1Y, int input2X, int input2Y, bool expectedSame)
        {
            var value1 = new Point(input1X, input1Y);
            var value2 = new Point(input2X, input2Y);

            var value1Hash = value1.GetHashCode();
            var value2Hash = value2.GetHashCode();

            Assert.Equal(expectedSame, value1Hash == value2Hash);
        }

        [Fact]
        public void ToString_Equal()
        {
            var inputX = 1234;
            var inputY = 5678;

            var value = new Point(inputX, inputY);

            Assert.Equal($"X: {inputX}, Y: {inputY}", value.ToString());
        }
    }
}
