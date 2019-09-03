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
    public static partial class Account
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static void Initialize()
        {
        }

        public static Result OpenUser(ref UserHandle pOutHandle, Uid user)
        {
            return new Result();
        }

        public static bool TryOpenPreselectedUser(ref UserHandle pOutHandle)
        {
            return true;
        }

        public static Result ListQualifiedUsers(ref int pOutActualLength, Uid[] outUsers, int arrayLength)
        {
            return new Result();
        }

        public static void CloseUser(UserHandle handle)
        {
        }

        public static Result GetUserId(ref Uid pOut, UserHandle handle)
        {
            return new Result();
        }

        public static Result StoreSaveDataThumbnailImage(Uid user, byte[] imageBuffer)
        {
            return new Result();
        }

        public static Result DeleteSaveDataThumbnailImage(Uid user)
        {
            return new Result();
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_Initialize")]
        public static extern void Initialize();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_OpenUser")]
        public static extern Result OpenUser(ref UserHandle pOutHandle, Uid user);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_TryOpenPreselectedUser")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool TryOpenPreselectedUser(ref UserHandle pOutHandle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ListQualifiedUsers")]
        public static extern Result ListQualifiedUsers(ref int pOutActualLength, Uid[] outUsers, int arrayLength);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_CloseUser")]
        public static extern void CloseUser(UserHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_GetUserId")]
        public static extern Result GetUserId(ref Uid pOut, UserHandle handle);

        public static Result StoreSaveDataThumbnailImage(Uid user, byte[] imageBuffer)
        {
            return Account.StoreSaveDataThumbnailImage(user, imageBuffer, imageBuffer.Length);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_StoreSaveDataThumbnailImage")]
        private static extern Result StoreSaveDataThumbnailImage(
            Uid user, byte[] imageBuffer, int imageBufferSize);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_DeleteSaveDataThumbnailImage")]
        public static extern Result DeleteSaveDataThumbnailImage(Uid user);
#endif
    }

    public static partial class NetworkServiceAccount
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result EnsureAvailable(UserHandle handle)
        {
            return new Result();
        }

        public static Result IsAvailable(ref bool pOut, UserHandle handle)
        {
            pOut = false;
            return new Result();
        }

        public static Result GetId(ref NetworkServiceAccountId pOutId, UserHandle handle)
        {
            return new Result();
        }

        public static Result EnsurIdTokenCacheAsync(AsyncContext pOutContext, UserHandle handle)
        {
            return new Result();
        }

        public static Result LoadIdTokenCache(ref ulong pOutActualSize, byte[] buffer, UserHandle handle)
        {
            pOutActualSize = 0;
            return new Result();
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_EnsureNetworkServiceAccountAvailable")]
        public static extern Result EnsureAvailable(UserHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_IsNetworkServiceAccountAvailable")]
        public static extern Result IsAvailable([MarshalAs(UnmanagedType.U1)] ref bool pOut, UserHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_GetNetworkServiceAccountId")]
        public static extern Result GetId(ref NetworkServiceAccountId pOutId, UserHandle handle);

        public static Result EnsurIdTokenCacheAsync(AsyncContext pOutContext, UserHandle handle)
        {
            return NetworkServiceAccount.EnsurIdTokenCacheAsync(pOutContext._context, handle);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_EnsureNetworkServiceAccountIdTokenCacheAsync")]
        public static extern Result EnsurIdTokenCacheAsync(IntPtr pOutContext, UserHandle handle);

        public static Result LoadIdTokenCache(ref ulong pOutActualSize, byte[] buffer, UserHandle handle)
        {
            return NetworkServiceAccount.LoadIdTokenCache(ref pOutActualSize, buffer, buffer.Length, handle);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_LoadNetworkServiceAccountIdTokenCache")]
        private static extern Result LoadIdTokenCache(
            ref ulong pOutActualSize, byte[] buffer, int bufferSize, UserHandle handle);
#endif
    }
}
#endif
