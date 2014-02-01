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
    public class MapGraphicsComponent : SpriteComponent
    {
        private readonly Sprite tileset = new Sprite(Content.Tileset);

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly RenderTexture texture;

        public MapGraphicsComponent(TmxMap Map)
        {
            var tileWidth = Map.Tilesets.Single().TileWidth;
            var tileHeight = Map.Tilesets.Single().TileHeight;
            var tileSpacing = Map.Tilesets.Single().Spacing;
            var tileMargins = Map.Tilesets.Single().Margin;

            texture = new RenderTexture((uint)(Map.Width * tileWidth), (uint)(Map.Height * tileHeight));

            foreach (var layer in Map.Layers)
            {
                foreach (var tile in layer.Tiles)
                {
                    var columns = (int)tileset.Texture.Size.X / (tileWidth + tileSpacing);

                    int x = (tile.Gid - 1) % columns,
                        y = (tile.Gid - 1) / columns;

                    tileset.TextureRect = new IntRect(x * (tileWidth + tileSpacing) + tileMargins,
                                                        y * (tileHeight + tileSpacing) + tileMargins, tileWidth,
                                                        tileHeight);
                    tileset.Position = new Vector2f(tile.X * tileWidth, tile.Y * tileHeight);

                    texture.Draw(tileset);
                }
            }

            texture.Display();
            Sprite = new Sprite(texture.Texture);
        }
    }
}
