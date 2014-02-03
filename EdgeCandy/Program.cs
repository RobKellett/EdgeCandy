using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Framework;
using EdgeCandy.Objects;
using EdgeCandy.States;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy
{
    class Program
    {
        public static Window Window { get; set; }
        public static IGameState GameState { get; private set; }

        public static void TransitionToState<T>() where T : IGameState, new()
        {
            ChangeState<TransitionState<T>>();
        }

        public static void ChangeState<T>() where T : IGameState, new()
        {
            if (Content.Music.Status == SoundStatus.Playing)
                Content.Music.Stop();

            UpdateSubsystem.Instance.Kill();
            PhysicsSubsystem.Instance.Kill();
            GraphicsSubsystem.Instance.Kill();
            GameState = new T();
        }

        static void Main(string[] args)
        {
            using (var window = new RenderWindow(new VideoMode(Graphics.Width, Graphics.Height), "The Edge of Candy: Scroll Saga Deluxe"))
            {
                Window = window;
                Graphics.Initialize();
                Content.Load();

                ConvertUnits.SetDisplayUnitToSimUnitRatio(32);

                window.SetActive();
                window.Closed += (sender, eventArgs) => window.Close();
                window.Resized += (sender, eventArgs) =>
                                  {
                                      window.SetView(new View(new Vector2f(eventArgs.Width / 2f, eventArgs.Height / 2f), new Vector2f(eventArgs.Width, eventArgs.Height)));
                                      Graphics.Resize(eventArgs.Width, eventArgs.Height);
                                  };

                var stopwatch = new Stopwatch();

                GameState = new TitleScreenState();

                while (window.IsOpen())
                {
                    var elapsed = stopwatch.Elapsed.TotalSeconds;
                    stopwatch.Restart();

                    window.DispatchEvents();
                    
                    GameState.Update(elapsed);

                    window.Clear();

                    GameState.Draw(elapsed);

                    Graphics.RenderTo(window);
                    window.Display();
                }
            }
        }
    }
}
