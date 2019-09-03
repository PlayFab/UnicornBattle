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
    public struct Color4u8 : IEquatable<Color4u8>
    {
        public byte r;
        public byte g;
        public byte b;
        public byte a;

        public void Set(byte r, byte g, byte b, byte a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3})", this.r, this.g, this.b, this.a);
        }

        #region Equality
        public static bool operator ==(Color4u8 lhs, Color4u8 rhs)
        {
            return lhs.r == rhs.r && lhs.g == rhs.g && lhs.b == rhs.b && lhs.a == rhs.a;
        }

        public static bool operator !=(nn.util.Color4u8 lhs, nn.util.Color4u8 rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is Color4u8)) { return false; }
            return Equals((Color4u8)right);
        }

        public bool Equals(Color4u8 other)
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
