using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeCandy.Subsystems
{
    /// <summary>
    /// Manages syncronizing the components of all game components
    /// </summary>
    public class GameObjectSubsystem : Subsystem<GameObjectSubsystem, GameObject>
    {
        /// <summary>
        /// Synchronize the different components of the game objects.
        /// </summary>
        public void Synchronize()
        {
            foreach(var obj in components)
                obj.SyncComponents();

            Clean();
        }
    }
}
