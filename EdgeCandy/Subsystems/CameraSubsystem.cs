using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;

namespace EdgeCandy.Subsystems
{
    public class CameraSubsystem : Subsystem<CameraSubsystem, CameraComponent>
    {
        public void Update(double elapsedTime)
        {
            foreach (var component in components)
                component.Update(elapsedTime);
        }
    }
}
