using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    /// <summary>
    /// Stores a Color with floating-point components.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// The red color component
        /// </summary>
        public double R;

        /// <summary>
        /// The green color component
        /// </summary>
        public double G;

        /// <summary>
        /// The blue color component
        /// </summary>
        public double B;

        /// <summary>
        /// The alpha color component
        /// </summary>
        public double A;

        /// <summary>
        /// Initializes the color with RGB components and alpha set to 1.0.
        /// </summary>
        /// <param name="red">The red component from 0 to 1.</param>
        /// <param name="green">The green component from 0 to 1.</param>
        /// <param name="blue">The blue component from 0 to 1.</param>
        public Color(double red, double green, double blue)
        {
            R = red;
            G = green;
            B = blue;
            A = 1.0;
        }

        /// <summary>
        /// Initializes the color with the RGBA components.
        /// </summary>
        /// <param name="red">The red component from 0 to 1.</param>
        /// <param name="green">The green component from 0 to 1.</param>
        /// <param name="blue">The blue component from 0 to 1.</param>
        /// <param name="alpha">The alpha component from 0 to 1.</param>
        public Color(double red, double green, double blue, double alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        /// <summary>
        /// Gets the Color object for white.
        /// </summary>
        public static Color White
        {
            get { return new Color(1.0, 1.0, 1.0); }
        }

        /// <summary>
        /// Gets the Color object for black.
        /// </summary>
        public static Color Black
        {
            get { return new Color(0.0, 0.0, 0.0); }
        }

        /// <summary>
        /// Converts the Color to an array of floats.
        /// </summary>
        /// <returns>The Color stored in an array.</returns>
        public float[] ToFloatArray()
        {
            return new float[] { (float)R, (float)G, (float)B, (float)A };
        }
    }
}
