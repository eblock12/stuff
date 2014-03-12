using System;
using System.Collections.Generic;
using System.Text;

using Tao.OpenGl;

namespace Tank.Common
{
    /// <summary>
    /// Performs a set of standard transformation to a model prior to rendering
    /// and provides resulting 3-D to 2-D projection calculations.
    /// </summary>
    public class ModelTransformer
    {
        private Vector3 displacement;
        private double rotationAngle;
        private Vector3 rotationAxis;
        private Vector3 scale;
        private Model model;
        private bool enableProjection;
        private Vector2 projectedPosition;

        /// <summary>
        /// Initializes a ModelTransformer for the specified model.
        /// </summary>
        /// <param name="model">The Model to apply transformations to</param>
        public ModelTransformer(Model model)
        {
            this.model = model;

            displacement = Vector3.Zero;
            rotationAngle = 0;
            rotationAxis = Vector3.UnitY;
            scale = Vector3.One;

            enableProjection = false;
            projectedPosition = Vector2.Zero;
        }

        /// <summary>
        /// Gets the model that transformations will be applied to.
        /// </summary>
        public Model Model
        {
            get { return model; }
        }

        /// <summary>
        /// Gets or sets if calculation of 3-D to 2-D projection is enabled.
        /// </summary>
        public bool EnableProjection
        {
            get { return enableProjection; }
            set { enableProjection = value; }
        }

        /// <summary>
        /// Gets or sets the 3-D displacement from the world origin.
        /// </summary>
        public Vector3 Displacement
        {
            get { return displacement; }
            set { displacement = value; }
        }

        /// <summary>
        /// Gets or sets the angle to rotate the model by in degrees.
        /// </summary>
        public double RotationAngle
        {
            get { return rotationAngle; }
            set { rotationAngle = value; }
        }

        /// <summary>
        /// Gets or sets the 3-D axis that rotations will be applied around.
        /// </summary>
        public Vector3 RotationAxis
        {
            get { return rotationAxis; }
            set { rotationAxis = value; }
        }

        /// <summary>
        /// Gets or sets the scale multiplier for 3 dimensions.
        /// </summary>
        public Vector3 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        /// <summary>
        /// Gets the results from the 3-D to 2-D projection calculation.
        /// </summary>
        public Vector2 ProjectedPosition
        {
            get { return projectedPosition; }
        }

        /// <summary>
        /// Applies the transformation to the matrix stack and renders the Model.
        /// </summary>
        public void Apply()
        {
            // push current matrix to preserve it after render
            Gl.glPushMatrix();

            // apply translation from world origin first
            Gl.glTranslated(displacement.X, displacement.Y, displacement.Z);

            // now apply any scaling
            Gl.glScaled(scale.X, scale.Y, scale.Z);

            // rotate object around the axis
            Gl.glRotated(rotationAngle, rotationAxis.X, rotationAxis.Y, rotationAxis.Z);

            // calculate the projection if it is enabled
            if (enableProjection)
            {
                // TODO: These calculations should be moved to the Renderer and
                // redundant matrix gets should be avoided

                double[] modelViewMatrix = new double[16];
                double[] projectionMatrix = new double[16];
                int[] viewport = new int[4];

                Gl.glGetDoublev(Gl.GL_MODELVIEW_MATRIX, modelViewMatrix);
                Gl.glGetDoublev(Gl.GL_PROJECTION_MATRIX, projectionMatrix);
                Gl.glGetIntegerv(Gl.GL_VIEWPORT, viewport);

                double unused = 0;
                Glu.gluProject(0, 0, 0, modelViewMatrix, projectionMatrix, viewport,
                    out projectedPosition.X, out projectedPosition.Y, out unused);

                projectedPosition.Y = (double)Renderer.Instance.FrameHeight - projectedPosition.Y;
            }

            model.Show();

            // restore the model view matrix
            Gl.glPopMatrix();
        }
    }
}
