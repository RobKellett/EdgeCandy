using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.Objects
{
    public class Player : GameObject
    {
        public SpriteComponent LegGraphic = new SpriteComponent();
        public AnimatableGraphicsComponent Graphics = new AnimatableGraphicsComponent();
        public RectangleCompontent sensorGraphic = new RectangleCompontent(Color.Red);
        public InputComponent Input = new InputComponent();
        public PhysicsComponent Torso = new PhysicsComponent();
        public PhysicsComponent Legs = new PhysicsComponent();
        private Fixture footSensor;
        private RevoluteJoint axis;

        private Animation standingAnimation = new Animation(0, 0, 0);
        private Animation walkingAnimation = new Animation(1, 6, 0.667, true);
        private Animation jumpingAnimation = new Animation(17, 17, 0);
        private Animation fallingAnimation = new Animation(18, 19, 0.1);

        public const float playerWidth = 0.5f;
        public const float playerHeight = 1.5f;
        public const float playerSpeed = 20;
        public const float playerAirSpeed = 0.03f;

        public Player()
        {
            // To locomote the player, we're going to create a model like this:
            //   +-----+
            //   |     |
            //   |torso|  <-- Rectangle
            //   |/---\|
            //   |legs |  <-- circle
            //    \---/
            // With the torso having a fixed angle joint, and the legs having a motorized joint. 
            var torsoWidth = playerWidth;
            var torsoHeight = playerHeight - playerWidth/2;
            Torso.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, torsoWidth, playerHeight - playerWidth / 2, 0.1f, new Vector2(13, 27));
            Torso.Body.IgnoreGravity = true;
            Torso.Body.BodyType = BodyType.Dynamic;
            Torso.Body.FixedRotation = true;
            Torso.Body.Friction = 0;

            Legs.Body = BodyFactory.CreateCircle(PhysicsSubsystem.Instance.World, playerWidth/2, 4f, new Vector2(13, 27 + torsoHeight / 2));
            Legs.Body.BodyType = BodyType.Dynamic;
            Legs.Body.Friction = 1000;

            axis = JointFactory.CreateRevoluteJoint(PhysicsSubsystem.Instance.World, Torso.Body, Legs.Body, Vector2.Zero);
            axis.CollideConnected = false;
            axis.MotorEnabled = true;
            axis.MotorImpulse = 1000;
            axis.MaxMotorTorque = 1000;

            footSensor = FixtureFactory.AttachRectangle(playerWidth/2, playerWidth/2, 0,
                new Vector2(0, torsoHeight/2 + playerWidth/2), Torso.Body);
            footSensor.IsSensor = true;
            
            LegGraphic.Sprite = new Sprite(Content.Ball);
            Graphics.Sprite = new Sprite(Content.Player);
            Graphics.Animation = standingAnimation;
            Graphics.FrameSize = new Vector2i(64, 64);
            Graphics.Sprite.Origin = new Vector2f(32, 40);
            LegGraphic.Sprite.Origin = new Vector2f(16, 32);
            LegGraphic.Sprite.Scale = new Vector2f(playerWidth, playerWidth);

            bool jumpInProgress = false;
            bool touchingGround = false;
            // Map the input to the legs
            Input.NoInput += () =>
                             {
                                 axis.MotorSpeed = 0;
                                 if (!jumpInProgress)
                                    Graphics.Animation = standingAnimation;
                             };

            Input.Events[Keyboard.Key.A] = (key, mods) =>
                                           {
                                               if (!jumpInProgress)
                                               {
                                                   axis.MotorSpeed = -playerSpeed;
                                                   Graphics.Animation = walkingAnimation;
                                               }
                                               else
                                                   Legs.Body.ApplyLinearImpulse(new Vector2(-playerAirSpeed, 0));

                                               Graphics.Sprite.Scale = new Vector2f(-1, 1); // just flip it
                                           };

            Input.Events[Keyboard.Key.D] = (key, mods) =>
                                           {
                                               if (!jumpInProgress)
                                               {
                                                   axis.MotorSpeed = playerSpeed;
                                                   Graphics.Animation = walkingAnimation;
                                               }
                                               else
                                                   Legs.Body.ApplyLinearImpulse(new Vector2(playerAirSpeed, 0));

                                               Graphics.Sprite.Scale = new Vector2f(1, 1); // flip it good
                                           };

            Input.Events[Keyboard.Key.W] = (key, mods) =>
            {
                if (!jumpInProgress)
                {
                    jumpInProgress = true;
                    Legs.Body.ApplyLinearImpulse(new Vector2(0, -7));
                    axis.MotorSpeed = 0;
                    Graphics.Animation = jumpingAnimation;
                    Legs.Body.Friction = 0;
                }
            };
            Legs.Body.OnCollision += (a, b, c) =>
            {
                if (touchingGround)
                {
                    Legs.Body.Friction = c.Friction = 1000;
                }
                return true;
            };

            footSensor.OnCollision += (a, b, c) =>
            {
                jumpInProgress = false;
                touchingGround = true;
                LegGraphic.Sprite.Color = Color.Red;
                sensorGraphic.Color = Color.Red;
                Graphics.Animation = standingAnimation;
                return true;
            };
            footSensor.OnSeparation += (a, b) =>
            {
                touchingGround = false;
                Graphics.Animation = fallingAnimation;                
                LegGraphic.Sprite.Color = Color.White;
                sensorGraphic.Color = Color.White;
            };
        }

        public override void SyncComponents()
        {
            // Physics is the be-all end-all for position
            // We could have multiple graphics components, some physical some purely visual,
            // we could apply an offset here, etc.  Pretty powerful model.
            
            // Since torso is shorter than the full player (by playerwidth/2), our actual centerpoint for the graphic
            // needs to be offset by playerwidth / 4.
            Graphics.Sprite.Position = new Vector2f(ConvertUnits.ToDisplayUnits(Torso.Position.X),
                                                    ConvertUnits.ToDisplayUnits(Torso.Position.Y + playerWidth / 4));
            var x = Torso.Position.X;
            var y = Torso.Position.Y + (playerHeight - playerWidth/2)/2 + playerWidth/2;
            sensorGraphic.Rectangle = new FloatRect(x - playerWidth/4, y - playerWidth/4, playerWidth/2, playerWidth/2);
            Graphics.Sprite.Rotation = Torso.Rotation;
            LegGraphic.Sprite.Position = new Vector2f(ConvertUnits.ToDisplayUnits(Legs.Position.X), ConvertUnits.ToDisplayUnits(Legs.Position.Y));
            LegGraphic.Sprite.Rotation = MathHelper.ToDegrees(Legs.Rotation); // TIL SFML uses degrees, not radians
        }
    }
}
