using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EdgeCandy.Components
{
    public class TextComponent : GraphicsComponent
    {
        /// <summary>
        /// Our text!
        /// </summary>
        public Text Text { get; set; }

        public override void Draw()
        {
            Graphics.DrawText(Text);
        }
    }
}
