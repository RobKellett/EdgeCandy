using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using EdgeCandy.Framework;
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
        public TimerComponent DecayTimer = new TimerComponent(5);

        public float RepeatsX = 1, RepeatsY = 1; // 1WEEK

        public double Health
        {
            get; set;
        }

        public double OriginalHealth
        {
            get; set;
        }

        public CandyObject(Body body, Texture tex, Vector2 position)
        {
            Sprite.Sprite = new Sprite(tex);
            Sprite.Sprite.Origin = new Vector2f(Sprite.Sprite.Texture.Size.X / 2, Sprite.Sprite.Texture.Size.Y / 2);
            Physics.Body = body;
            DecayTimer.DingDingDing += (sender, args) => Kill();
        }

        public CandyObject(CandyKind kind, Vector2 position, Vector2 size = default(Vector2))
        {
            switch (kind)
            {
                case CandyKind.CandyCane:
                    Sprite.Sprite = new Sprite(Content.CandyCane);
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
                    break;
                case CandyKind.DoubleCandyCane:
                    Sprite.Sprite = new Sprite(Content.DoubleCandyCane);
                    break;
                case CandyKind.Rancher:
                    Sprite.Sprite = new Sprite(Content.Rancher);
                    break;
            }
            Health = 100;
            OriginalHealth = 100;
            if (kind != CandyKind.Chocolate)
            {
                Sprite.Sprite.Origin = new Vector2f(Sprite.Sprite.Texture.Size.X / 2, Sprite.Sprite.Texture.Size.Y / 2);
                Physics.Body = BodyFactory.CreateRectangle(PhysicsSubsystem.Instance.World, ConvertUnits.ToSimUnits(Sprite.Sprite.Texture.Size.X),
                                                           ConvertUnits.ToSimUnits(Sprite.Sprite.Texture.Size.Y), 25);
                Physics.Body.Position = ConvertUnits.ToSimUnits(position);

                Physics.Body.BodyType = BodyType.Dynamic;
            }
            Physics.Body.UserData = this;

            DecayTimer.DingDingDing += (sender, args) => Kill();
        }

        public override void SyncComponents()
        {
            Sprite.Sprite.Position = new Vector2f(ConvertUnits.ToDisplayUnits(Physics.Position.X), ConvertUnits.ToDisplayUnits(Physics.Position.Y));
            Sprite.Sprite.Rotation = MathHelper.ToDegrees(Physics.Rotation);
        }

        public bool Slice(Vector2 entryPoint, Vector2 exitPoint)
        {
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
            var originalArea = first.GetArea() + second.GetArea();
            if (first.GetArea() * fixture.Shape.Density < 0.5 ||
                second.GetArea() * fixture.Shape.Density < 0.5)
                return false;
            if (first.CheckPolygon() == PolygonError.NoError)
            {
                Body firstFixture = BodyFactory.CreatePolygon(PhysicsSubsystem.Instance.World, first, fixture.Shape.Density * 0.9f, fixture.Body.Position);
                firstFixture.Rotation = fixture.Body.Rotation;
                firstFixture.LinearVelocity = fixture.Body.LinearVelocity;
                firstFixture.AngularVelocity = fixture.Body.AngularVelocity;
                firstFixture.BodyType = BodyType.Dynamic;
                firstFixture.UserData = this;
                Physics.Body = firstFixture;
                Sprite.Sprite.Texture = textureA;
                RepeatsX = RepeatsY = 1; // 1WEEK
                Health = OriginalHealth*(first.GetArea()/originalArea);
                if (first.GetArea() * fixture.Shape.Density < 5)
                    DecayTimer.Start();
            }

            if (second.CheckPolygon() == PolygonError.NoError)
            {
                Body secondFixture = BodyFactory.CreatePolygon(PhysicsSubsystem.Instance.World, second, fixture.Shape.Density * 0.9f, fixture.Body.Position);
                secondFixture.Rotation = fixture.Body.Rotation;
                secondFixture.LinearVelocity = fixture.Body.LinearVelocity;
                secondFixture.AngularVelocity = fixture.Body.AngularVelocity;
                secondFixture.BodyType = BodyType.Dynamic;
                var secondCandy = new CandyObject(secondFixture, textureB, secondFixture.Position);
                secondFixture.UserData = secondCandy;
                secondCandy.Health = OriginalHealth * (second.GetArea() / originalArea);
                secondCandy.OriginalHealth = secondCandy.Health;
                if (second.GetArea() * fixture.Shape.Density < 5)
                    secondCandy.DecayTimer.Start();
            }

            OriginalHealth = Health;

            PhysicsSubsystem.Instance.World.RemoveBody(fixture.Body);
            return true;
        }

        public void Crush(Vector2 impactPoint, Vector2 direction)
        {
            Physics.Body.ApplyForce(direction*1500, impactPoint);
            Health -= 10;
            if (Health < 10)
            {
                Slice(impactPoint, impactPoint + direction * 10);
            }
        }

        public void Kill()
        {
            GraphicsSubsystem.Instance.Unregister(Sprite);
            PhysicsSubsystem.Instance.Unregister(Physics);
            UpdateSubsystem.Instance.Unregister(DecayTimer);
            GameObjectSubsystem.Instance.Unregister(this);
            //this = null; // SEPPUKU
        }
    }
}
