using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    /// <summary>
    /// Interface which allows an object to act as a task in the Engine.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// The task should perform a single iteration of its execution.
        /// </summary>
        void OnRun();

        /// <summary>
        /// The task should perform any necessary initialization.
        /// </summary>
        void OnStart();

        /// <summary>
        /// The task should perform any necessary cleanup code.
        /// </summary>
        void OnStop();
    }
}
