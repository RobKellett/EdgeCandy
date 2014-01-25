using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;

namespace EdgeCandy.Objects
{
    public class Player : GameObject
    {
        public GraphicsComponent Sprite = new GraphicsComponent();
        public PhysicsComponent Physics = new PhysicsComponent();

        public override void SyncComponents()
        {
            // Physics is the be-all end-all for position
            // We could have multiple graphics components, some physical some purely visual,
            // we could apply an offset here, etc.  Pretty powerful model.
            Sprite.RenderPosition = Physics.Position;
        }
    }
}
