using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Framework;
using FarseerPhysics;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.Components
{
    public class RectangleCompontent : GraphicsComponent
    {
        private Sprite sprite;

        public FloatRect Rectangle { get; set; }
        public Color Color { get; set; }

        public RectangleCompontent(Color color)
        {
            sprite = new Sprite(Content.Pixel);

            Color = color;
        }

        public override void Draw()
        {
            sprite.Position = new Vector2f(ConvertUnits.ToDisplayUnits(Rectangle.Left),
                                           ConvertUnits.ToDisplayUnits(Rectangle.Top));

            sprite.Scale = new Vector2f(ConvertUnits.ToDisplayUnits(Rectangle.Width),
                                        ConvertUnits.ToDisplayUnits(Rectangle.Height));

            sprite.Color = Color;

            Graphics.Draw(sprite);
        }
    }
}
