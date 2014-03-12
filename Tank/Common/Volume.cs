using System;
using System.Collections.Generic;
using System.Text;

namespace Tank.Common
{
    /// <summary>
    /// Describes the locations of 6 planes composing a 3-D volume.
    /// </summary>
    public struct Volume
    {
        public double Top, Bottom, Left, Right, Front, Back;

        public Volume(double top, double bottom, double left, double right,
            double front, double back)
        {
            this.Top = top;
            this.Bottom = bottom;
            this.Left = left;
            this.Right = right;
            this.Front = front;
            this.Back = back;
        }
    }
}
