using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using Transform = SFML.Graphics.Transform;

namespace EdgeCandy.Objects
{
    public class Player : GameObject
    {
        public SpriteComponent LegGraphic = new SpriteComponent();
        public AnimatableGraphicsComponent Graphics = new AnimatableGraphicsComponent();
        public RectangleCompontent sensorGraphic = new RectangleCompontent(Color.White);
        public RectangleCompontent pistonGraphic = new RectangleCompontent(new Color(255, 0, 255, 128));
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
        private Animation vSwingAnimation = new Animation(20, 23, .1);
        private Animation vSwingAerialAnimation = new Animation(40, 43, .1);

        private TimerComponent attackTimer = new TimerComponent(0.33);
        private TimerComponent jumpTimer = new TimerComponent(0.25);
        private TimerComponent springResetTimer = new TimerComponent(0.1);

        public const float playerWidth = 0.5f;
        public const float playerHeight = 1.5f;
        public const float playerSpeed = 20;
        public const float playerAirSpeed = 0.03f;
        public const float playerJumpForce = 1.5f;

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

            Piston.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, playerWidth, playerHeight / 2,
                                                      0.5f, new Vector2(13, 27));
            Piston.Body.BodyType = BodyType.Dynamic;

            axis = JointFactory.CreateRevoluteJoint(PhysicsSubsystem.Instance.World, Torso.Body, Legs.Body, Vector2.Zero);
            axis.CollideConnected = false;
            axis.MotorEnabled = true;
            axis.MotorImpulse = 1000;
            axis.MaxMotorTorque = 10;

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
            
            LegGraphic.Sprite = new Sprite(Content.Ball);
            Graphics.Sprite = new Sprite(Content.Player);
            Graphics.Animation = standingAnimation;
            Graphics.FrameSize = new Vector2i(64, 64);
            Graphics.Sprite.Origin = new Vector2f(32, 40);
            LegGraphic.Sprite.Origin = new Vector2f(16, 32);
            LegGraphic.Sprite.Scale = new Vector2f(playerWidth, playerWidth);

            bool jumpInProgress = false, canJump = true, attacking = false, canAttack = true; // 1WEEK
            // Map the input to the legs
            Input.NoInput += () =>
                             {
                                 sensorGraphic.Color = Color.Red;
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
                switch (btn)
                {
                    case Mouse.Button.Left:
                        if (!canAttack) break;

                        canAttack = false;
                        sensorGraphic.Color = Color.Blue;
                        Graphics.Animation = jumpInProgress ? vSwingAerialAnimation : vSwingAnimation;
                        axis.MotorSpeed = 0;
                        attacking = true;
                        attackTimer.Start();

                        var mousePos = new Vector2(ConvertUnits.ToSimUnits(Input.MousePosition.X),
                            ConvertUnits.ToSimUnits(Input.MousePosition.Y));
                        var position = new Vector2(Torso.Position.X, Torso.Position.Y);
                        var direction = mousePos - position;
                        direction.Normalize();
                        direction *= 20.5f;
                        var endPos = position + direction;
//                        mousePos.Normalize();
//                        mousePos *= 10;
                        ////PhysicsSubsystem.Instance.World.RayCast((fix, point, floatA, floatB) =>
                        ////{
                        ////    var userData = fix.Body.UserData as CandyObject;
                        ////    if (floatB != 1 && userData != null)
                        ////    {

                        ////        Vertices first;
                        ////        Vertices second;
                        ////        FarseerPhysics.Common.PolygonManipulation.CuttingTools.SplitShape(fix, );
                        ////        textureA.CopyToImage().SaveToFile("a.png");
                        ////        textureB.CopyToImage().SaveToFile("b.png");
                        ////        var pos = point; //ConvertUnits.ToDisplayUnits(point);
                        ////        new RectangleCompontent(Color.White)
                        ////        {
                        ////            Rectangle = new FloatRect(pos.X, pos.Y, 0.1f, 0.1f)
                        ////        };
                        ////    }
                        //    return 0;
                        //}, 
                        //position, position + direction);
                        
                        // Find the first thing hit, and if it's a candy, keep track of it.
                        Vector2 hitPoint;
                        Fixture candyHit;
                        PhysicsSubsystem.Instance.World.RayCast((f, p, n, fr) =>
                        {
                            if (f.Body.UserData is CandyObject)
                            {
                                hitPoint = p;
                                candyHit = f;
                            }
                            return 0;
                        }, position, position + direction);

                        List<Fixture> fixtures = new List<Fixture>();
                        List<Vector2> entryPoints = new List<Vector2>();
                        List<Vector2> exitPoints = new List<Vector2>();

                        //Get the entry points
                        PhysicsSubsystem.Instance.World.RayCast((f, p, n, fr) =>
                                          {
                                              if (f.Body.UserData is CandyObject)
                                              {
                                                  fixtures.Add(f);
                                                  entryPoints.Add(p);
                                              }
                                              return 1;
                                          }, position, endPos);

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

//                        exitPoints.Reverse();

                        for (int i = 0; i < fixtures.Count; i++)
                        {
                            if (fixtures[i].Body.Mass < 2.5) continue;
                            var originalCandy = fixtures[i].Body.UserData as CandyObject;
                            Debug.Assert(originalCandy != null);
                            var textureOrigin = originalCandy.Sprite.Sprite.Texture;
                            Texture textureA, textureB;
                            var spriteOrigin = originalCandy.Sprite.Sprite.Origin;
                            var relativeEntryPoint =
                                new Vector2f(ConvertUnits.ToDisplayUnits(entryPoints[i].X - originalCandy.Physics.Position.X),
                                                ConvertUnits.ToDisplayUnits(entryPoints[i].Y - originalCandy.Physics.Position.Y));
                            var rotation = Transform.Identity;
                            rotation.Rotate(-originalCandy.Sprite.Sprite.Rotation);
                            var rotatedEntryPoint = rotation.TransformPoint(relativeEntryPoint);
                            var startPoint = spriteOrigin + rotatedEntryPoint;
                            var relativeExitPoint =
                                new Vector2f(ConvertUnits.ToDisplayUnits(exitPoints[i].X - originalCandy.Physics.Position.X),
                                                ConvertUnits.ToDisplayUnits(exitPoints[i].Y - originalCandy.Physics.Position.Y));
                            var rotatedExitPoint = rotation.TransformPoint(relativeExitPoint);
                            var endPoint = spriteOrigin + rotatedExitPoint; 
                            TextureSlicer.SliceAndDice(startPoint, endPoint, textureOrigin, out textureA,
                                out textureB, originalCandy.RepeatsX, originalCandy.RepeatsY);
                            textureA.CopyToImage().SaveToFile("a.png");
                            textureB.CopyToImage().SaveToFile("b.png");
                            Vertices first;
                            Vertices second;
                            FarseerPhysics.Common.PolygonManipulation.CuttingTools.SplitShape(fixtures[i], entryPoints[i], exitPoints[i], out first, out second);
                            //Delete the original shape and create two new. Retain the properties of the body.
                            if (first.GetArea()*fixtures[i].Shape.Density < 0.5 ||
                                second.GetArea()*fixtures[i].Shape.Density < 0.5)
                                return;
                            if (first.CheckPolygon() == PolygonError.NoError)
                            {
                                Body firstFixture = BodyFactory.CreatePolygon(PhysicsSubsystem.Instance.World, first, fixtures[i].Shape.Density, fixtures[i].Body.Position);
                                firstFixture.Rotation = fixtures[i].Body.Rotation;
                                firstFixture.LinearVelocity = fixtures[i].Body.LinearVelocity;
                                firstFixture.AngularVelocity = fixtures[i].Body.AngularVelocity;
                                firstFixture.BodyType = BodyType.Dynamic;
                                firstFixture.UserData = originalCandy;
                                originalCandy.Physics.Body = firstFixture;
                                originalCandy.Sprite.Sprite.Texture = textureA;
                                originalCandy.RepeatsX = originalCandy.RepeatsY = 1; // 1WEEK

                                if (first.GetArea()*fixtures[i].Shape.Density < 5)
                                    originalCandy.DecayTimer.Start();
                            }

                            if (second.CheckPolygon() == PolygonError.NoError)
                            {
                                Body secondFixture = BodyFactory.CreatePolygon(PhysicsSubsystem.Instance.World, second, fixtures[i].Shape.Density, fixtures[i].Body.Position);
                                secondFixture.Rotation = fixtures[i].Body.Rotation;
                                secondFixture.LinearVelocity = fixtures[i].Body.LinearVelocity;
                                secondFixture.AngularVelocity = fixtures[i].Body.AngularVelocity;
                                secondFixture.BodyType = BodyType.Dynamic;
                                var secondCandy = new CandyObject(secondFixture, textureB, secondFixture.Position);
                                secondFixture.UserData = secondCandy;

                                if (second.GetArea()*fixtures[i].Shape.Density < 5)
                                    secondCandy.DecayTimer.Start();
                            }

                            PhysicsSubsystem.Instance.World.RemoveBody(fixtures[i].Body);
                        }

                        break;
                    case Mouse.Button.Middle:
                        sensorGraphic.Color = Color.Black;
                        break;
                    case Mouse.Button.Right:
                        sensorGraphic.Color = Color.Green;
                        break;
                }
            };

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
                    LegGraphic.Sprite.Color = Color.Red;
                    if (!attacking)
                        Graphics.Animation = standingAnimation;
                }
                return true;
            };

            Piston.Body.OnCollision += (a, b, c) =>
                                       {
                                           Graphics.Animation = jumpingAnimation;
                                           return true;
                                       };

            Legs.Body.OnSeparation += (a, b) => { LegGraphic.Sprite.Color = Color.White; };

            var swingFinished = new EventHandler((sender, args) => 
                                      {
                                          attacking = false;
                                          Graphics.Animation = jumpInProgress ? aerialAnimation : standingAnimation;
                                      });

            vSwingAnimation.Finished += swingFinished;
            vSwingAerialAnimation.Finished += swingFinished;

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
            sensorGraphic.Rectangle = new FloatRect(x - playerWidth/4, y - playerWidth/4 + 0.1f, playerWidth/2, playerWidth/2 + 0.1f);
            pistonGraphic.Rectangle = new FloatRect(Piston.Body.Position.X - playerWidth/2, Piston.Body.Position.Y - playerHeight/4, playerWidth, playerHeight/2);
            Graphics.Sprite.Rotation = Torso.Rotation;
            LegGraphic.Sprite.Position = new Vector2f(ConvertUnits.ToDisplayUnits(Legs.Position.X), ConvertUnits.ToDisplayUnits(Legs.Position.Y));
            LegGraphic.Sprite.Rotation = MathHelper.ToDegrees(Legs.Rotation); // TIL SFML uses degrees, not radians
        }
    }
}
