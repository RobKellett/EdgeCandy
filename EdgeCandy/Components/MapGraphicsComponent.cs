using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
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
            var tileWidth = Map.Tilesets.Single().TileWidth;
            var tileHeight = Map.Tilesets.Single().TileHeight;
            var tileSpacing = Map.Tilesets.Single().Spacing;
            var tileMargins = Map.Tilesets.Single().Margin;

            var cameraOffset = GraphicsSubsystem.Instance.GetCameraOffset(); 

            foreach (var layer in Map.Layers)
            {
                foreach (var tile in layer.Tiles)
                {
                    var yPos = tile.Y * tileHeight;
                    if (yPos + tileHeight < cameraOffset.Y || yPos > cameraOffset.Y + Graphics.Height)
                        continue;

                    var columns = (int)sprite.Texture.Size.X / (tileWidth + tileSpacing);

                    int x = (tile.Gid - 1) % columns,
                        y = (tile.Gid - 1) / columns;

                    sprite.TextureRect = new IntRect(x * (tileWidth + tileSpacing) + tileMargins,
                                                        y * (tileHeight + tileSpacing) + tileMargins, tileWidth,
                                                        tileHeight);
                    sprite.Position = new Vector2f(tile.X * tileWidth, tile.Y * tileHeight);

                    Graphics.Draw(sprite);
                }
            }
        }
    }
}
