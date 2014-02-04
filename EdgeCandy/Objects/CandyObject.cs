using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.States;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using EdgeCandy.Framework;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;
using Transform = SFML.Graphics.Transform;

namespace EdgeCandy.Objects
{
    public enum CandyKind
    {
        CandyCane,
        DoubleCandyCane,
        Chocolate,
        Rancher
    }

    public class CandyObject : GameObject
    {
        public SpriteComponent Sprite = new SpriteComponent();
        public PhysicsComponent Physics = new PhysicsComponent();

        public float RepeatsX = 1, RepeatsY = 1; // 1WEEK

        public float HitPoints;

        public delegate void SliceEvent();

        public event SliceEvent OnSlice;

        public CandyObject(Body body, Texture tex, Vector2 position)
        {
            Sprite.Sprite = new Sprite(tex);
            Sprite.Sprite.Origin = new Vector2f(Sprite.Sprite.Texture.Size.X / 2, Sprite.Sprite.Texture.Size.Y / 2);
            Physics.Body = body;
        }

        public CandyObject(CandyKind kind, Vector2 position, Vector2 size = default(Vector2))
        {
            var density = 25f;
            switch (kind)
            {
                case CandyKind.CandyCane:
                    Sprite.Sprite = new Sprite(Content.CandyCane);
                    HitPoints = 5;
                    break;
                case CandyKind.Chocolate:
                    Sprite.Sprite = new Sprite(Content.Chocolate) 
                        { 
                            Origin = new Vector2f(size.X / 2, size.Y / 2),
                            TextureRect = new IntRect(0, 0, (int)size.X, (int)size.Y)
                        };
                    Physics.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World,
                                                               ConvertUnits.ToSimUnits(size.X),
                                                               ConvertUnits.ToSimUnits(size.Y), 25);
                    Physics.Body.Position = new Vector2(ConvertUnits.ToSimUnits(position.X + size.X / 2), ConvertUnits.ToSimUnits(position.Y + size.Y / 2));
                    RepeatsX = size.X / Content.Chocolate.Size.X;
                    RepeatsY = size.Y / Content.Chocolate.Size.Y;
                    HitPoints = 3;
                    break;
                case CandyKind.DoubleCandyCane:
                    Sprite.Sprite = new Sprite(Content.DoubleCandyCane);
                    HitPoints = 12;
                    OnSlice += () =>
                    {
                        var hint = (Program.GameState as GameplayState).Map.Hints[3]; // 1 HOUR
                        hint.Text = new Text("Now BASH IT!", hint.Text.Font, 16)
                        {
                            Position = hint.Text.Position,
                        };
                    };
                    break;
                case CandyKind.Rancher:
                    Sprite.Sprite = new Sprite(Content.Rancher);
                    HitPoints = 4;
                    density = 5;
                    break;
            }

            if (kind != CandyKind.Chocolate)
            {
                Sprite.Sprite.Origin = new Vector2f(Sprite.Sprite.Texture.Size.X / 2, Sprite.Sprite.Texture.Size.Y / 2);
                Physics.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, ConvertUnits.ToSimUnits(Sprite.Sprite.Texture.Size.X),
                                                           ConvertUnits.ToSimUnits(Sprite.Sprite.Texture.Size.Y), density);
                Physics.Body.Position = ConvertUnits.ToSimUnits(position);

                Physics.Body.BodyType = BodyType.Dynamic;
            }
            Physics.Body.UserData = this;
        }

        public override void SyncComponents()
        {
            Sprite.Sprite.Position = new Vector2f(ConvertUnits.ToDisplayUnits(Physics.Position.X), ConvertUnits.ToDisplayUnits(Physics.Position.Y));
            Sprite.Sprite.Rotation = MathHelper.ToDegrees(Physics.Rotation);

            if (Sprite.Sprite.Position.Y - Sprite.Sprite.Texture.Size.Y - 64 > GraphicsSubsystem.Instance.GetCameraOffset().Y + Graphics.Height)
                Kill();
        }

        public void Hit(float damage, Vector2 point, Vector2 direction)
        {
            if (HitPoints <= 0) return;

            HitPoints -= damage;

            if (HitPoints <= 0)
            {
                Crush(direction);

                GameplayState.Score += 200;
                Content.ShatterSound.Play();
                GraphicsSubsystem.Instance.ShakeCamera(3);
            }
            else
            {
                Physics.Body.ApplyLinearImpulse(direction * 10, point);

                GameplayState.Score += (int)(damage * 10);
                Content.HitSound.Play();
                GraphicsSubsystem.Instance.ShakeCamera(1);
            }
        }

        public void Slice(Vector2 entryPoint, Vector2 exitPoint, bool crush = false, Vector2 direction = default(Vector2))
        {
            HitPoints = Math.Min(HitPoints, 1);

            var textureOrigin = Sprite.Sprite.Texture;
            Texture textureA, textureB;
            var spriteOrigin = Sprite.Sprite.Origin;
            var relativeEntryPoint =
                new Vector2f(ConvertUnits.ToDisplayUnits(entryPoint.X - Physics.Position.X),
                                ConvertUnits.ToDisplayUnits(entryPoint.Y - Physics.Position.Y));
            var rotation = Transform.Identity;
            rotation.Rotate(-Sprite.Sprite.Rotation);
            var rotatedEntryPoint = rotation.TransformPoint(relativeEntryPoint);
            var startPoint = spriteOrigin + rotatedEntryPoint;
            var relativeExitPoint =
                new Vector2f(ConvertUnits.ToDisplayUnits(exitPoint.X - Physics.Position.X),
                                ConvertUnits.ToDisplayUnits(exitPoint.Y - Physics.Position.Y));
            var rotatedExitPoint = rotation.TransformPoint(relativeExitPoint);
            var endPoint = spriteOrigin + rotatedExitPoint;
            TextureSlicer.SliceAndDice(startPoint, endPoint, textureOrigin, out textureA,
                out textureB, RepeatsX, RepeatsY);
            Vertices first;
            Vertices second;
            var fixture = Physics.Body.FixtureList[0];
            FarseerPhysics.Common.PolygonManipulation.CuttingTools.SplitShape(fixture, entryPoint, exitPoint, out first, out second);
            //Delete the original shape and create two new. Retain the properties of the body.
            if (first.GetArea() < 0.005f ||
                second.GetArea() < 0.005f || first.CheckPolygon() != PolygonError.NoError ||
                second.CheckPolygon() != PolygonError.NoError)
                return;

            Body firstFixture = BodyFactory.CreatePolygon(PhysicsSubsystem.Instance.World, first, 1f,
                                                            fixture.Body.Position);
            firstFixture.Rotation = fixture.Body.Rotation;
            firstFixture.LinearVelocity = fixture.Body.LinearVelocity;
            firstFixture.AngularVelocity = fixture.Body.AngularVelocity;
            firstFixture.BodyType = BodyType.Dynamic;
            firstFixture.UserData = this;
            Physics.Body = firstFixture;
            Sprite.Sprite.Texture = textureA;
            RepeatsX = RepeatsY = 1; // 1WEEK

            if (crush)
            {
                Physics.Body.IgnoreCollisionWith(GameplayState.Player.Torso.Body);
                Physics.Body.IgnoreCollisionWith(GameplayState.Player.Legs.Body);
                Physics.Body.IgnoreCollisionWith(GameplayState.Player.Piston.Body);
                if (first.GetArea() > 0.25f)
                    Crush(direction);
            }

            Body secondFixture = BodyFactory.CreatePolygon(PhysicsSubsystem.Instance.World, second, 1f,
                                                            fixture.Body.Position);
            secondFixture.Rotation = fixture.Body.Rotation;
            secondFixture.LinearVelocity = fixture.Body.LinearVelocity;
            secondFixture.AngularVelocity = fixture.Body.AngularVelocity;
            secondFixture.BodyType = BodyType.Dynamic;
            var secondCandy = new CandyObject(secondFixture, textureB, secondFixture.Position) { HitPoints = HitPoints };
            secondFixture.UserData = secondCandy;

            if (crush)
            {
                secondCandy.Physics.Body.IgnoreCollisionWith(GameplayState.Player.Torso.Body);
                secondCandy.Physics.Body.IgnoreCollisionWith(GameplayState.Player.Legs.Body);
                secondCandy.Physics.Body.IgnoreCollisionWith(GameplayState.Player.Piston.Body);
                if (second.GetArea() > 0.25f)
                    secondCandy.Crush(direction);
            }

            PhysicsSubsystem.Instance.World.RemoveBody(fixture.Body);

            if(OnSlice != null)
                OnSlice();
        }

        private void Crush(Vector2 direction)
        {
            var rotation = Transform.Identity;
            rotation.Rotate((float)Content.Random.NextDouble() * 180); // 1WEEK

            var start = rotation.TransformPoint(-5, 0);
            var end = rotation.TransformPoint(5, 0);

            Vector2 startPoint = Vector2.Zero, endPoint = Vector2.Zero;

            PhysicsSubsystem.Instance.World.RayCast((fixture, pos, arg3, arg4) =>
                                                    {
                                                        if (fixture.Body.UserData == this)
                                                        {
                                                            startPoint = pos;
                                                            return 0;
                                                        }

                                                        return 1;
                                                    }, Physics.Body.Position + new Vector2(start.X, start.Y),
                                                    Physics.Body.Position + new Vector2(end.X, end.Y));
            PhysicsSubsystem.Instance.World.RayCast((fixture, pos, arg3, arg4) =>
                                                    {
                                                        if (fixture.Body.UserData == this)
                                                        {
                                                            endPoint = pos;
                                                            return 0;
                                                        }

                                                        return 1;
                                                    }, Physics.Body.Position + new Vector2(end.X, end.Y),
                                                    Physics.Body.Position + new Vector2(start.X, start.Y));

            if (startPoint != Vector2.Zero && endPoint != Vector2.Zero)
                Slice(startPoint, endPoint, true);

            Physics.Body.ApplyLinearImpulse(direction * 10);
        }

        public void Kill()
        {
            GraphicsSubsystem.Instance.Unregister(Sprite);
            PhysicsSubsystem.Instance.Unregister(Physics);
            GameObjectSubsystem.Instance.Unregister(this);
            //this = null; // SEPPUKU
        }
    }
}
