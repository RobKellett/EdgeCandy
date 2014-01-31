using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Subsystems;

namespace EdgeCandy.Components
{
    public class TimerComponent : IUpdateableComponent
    {
        private double timeToCount, timeLeft;
        private bool running = false;

        public event EventHandler DingDingDing;

        public TimerComponent(double timeToCount)
        {
            this.timeToCount = timeLeft = timeToCount;

            UpdateSubsystem.Instance.Register(this);
        }

        public void Update(double elapsedTime)
        {
            if (!running) return;

            timeLeft -= elapsedTime;

            if (timeLeft <= 0)
            {
                running = false;
                if (DingDingDing != null)
                    DingDingDing(this, null);
            }
        }

        public void Start()
        {
            timeLeft = timeToCount;
            running = true;
        }
    }
}
