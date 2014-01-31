using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;

namespace EdgeCandy.Subsystems
{
    class UpdateSubsystem : Subsystem<UpdateSubsystem, IUpdateableComponent>
    {
        public void Update(double elapsedTime)
        {
            foreach (var component in components)
                component.Update(elapsedTime);
        }
    }
}
