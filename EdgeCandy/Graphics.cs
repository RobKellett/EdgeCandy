using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy
{
    public static class Graphics
    {
        private static RenderTexture windowTexture;
        public static uint Width = 480, Height = 270;
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

            Height = (uint)(height / scale) + 1;

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

            text.Position = new Vector2f(text.Position.X + 1, text.Position.Y + 1);
            text.Color = Color.Black;

            windowTexture.Draw(text);

            text.Position = position;
            text.Color = color;

            windowTexture.Draw(text);
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
    }
}
