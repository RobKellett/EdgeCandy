using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Subsystems;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace EdgeCandy.Objects
{
    public class WallObject : GameObject
    {
        public PhysicsComponent Physics = new PhysicsComponent();

        public WallObject(float x, float y, float width, float height)
        {
            Physics.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, width, height, 100,
                                                        new Vector2(x + width / 2, y + height / 2));

            Physics.Body.IsStatic = true; // platforms shouldn't be pushed around
            Physics.Body.Friction = 0;
            Physics.Body.UserData = this;

            GameObjectSubsystem.Instance.Register(this);
        }

        public override void SyncComponents()
        {
            // uh
        }
    }
}
