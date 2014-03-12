using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    /// <summary>
    /// Provides generic methods for various uses.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Parses a comma-delimited list of integers.
        /// </summary>
        /// <param name="list">The comma-delimited list.</param>
        /// <returns>An array of integers.</returns>
        public static int[] ParseIntegerList(string list)
        {
            string[] listTokens = list.Split(',');

            int[] intList = new int[listTokens.Length];

            for (int i = 0; i < listTokens.Length; i++)
            {
                intList[i] = Int32.Parse(listTokens[i]);
            }

            return intList;
        }

        /// <summary>
        /// Parses a space-delimited list of 2-D vectors.
        /// </summary>
        /// <param name="list">The space-delimited list.</param>
        /// <returns>An array of 2-D vectors.</returns>
        public static Vector2[] ParseVector2List(string list)
        {
            string[] vectorTokens = list.Split(' ');
            Vector2[] vectorList = new Vector2[vectorTokens.Length];

            for (int i = 0; i < vectorTokens.Length; i++)
            {
                string vectorToken = vectorTokens[i];
                string[] componentTokens = vectorToken.Split(',');

                double x = Double.Parse(componentTokens[0]);
                double y = Double.Parse(componentTokens[1]);

                vectorList[i] = new Vector2(x, y);
            }

            return vectorList;
        }

        /// <summary>
        /// Parses a space-delimited list of 3-D vectors.
        /// </summary>
        /// <param name="list">The space-delimited list.</param>
        /// <returns>An array of 3-D vectors.</returns>
        public static Vector3[] ParseVector3List(string list)
        {
            string[] vectorTokens = list.Split(' ');
            Vector3[] vectorList = new Vector3[vectorTokens.Length];

            for (int i = 0; i < vectorTokens.Length; i++)
            {
                string vectorToken = vectorTokens[i];
                string[] componentTokens = vectorToken.Split(',');

                double x = Double.Parse(componentTokens[0]);
                double y = Double.Parse(componentTokens[1]);
                double z = Double.Parse(componentTokens[2]);

                vectorList[i] = new Vector3(x, y, z);
            }

            return vectorList;
        }

        /// <summary>
        /// Flattens the components of 3-D vectors into an array of floating point values.
        /// </summary>
        /// <param name="vectors">The array of vector objects to convert.</param>
        /// <returns>An array of scalar floating-point values</returns>
        public static double[] ConvertVectorArrayToFloatArray(Vector3[] vectors)
        {
            double[] floatArray = new double[vectors.Length * 3];

            for (int i = 0; i < vectors.Length; i++)
            {
                floatArray[i * 3 + 0] = vectors[i].X;
                floatArray[i * 3 + 1] = vectors[i].Y;
                floatArray[i * 3 + 2] = vectors[i].Z;
            }

            return floatArray;
        }

        /// <summary>
        /// Converts the specified angle in degrees to radians.
        /// </summary>
        /// <param name="angle">The angle stored as degrees.</param>
        /// <returns>The angle in radians.</returns>
        public static double ConvertDegreesToRadians(double angle)
        {
            return angle * (Math.PI / 180.0);
        }

        /// <summary>
        /// Converts the specified angle in radians to degrees.
        /// </summary>
        /// <param name="radians">The angle stored as radians.</param>
        /// <returns>The angle in degrees.</returns>
        public static double ConvertRadiansToDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        /// <summary>
        /// Returns true if the two floating-point values are equal within 
        /// a specified amount of error.
        /// </summary>
        /// <param name="a">The first floating-point value to compare.</param>
        /// <param name="b">The second floating-point value to compare.</param>
        /// <param name="err">The amount of error to allow in comparison.</param>
        /// <returns>True if the two are equal within error, false if not.</returns>
        public static bool IsNearlyEqual(double a, double b, double err)
        {
            return (a - err < b && a + err > b);
        }
    }
}
