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
        public RectangleCompontent sensorGraphic = new RectangleCompontent(Color.White);
        public InputComponent Input = new InputComponent();
        public PhysicsComponent Torso = new PhysicsComponent();
        public PhysicsComponent Legs = new PhysicsComponent();
        private RevoluteJoint axis;

        private Animation standingAnimation = new Animation(0, 0, 0);
        private Animation walkingAnimation = new Animation(1, 6, 0.667, true);
        private Animation jumpingAnimation = new Animation(17, 17, 0);
        private Animation fallingAnimation = new Animation(18, 19, 0.1);

        public const float playerWidth = 0.5f;
        public const float playerHeight = 1.5f;
        public const float playerSpeed = 20;
        public const float playerAirSpeed = 0.03f;
        public const float playerJumpForce = 8f;

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
            var torsoWidth = playerWidth + 0.25f;
            var torsoHeight = playerHeight - playerWidth;
            Torso.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, torsoWidth, playerHeight - playerWidth / 4, 0.1f, new Vector2(13, 27));
            Torso.Body.IgnoreGravity = true;
            Torso.Body.BodyType = BodyType.Dynamic;
            Torso.Body.FixedRotation = true;
            Torso.Body.Friction = 0;

            Legs.Body = BodyFactory.CreateCircle(PhysicsSubsystem.Instance.World, playerWidth/2, 4f, new Vector2(13, 27 + torsoHeight / 2 + playerWidth / 4));
            Legs.Body.BodyType = BodyType.Dynamic;
            Legs.Body.Friction = 1000;

            axis = JointFactory.CreateRevoluteJoint(PhysicsSubsystem.Instance.World, Torso.Body, Legs.Body, Vector2.Zero);
            axis.CollideConnected = false;
            axis.MotorEnabled = true;
            axis.MotorImpulse = 1000;
            axis.MaxMotorTorque = 10;
            
            LegGraphic.Sprite = new Sprite(Content.Ball);
            Graphics.Sprite = new Sprite(Content.Player);
            Graphics.Animation = standingAnimation;
            Graphics.FrameSize = new Vector2i(64, 64);
            Graphics.Sprite.Origin = new Vector2f(32, 40);
            LegGraphic.Sprite.Origin = new Vector2f(16, 32);
            LegGraphic.Sprite.Scale = new Vector2f(playerWidth, playerWidth);

            bool jumpInProgress = false;
            // Map the input to the legs
            Input.NoInput += () =>
                             {
                                 sensorGraphic.Color = Color.Red;
                                 axis.MotorSpeed = 0;
                                 if (!jumpInProgress)
                                    Graphics.Animation = standingAnimation;
                             };

            Input.KeyEvents[Keyboard.Key.A] = (key, mods) =>
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

            Input.KeyEvents[Keyboard.Key.D] = (key, mods) =>
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

            Input.KeyEvents[Keyboard.Key.W] = (key, mods) =>
            {
                if (!jumpInProgress)
                {
                    jumpInProgress = true;
                    Legs.Body.ApplyLinearImpulse(new Vector2(0, -playerJumpForce));
                    axis.MotorSpeed = 0;
                    Graphics.Animation = jumpingAnimation;
                    Legs.Body.Friction = 0;
                }
            };
            Input.MouseInput += (btn) =>
            {
                switch (btn)
                {
                    case Mouse.Button.Left:
                        var mousePos = new Vector2(ConvertUnits.ToSimUnits(Input.MousePosition.X),
                            ConvertUnits.ToSimUnits(Input.MousePosition.Y + 780));
                        var position = new Vector2(Torso.Position.X, Torso.Position.Y);
                        var direction = mousePos - position;
                        direction.Normalize();
//                        mousePos.Normalize();
//                        mousePos *= 10;
                        PhysicsSubsystem.Instance.World.RayCast((fix, point, floatA, floatB) =>
                        {
                            if (floatB != 1 && fix.Body.UserData != null && ((dynamic) fix.Body.UserData).isCandy)
                            {
                                var pos = point; //ConvertUnits.ToDisplayUnits(point);
                                new RectangleCompontent(Color.White)
                                {
                                    Rectangle = new FloatRect(pos.X, pos.Y, 0.1f, 0.1f)
                                };
                            }
                            return 0;
                        }, 
                        position, position + direction);
                        break;
                }
            };

            Torso.OnFalling += (isFalling) =>
            {
                if (isFalling)
                {
                    Graphics.Animation = fallingAnimation;
                    jumpInProgress = true;
                }
            };

            Legs.Body.OnCollision += (a, b, c) =>
            {
                dynamic userData = a.Body.UserData ?? b.Body.UserData;
                if ((userData == null || !userData.isWall))
                {
                    jumpInProgress = false;
                    Legs.Body.Friction = c.Friction = 1000;
                    LegGraphic.Sprite.Color = Color.Red;
                    Graphics.Animation = standingAnimation;
                }
                return true;
            };

            Legs.Body.OnSeparation += (a, b) => { LegGraphic.Sprite.Color = Color.White; };
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
            sensorGraphic.Rectangle = new FloatRect(x - playerWidth/4, y - playerWidth/4 + 0.1f, playerWidth/2, playerWidth/2 + 0.1f);
            Graphics.Sprite.Rotation = Torso.Rotation;
            LegGraphic.Sprite.Position = new Vector2f(ConvertUnits.ToDisplayUnits(Legs.Position.X), ConvertUnits.ToDisplayUnits(Legs.Position.Y));
            LegGraphic.Sprite.Rotation = MathHelper.ToDegrees(Legs.Rotation); // TIL SFML uses degrees, not radians
        }
    }
}
