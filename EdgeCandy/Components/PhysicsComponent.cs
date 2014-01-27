using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Subsystems;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using SFML.Window;

namespace EdgeCandy.Components
{
    /// <summary>
    /// A piece of a game object which is physically simulated.
    /// </summary>
    public class PhysicsComponent
    {
        private Body body;
        public Body Body { get { return body; } set { body = value; } }

        public delegate void FallingEvent(bool falling);

        public event FallingEvent OnFalling;
        /// <summary>
        /// Constructor
        /// </summary>
        public PhysicsComponent()
        {
            // Automatically register with the physics subsystem.
            PhysicsSubsystem.Instance.Register(this);
            //body = FarseerPhysics.Factories.BodyFactory.CreateCircle(PhysicsSubsystem.Instance.World, 10.0f, 1.0f);
            //body.BodyType = BodyType.Dynamic;
            //body.ApplyTorque(100);
        }

        /// <summary>
        /// The position that we have been simulated at.
        /// </summary>
        public Vector2f Position
        {
            get { return new Vector2f(body.Position.X, body.Position.Y); }
        }

        private int fallingFrameCount = 0;
        public void Update()
        {
            if (body.LinearVelocity.Y > 0.01f)
                fallingFrameCount++;
            else
                fallingFrameCount = 0;
            if (OnFalling != null)
                OnFalling(fallingFrameCount > 3);
        }

        /// <summary>
        /// Objects rotation
        /// </summary>
        public float Rotation
        {
            get { return body.Rotation; }
        }
    }
}
