using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;

namespace EdgeCandy
{
    public class CandyGame
    {
        private Sprite sprite;
        public CandyGame()
        {
            // load content
            sprite = new Sprite(Content.TestSplash);
        }

        public void Update(double elapsedTime)
        {
            // update
        }

        public void Draw(double elapsedTime)
        {
            // render
            Graphics.Draw(sprite);
        }
    }
}
