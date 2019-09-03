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

namespace nn.fs
{
    [Flags]
    public enum ReadOption
    {
        None = 0,
    }

    [Flags]
    public enum WriteOption
    {
        None = 0,
        Flush = 1 << 0,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FileHandle
    {
        public IntPtr handle;
    }

    public static partial class File
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_ReadFile0")]
        public static extern nn.Result Read(
            FileHandle handle, long offset, byte[] buffer, long size, ReadOption option);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_ReadFile1")]
        public static extern nn.Result Read(
            FileHandle handle, long offset, byte[] buffer, long size);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_ReadFile2")]
        public static extern nn.Result Read(
            ref long outValue, FileHandle handle, long offset, byte[] buffer, long size, ReadOption option);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_ReadFile3")]
        public static extern nn.Result Read(
            ref long outValue, FileHandle handle, long offset, byte[] buffer, long size);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_WriteFile")]
        public static extern nn.Result Write(
            FileHandle handle, long offset, byte[] buffer, long size, WriteOption option);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_FlushFile")]
        public static extern nn.Result Flush(FileHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_SetFileSize")]
        public static extern nn.Result SetSize(FileHandle handle, long size);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_GetFileSize")]
        public static extern nn.Result GetSize(ref long outValue, FileHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_GetFileOpenMode")]
        public static extern OpenFileMode GetOpenMode(FileHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_CloseFile")]
        public static extern void Close(FileHandle handle);
    }
}
#endif
