using System;
using System.Collections.Generic;
using System.Text;

using Tao.Sdl;

namespace Tank.Common
{
    public class ControlManager : Singleton<ControlManager>, ITask
    {
        private int keyCount = -1;
        private byte[] keyStates = null;

        private int mouseX, mouseY;
        private bool leftMousePressed, rightMousePressed;

        public ControlManager()
        { }

        public int MouseX
        {
            get { return mouseX; }
        }

        public int MouseY
        {
            get { return mouseY; }
        }

        #region ITask Members

        public void OnRun()
        {
            keyCount = 0;
            keyStates = Sdl.SDL_GetKeyState(out keyCount);

            byte mouseButtons = Sdl.SDL_GetMouseState(out mouseX, out mouseY);
            leftMousePressed = (mouseButtons & Sdl.SDL_BUTTON(Sdl.SDL_BUTTON_LEFT)) != 0;
            rightMousePressed = (mouseButtons & Sdl.SDL_BUTTON(Sdl.SDL_BUTTON_RIGHT)) != 0;
        }

        public void OnStart()
        {
            //Sdl.SDL_ShowCursor(Sdl.SDL_DISABLE);
        }

        public void OnStop()
        {
        }

        #endregion

        public bool IsPressed(ControlBinding binding)
        {
            if (binding.KeyCode >= 0 && binding.KeyCode < keyCount)
            {
                if (keyStates == null)
                {
                    return false; 
                }

                if (keyStates[binding.KeyCode] == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (binding.MouseButton != MouseButton.None)
            {
                switch (binding.MouseButton)
                {
                    case MouseButton.Left:
                        return leftMousePressed;
                    case MouseButton.Right:
                        return rightMousePressed;
                    default:
                        return false;
                }
            }

            return false;
        }
    }

    public class ControlBinding
    {
        private string name;
        private int keyCode;
        private MouseButton mouseButton;

        public const int UnusedKeyCode = -1;

        public ControlBinding()
        {
            name = "UnknownBinding";
            keyCode = ControlBinding.UnusedKeyCode;
            mouseButton = MouseButton.None;
        }

        public ControlBinding(String name, int keyCode)
        {
            this.name = name;
            this.keyCode = keyCode;
        }

        public ControlBinding(String name, MouseButton mouseButton)
        {
            this.name = name;
            this.keyCode = ControlBinding.UnusedKeyCode;
            this.mouseButton = mouseButton;
        }

        public static int KeyEscape
        {
            get { return Sdl.SDLK_ESCAPE; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

       public int KeyCode
        {
            get { return keyCode; }
            set { keyCode = value; }
        }

        public MouseButton MouseButton
        {
            get { return mouseButton; }
            set { mouseButton = value; }
        }

        public override string ToString()
        {
            return name;
        }
    }

    public enum MouseButton
    {
        None,
        Left,
        Right
    }
}
