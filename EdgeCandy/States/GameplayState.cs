using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using EdgeCandy.Objects;
using EdgeCandy.Subsystems;
using FarseerPhysics;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.States
{
    public class GameplayState : IGameState
    {
        public static Player Player;
        private CameraComponent camera;
        private SpriteComponent meterBack, meterFront;
        private TextComponent score;
        public MapObject Map;
        public static int Score;

        public GameplayState()
        {
            Map = new MapObject(Content.Level);
            Player = new Player(Map.Spawn);
            camera = new CameraComponent("scroll", Map.Map.Height * Map.Map.TileHeight, 160); // could be worse

            GraphicsSubsystem.Instance.SwitchCamera("scroll");
            
            score = new TextComponent { Text = new Text("", Content.Font, 16) };
            meterBack = new SpriteComponent { Sprite = new Sprite(Content.MeterBack) };
            meterFront = new SpriteComponent { Sprite = new Sprite(Content.MeterFront) };

            Score = 0;
            Content.Music.PlayingOffset = TimeSpan.Zero;
            Content.Music.Play();

        }

        public void Update(double elapsedTime)
        {
            // update
            UpdateSubsystem.Instance.Update(elapsedTime);
            PhysicsSubsystem.Instance.Update(elapsedTime);
            GameObjectSubsystem.Instance.Synchronize();

            if (Player.Torso.Position.Y > ConvertUnits.ToSimUnits(camera.Position.Y + Graphics.Height) + 1)
                Program.TransitionToState<GameplayState>();

            score.Text.Position = new Vector2f(8, (int)camera.Position.Y + 8);
            score.Text.DisplayedString = "Score: " + Score.ToString("00000");

            meterBack.Sprite.Position =
                meterFront.Sprite.Position =
                new Vector2f(Graphics.Width - meterBack.Sprite.Texture.Size.X - 8, (int)camera.Position.Y + 8);
            meterFront.Sprite.TextureRect = new IntRect(0, 0, (int)(100 * Player.Slicing / Player.MaxSlicing), 8);
        }

        public void Draw(double elapsedTime)
        {
            // draw
            GraphicsSubsystem.Instance.Draw();
        }
    }
}
