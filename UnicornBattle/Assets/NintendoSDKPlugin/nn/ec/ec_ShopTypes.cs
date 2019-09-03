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

namespace nn.ec
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NsUid : IEquatable<NsUid>
    {
        public ulong value;

        public NsUid(ulong _value)
        {
            value = _value;
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static NsUid GetInvalidId()
        {
            NsUid id = new NsUid() { value = 0 };
            return id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NsUid)) { return false; }
            return Equals((NsUid)obj);
        }
        public bool Equals(NsUid other) { return this == other; }
        public override int GetHashCode() { return base.GetHashCode(); }
        public static bool operator ==(NsUid lhs, NsUid rhs) { return lhs.value == rhs.value; }
        public static bool operator !=(NsUid lhs, NsUid rhs) { return !(lhs == rhs); }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CourseId
    {
        public const int MaxStringLength = 16;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxStringLength + 1)]
        public string value;

        public CourseId(string _value)
        {
            value = _value;
        }

        public override string ToString()
        {
            return value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ConsumableId
    {
        public const int MaxStringLength = 16;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxStringLength + 1)]
        public string value;

        public ConsumableId(string _value)
        {
            value = _value;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
#endif