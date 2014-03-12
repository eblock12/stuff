using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    /// <summary>
    /// Base singleton class for implementing singleton patterns.
    /// </summary>
    /// <typeparam name="T">Type of singleton instance</typeparam>
    public abstract class Singleton<T> where T : new()
    {
        /// <summary>
        /// Gets the Singleton instance of this class.
        /// </summary>
        public static T Instance
        {
            get { return SingletonFactory.instance; }
        }

        private class SingletonFactory
        {
            static SingletonFactory()
            {
            }

            internal static readonly T instance = new T();
        }
    }
}
