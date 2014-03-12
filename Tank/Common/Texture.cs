using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Text;

using Tao.OpenGl;
using System.Drawing;
using System.IO;

namespace Tank.Common
{
    /// <summary>
    /// Loads and stores textures for rendering on 3-D geometry surfaces.
    /// </summary>
    public sealed class Texture
    {
        private string name;
        private uint id;
        private int width, height;

        private Texture(string name, uint id, int width, int height)
        {
            this.name = name;
            this.id = id;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Gets the name of this texture.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the width in pixels of this texture.
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// Gets the height in pixels of this texture.
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Constructs a Texture using an image stored in a file.
        /// </summary>
        /// <param name="file">The file name of the texture to load.</param>
        /// <returns>The Texture loaded from the file.</returns>
        public static Texture FromFile(string file)
        {
            try
            {
                Logger.Instance.LogInformation(Resources.TextureTag, Resources.TextureLoading, file);

                // search within the Data directory
                string fullPath = Path.Combine(Resources.DataPath, file);

                // load using the GDI+ imaging classes
                using (Bitmap bitmap = new Bitmap(Image.FromFile(fullPath)))
                {
                    // have OpenGL create a new texture
                    uint id;
                    Gl.glGenTextures(1, out id);

                    // bind the newly created texture and set alignment
                    Gl.glBindTexture(Gl.GL_TEXTURE_2D, 1);
                    Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

                    // lock the loaded image data for giving to OpenGL
                    Rectangle lockRect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                    BitmapData bitmapData = bitmap.LockBits(lockRect, ImageLockMode.ReadOnly,
                        PixelFormat.Format24bppRgb);

                    // set up linear texture filtering
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR);
                    Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

                    // generate the texture from the loaded image data
                    Gl.glTexImage2D(Gl.GL_TEXTURE_2D, 0, Gl.GL_RGB8, bitmap.Width, bitmap.Height, 
                        0, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

                    // unlock the loaded image data
                    bitmap.UnlockBits(bitmapData);

                    return new Texture(file, id, bitmap.Width, bitmap.Height);
                }                
            }
            catch (Exception e)
            {
                throw new DataLoadException(String.Format(Resources.DataLoadError, file), e);
            }
        }

        /// <summary>
        /// Constructs a Texture made of a solid color.
        /// </summary>
        /// <param name="c">The color the texture is made of.</param>
        /// <returns>A Texture containing a solid color.</returns>
        public static Texture FromColor(Color c)
        {
            byte[] buf = new byte[12]; // store 2x2, 24-bit texture
            for (int i = 0; i < 12; i += 3)
            {
                buf[i + 0] = (byte)(c.R * 255.0);
                buf[i + 1] = (byte)(c.G * 255.0);
                buf[i + 2] = (byte)(c.B * 255.0);
            }

            uint id;
            Gl.glGenTextures(1, out id); // get the texture ID

            // bind texture to update it
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, id);

            Gl.glPixelStorei(Gl.GL_UNPACK_ALIGNMENT, 1);

            // setup texture filtering
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_NEAREST);
            Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);

            // build texture mipmaps
            Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, 3, 2, 2, Gl.GL_RGB, Gl.GL_UNSIGNED_BYTE, buf);

            return new Texture("SolidColor", id, 2, 2);
        }

        /// <summary>
        /// Makes this the active texture for rendering onto surfaces.
        /// </summary>
        public void Bind()
        {
            Gl.glBindTexture(Gl.GL_TEXTURE_2D, id);
        }
    }
}
