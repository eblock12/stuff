using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    /// <summary>
    /// A 3-D dimensional vector
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>
    {
        /// <summary>
        /// The X component of the 2-D vector.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y component of the 2-D vector.
        /// </summary>
        public double Y;

        /// <summary>
        /// Constructs a 2-D vector with the specified X and Y components.
        /// </summary>
        /// <param name="x">The X axis component</param>
        /// <param name="y">The Y axis component</param>
        public Vector2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Constructs a 2-D vector using the same value as X and Y components.
        /// </summary>
        /// <param name="s">The value for both X and Y axis components.</param>
        public Vector2(double s)
        {
            this.X = this.Y = s;
        }

        /// <summary>
        /// Gets a 2-D vector with all components set to 0.
        /// </summary>
        public static Vector2 Zero
        {
            get { return new Vector2(0, 0); }
        }

        /// <summary>
        /// Gets a 2-D vector with all components set to 1.
        /// </summary>
        public static Vector2 One
        {
            get { return new Vector2(1.0, 1.0); }
        }

        /// <summary>
        /// Gets the standard 2-D unit vector for the X axis.
        /// </summary>
        public static Vector2 UnitX
        {
            get { return new Vector2(1.0, 0); }
        }

        /// <summary>
        /// Gets the standard 2-D unit vector for the Y axis.
        /// </summary>
        public static Vector2 UnitY
        {
            get { return new Vector2(0, 1.0); }
        }

        /// <summary>
        /// Calculates the dot product of two 2-D vectors.
        /// </summary>
        /// <param name="a">The first vector</param>
        /// <param name="b">The second vector</param>
        /// <returns>The result of the calculation.</returns>
        public static double DotProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        /// <summary>
        /// Calculates the vector's angle between -Pi and Pi.
        /// </summary>
        /// <returns>The vector angle in radians.</returns>
        public double Angle()
        {
            return Math.Atan2(Y, X);
        }

        public override bool Equals(object obj)
        {
            return (obj is Vector2) ? this == (Vector2)obj : false;
        }

        public bool Equals(Vector2 v)
        {
            return this == v;
        }

        public override int GetHashCode()
        {
            return (int)(this.X + this.Y);
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public double LengthSquared()
        {
            return (X * X + Y * Y);
        }

        public void Normalize()
        {
            double length = this.Length();
            this.X /= length;
            this.Y /= length;
        }

        public static Vector2 Normalize(Vector2 v)
        {
            double length = v.Length();
            return new Vector2(v.X / length, v.Y / length);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(48);
            builder.Append("{X:");
            builder.Append(this.X);
            builder.Append(" Y:");
            builder.Append(this.Y);
            builder.Append("}");
            return builder.ToString();
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return (a.X == b.X && a.Y == b.Y);
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }

        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2 operator *(Vector2 v, double s)
        {
            return new Vector2(v.X * s, v.Y * s);
        }

        public static Vector2 operator *(double s, Vector2 v)
        {
            return new Vector2(v.X * s, v.Y * s);
        }

        public static Vector2 operator /(Vector2 v, double s)
        {
            return new Vector2(v.X / s, v.Y / s);
        }
    }
}
