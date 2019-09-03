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
using nn.account;

namespace nn.fs
{
    public static partial class SaveData
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_EnsureSaveData")]
        public static extern nn.Result Ensure(Uid user);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_MountSaveData")]
        public static extern nn.Result Mount(string name, Uid user);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_MountSaveDataReadOnly")]
        public static extern nn.Result MountSaveDataReadOnly(string name, ulong applicationId, Uid user);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_IsSaveDataExisting0")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsExisting(Uid user);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_IsSaveDataExisting1")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsExisting(ulong applicationId, Uid user);
    }
}
#endif
