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
    public class PlatformObject : GameObject
    {
        public PhysicsComponent Physics = new PhysicsComponent();

        public PlatformObject(int x, int y, int width, int height)
        {
            Physics.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, width, height, 0,
                                                       new Vector2(x, y));
            Physics.Body.IsStatic = true; // platforms shouldn't be pushed around
            Physics.Body.BodyType = BodyType.Static; // Is this different from the line above???

            GameObjectSubsystem.Instance.Register(this);
        }

        public override void SyncComponents()
        {
            // uh
        }
    }
}
