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
    public static partial class AddOnContent
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static nn.Result QueryMountCacheSize(ref long pOutValue, int targetIndex)
        {
            pOutValue = 0;
            return new Result();
        }

        public static nn.Result Mount(
            string name, int targetIndex, byte[] pFileSystemCacheBuffer, long fileSystemCacheBufferSize)
        {
            return new Result();
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_QueryMountAddOnContentCacheSize")]
        public static extern nn.Result QueryMountCacheSize(ref long pOutValue, int targetIndex);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_MountAddOnContent")]
        public static extern nn.Result Mount(
            string name, int targetIndex, byte[] pFileSystemCacheBuffer, long fileSystemCacheBufferSize);
#endif
    }
}
#endif
