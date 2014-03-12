using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    /// <summary>
    /// A 3-D dimensional vector
    /// </summary>
    public struct Vector3 : IEquatable<Vector3>
    {
        public double X;
        public double Y;
        public double Z;

        public Vector3(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3(double s)
        {
            this.X = this.Y = this.Z = s;
        }

        public static Vector3 Zero
        {
            get { return new Vector3(0, 0, 0); }
        }

        public static Vector3 One
        {
            get { return new Vector3(1.0, 1.0, 1.0); }
        }

        public static Vector3 UnitX
        {
            get { return new Vector3(1.0, 0, 0); }
        }

        public static Vector3 UnitY
        {
            get { return new Vector3(0, 1.0, 0); }
        }

        public static Vector3 UnitZ
        {
            get { return new Vector3(0, 0, 1.0); }
        }

        public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return new Vector3(a.Y * b.Z - b.Y * a.Z, -(a.X * b.Z - b.X * a.Z), a.X * b.Y - b.X * a.Y);
        }

        public static double DotProduct(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        public override bool Equals(object obj)
        {
            return (obj is Vector3) ? this == (Vector3)obj : false;
        }

        public bool Equals(Vector3 v)
        {
            return this == v;
        }

        public override int GetHashCode()
        {
            return (int)(this.X + this.Y + this.Z);
        }

        public double Length()
        {
            return Math.Sqrt(X * X + Y * Y + Z * Z);
        }

        public double LengthSquared()
        {
            return (X * X + Y * Y + Z * Z);
        }

        public void Normalize()
        {
            double length = this.Length();
            this.X /= length;
            this.Y /= length;
            this.Z /= length;
        }

        public static Vector3 Normalize(Vector3 v)
        {
            double length = v.Length();
            return new Vector3(v.X / length, v.Y / length, v.Z / length);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(64);
            builder.Append("{X:");
            builder.Append(this.X);
            builder.Append(" Y:");
            builder.Append(this.Y);
            builder.Append(" Z:");
            builder.Append(this.Z);
            builder.Append("}");
            return builder.ToString();
        }

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return (a.X == b.X && a.Y == b.Y && a.Z == b.Z);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector3 operator *(Vector3 v, double s)
        {
            return new Vector3(v.X * s, v.Y * s, v.Z * s);
        }

        public static Vector3 operator *(double s, Vector3 v)
        {
            return new Vector3(v.X * s, v.Y * s, v.Z * s);
        }

        public static Vector3 operator /(Vector3 v, double s)
        {
            return new Vector3(v.X / s, v.Y / s, v.Z / s);
        }
    }
}
