using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeCandy.Framework
{
    /// <summary>
    /// Animation logic for sprites.  Each Animation represents one action (walking, jumping, etc.)
    /// </summary>
    public class Animation
    {
        public int StartingFrame { get; private set; }
        public int EndingFrame { get; private set; }
        public double Duration { get; private set; }
        public bool Loop { get; private set; }

        public event EventHandler Finished;

        private bool finished;
        private double currentFrame;
        public int CurrentFrame { get { return (int)currentFrame; } }

        public Animation(int startingFrame, int endingFrame, double duration, bool loop = false)
        {
            StartingFrame = startingFrame;
            EndingFrame = endingFrame;
            currentFrame = StartingFrame;
            Duration = duration;
            Loop = loop;
        }

        public void Reset()
        {
            currentFrame = StartingFrame;
            finished = false;
        }

        public void Update(double elapsedTime)
        {
            if (finished || StartingFrame == EndingFrame) return;

            currentFrame += (EndingFrame - StartingFrame) / Duration * elapsedTime;

            if (currentFrame >= EndingFrame + 1)
            {
                if (Loop)
                {
                    currentFrame -= (EndingFrame - StartingFrame) + 1;
                }
                else
                {
                    currentFrame = EndingFrame;
                    if (Finished != null)
                        Finished(this, new EventArgs());
                    finished = true;
                }
            }
        }
    }
}
