using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Subsystems;

namespace EdgeCandy.Components
{
    public abstract class InputComponent
    {
        protected InputComponent()
        {
            InputSubsystem.Instance.Register(this);
        }

        public abstract void HandleInput();
    }
}
