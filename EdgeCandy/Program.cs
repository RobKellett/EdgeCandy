using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Framework;
using EdgeCandy.Objects;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var window = new RenderWindow(new VideoMode(Graphics.Width, Graphics.Height), "The Edge of Candy: Scroll Saga Deluxe"))
            {
                Graphics.Initialize();
                Content.Load();

                window.SetActive();
                window.Closed += (sender, eventArgs) => window.Close();
                window.Resized += (sender, eventArgs) =>
                                  {
                                      window.SetView(new View(new Vector2f(eventArgs.Width / 2f, eventArgs.Height / 2f), new Vector2f(eventArgs.Width, eventArgs.Height)));
                                      Graphics.Resize(eventArgs.Width, eventArgs.Height);
                                  };

                var stopwatch = new Stopwatch();

                var game = new CandyGame();

                while (window.IsOpen())
                {
                    var elapsed = stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();

                    window.DispatchEvents();
                    
                    game.Update(elapsed);

                    window.Clear();

                    game.Draw(elapsed);

                    Graphics.RenderTo(window);
                    window.Display();
                }
            }
        }
    }
}
