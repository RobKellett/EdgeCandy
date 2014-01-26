using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using SFML.Window;

namespace EdgeCandy.Subsystems
{
    public class InputSubsystem : Subsystem<InputSubsystem, InputComponent>
    {
        public void HandleInput()
        {
            foreach (var component in components)
            {
                component.HandleInput();
            }
        }
    }
}
