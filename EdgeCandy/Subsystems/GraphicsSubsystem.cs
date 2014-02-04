using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeCandy.Components;
using EdgeCandy.Framework;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;

namespace EdgeCandy.Subsystems
{
    /// <summary>
    /// Draws things
    /// </summary>
    public class GraphicsSubsystem : Subsystem<GraphicsSubsystem, GraphicsComponent>
    {
        Dictionary<string, CameraComponent> cameraComponents = new Dictionary<string, CameraComponent>();
        private CameraComponent activeCameraComponent;

        public void Register(string name, CameraComponent camera)
        {
            cameraComponents[name] = camera;
        }

        public void SwitchCamera(string name)
        {
            activeCameraComponent = name == null ? null : cameraComponents[name];
        }

        public Vector2f GetCameraOffset()
        {
            if (activeCameraComponent != null)
                return activeCameraComponent.Position;
            return new Vector2f(0, 0);
        }

        public void ShakeCamera(float shake)
        {
            activeCameraComponent.Shake = shake;
        }

        /// <summary>
        /// Draw!
        /// </summary>
        public void Draw()
        {
            Clean();

            if (activeCameraComponent != null)
                Graphics.SetView(new View(new FloatRect((int)activeCameraComponent.Position.X, (int)activeCameraComponent.Position.Y, Graphics.Width, Graphics.Height)));
            else
                Graphics.SetView(new View(new FloatRect(0, 0, Graphics.Width, Graphics.Height)));

            Graphics.Clear();

            foreach (var component in components)
            {
                component.Draw();
            }
        }
    }
}
