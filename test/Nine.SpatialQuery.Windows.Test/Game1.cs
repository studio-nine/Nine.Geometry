namespace Nine.SpatialQuery.Test
{
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    interface ISpriteBatchDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }

    class SampleObject : ISpatialQueryable, ISpriteBatchDrawable
    {
        public BoundingBox BoundingBox { get; private set; }
        public object SpatialData { get; set; }
        public event EventHandler<EventArgs> BoundingBoxChanged;

        public readonly Rectangle Rectangle;

        public SampleObject(Vector2 position)
            : this(new Rectangle((int)position.X, (int)position.Y, 10, 10))
        {

        }

        public SampleObject(Rectangle rectangle)
        {
            this.Rectangle = rectangle;

            var min = new Vector3(rectangle.Left, rectangle.Top, 0);
            var max = new Vector3(rectangle.Right, rectangle.Bottom, 0);
            this.BoundingBox = new BoundingBox(min, max);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.Rectangle, Color.Gray);
        }
    }

    class Game1 : Microsoft.Xna.Framework.Game
    {
        private QuadTreeCollection quadtree;
        private SpriteBatch spriteBatch;
        private MouseState previusMouse;

        public Game1()
        {
            var graphics = new GraphicsDeviceManager(this);
            
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;

            this.IsMouseVisible = true;
            this.IsFixedTimeStep = false;

            this.Window.AllowUserResizing = true;
        }

        protected override void LoadContent()
        {
            this.quadtree = new QuadTreeCollection();

            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            
            base.LoadContent();
        }
        
        protected override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed &&
                previusMouse.LeftButton != ButtonState.Pressed)
            {
                this.quadtree.Add(new SampleObject(new Vector2(mouse.X, mouse.Y)));
            }

            this.previusMouse = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            this.spriteBatch.Begin();

            // TODO: Add a way to visually debug the tree nodes

            // TODO: Query just the screen here
            foreach (var item in quadtree)
            {
                var drawable = item as ISpriteBatchDrawable;
                if (drawable != null)
                {
                    drawable.Draw(this.spriteBatch);
                }
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
        
        static void Main(string[] args)
        {
            var game = new Game1();
            game.Run();
        }
    }

    public static class SpriteBatchExtensions
    {
        private static Texture2D blankTexture;

        public static void DrawRectangle(this SpriteBatch spriteBatch, BoundingRectangle rect, Color color, float thickness = 1)
        {
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(rect.X + 1f, rect.Y), new Vector2(rect.X + 1f, rect.Bottom + thickness), color, thickness);
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Bottom), new Vector2(rect.Right, rect.Bottom), color, thickness);
            DrawLine(spriteBatch, new Vector2(rect.Right + 1f, rect.Y), new Vector2(rect.Right + 1f, rect.Bottom + thickness), color, thickness);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness = 1)
        {
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Y), new Vector2(rect.Right, rect.Y), color, thickness);
            DrawLine(spriteBatch, new Vector2(rect.X + 1f, rect.Y), new Vector2(rect.X + 1f, rect.Bottom + thickness), color, thickness); 
            DrawLine(spriteBatch, new Vector2(rect.X, rect.Bottom), new Vector2(rect.Right, rect.Bottom), color, thickness);
            DrawLine(spriteBatch, new Vector2(rect.Right + 1f, rect.Y), new Vector2(rect.Right + 1f, rect.Bottom + thickness), color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness = 1)
        {
            float distance = Vector2.Distance(start, end);
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            DrawLine(spriteBatch, start, distance, angle, color, thickness);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, float length, float angle, Color color, float thickness = 1)
        {
            if (blankTexture == null)
                CreateBlankTexture(spriteBatch.GraphicsDevice);

            spriteBatch.Draw(blankTexture, start, null, color, angle,
                Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        private static void CreateBlankTexture(GraphicsDevice graphics)
        {
            blankTexture = new Texture2D(graphics, 1, 1);
            blankTexture.SetData(new Color[] { Color.White });
        }
    }
}
