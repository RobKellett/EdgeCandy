using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Subsystems;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.Components
{
    /// <summary>
    /// Represents a piece of a game object that gets drawn.
    /// </summary>
    public class GraphicsComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GraphicsComponent()
        {
            // Automatically register with the graphics subsystem, so we get drawn!
            GraphicsSubsystem.Instance.Register(this);
        }

        public virtual void Draw() { }
    }
}
