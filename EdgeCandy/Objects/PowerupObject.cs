using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.Objects
{
    public class PowerupObject : GameObject
    {
        public PhysicsComponent Physics = new PhysicsComponent();
        public AnimatableGraphicsComponent Sprite = new AnimatableGraphicsComponent();

        private Animation animation = new Animation(0, 3, 0.33, true);

        public PowerupObject(Vector2f position)
        {
            Sprite.Sprite = new Sprite(Content.Powerup)
                            {
                                Origin =
                                    new Vector2f(Content.Powerup.Size.X / 2,
                                                 Content.Powerup.Size.Y / 2),
                                Position = position
                            };
            Sprite.Animation = animation;
            Sprite.FrameSize = new Vector2i(21, 21);

            Physics.Body = new Body(PhysicsSubsystem.Instance.World,
                                    new Vector2(ConvertUnits.ToSimUnits(position.X), ConvertUnits.ToSimUnits(position.Y)))
                           {
                               IgnoreGravity = true
                           };

            var sensor = FixtureFactory.AttachRectangle(.5f, .5f, 0, Vector2.Zero, Physics.Body);
            sensor.IsSensor = true;
        }

        public override void SyncComponents()
        {
            // uh
        }
    }
}
