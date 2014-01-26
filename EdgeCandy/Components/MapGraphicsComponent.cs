using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Framework;
using SFML.Graphics;
using SFML.Window;
using TiledSharp;

namespace EdgeCandy.Components
{
    public class MapGraphicsComponent : GraphicsComponent
    {
        public TmxMap Map { get; set; }
        private readonly Sprite sprite = new Sprite(Content.Tileset);

        public override void Draw()
        {
            foreach (var layer in Map.Layers)
            {
                foreach (var tile in layer.Tiles)
                {
                    var columns = (int)sprite.Texture.Size.X / 23; // sorry pi

                    int x = (tile.Gid - 1) % columns,
                        y = (tile.Gid - 1) / columns;

                    sprite.TextureRect = new IntRect(x * 23 + 2, y * 23 + 2, 21, 21);
                    sprite.Position = new Vector2f(tile.X * 21, tile.Y * 21);

                    Graphics.Draw(sprite);
                }
            }
        }
    }
}
