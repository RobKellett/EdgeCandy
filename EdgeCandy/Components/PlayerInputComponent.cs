using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;

namespace EdgeCandy.Components
{
    public class PlayerInputComponent : InputComponent
    {
        /// <summary>
        /// The player's movment as a percentage of his maximum speed.
        /// -1 for left, 1 for right, 0 for no movment.
        /// </summary>
        public float Movement { get; protected set; }

        public override void HandleInput()
        {
            Movement = 0;
            if (Keyboard.IsKeyPressed(Keyboard.Key.A))
                Movement = -1;
            else if (Keyboard.IsKeyPressed(Keyboard.Key.D))
                Movement = 1;
        }
    }
}
