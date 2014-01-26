using System;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.Framework
{
    public static class Graphics
    {
        private static RenderTexture windowTexture;
        public const uint Width = 483, MaxHeight = 512;
        public static uint Height = 270;
        private static uint windowWidth = Width, windowHeight = Height;
        private static float scale = 1;

        public static void Initialize()
        {
            windowTexture = new RenderTexture(Width, Height);
        }

        public static void Resize(uint width, uint height)
        {
            windowWidth = width;
            windowHeight = height;

            scale = Math.Max(1, width / Width);

            Height = Math.Min((uint)(height / scale) + 1, MaxHeight);

            windowTexture.Dispose();
            windowTexture = new RenderTexture(Width, Height);

            windowTexture.Clear(Color.White);
        }

        public static void Draw(Drawable drawable)
        {
            windowTexture.Draw(drawable);
        }

        public static void DrawText(Text text)
        {
            var color = text.Color;
            var position = text.Position;

            // The line spacing is too high, so offset the Y-position to compensate
            text.Position = new Vector2f(text.Position.X + 1, text.Position.Y - 4);
            text.Color = Color.Black;

            windowTexture.Draw(text);
            
            text.Position = new Vector2f(text.Position.X - 1, text.Position.Y - 1);
            text.Color = color;

            windowTexture.Draw(text);

            text.Position = position;
        }

        public static void RenderTo(RenderTarget target)
        {
            windowTexture.Display();

            var sprite = new Sprite(windowTexture.Texture)
                         {
                             Scale = new Vector2f(scale, scale),
                             Position =
                                 new Vector2f((int)(windowWidth - Width * scale) / 2,
                                              (int)(windowHeight - Height * scale) / 2)
                         };

            target.Draw(sprite);
        }

        public static void Clear()
        {
            windowTexture.Clear(Color.Black);
        }

        public static void SetView(View view)
        {
            windowTexture.SetView(view);
        }
    }
}
