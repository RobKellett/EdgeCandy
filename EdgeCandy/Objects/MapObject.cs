using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using TiledSharp;

namespace EdgeCandy.Objects
{
    public class MapObject : GameObject
    {
        public MapGraphicsComponent Graphics;
        public List<GameObject> Platforms = new List<GameObject>();
        public List<CandyObject> Candies = new List<CandyObject>(); 
        public List<TextComponent> Hints = new List<TextComponent>(); 
        public List<PowerupObject> Powerups = new List<PowerupObject>();
        public TmxMap Map { get; set; }

        public Vector2 Spawn;
        public ExitObject Exit;

        public MapObject(TmxMap map)
        {
            Map = map;
            Graphics = new MapGraphicsComponent(map);

            foreach (var platform in map.ObjectGroups["Solid"].Objects)
                Platforms.Add(new PlatformObject(ConvertUnits.ToSimUnits(platform.X),
                                                 ConvertUnits.ToSimUnits(platform.Y),
                                                 ConvertUnits.ToSimUnits(platform.Width),
                                                 ConvertUnits.ToSimUnits(platform.Height)));

            // "implicit" walls to prevent leaving the screen
            Platforms.Add(new WallObject(ConvertUnits.ToSimUnits(-map.TileWidth),
                                             0,
                                             ConvertUnits.ToSimUnits(map.TileWidth),
                                             ConvertUnits.ToSimUnits(map.Height * map.TileHeight)));

            Platforms.Add(new WallObject(ConvertUnits.ToSimUnits(map.Width * map.TileWidth),
                                             0,
                                             ConvertUnits.ToSimUnits(map.TileWidth),
                                             ConvertUnits.ToSimUnits(map.Height * map.TileHeight)));

            foreach (var candy in map.ObjectGroups["Candy"].Objects)
            {
                CandyKind kind;
                if (Enum.TryParse(candy.Name, out kind))
                {
                    CandyObject cobj;
                    if (kind == CandyKind.Chocolate)
                        cobj = new CandyObject(kind, new Vector2(candy.X, candy.Y), new Vector2(candy.Width, candy.Height));
                    else
                        cobj = new CandyObject(kind, new Vector2(candy.X + candy.Width / 2, candy.Y + candy.Height / 2));
                    Candies.Add(cobj);
                }
            }

            foreach (var obj in map.ObjectGroups["Misc"].Objects)
            {
                if (obj.Name == "Text")
                {
                    Hints.Add(new TextComponent
                              {
                                  Text =
                                      new Text(obj.Properties["Text"], Content.Font, 16)
                                      {
                                          Position = new Vector2f(obj.X, obj.Y)
                                      }
                              });
                }
                else if (obj.Name == "Powerup")
                {
                    Powerups.Add(new PowerupObject(new Vector2f(obj.X + obj.Width / 2, obj.Y + obj.Height / 2)));
                }
                else if (obj.Name == "Spawn")
                    Spawn = new Vector2(ConvertUnits.ToSimUnits(obj.X + obj.Width / 2), ConvertUnits.ToSimUnits(obj.Y + obj.Height / 2));
                else if (obj.Name == "Exit")
                    Exit = new ExitObject(new Vector2(ConvertUnits.ToSimUnits(obj.X + obj.Width/2), ConvertUnits.ToSimUnits(obj.Y + obj.Height/2)));
            }
        }

        public override void SyncComponents()
        {
            // uh
        }
    }
}
