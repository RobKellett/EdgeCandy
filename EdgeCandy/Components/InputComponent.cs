using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Subsystems;
using SFML.Window;

namespace EdgeCandy.Components
{
    public class InputComponent
    {
        [Flags]
        public enum Modifiers
        {
            None = 0,
            Shift = 1,
            Ctrl = 2,
            Alt = 4,
        }

        public delegate void InputEvent(Keyboard.Key key, Modifiers mod);

        public delegate void NoInputEvent();

        public event NoInputEvent NoInput;
        public Dictionary<Keyboard.Key, InputEvent> Events
        {
            get;
            set;
        }

        public InputComponent()
        {
            InputSubsystem.Instance.Register(this);
            Events = new Dictionary<Keyboard.Key, InputEvent>();
        }

        public void HandleInput()
        {
            Modifiers mods = Modifiers.None;
            if(Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift))
                mods |= Modifiers.Shift;
            if (Keyboard.IsKeyPressed(Keyboard.Key.LAlt) || Keyboard.IsKeyPressed(Keyboard.Key.RAlt))
                mods |= Modifiers.Alt;
            if (Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl))
                mods |= Modifiers.Ctrl;
            bool Any = false;
            foreach(var kvp in Events)
                if (Keyboard.IsKeyPressed(kvp.Key))
                {
                    kvp.Value(kvp.Key, mods);
                    Any = true;
                }
            if (!Any)
                NoInput();
        }
    }
}
