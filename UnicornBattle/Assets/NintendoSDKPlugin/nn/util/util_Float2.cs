/*--------------------------------------------------------------------------------*
  Copyright (C)Nintendo All rights reserved.

  These coded instructions, statements, and computer programs contain proprietary
  information of Nintendo and/or its licensed developers and are protected by
  national and international copyright laws. They may not be disclosed to third
  parties or copied or duplicated in any form, in whole or in part, without the
  prior written consent of Nintendo.

  The content herein is highly confidential and should be handled accordingly.
 *--------------------------------------------------------------------------------*/

#if UNITY_SWITCH || UNITY_EDITOR || NN_PLUGIN_ENABLE 
using System;
using System.Runtime.InteropServices;

namespace nn.util
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Float2 : IEquatable<Float2>
    {
        public float x;
        public float y;

        public Float2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void Set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format("({0} {1})", this.x, this.y);
        }
        #region Equality
        public static bool operator ==(Float2 lhs, Float2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Float2 lhs, Float2 rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is Float2)) { return false; }
            return Equals((Float2)right);
        }

        public bool Equals(Float2 other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }
}
#endif
