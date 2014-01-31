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
    public class CameraComponent : IUpdateableComponent
    {
        public Vector2f Position
        {
            get
            {
                return new Vector2f(ShakeOffset(), scroll + ShakeOffset() - Graphics.Height);
            }
        }

        private float scroll, scrollTime = 1, shake;

        public CameraComponent(string name, float initialScroll)
        {
            scroll = initialScroll;

            GraphicsSubsystem.Instance.Register(name, this);
            UpdateSubsystem.Instance.Register(this);
        }

        public void Update(double elapsedTime)
        {
            // linearly decay the shake
            shake = Math.Max(0, shake - (float)elapsedTime * 2);

            scrollTime += (float)elapsedTime;
            //scroll -= (float)Math.Log(scrollTime) / 33; // TODO: fine the tuning
        }

        private float ShakeOffset()
        {
            return shake * (((DateTime.Now.Ticks % 255) / 255f) * 2 - 1);
        }
    }
}
