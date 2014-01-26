using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using TiledSharp;

namespace EdgeCandy.Objects
{
    public class MapObject : GameObject
    {
        public MapGraphicsComponent Graphics;
        public List<PlatformObject> Platforms = new List<PlatformObject>();

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
                                             ConvertUnits.ToSimUnits(map.Height * map.TileHeight), false));

            Platforms.Add(new PlatformObject(ConvertUnits.ToSimUnits(map.Width * map.TileWidth),
                                             0,
                                             ConvertUnits.ToSimUnits(map.TileWidth),
                                             ConvertUnits.ToSimUnits(map.Height * map.TileHeight), false));

            GameObjectSubsystem.Instance.Register(this);
        }

        public override void SyncComponents()
        {
            // uh
        }
    }
}
