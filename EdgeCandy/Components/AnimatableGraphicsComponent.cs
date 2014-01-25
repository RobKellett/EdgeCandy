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
        public Animation Animation { get; set; }

        public AnimatableGraphicsComponent()
        {
            AnimationSubsystem.Instance.Register(this);
        }

        public void Update(double elapsedTime)
        {
            Animation.Update(elapsedTime);
        }

        public override void Draw()
        {
            // Crop the sprite to the current animation frame
            var columns = (int)Sprite.Texture.Size.X / FrameSize.X;

            Sprite.TextureRect = new IntRect((Animation.CurrentFrame % columns) * FrameSize.X, (Animation.CurrentFrame / columns) * FrameSize.Y, FrameSize.X, FrameSize.Y);

            // Draw the sprite as usual
            base.Draw();
        }
    }
}
