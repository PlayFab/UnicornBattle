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

namespace nn.fs
{
    public static partial class Host
    {
#if DEVELOPMENT_BUILD || NN_FS_HOST_ENABLE
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_MountHost")]
        public static extern nn.Result MountHost(string name, string rootPath);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_MountHostRoot")]
        public static extern nn.Result MountHostRoot();
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_UnMountHostRoot")]
        public static extern void UnMountHostRoot();
#else
        public static nn.Result MountHost(string name, string rootPath)
        {
            return new nn.Result();
        }
        public static nn.Result MountHostRoot()
        {
            return new nn.Result();
        }
        public static void UnMountHostRoot()
        {
        }
#endif
    }
}
#endif
