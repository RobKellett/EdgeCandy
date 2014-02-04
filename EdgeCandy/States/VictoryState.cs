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
    public class VictoryState : IGameState
    {
        private TextComponent text;
        private InputComponent input = new InputComponent();

        private int timesPressed = 0;

        public VictoryState()
        {
            text = new TextComponent { Text = new Text(string.Format(
"Congratulations!\n" +
"Your score: {0:D5}\n\n" +
"Press RIGHT SHIFT twice to play again.\n\n\n" +
"Brought to you by:\n" +
"@TheRobKellett - \"Finally, I can sleep!\"\n" + 
"@Quantumplation (π) - \"Please do not put my name in this crappy game.\"\n", GameplayState.Score), Content.Font, 16)
            };

            input.KeyEvents[Keyboard.Key.RShift] = (key, mod) =>
                                                   {
                                                       timesPressed++;
                                                       if (timesPressed >= 2)
                                                           Program.TransitionToState<GameplayState>();
                                                   };
        }

        public void Update(double elapsedTime)
        {
            UpdateSubsystem.Instance.Update(elapsedTime);

            var textBounds = text.Text.GetLocalBounds();
            text.Text.Position = new Vector2f(Graphics.Width / 2 - (int)(textBounds.Width / 2), Graphics.Height / 2 - (int)(textBounds.Height / 2));
        }

        public void Draw(double elapsedTime)
        {
            GraphicsSubsystem.Instance.Draw();
        }
    }
}
