using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Subsystems;
using SFML.Window;

namespace EdgeCandy.Components
{
    /// <summary>
    /// A piece of a game object which is physically simulated.
    /// </summary>
    public class PhysicsComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PhysicsComponent()
        {
            // Automatically register with the physics subsystem.
            PhysicsSubsystem.Instance.Register(this);
        }

        /// <summary>
        /// The position that we have been simulated at.
        /// </summary>
        public Vector2f Position
        {
            get; 
            set;
        }
    }
}
