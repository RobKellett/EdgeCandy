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
using TiledSharp;

namespace EdgeCandy.Objects
{
    public class MapObject : GameObject
    {
        public MapGraphicsComponent Graphics;
        public List<PlatformObject> Platforms = new List<PlatformObject>();
        public List<CandyObject> Candies = new List<CandyObject>(); 

        public MapObject(TmxMap map)
        {
            Graphics = new MapGraphicsComponent { Map = map };

            foreach (var platform in map.ObjectGroups["Solid"].Objects)
                Platforms.Add(new PlatformObject(ConvertUnits.ToSimUnits(platform.X),
                                                 ConvertUnits.ToSimUnits(platform.Y),
                                                 ConvertUnits.ToSimUnits(platform.Width),
                                                 ConvertUnits.ToSimUnits(platform.Height)));

            // "implicit" walls to prevent leaving the screen
            Platforms.Add(new PlatformObject(ConvertUnits.ToSimUnits(-map.TileWidth),
                                             0,
                                             ConvertUnits.ToSimUnits(map.TileWidth),
                                             ConvertUnits.ToSimUnits(map.Height * map.TileHeight), true));

            Platforms.Add(new PlatformObject(ConvertUnits.ToSimUnits(map.Width * map.TileWidth),
                                             0,
                                             ConvertUnits.ToSimUnits(map.TileWidth),
                                             ConvertUnits.ToSimUnits(map.Height * map.TileHeight), true));

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

            GameObjectSubsystem.Instance.Register(this);
        }

        public override void SyncComponents()
        {
            // uh
        }
    }
}
