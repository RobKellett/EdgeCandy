
using EdgeCandy.Subsystems;
using SFML.Window;

namespace EdgeCandy
{
    /// <summary>
    /// Represents some object in the game world.
    /// </summary>
    public abstract class GameObject
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public GameObject()
        {
            GameObjectSubsystem.Instance.Register(this);
        }

        /// <summary>
        /// A graphics component has a position at which it draws things,
        /// and a physics component has a position which it simulates.
        /// These need to be synchronized each frame.
        /// </summary>
        public abstract void SyncComponents();
    }
}
