using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;

namespace EdgeCandy.Subsystems
{
    /// <summary>
    /// Animates things
    /// </summary>
    public class AnimationSubsystem : Subsystem<AnimationSubsystem, AnimatableGraphicsComponent>
    {
        /// <summary>
        /// Animate!
        /// </summary>
        public void Update(double elapsedTime)
        {
            foreach (var component in components)
            {
                component.Update(elapsedTime);
            }
        }
    }
}
