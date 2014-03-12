using System;
using System.Collections.Generic;
using System.Text;

using Tank.Common;
using Tank.Game;

namespace Tank
{
    public class Program
    {
        public static void Main()
        {
            try
            {
                Engine.Instance.AddTask(Renderer.Instance);
                Engine.Instance.AddTask(EventManager.Instance);
                Engine.Instance.AddTask(ControlManager.Instance);
                Engine.Instance.AddTask(new GameClient());

                Engine.Instance.Run();
            }
            catch (InitializationException e)
            {
                Logger.Instance.LogError(Resources.InitializationError, e);
                Engine.Instance.AttemptShutdown();
            }
            catch (Exception e) // traps unhandled exceptions that didn't occur during task execution
            {
                Logger.Instance.LogUnhandledException(e);
                Engine.Instance.AttemptShutdown();
            }
        }
    }
}
