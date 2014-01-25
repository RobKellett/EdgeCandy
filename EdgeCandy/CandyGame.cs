using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Objects;
using EdgeCandy.Subsystems;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy
{
    public class CandyGame
    {
        private Player player;
        public CandyGame()
        {
            // load content
            new SpriteComponent { Sprite = new Sprite(Content.TestSplash)};
            player = new Player();
            new TextComponent { Text = new Text("Hello, world!", Content.Font, 16) };
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
        }
    }
}
