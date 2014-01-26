using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
using SFML.Window;

namespace EdgeCandy.Components
{
    public class CameraComponent
    {
        public Vector2f Position
        {
            get
            {
                return new Vector2f(ShakeOffset(), scroll + ShakeOffset());
            }
        }

        private float scroll = 0, shake = 0;

        public CameraComponent(string name, float initialScroll)
        {
            scroll = initialScroll;

            GraphicsSubsystem.Instance.Register(name, this);
            CameraSubsystem.Instance.Register(this);
        }

        public void Update(double elapsedTime)
        {
            // linearly decay the shake
            shake = Math.Max(0, shake - (float)elapsedTime * 2);
        }

        private float ShakeOffset()
        {
            return shake * (((DateTime.Now.Ticks % 255) / 255f) * 2 - 1);
        }
    }
}
