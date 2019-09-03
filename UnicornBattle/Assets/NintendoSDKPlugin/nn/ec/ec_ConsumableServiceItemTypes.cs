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
using System.Runtime.InteropServices;

namespace nn.ec
{
    public static partial class ConsumableServiceItem
    {
        public const int RequiredWorkMemorySize = 256 * 1024;
        public const int RequiredUserSaveDataSize = 32 * 1024;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ConsumableServiceItemId
    {
        public const int Length = 16;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Length + 1)]
        public string value;

        public ConsumableServiceItemId(string _value)
        {
            value = _value;
        }

        public override string ToString()
        {
            return value;
        }

#if !UNITY_SWITCH || UNITY_EDITOR
        public bool IsValid()
        {
            return false;
        }
#else
        public bool IsValid()
        {
            return ConsumableServiceItemId.IsValid(this);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemId_IsVailed")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IsValid(ConsumableServiceItemId itemId);
#endif
    }
}
#endif
