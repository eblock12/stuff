using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

using Tao.OpenGl;

namespace Tank.Common
{
    /// <summary>
    /// A 3-D model
    /// </summary>
    public class Model
    {
        // store mesh data
        private int[] triangleIndices;
        private Vector3[] normals;
        private Vector3[] positions;
        private Vector2[] textureCoords;

        // binded material to model
        private Material material = new Material();

        // result of bounding volume calculation
        private Volume boundingVolume;
        
        // OpenGL display list ID
        private int? displayListID;

        private Model(int[] triangleIndices, Vector3[] normals, Vector3[] positions)
        {
            this.triangleIndices = triangleIndices;
            this.normals = normals;
            this.positions = positions;
            this.textureCoords = null;

            this.CalculateBounds();
        }

        private Model(int[] triangleIndices, Vector3[] normals, Vector3[] positions, Vector2[] textureCoords)
        {
            this.triangleIndices = triangleIndices;
            this.normals = normals;
            this.positions = positions;
            this.textureCoords = textureCoords;

            this.CalculateBounds();
        }

        /// <summary>
        /// Gets a volume that contains exactly the dimensions of the model.
        /// </summary>
        public Volume BoundingVolume
        {
            get { return boundingVolume; }
        }

        ~Model()
        {
            if (displayListID.HasValue)
            {
                // TODO: Figure out why this crashes
                //Gl.glDeleteLists(displayListID.Value, 1);
            }
        }

        /// <summary>
        /// Gets or sets a Material that describes the surface of this Model.
        /// </summary>
        public Material Material
        {
            get { return material; }
            set { material = value; }
        }

        /// <summary>
        /// Creates a Model describing a plane on the XZ axis.
        /// </summary>
        /// <param name="centerPoint">The point in the center of the plane.</param>
        /// <param name="width">The width of the plane (X-axis)</param>
        /// <param name="height">The height of the plane (Z-axis)</param>
        /// <returns>A Model containing the newly created plane</returns>
        public static Model CreateTexturedPlane(Vector3 centerPoint, double width, double height,
            double repeatTextureU, double repeatTextureV)
        {
            double halfWidth = width / 2.0;
            double halfHeight = height / 2.0;

            // create each vertex for the plane
            Vector3[] positionVerts = new Vector3[4];
            positionVerts[0] = new Vector3(centerPoint.X - halfWidth, centerPoint.Y, centerPoint.Z - halfHeight);
            positionVerts[1] = new Vector3(centerPoint.X + halfWidth, centerPoint.Y, centerPoint.Z - halfHeight);
            positionVerts[2] = new Vector3(centerPoint.X + halfWidth, centerPoint.Y, centerPoint.Z + halfHeight);
            positionVerts[3] = new Vector3(centerPoint.X - halfWidth, centerPoint.Y, centerPoint.Z + halfHeight);

            // plane is flat so all normals are simply the Y-axis unit vector
            Vector3[] normalVerts = new Vector3[4];
            normalVerts[0] = Vector3.UnitY;
            normalVerts[1] = Vector3.UnitY;
            normalVerts[2] = Vector3.UnitY;
            normalVerts[3] = Vector3.UnitY;

            // generate the UV texture coordinates
            Vector2[] textureCoords = new Vector2[4];
            textureCoords[0] = new Vector2(0.0, 0.0);
            textureCoords[1] = new Vector2(repeatTextureU, 0.0);
            textureCoords[2] = new Vector2(repeatTextureU, repeatTextureV);
            textureCoords[3] = new Vector2(0.0, repeatTextureV);

            // plane is made of two triangles
            int[] triangleIndices = new int[6];
            triangleIndices[0] = 3;
            triangleIndices[1] = 1;
            triangleIndices[2] = 0;
            triangleIndices[3] = 3;
            triangleIndices[4] = 2;
            triangleIndices[5] = 1;

            return new Model(triangleIndices, normalVerts, positionVerts, textureCoords);
        }

        /// <summary>
        /// Loads a model stored in a X3M file.
        /// </summary>
        /// <param name="x3mFile">The file name of the X3M file to load</param>
        /// <returns>A Model that was stored in the X3M file.</returns>
        public static Model LoadModelFromX3m(string x3mFile)
        {
            try
            {
                Logger.Instance.LogInformation(Resources.ModelTag, Resources.ModelLoading, x3mFile);

                string fullPath = Path.Combine(Resources.DataPath, x3mFile);

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fullPath);

                XmlNode indicesNode = xmlDoc.SelectSingleNode("/Model/TriangleIndices");
                XmlNode positionsNode = xmlDoc.SelectSingleNode("/Model/Positions");
                XmlNode normalsNode = xmlDoc.SelectSingleNode("/Model/Normals");
                XmlNode texCoordNode = xmlDoc.SelectSingleNode("/Model/TextureCoordinates");

                int[] triangleIndices = Utility.ParseIntegerList(indicesNode.InnerText);
                Vector3[] positions = Utility.ParseVector3List(positionsNode.InnerText);
                Vector3[] normals = Utility.ParseVector3List(normalsNode.InnerText);

                if (texCoordNode == null)
                {
                    return new Model(triangleIndices, normals, positions);
                }
                else
                {
                    Vector2[] textureCoords = Utility.ParseVector2List(texCoordNode.InnerText);
                    return new Model(triangleIndices, normals, positions, textureCoords);
                }
            }
            catch (Exception e)
            {
                throw new DataLoadException(String.Format(Resources.DataLoadError, x3mFile), e);
            }
        }

        /// <summary>
        /// Renders the Model using the Renderer task.
        /// </summary>
        public void Show()
        {
            // apply the material first
            material.Apply();

            if (!displayListID.HasValue)
            {
                // compilation also performs actual rendering
                this.Compile();
            }
            else
            {
                // already compiled list so just run it
                Gl.glCallList(displayListID.Value);
            }
        }

        private void CalculateBounds()
        {
            double top, bottom, left, right, front, back;

            top = right = front = Double.NegativeInfinity;
            bottom = left = back = Double.PositiveInfinity;

            foreach (int triangleIndex in triangleIndices)
            {
                Vector3 vertPosition = positions[triangleIndex];

                if (vertPosition.X < left)
                {
                    left = vertPosition.X;
                }
                if (vertPosition.X > right)
                {
                    right = vertPosition.X;
                }
                if (vertPosition.Y < bottom)
                {
                    bottom = vertPosition.Y;
                }
                if (vertPosition.Y > top)
                {
                    top = vertPosition.Y;
                }
                if (vertPosition.Z < back)
                {
                    back = vertPosition.Z;
                }
                if (vertPosition.Z > front)
                {
                    front = vertPosition.Z;
                }
            }

            boundingVolume = new Volume(top, bottom, left, right, front, back);
        }

        private void Compile()
        {
            if (displayListID.HasValue)
            {
                Gl.glDeleteLists(displayListID.Value, 1);
            }

            displayListID = Gl.glGenLists(1);

            Logger.Instance.LogInformation(Resources.ModelTag, Resources.DisplayListCompiling, displayListID);

            Gl.glNewList(displayListID.Value, Gl.GL_COMPILE_AND_EXECUTE);
            Gl.glBegin(Gl.GL_TRIANGLES);

            int triangleCount = triangleIndices.Length / 3;
            foreach (int triangleIndex in triangleIndices)
            {
                Vector3 vertPosition = positions[triangleIndex];
                Vector3 vertNormal = normals[triangleIndex];

                if (textureCoords != null)
                {
                    Vector2 textureCoordinate = textureCoords[triangleIndex];
                    Gl.glTexCoord2d(textureCoordinate.X, textureCoordinate.Y); 
                }

                Gl.glNormal3d(vertNormal.X, vertNormal.Y, vertNormal.Z);
                Gl.glVertex3d(vertPosition.X, vertPosition.Y, vertPosition.Z);
            }

            Gl.glEnd();
            Gl.glEndList();
        }
    }
}
