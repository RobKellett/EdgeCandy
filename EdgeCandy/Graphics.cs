using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy
{
    public static class Graphics
    {
        private static RenderTexture windowTexture;
        public const uint Width = 480, Height = 270;
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

            scale = Math.Max(1, windowWidth / Width);
        }

        public static void Draw(Drawable drawable)
        {
            windowTexture.Draw(drawable);
        }

        public static void RenderTo(RenderTarget target)
        {
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
