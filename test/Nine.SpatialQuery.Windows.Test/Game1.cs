namespace Nine.SpatialQuery.Test
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    
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
            this.quadtree = new QuadTreeCollection(
                new BoundingRectangle(1280, 800), 5);

            for (int i = 0; i < 20; i++)
            {
                this.quadtree.Add(new SampleObject(new Rectangle(10 * i, 10 * i, 10, 10), Color.Gray));
            }

            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            
            base.LoadContent();
        }
        
        protected override void Update(GameTime gameTime)
        {
            var mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed &&
                previusMouse.LeftButton != ButtonState.Pressed)
            {
                this.quadtree.Add(new SampleObject(new Rectangle(mouse.X, mouse.Y, 10, 10), Color.Gray));
            }

            this.previusMouse = mouse;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            this.spriteBatch.Begin();

            // TODO: Query just the screen here
            foreach (var item in quadtree)
            {
                var drawable = item as ISpriteBatchDrawable;
                if (drawable != null)
                {
                    drawable.Draw(this.spriteBatch);
                }
            }

            quadtree.DrawDiagnostics(this.spriteBatch, Color.Red);

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
        
        static void Main(string[] args)
        {
            var game = new Game1();
            game.Run();
        }
    }
}
