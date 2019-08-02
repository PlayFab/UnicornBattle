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

namespace nn.account
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Uid : IEquatable<Uid>
    {
        public static Uid Invalid { get { return new Uid(); } }

        public ulong _data0;
        public ulong _data1;

        public bool IsValid()
        {
            return (_data0 != 0) || (_data1 != 0);
        }

        public override string ToString()
        {
            return string.Format("{0,0:X16}{1,0:X16}", _data0, _data1);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Uid)) { return false; }
            return Equals((Uid)obj);
        }
        public bool Equals(Uid other) { return this == other; }
        public override int GetHashCode() { return base.GetHashCode(); }
        public static bool operator ==(Uid lhs, Uid rhs) { return lhs._data0 == rhs._data0 && lhs._data1 == rhs._data1; }
        public static bool operator !=(Uid lhs, Uid rhs) { return !(lhs == rhs); }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct UserHandle
    {
        public ulong _data0;
        public ulong _data1;
        public ulong _context;

        public override string ToString()
        {
            return string.Format("{0,0:X16}{1,0:X16}_{2,0:X16}", _data0, _data1, _context);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Nickname
    {
        public const int NameBytesMax = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NameBytesMax + 1)]
        public string name;

        public override string ToString()
        {
            return name;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NetworkServiceAccountId : IEquatable<NetworkServiceAccountId>
    {
        public ulong id;

        public override string ToString()
        {
            return string.Format("{0:X}", id);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is NetworkServiceAccountId)) { return false; }
            return Equals((NetworkServiceAccountId)obj);
        }
        public bool Equals(NetworkServiceAccountId other) { return this == other; }
        public override int GetHashCode() { return base.GetHashCode(); }
        public static bool operator ==(NetworkServiceAccountId lhs, NetworkServiceAccountId rhs)
        {
            return lhs.id == rhs.id;
        }
        public static bool operator !=(NetworkServiceAccountId lhs, NetworkServiceAccountId rhs)
        {
            return !(lhs == rhs);
        }
    }
}
#endif
