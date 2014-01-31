using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Subsystems;
using Microsoft.Xna.Framework;
using SFML.Window;

namespace EdgeCandy.Components
{
    public class InputComponent : IUpdateableComponent
    {
        [Flags]
        public enum Modifiers
        {
            None = 0,
            Shift = 1,
            Ctrl = 2,
            Alt = 4,
        }

        public delegate void KeyInputEvent(Keyboard.Key key, Modifiers mod);

        public delegate void NoInputEvent();

        public delegate void MouseInputEvent(Mouse.Button button);

        public event NoInputEvent NoInput;
        public event MouseInputEvent MouseInput;
        public Dictionary<Keyboard.Key, KeyInputEvent> KeyEvents
        {
            get;
            set;
        }

        public Vector2i MousePosition
        {
            get { return Mouse.GetPosition(Program.Window); }
        }

        public InputComponent()
        {
            UpdateSubsystem.Instance.Register(this);
            KeyEvents = new Dictionary<Keyboard.Key, KeyInputEvent>();
        }

        public void Update(double elapsedTime)
        {
            Modifiers mods = Modifiers.None;
            if(Keyboard.IsKeyPressed(Keyboard.Key.LShift) || Keyboard.IsKeyPressed(Keyboard.Key.RShift))
                mods |= Modifiers.Shift;
            if (Keyboard.IsKeyPressed(Keyboard.Key.LAlt) || Keyboard.IsKeyPressed(Keyboard.Key.RAlt))
                mods |= Modifiers.Alt;
            if (Keyboard.IsKeyPressed(Keyboard.Key.LControl) || Keyboard.IsKeyPressed(Keyboard.Key.RControl))
                mods |= Modifiers.Ctrl;
            bool Any = false;
            foreach(var kvp in KeyEvents)
                if (Keyboard.IsKeyPressed(kvp.Key))
                {
                    kvp.Value(kvp.Key, mods);
                    Any = true;
                }
            var btns = new [] {Mouse.Button.Left, Mouse.Button.Middle, Mouse.Button.Right};
            foreach (var btn in btns.Where(Mouse.IsButtonPressed))
            {
                MouseInput(btn);
                Any = true;
            }
            if (!Any)
                NoInput();
        }
    }
}
