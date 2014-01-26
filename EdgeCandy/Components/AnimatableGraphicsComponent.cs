using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.Components
{
    /// <summary>
    /// A SpriteComponent with animations
    /// </summary>
    public class AnimatableGraphicsComponent : SpriteComponent
    {
        public Vector2i FrameSize { get; set; }

        private Animation animation;

        public Animation Animation
        {
            get { return animation; }
            set
            {
                if (animation == value) return;

                animation = value;
                animation.Reset();
            }
        }

        public AnimatableGraphicsComponent()
        {
            AnimationSubsystem.Instance.Register(this);
        }

        public void Update(double elapsedTime)
        {
            animation.Update(elapsedTime);
        }

        public override void Draw()
        {
            // Crop the sprite to the current animation frame
            var columns = (int)Sprite.Texture.Size.X / FrameSize.X;

            Sprite.TextureRect = new IntRect((animation.CurrentFrame % columns) * FrameSize.X, (animation.CurrentFrame / columns) * FrameSize.Y, FrameSize.X, FrameSize.Y);

            // Draw the sprite as usual
            base.Draw();
        }
    }
}
