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
        protected List<U> componentsToAdd = new List<U>(); 
        protected List<U> componentsOutToPasture = new List<U>();

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
            componentsToAdd.Add(component);
        }

        public virtual void Unregister(U component)
        {
            componentsOutToPasture.Add(component);
        }

        public virtual void Clean()
        {
            foreach (var component in componentsToAdd)
                components.Add(component);

            foreach (var component in componentsOutToPasture)
                components.Remove(component);

            componentsToAdd.Clear();
            componentsOutToPasture.Clear();
        }
    }
}
