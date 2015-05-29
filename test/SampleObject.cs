//namespace Nine.SpatialQuery
//{
//    using Microsoft.Xna.Framework;
//    using Microsoft.Xna.Framework.Graphics;
//    using System;

//    public interface ISpriteBatchDrawable
//    {
//        void Draw(SpriteBatch spriteBatch);
//    }

//    public class SampleObject : ISpatialQueryable, ISpriteBatchDrawable
//    {
//        public BoundingBox BoundingBox { get; private set; }
//        public object SpatialData { get; set; }
//        public event EventHandler<EventArgs> BoundingBoxChanged;

//        public readonly Rectangle Rectangle;
//        public readonly Color Color;

//        public SampleObject(Rectangle rectangle, Color color)
//        {
//            this.Rectangle = rectangle;
//            this.Color = color;

//            var min = new Vector3(rectangle.Left, rectangle.Top, 0);
//            var max = new Vector3(rectangle.Right, rectangle.Bottom, 0);
//            this.BoundingBox = new BoundingBox(min, max);
//        }
        
//        public SampleObject(Vector3 min, Vector3 size, Color color)
//        {
//            this.Color = color;

//            this.BoundingBox = new BoundingBox(min, min + size);
//        }

//        public void Draw(SpriteBatch spriteBatch)
//        {
//            spriteBatch.DrawRectangle(this.Rectangle, this.Color);
//        }
//    }
//}
