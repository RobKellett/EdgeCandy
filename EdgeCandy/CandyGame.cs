using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Objects;
using EdgeCandy.Subsystems;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy
{
    public class CandyGame
    {
        private Sprite sprite;
        private Player player;
        public CandyGame()
        {
            // load content
            sprite = new Sprite(Content.TestSplash);
            player = new Player();
        }

        public void Update(double elapsedTime)
        {
            // update
            PhysicsSubsystem.Instance.Update(elapsedTime);
            GameObjectSubsystem.Instance.Synchronize();
        }

        public void Draw(double elapsedTime)
        {
            // render
            GraphicsSubsystem.Instance.Draw();
            Graphics.Draw(sprite);
            Graphics.DrawString("Hello, world!", new Vector2f(4, 4));
        }
    }
}
