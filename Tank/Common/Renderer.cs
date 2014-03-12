using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using Tao.Sdl;
using Tao.OpenGl;
using System.Reflection;

namespace Tank.Common
{
    /// <summary>
    /// Performs initialization and management of the video display
    /// and rendering of all graphical objects.
    /// </summary>
    public class Renderer : Singleton<Renderer>, ITask
    {
        private const int DefaultFrameWidth = 800;
        private const int DefaultFrameHeight = 600;

        private Vector3 cameraPosition, cameraFocusPoint, cameraUp;

        private LinkedList<ModelTransformer> modelList = new LinkedList<ModelTransformer>();

        private int frameWidth, frameHeight;

#if MACOSX
        [DllImport("/System/Library/Frameworks/Cocoa.framework/Cocoa", EntryPoint="NSApplicationLoad")]
        private static extern void NSApplicationLoad();
        private static object nsPool;
#endif

        public Renderer()
        {
        }

        /// <summary>
        /// Gets the width of the video frame buffer.
        /// </summary>
        public int FrameWidth
        {
            get { return frameWidth; }
        }

        /// <summary>
        /// Gets the height of the video frame buffer.
        /// </summary>
        public int FrameHeight
        {
            get { return frameHeight; }
        }

        /// <summary>
        /// Gets or sets the 3-D position of the camera.
        /// </summary>
        public Vector3 CameraPosition
        {
            get { return cameraPosition; }
            set { cameraPosition = value; }
        }

        /// <summary>
        /// Gets or sets the 3-D point that the camera is pointed at.
        /// </summary>
        public Vector3 CameraFocusPoint
        {
            get { return cameraFocusPoint; }
            set { cameraFocusPoint = value; }
        }

        /// <summary>
        /// Gets or sets the 3-D direction that points upward from the camera.
        /// </summary>
        public Vector3 CameraUp
        {
            get { return cameraUp; }
            set { cameraUp = value; }
        }

        #region ITask Members

        /// <summary>
        /// Perform all pending rendering operations
        /// </summary>
        public void OnRun()
        {
            this.RenderVideo();
        }

        /// <summary>
        /// Perform initialization of video subsystem.
        /// </summary>
        public void OnStart()
        {
            try
            {
                this.InitializeVideo(DefaultFrameWidth, DefaultFrameHeight, false);
            }
            catch (SdlException ex)
            {
                throw new RendererInitializationException(Resources.RendererInitializationFailure, ex);
            }
        }

        /// <summary>
        /// Perform the shut down of the video subsystem.
        /// </summary>
        public void OnStop()
        {
            this.ShutdownVideo();
        }

        #endregion

        /// <summary>
        /// Draws the specified Model using its ModelTransformer.
        /// </summary>
        /// <param name="model">The ModelTransfer to use when transforming its model.</param>
        public void DrawModel(ModelTransformer model)
        {
            modelList.AddLast(model);
        }

        /// <summary>
        /// Draws the specified Model using a default ModelTransformer.
        /// </summary>
        /// <param name="model">The Model to render.</param>
        public void DrawModel(Model model)
        {
            DrawModel(new ModelTransformer(model));
        }

        private void InitializeVideo(int frameWidth, int frameHeight, bool useFullscreen)
        {
            Logger.Instance.LogInformation(Resources.RendererTag, Resources.VideoInitializing);

#if MACOSX
            Assembly asm = Assembly.LoadWithPartialName("cocoa-sharp");
            
            if (asm == null)
            {
                throw new System.IO.FileNotFoundException("Failed to find Apple.Foundation.dll assembly");
            }

            Type nsAutoReleasePool = asm.GetType("NSAutoreleasePool");
            nsPool = asm.CreateInstance("Cocoa.AutoreleasePool");
            nsPool.GetType().GetMethod("Initialize").Invoke(nsPool, null);

            NSApplicationLoad();
#endif


            // bring up SDL's video subsystem
            if (Sdl.SDL_InitSubSystem(Sdl.SDL_INIT_VIDEO) < 0)
            {
                throw new SdlException(Sdl.SDL_GetError());
            }

            // get the pointer to the video info structure
            IntPtr videoInfoPtr = Sdl.SDL_GetVideoInfo();
            if (videoInfoPtr == IntPtr.Zero)
            {
                throw new SdlException(Sdl.SDL_GetError());
            }

            // marshal video info structure pointer
            Sdl.SDL_VideoInfo videoInfo = (Sdl.SDL_VideoInfo)Marshal.PtrToStructure(videoInfoPtr, 
                typeof(Sdl.SDL_VideoInfo));

            // marshal pixel format structure pointer
            Sdl.SDL_PixelFormat pixelFormat = (Sdl.SDL_PixelFormat)Marshal.PtrToStructure(videoInfo.vfmt,
                typeof(Sdl.SDL_PixelFormat));

            int bitsPerPixel = pixelFormat.BitsPerPixel;

            // set at least 5 bits per color component
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_RED_SIZE, 5);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_GREEN_SIZE, 5);
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_BLUE_SIZE, 5);
            
            // use 16-bit depth buffer
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DEPTH_SIZE, 16);

            // enable double-buffering
            Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DOUBLEBUFFER, 1);

            int flags = useFullscreen ? Sdl.SDL_OPENGL | Sdl.SDL_FULLSCREEN : Sdl.SDL_OPENGL;

            // set the new video mode
            if (Sdl.SDL_SetVideoMode(frameWidth, frameHeight, bitsPerPixel, flags) == IntPtr.Zero)
            {
                throw new SdlException(Sdl.SDL_GetError());
            }

            // set window title
            Sdl.SDL_WM_SetCaption(Resources.WindowCaption, Resources.WindowCaption);

            this.frameWidth = frameWidth;
            this.frameHeight = frameHeight;
        }

        private void ShutdownVideo()
        {
            Logger.Instance.LogInformation(Resources.RendererTag, Resources.VideoShutdown);

            Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_VIDEO);
        }

        private void RenderVideo()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            this.Setup3DPerspective();
            this.SetupLighting();

            foreach (ModelTransformer model in modelList)
            {
                model.Apply();
            }
            modelList.Clear();

            Sdl.SDL_GL_SwapBuffers();
        }

        private void SetupLighting()
        {
            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(Gl.GL_LIGHT0);
            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glLightfv(Gl.GL_LIGHT0, Gl.GL_SPOT_DIRECTION, new float[] { 0.0f, -1.0f, 0.3f });
        }

        private void Setup3DPerspective()
        {
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(45, (double)frameWidth / (double)frameHeight, 0.1, 100.0);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();


            
            Glu.gluLookAt(cameraPosition.X, cameraPosition.Y, cameraPosition.Z,
                cameraFocusPoint.X, cameraFocusPoint.Y, cameraFocusPoint.Z,
                cameraUp.X, cameraUp.Y, cameraUp.Z);

            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_CULL_FACE);

            Gl.glClearColor(0.5f, 0.5f, 1.0f, 1.0f);
        }
    }
}
