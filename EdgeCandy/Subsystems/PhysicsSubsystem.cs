using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SFML.Window;

namespace EdgeCandy.Subsystems
{
    /// <summary>
    /// Simulates physics
    /// </summary>
    public class PhysicsSubsystem : Subsystem<PhysicsSubsystem, PhysicsComponent>
    {
        private World _world;

        public World World
        {
            get { return _world; }
        }

        public PhysicsSubsystem()
        {
            _world = new World(new Vector2(0, 9.8f));
        }

        private double frameCounter = 0;
        private const float simRate = 0.016f;
        /// <summary>
        /// Simulate!
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since last frame, in seconds</param>
        public void Update(double elapsedTime)
        {
            frameCounter += elapsedTime;
            while (frameCounter > simRate)
            {
                _world.Step(simRate);
                frameCounter -= simRate;

                foreach (var component in components)
                    component.Update();
            }

            Clean();
        }

        public override void Unregister(PhysicsComponent component)
        {
            _world.RemoveBody(component.Body);

            base.Unregister(component);
        }
    }
}
