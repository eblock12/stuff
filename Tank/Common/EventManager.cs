using System;
using System.Collections.Generic;
using System.Text;

using Tao.Sdl;

namespace Tank.Common
{
    public class EventManager : Singleton<EventManager>, ITask
    {
        #region ITask Members

        public void OnRun()
        {
            Sdl.SDL_Event e;

            while (Sdl.SDL_PollEvent(out e) != 0)
            {
                switch (e.type)
                {
                    case Sdl.SDL_QUIT: // user requested quit
                        Logger.Instance.LogInformation(Resources.EventTag, Resources.UserQuitRequest);
                        Engine.Instance.RemoveAllTasks();
                        break;
                }
            }
        }

        public void OnStart()
        {
        }

        public void OnStop()
        {
        }

        #endregion
    }
}
