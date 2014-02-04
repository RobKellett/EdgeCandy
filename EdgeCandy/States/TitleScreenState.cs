using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using EdgeCandy.Subsystems;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.States
{
    public class TitleScreenState : IGameState
    {
        private Keyboard.Key startKey;
        private SpriteComponent logo;
        private TextComponent prompt;
        private InputComponent input;

        public TitleScreenState()
        {
            var possibleKeys = new[]
                               {
                                   Keyboard.Key.Delete, Keyboard.Key.P, Keyboard.Key.BackSlash, Keyboard.Key.Escape,
                                   Keyboard.Key.Tab, Keyboard.Key.Period, Keyboard.Key.F5, Keyboard.Key.D
                               };

            startKey = possibleKeys[Content.Random.Next(possibleKeys.Length)];

            logo = new SpriteComponent { Sprite = new Sprite(Content.Logo) };
            prompt = new TextComponent { Text = new Text("Press " + startKey.ToString().ToUpper(), Content.Font, 16) };
            input = new InputComponent();
            input.KeyEvents[startKey] = (key, mod, time) => Program.TransitionToState<GameplayState>();
        }

        public void Update(double elapsedTime)
        {
            UpdateSubsystem.Instance.Update(elapsedTime);

            logo.Sprite.Position = new Vector2f(Graphics.Width / 2 - logo.Sprite.Texture.Size.X / 2, 100);
            prompt.Text.Position = new Vector2f(Graphics.Width / 2 - (int)(prompt.Text.GetLocalBounds().Width / 2), Graphics.Height - 100);
        }

        public void Draw(double elapsedTime)
        {
            GraphicsSubsystem.Instance.Draw();
        }
    }
}
