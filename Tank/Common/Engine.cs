using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    /// <summary>
    /// Contains the main loop for the game and handles task execution.
    /// </summary>
    public class Engine : Singleton<Engine>
    {
        private LinkedList<ITask> runningTasks = new LinkedList<ITask>();
        private LinkedList<ITask> newTasks = new LinkedList<ITask>();
        private LinkedList<ITask> deadTasks = new LinkedList<ITask>();

        /// <summary>
        /// Adds a new task to the engine and starts it.
        /// </summary>
        /// <param name="task">The task to start running in the engine.</param>
        public void AddTask(ITask task)
        {
            if (runningTasks.Contains(task))
            {
                throw new TaskException(Resources.TaskAddAlreadyExists);
            }

            newTasks.AddLast(task);
        }

        /// <summary>
        /// Stops the task and removes it from the engine.
        /// </summary>
        /// <param name="task">The task to stop running in the engine.</param>
        public void RemoveTask(ITask task)
        {
            if (!runningTasks.Contains(task))
            {
                throw new TaskException(Resources.TaskRemoveDoesNotExist);
            }

            deadTasks.AddLast(task);
        }

        /// <summary>
        /// Stops all tasks and removes from the engine ending execution.
        /// </summary>
        public void RemoveAllTasks()
        {
            foreach (ITask task in runningTasks)
            {
                deadTasks.AddLast(task);
            }

            newTasks.Clear();
        }

        /// <summary>
        /// Attempts to shutdown any remaining tasks when an unrecoverable error occurs.
        /// </summary>
        public void AttemptShutdown()
        {
            newTasks.Clear();

            foreach (ITask task in runningTasks)
            {
                try
                {
                    // try to shut down the task
                    task.OnStop();
                }
                catch (Exception) // absorb exceptions
                { } 
            }
        }

        /// <summary>
        /// Runs all engine tasks until there are no more tasks left.
        /// </summary>
        public void Run()
        {
            Logger.Instance.LogInformation(Resources.EngineTag, Resources.EnterMainLoop);

            // main loop, exit when there's no tasks left
            while (runningTasks.Count > 0 || newTasks.Count > 0)
            {
                // add any new tasks
                if (newTasks.Count > 0)
                {
                    foreach (ITask newTask in newTasks)
                    {
                        newTask.OnStart();
                        runningTasks.AddLast(newTask);
                    }

                    newTasks.Clear();
                }

                // execute all tasks that are running
                foreach (ITask task in runningTasks)
                {
                    try
                    {
                        task.OnRun();
                    }
                    catch (Exception e) // trap unhandled exceptions in a task
                    {
                        Logger.Instance.LogUnhandledException(e);
                        RemoveAllTasks();
                    }
                }

                // remove any dead tasks
                if (deadTasks.Count > 0)
                {
                    foreach (ITask deadTask in deadTasks)
                    {
                        deadTask.OnStop();
                        runningTasks.Remove(deadTask);
                    }

                    deadTasks.Clear();
                }
            }

            Logger.Instance.LogInformation(Resources.EngineTag, Resources.ExitMainLoop);
        }
    }
}
