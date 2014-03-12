using System;
using System.Collections.Generic;
using System.Text;

using Tao.OpenGl;

namespace Tank.Common
{
    public class Material
    {
        private Color diffuseColor;
        private Color specularColor;
        private int shininess; // 0-128

        private Texture texture;

        public Material()
        {
            diffuseColor = Color.White;
            specularColor = Color.White;
            shininess = 64;
        }

        public Color DiffuseColor
        {
            get { return diffuseColor; }
            set { diffuseColor = value; }
        }

        public Color SpecularColor
        {
            get { return specularColor; }
            set { specularColor = value; }
        }

        public int Shininess
        {
            get { return shininess; }
            set { shininess = value; }
        }

        public Texture Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public void Apply()
        {
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, diffuseColor.ToFloatArray());
            Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, specularColor.ToFloatArray());
            Gl.glMateriali(Gl.GL_FRONT, Gl.GL_SHININESS, shininess);

            if (texture != null)
            {
                Gl.glEnable(Gl.GL_TEXTURE_2D);
                texture.Bind();
            }
            else
            {
                Gl.glDisable(Gl.GL_TEXTURE_2D);
            }
        }
    }
}
