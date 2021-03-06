﻿using System;
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

        private float scroll, totalTime;
        private readonly float initialScroll, timeToFinish;

        public float Shake { get; set; }

        public CameraComponent(string name, float initialScroll, float timeToFinish)
        {
            this.initialScroll = scroll = initialScroll;
            this.timeToFinish = timeToFinish;

            GraphicsSubsystem.Instance.Register(name, this);
            UpdateSubsystem.Instance.Register(this);
        }

        public void Update(double elapsedTime)
        {
            // linearly decay the shake
            Shake = Math.Max(0, Shake - (float)elapsedTime * 4);

            totalTime += (float)elapsedTime;
            scroll = initialScroll * (float)Math.Cos((totalTime / timeToFinish) * Math.PI / 2); // TODO: fine the tuning
        }

        private float ShakeOffset()
        {
            return Shake * (((DateTime.Now.Ticks % 255) / 255f) * 2 - 1);
        }
    }
}
