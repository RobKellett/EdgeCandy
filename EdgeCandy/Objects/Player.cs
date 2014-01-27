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
        public InputComponent Input = new InputComponent();
        public PhysicsComponent Torso = new PhysicsComponent();
        public PhysicsComponent Legs = new PhysicsComponent();
        private RevoluteJoint axis;

        private Animation standingAnimation = new Animation(0, 0, 0);
        private Animation walkingAnimation = new Animation(1, 6, 0.667, true);

        public const float playerWidth = 1;
        public const float playerHeight = 2;
        public const float playerSpeed = 10;

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

            Legs.Body = BodyFactory.CreateCircle(PhysicsSubsystem.Instance.World, playerWidth/2, 1f, new Vector2(13, 27 + torsoHeight / 2));
            Legs.Body.BodyType = BodyType.Dynamic;
            Legs.Body.Friction = 1000;

            axis = JointFactory.CreateRevoluteJoint(PhysicsSubsystem.Instance.World, Torso.Body, Legs.Body, Vector2.Zero);
            axis.CollideConnected = false;
            axis.MotorEnabled = true;
            axis.MotorImpulse = 1000;
            axis.MaxMotorTorque = 1000;
            
            LegGraphic.Sprite = new Sprite(Content.Ball);
            Graphics.Sprite = new Sprite(Content.Player);
            Graphics.Animation = standingAnimation;
            Graphics.FrameSize = new Vector2i((int)ConvertUnits.ToDisplayUnits(playerWidth), (int)ConvertUnits.ToDisplayUnits(playerHeight));
            Graphics.Sprite.Origin = new Vector2f(ConvertUnits.ToDisplayUnits(playerWidth / 2), ConvertUnits.ToDisplayUnits(playerHeight / 2));
            LegGraphic.Sprite.Origin = new Vector2f(ConvertUnits.ToDisplayUnits(playerWidth / 2), ConvertUnits.ToDisplayUnits(playerHeight / 2));

            // Map the input to the legs
            Input.NoInput += () =>
                             {
                                 axis.MotorSpeed = 0;
                                 Graphics.Animation = standingAnimation;
                             };

            Input.Events[Keyboard.Key.A] = (key, mods) =>
                                           {
                                               axis.MotorSpeed = -playerSpeed;
                                               Graphics.Animation = walkingAnimation;
                                               Graphics.Sprite.Scale = new Vector2f(-1, 1); // just flip it
                                           };

            Input.Events[Keyboard.Key.D] = (key, mods) =>
                                           {
                                               axis.MotorSpeed = playerSpeed;
                                               Graphics.Animation = walkingAnimation;
                                               Graphics.Sprite.Scale = new Vector2f(1, 1); // flip it good
                                           };
            bool jumpInProgress = false;
            Input.Events[Keyboard.Key.W] = (key, mods) =>
            {
                if (!jumpInProgress)
                {
                    jumpInProgress = true;
                    Legs.Body.ApplyLinearImpulse(new Vector2(0, -8));
                }
            };
            Legs.Body.OnCollision += (a, b, c) =>
            {
                if (jumpInProgress)
                    jumpInProgress = false;
                return true;
            };
            Torso.OnFalling += (f) =>
            {
                LegGraphic.Sprite.Color = f ? Color.Red : Color.White;
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
            Graphics.Sprite.Rotation = Torso.Rotation;
            LegGraphic.Sprite.Position = new Vector2f(ConvertUnits.ToDisplayUnits(Legs.Position.X), ConvertUnits.ToDisplayUnits(Legs.Position.Y));
            LegGraphic.Sprite.Rotation = MathHelper.ToDegrees(Legs.Rotation); // TIL SFML uses degrees, not radians
        }
    }
}
