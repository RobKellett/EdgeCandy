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

        public MapObject(TmxMap map)
        {
            Graphics = new MapGraphicsComponent { Map = map };

            GameObjectSubsystem.Instance.Register(this);
        }

        public override void SyncComponents()
        {
            // uh
        }
    }
}
