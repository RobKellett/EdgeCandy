using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using SFML.Graphics;

namespace EdgeCandy.Objects
{
    public class Player : GameObject
    {
        public SpriteComponent Graphics = new SpriteComponent();
        public PhysicsComponent Physics = new PhysicsComponent();

        public Player()
        {
            Graphics.Sprite = new Sprite(Content.Player);
        }

        public override void SyncComponents()
        {
            // Physics is the be-all end-all for position
            // We could have multiple graphics components, some physical some purely visual,
            // we could apply an offset here, etc.  Pretty powerful model.
            Graphics.Sprite.Position = Physics.Position;
        }
    }
}
