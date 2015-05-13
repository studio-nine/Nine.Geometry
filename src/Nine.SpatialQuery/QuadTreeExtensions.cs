namespace Nine.SpatialQuery
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class QuadTreeExtensions
    {
        public static void DrawDiagnostics(this QuadTreeCollection quadtree, SpriteBatch spriteBatch, Color color)
        {
            quadtree.Tree.Traverse(quadtree.Tree.root, node =>
            {
                spriteBatch.DrawRectangle(node.bounds, color);
                return TraverseOptions.Continue;
            });
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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
