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

namespace EdgeCandy.States
{
    public class GameplayState : IGameState
    {
        public static Player Player;
        private CameraComponent camera;

        public GameplayState()
        {
            var map = new MapObject(Content.Level);
            Player = new Player(map.Spawn);
            camera = new CameraComponent("scroll", map.Map.Height * map.Map.TileHeight, 100); // could be worse

            GraphicsSubsystem.Instance.SwitchCamera("scroll");
        }

        public void Update(double elapsedTime)
        {
            // update
            UpdateSubsystem.Instance.Update(elapsedTime);
            PhysicsSubsystem.Instance.Update(elapsedTime);
            GameObjectSubsystem.Instance.Synchronize();

            if (Player.Torso.Position.Y > ConvertUnits.ToSimUnits(camera.Position.Y + Graphics.Height) + 1)
                Program.ChangeState<GameOverState>();
        }

        public void Draw(double elapsedTime)
        {
            // draw
            GraphicsSubsystem.Instance.Draw();
        }
    }
}
