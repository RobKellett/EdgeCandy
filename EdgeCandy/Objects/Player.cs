using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using EdgeCandy.States;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using Transform = SFML.Graphics.Transform;

namespace EdgeCandy.Objects
{
    public class Player : GameObject
    {
        public AnimatableGraphicsComponent Graphics = new AnimatableGraphicsComponent();
        public InputComponent Input = new InputComponent();
        public PhysicsComponent Torso = new PhysicsComponent();
        public PhysicsComponent Legs = new PhysicsComponent();
        public PhysicsComponent Piston = new PhysicsComponent();
        private RevoluteJoint axis;
        private DistanceJoint spring;
        private PrismaticJoint prismatic;

        private Animation standingAnimation = new Animation(0, 0, 0);
        private Animation walkingAnimation = new Animation(1, 6, 0.667, true);
        private Animation jumpingAnimation = new Animation(17, 17, 0);
        private Animation fallingAnimation = new Animation(18, 19, 0.1);
        private Animation aerialAnimation = new Animation(19, 19, 0);
        private Animation vSwingAnimation = new Animation(20, 23, .15);
        private Animation vSwingAerialAnimation = new Animation(40, 43, .15);
        private Animation hSwingAnimation = new Animation(24, 26, .15);
        private Animation hSwingAerialAnimation = new Animation(44, 46, .15);

        private TimerComponent attackTimer = new TimerComponent(0.33);
        private TimerComponent jumpTimer = new TimerComponent(0.25);
        private TimerComponent springResetTimer = new TimerComponent(0.1);

        public const float playerWidth = 0.5f;
        public const float playerHeight = 1.5f;
        public const float playerSpeed = 20;
        public const float playerAirSpeed = 0.015f;
        public const float playerJumpForce = 1.5f;

        public double Slicing
        {
            get; set;
        }

        public const double MaxSlicing = 200;

        public Player(Vector2 spawn)
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
            Torso.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, torsoWidth, playerHeight - playerWidth / 4, 0.1f, spawn);
            Torso.Body.IgnoreGravity = true;
            Torso.Body.BodyType = BodyType.Dynamic;
            Torso.Body.FixedRotation = true;
            Torso.Body.Friction = 0;
            Torso.Body.UserData = this;

            Legs.Body = BodyFactory.CreateCircle(PhysicsSubsystem.Instance.World, playerWidth/2, 4f, new Vector2(spawn.X, spawn.Y + torsoHeight / 2 + playerWidth / 4));
            Legs.Body.BodyType = BodyType.Dynamic;
            Legs.Body.Friction = 1000;

            Piston.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, playerWidth, playerHeight / 2,
                                                      0.5f, spawn);
            Piston.Body.BodyType = BodyType.Dynamic;

            axis = JointFactory.CreateRevoluteJoint(PhysicsSubsystem.Instance.World, Torso.Body, Legs.Body, Vector2.Zero);
            axis.CollideConnected = false;
            axis.MotorEnabled = true;
            axis.MaxMotorTorque = 20;

            //prismatic = JointFactory.CreatePrismaticJoint(PhysicsSubsystem.Instance.World, Piston.Body, Torso.Body,
            //                                              Vector2.Zero, -Vector2.UnitY);
            spring = JointFactory.CreateDistanceJoint(PhysicsSubsystem.Instance.World, Piston.Body, Torso.Body);
            spring.Frequency = 10f;
            spring.DampingRatio = 1f;
            spring.CollideConnected = false;

            prismatic = JointFactory.CreatePrismaticJoint(PhysicsSubsystem.Instance.World, Piston.Body, Torso.Body,
                                                          Vector2.Zero, Vector2.UnitY);
            prismatic.CollideConnected = false;
            Piston.Body.IgnoreCollisionWith(Legs.Body);
            
            Graphics.Sprite = new Sprite(Content.Player);
            Graphics.Animation = standingAnimation;
            Graphics.FrameSize = new Vector2i(64, 64);
            Graphics.Sprite.Origin = new Vector2f(32, 40);

            Slicing = 45;

            bool jumpInProgress = false, canJump = true, attacking = false, canAttack = true; // 1WEEK
            // Map the input to the legs
            Input.NoInput += () =>
                             {
                                 axis.MotorSpeed = 0;
                                 if (!jumpInProgress && !attacking)
                                    Graphics.Animation = standingAnimation;
                             };
            
            Input.KeyEvents[Keyboard.Key.A] = (key, mods) =>
                                           {
                                               if (attacking) return;

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
                                               if (attacking) return;

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
                if (canJump)// && !jumpInProgress && !attacking)
                {
                    jumpInProgress = true;
                    canJump = false;
                    //Legs.Body.ApplyLinearImpulse(new Vector2(0, -playerJumpForce));
                    //Piston.Body.ApplyLinearImpulse(new Vector2(0, -playerJumpForce));
                    spring.Length = playerJumpForce;
                    axis.MotorSpeed = 0;
                    Legs.Body.Friction = 0;
                    jumpTimer.Start();
                    springResetTimer.Start();
                }
            };

            Input.MouseInput += (btn) =>
                                {
                Vector2 mousePos, position, direction, endPos;

                switch (btn)
                {
                    case Mouse.Button.Left:
                        if (!canAttack) break;

                        canAttack = false;
                        Graphics.Animation = jumpInProgress ? vSwingAerialAnimation : vSwingAnimation;
                        axis.MotorSpeed = 0;
                        attacking = true;
                        attackTimer.Start();

                        mousePos = new Vector2(ConvertUnits.ToSimUnits(Input.MousePosition.X),
                            ConvertUnits.ToSimUnits(Input.MousePosition.Y));
                        position = new Vector2(Torso.Position.X, Torso.Position.Y);
                        direction = mousePos - position;
                        direction.Normalize();
                        endPos = position + (direction * 2.5f);

                        PhysicsSubsystem.Instance.World.RayCast((f, p, n, fr) =>
                        {
                            var candy = f.Body.UserData as CandyObject;
                            if (candy != null && candy.HitPoints > 0)
                            {
                                candy.Hit(1, p, direction);
                                return 0;
                            }
                            if (f.Body.UserData is PlatformObject)
                                return 0;
                            return 1;
                        }, position, endPos);

                        break;
                    case Mouse.Button.Middle:
                        break;
                    case Mouse.Button.Right:
                        if (!canAttack || Slicing <= 0) break;

                        canAttack = false;
                        Graphics.Animation = jumpInProgress ? hSwingAerialAnimation : hSwingAnimation;
                        axis.MotorSpeed = 0;
                        attacking = true;
                        attackTimer.Start();

                        mousePos = new Vector2(ConvertUnits.ToSimUnits(Input.MousePosition.X),
                            ConvertUnits.ToSimUnits(Input.MousePosition.Y));
                        position = new Vector2(Torso.Position.X, Torso.Position.Y);
                        direction = mousePos - position;
                        direction.Normalize();
                        direction *= 2.5f;
                        endPos = position + direction;
                        
                        // Find the first thing hit, and if it's a candy, keep track of it.
                        List<Fixture> fixtures = new List<Fixture>();
                        List<Vector2> entryPoints = new List<Vector2>();
                        List<Vector2> exitPoints = new List<Vector2>();

                        //Get the entry points
                        PhysicsSubsystem.Instance.World.RayCast((f, p, n, fr) =>
                                          {
                                              if (f.Body.UserData is PlatformObject)
                                              {
                                                  return 0;
                                              }
                                              if (f.Body.UserData is CandyObject)
                                              {
                                                  fixtures.Add(f);
                                                  entryPoints.Add(p);
                                              }
                                              return 1;
                                          }, position, endPos);
                        if (!entryPoints.Any())
                            return;
                        //Reverse the ray to get the exitpoints
                        PhysicsSubsystem.Instance.World.RayCast((f, p, n, fr) =>
                                          {
                                              if(f.Body.UserData is CandyObject)
                                                  exitPoints.Add(p);
                                              return 1;
                                          }, endPos, position);

                        while (fixtures.Count > exitPoints.Count)
                        {
                            fixtures.Remove(fixtures.Last());
                            entryPoints.Remove(entryPoints.Last());
                        }

                        GameplayState.Score += fixtures.Count * 500;
                        if (fixtures.Count > 0)
                            Content.SliceSound.Play();

                        for (int i = 0; i < fixtures.Count; i++)
                        {
                            if (fixtures[i].Body.Mass < 2.5) continue;
                            var originalCandy = fixtures[i].Body.UserData as CandyObject;
                            Debug.Assert(originalCandy != null);
                            Slicing = Math.Max(Slicing - originalCandy.Physics.Body.Mass, 0);
                            originalCandy.Slice(entryPoints[i], exitPoints[i]);
                        }

                        break;
                }
            };

#if DEBUG
            Input.KeyEvents[Keyboard.Key.RShift] = (key, mod) => Framework.Graphics.TakeScreenshot();
#endif

            Torso.OnFalling += (isFalling) =>
            {
                if (isFalling)
                {
                    if (!attacking)
                        Graphics.Animation = fallingAnimation;
                    jumpInProgress = true;
                }
            };

            Legs.Body.OnCollision += (a, b, c) =>
            {
                var userData = (a.Body.UserData ?? b.Body.UserData) as WallObject;
                if (userData == null)
                {
                    jumpInProgress = false;
                    Legs.Body.Friction = c.Friction = 1000;
                    if (!attacking)
                        Graphics.Animation = standingAnimation;
                }
                return true;
            };

            Piston.Body.OnCollision += (a, b, c) =>
                                       {
                                           Graphics.Animation = jumpingAnimation;
                                           Content.JumpSound.Play();
                                           return true;
                                       };

            var swingFinished = new EventHandler((sender, args) => 
                                      {
                                          attacking = false;
                                          Graphics.Animation = jumpInProgress ? aerialAnimation : standingAnimation;
                                      });

            vSwingAnimation.Finished += swingFinished;
            vSwingAerialAnimation.Finished += swingFinished;
            hSwingAnimation.Finished += swingFinished;
            hSwingAerialAnimation.Finished += swingFinished;

            attackTimer.DingDingDing += (sender, args) => canAttack = true;
            jumpTimer.DingDingDing += (sender, args) => canJump = true;
            springResetTimer.DingDingDing += (sender, args) => spring.Length = 0;
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
            Graphics.Sprite.Rotation = Torso.Rotation;
        }
    }
}
