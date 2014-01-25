using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using SFML.Graphics;

namespace EdgeCandy.Subsystems
{
    /// <summary>
    /// Draws things
    /// </summary>
    public class GraphicsSubsystem : Subsystem<GraphicsSubsystem, GraphicsComponent>
    {
        /// <summary>
        /// Draw!
        /// </summary>
        public void Draw()
        {
            foreach (var component in components)
            {
                component.Draw();
            }
        }
    }
}
