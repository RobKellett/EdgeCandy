using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Subsystems;
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
                Platforms.Add(new PlatformObject(platform.X, platform.Y, platform.Width, platform.Height));

            GameObjectSubsystem.Instance.Register(this);
        }

        public override void SyncComponents()
        {
            // uh
        }
    }
}
