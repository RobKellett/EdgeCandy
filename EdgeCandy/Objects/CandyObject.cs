using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using EdgeCandy.Framework;
using SFML.Graphics;
using SFML.Window;

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
