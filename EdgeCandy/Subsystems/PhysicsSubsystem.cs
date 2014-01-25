using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using SFML.Window;

namespace EdgeCandy.Subsystems
{
    /// <summary>
    /// Simulates physics
    /// </summary>
    public class PhysicsSubsystem : Subsystem<PhysicsSubsystem, PhysicsComponent>
    {
        /// <summary>
        /// Simulate!
        /// </summary>
        /// <param name="elapsedTime">Time elapsed since last frame, in seconds</param>
        public void Update(double elapsedTime)
        {
            foreach (var component in components)
                component.Position += new Vector2f((float)elapsedTime, 0);
        }
    }
}
