using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeCandy.Subsystems
{
    /// <summary>
    /// Something that is a singleton, and handles components
    /// </summary>
    /// <typeparam name="T">The type of instance for the singleton.  Should be the same as the declaring type</typeparam>
    /// <typeparam name="U">The type of component to manage/register</typeparam>
    public abstract class Subsystem<T, U> where T : Subsystem<T, U>, new()
    {
        private static T instance;
        protected List<U> components = new List<U>();

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static T Instance
        {
            get
            {
                return instance ?? (instance = new T());
            }
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="component">The component to register</param>
        public virtual void Register(U component)
        {
            components.Add(component);
        }
    }
}
