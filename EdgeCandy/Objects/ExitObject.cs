using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.States;
using EdgeCandy.Subsystems;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

namespace EdgeCandy.Objects
{
    public class ExitObject : GameObject
    {
        private PhysicsComponent Physics;

        private bool Victory = false;
        public ExitObject(Vector2 position)
        {
            Physics = new PhysicsComponent();
            Physics.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, 0.01f, 0.01f, 1, position, this);
            Physics.Body.IsSensor = true;
            Physics.Body.OnCollision += (a, b, contact) =>
            {
                var player = (a.Body.UserData as Player) ?? (b.Body.UserData as Player);
                if (player != null)
                    Victory = true;
                return true;
            };
        }

        public override void SyncComponents()
        {
            if(Victory)
                Program.TransitionToState<VictoryState>();
        }
    }
}
