using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EdgeCandy.Components
{
    /// <summary>
    /// Represents an object through a table of pixels
    /// </summary>
    public class SpriteComponent : GraphicsComponent
    {
        /// <summary>
        /// Our sprite!
        /// </summary>
        public Sprite Sprite { get; set; }

        /// <summary>
        /// Draws the sprite... surprise surprise
        /// </summary>
        public override void Draw()
        {
            Graphics.Draw(Sprite);
        }
    }
}
