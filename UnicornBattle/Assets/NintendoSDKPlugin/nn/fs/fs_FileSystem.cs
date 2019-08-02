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
    public enum EntryType
    {
        Directory,
        File,
    }

    public static partial class FileSystem
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_GetEntryType")]
        public static extern nn.Result GetEntryType(ref EntryType outValue, string path);
    }

    [Flags]
    public enum OpenFileMode
    {
        Read = 1 << 0,
        Write = 1 << 1,
        AllowAppend = 1 << 2,
    }

    public static partial class File
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_CreateFile")]
        public static extern nn.Result Create(string path, long size);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_DeleteFile")]
        public static extern nn.Result Delete(string path);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_RenameFile")]
        public static extern nn.Result Rename(string currentPath, string newPath);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_OpenFile")]
        public static extern nn.Result Open(
            ref FileHandle outValue, string path, OpenFileMode mode);
    }

    [Flags]
    public enum OpenDirectoryMode
    {
        Directory = 1 << 0,
        File = 1 << 1,
        All = OpenDirectoryMode.Directory | OpenDirectoryMode.File,
    }

    public static partial class Directory
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_CreateDirectory")]
        public static extern nn.Result Create(string path);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_DeleteDirectory")]
        public static extern nn.Result Delete(string path);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_DeleteDirectoryRecursively")]
        public static extern nn.Result DeleteRecursively(string path);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_CleanDirectoryRecursively")]
        public static extern nn.Result CleanRecursively(string path);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_RenameDirectory")]
        public static extern nn.Result Rename(string currentPath, string newPath);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_fs_OpenDirectory")]
        public static extern nn.Result Open(
            ref DirectoryHandle outValue, string path, OpenDirectoryMode mode);
    }
}
#endif
