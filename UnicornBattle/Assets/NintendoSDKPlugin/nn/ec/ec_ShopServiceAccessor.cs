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
using System.Text;
using System.Runtime.InteropServices;

namespace nn.ec
{
#if !UNITY_SWITCH || UNITY_EDITOR
    public sealed partial class ShopServiceAccessor : IDisposable
    {
        public static TimeSpan DefaultTimeout { get { return new TimeSpan(0, 0, 60); } }

        public static Result InitializeForShopServiceAccessors() { return new Result(); }
        public static Result FinalizeForShopServiceAccessors() { return new Result(); }

        public Result Initialize(ShopService type) { return new Result(); }

        public Result Request(
            AsyncResponse outAsyncResponse,
            nn.account.Uid userId,
            ShopServiceMethod method,
            string requestPath,
            string postData,
            TimeSpan timeout)
        {
            return new Result();
        }

        public Result Request(
            AsyncResponse outAsyncResponse,
            nn.account.Uid userId,
            ShopServiceMethod method,
            string requestPath,
            TimeSpan timeout)
        {
            return new Result();
        }

        public Result Request(
            AsyncResponse outAsyncResponse,
            nn.account.Uid userId,
            ShopServiceMethod method,
            string requestPath,
            string postData)
        {
            return new Result();
        }

        public Result Request(
            AsyncResponse outAsyncResponse,
            nn.account.Uid userId,
            ShopServiceMethod method,
            string requestPath)
        {
            return new Result();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public sealed class AsyncResponse : IDisposable
        {
            public AsyncResponse() { }
            public AsyncResponse(IntPtr asyncResponsePtr) { }

            public Result Get(ref string outJson)
            {
                return new Result();
            }

            public void Wait() { }
            public bool TryWait() { return false; }
            public void Cancel() { }

            public nn.err.ErrorCode GetErrorCode()
            {
                return nn.err.ErrorCode.GetInvalidErrorCode();
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
        }
    }
#else
    //----------------------------------------------------------------------------
    // ShopServiceAccessor
    //----------------------------------------------------------------------------
    public sealed partial class ShopServiceAccessor : IDisposable
    {
        private IntPtr _shopServiceAccessor = IntPtr.Zero;
        private bool isDisposed = false;

        internal IntPtr Ptr
        {
            get { return _shopServiceAccessor; }
        }

        public static TimeSpan DefaultTimeout { get { return new TimeSpan(0, 0, 60); } }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_InitializeForShopServiceAccessors")]
        public static extern Result InitializeForShopServiceAccessors();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_FinalizeForShopServiceAccessors")]
        public static extern Result FinalizeForShopServiceAccessors();

        public ShopServiceAccessor()
        {
            ShopServiceAccessor.New(ref _shopServiceAccessor);
        }

        ~ShopServiceAccessor()
        {
            _Dispose();
        }

        public Result Initialize(ShopService targetType)
        {
            return ShopServiceAccessor.Initialize(_shopServiceAccessor, targetType);
        }

        public Result Request(
            AsyncResponse outAsyncResponse,
            nn.account.Uid userId,
            ShopServiceMethod method,
            string requestPath,
            string postData,
            TimeSpan timeout)
        {
            Result result = ShopServiceAccessor.Request(
                _shopServiceAccessor, ref outAsyncResponse._asyncResponse, userId, method,
                requestPath, postData, (long)timeout.TotalMilliseconds);
            return result;
        }

        public Result Request(
            AsyncResponse outAsyncResponse,
            nn.account.Uid userId,
            ShopServiceMethod method,
            string requestPath,
            TimeSpan timeout)
        {
            Result result = ShopServiceAccessor.Request(
                _shopServiceAccessor, ref outAsyncResponse._asyncResponse, userId, method,
                requestPath, (long)timeout.TotalMilliseconds);
            return result;
        }

        public Result Request(
            AsyncResponse outAsyncResponse,
            nn.account.Uid userId,
            ShopServiceMethod method,
            string requestPath,
            string postData)
        {
            Result result = ShopServiceAccessor.Request(
                _shopServiceAccessor, ref outAsyncResponse._asyncResponse, userId, method,
                requestPath, postData, postData.Length);
            return result;
        }

        public Result Request(
            AsyncResponse outAsyncResponse,
            nn.account.Uid userId,
            ShopServiceMethod method,
            string requestPath)
        {
            Result result = ShopServiceAccessor.Request(
                _shopServiceAccessor, ref outAsyncResponse._asyncResponse, userId, method,
                requestPath);
            return result;
        }

        public void Dispose()
        {
            _Dispose();
            GC.SuppressFinalize(this);
        }

        private void _Dispose()
        {
            if (isDisposed) { return; }

            ShopServiceAccessor.Finalize(_shopServiceAccessor);

            if (_shopServiceAccessor != IntPtr.Zero)
            {
                ShopServiceAccessor.Delete(_shopServiceAccessor);
                _shopServiceAccessor = IntPtr.Zero;
            }

            isDisposed = true;
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShopServiceAccessor_Initialize")]
        private static extern Result Initialize(IntPtr shopServiceAccessor, ShopService target);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShopServiceAccessor_Finalize")]
        private static extern Result Finalize(IntPtr shopServiceAccessor);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShopServiceAccessor_Request1")]
        private static extern Result Request(
            IntPtr shopServiceAccessor, ref IntPtr outAsyncResponse,
            nn.account.Uid userId, ShopServiceMethod method, string requestPath,
            string postData, Int64 timeoutMilliseconds);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShopServiceAccessor_Request2")]
        private static extern Result Request(
            IntPtr shopServiceAccessor, ref IntPtr outAsyncResponse,
            nn.account.Uid userId, ShopServiceMethod method, string requestPath,
            Int64 timeoutMilliseconds);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShopServiceAccessor_Request3")]
        private static extern Result Request(
            IntPtr shopServiceAccessor, ref IntPtr outAsyncResponse,
            nn.account.Uid userId, ShopServiceMethod method, string requestPath,
            string postData);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShopServiceAccessor_Request4")]
        private static extern Result Request(
            IntPtr shopServiceAccessor, ref IntPtr outAsyncResponse,
            nn.account.Uid userId, ShopServiceMethod method, string requestPath);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShopServiceAccessor_New")]
        private static extern void New(ref IntPtr shopServiceAccessor);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShopServiceAccessor_Delete")]
        private static extern void Delete(IntPtr shopServiceAccessor);

        //----------------------------------------------------------------------------
        // AsyncResponce
        //----------------------------------------------------------------------------
        public sealed class AsyncResponse : IDisposable
        {
            internal IntPtr _asyncResponse = IntPtr.Zero;
            private bool isDisposed = false;

            public AsyncResponse()
            {
                AsyncResponse.New(ref _asyncResponse);
            }

            public AsyncResponse(IntPtr asyncResponsePtr)
            {
                _asyncResponse = asyncResponsePtr;
            }

            ~AsyncResponse()
            {
                Dispose();
            }

            public Result Get(ref string outJson)
            {
                int size = 0;
                Result result = AsyncResponse.GetSize(_asyncResponse, ref size);
                if(!result.IsSuccess())
                {
                    return result;
                }

                byte[] data = new byte[size + 1];
                result = AsyncResponse.Get(_asyncResponse, data, size);
                outJson = System.Text.Encoding.UTF8.GetString(data);

                return result;
            }

            public void Wait()
            {
                AsyncResponse.Wait(_asyncResponse);
            }

            public bool TryWait()
            {
                return AsyncResponse.TryWait(_asyncResponse);
            }

            public void Cancel()
            {
                AsyncResponse.Cancel(_asyncResponse);
            }

            public nn.err.ErrorCode GetErrorCode()
            {
                return AsyncResponse.GetErrorCode(_asyncResponse);
            }

            public void Dispose()
            {
                if (isDisposed) { return; }

                if (_asyncResponse != IntPtr.Zero)
                {
                    AsyncResponse.Delete(_asyncResponse);
                    _asyncResponse = IntPtr.Zero;
                }

                GC.SuppressFinalize(this);
                isDisposed = true;
            }

            [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncResponse_GetSize")]
            private static extern Result GetSize(
                IntPtr asyncResponse,
                ref int outSize);

            [DllImport(Nn.DllName,
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "nn_ec_AsyncResponse_Get")]
            private static extern Result Get(
                IntPtr asyncResponse,
                byte[] outReceivedData,
                int bufferCapacity);

            [DllImport(Nn.DllName,
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "nn_ec_AsyncResponse_Wait")]
            private static extern void Wait(
                IntPtr asyncResponce);

            [DllImport(Nn.DllName,
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "nn_ec_AsyncResponse_TryWait")]
            [return: MarshalAs(UnmanagedType.U1)]
            private static extern bool TryWait(
                 IntPtr asyncResponce);

            [DllImport(Nn.DllName,
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "nn_ec_AsyncResponse_Cancel")]
            private static extern void Cancel(
                IntPtr asyncResponce);

            [DllImport(Nn.DllName,
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "nn_ec_AsyncResponse_GetErrorCode")]
            private static extern nn.err.ErrorCode GetErrorCode(
                 IntPtr asyncResponce);

            [DllImport(Nn.DllName,
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "nn_ec_AsyncResponse_New")]
            private static extern void New(
                ref IntPtr asyncResponce);

            [DllImport(Nn.DllName,
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = "nn_ec_AsyncResponse_Delete")]
            private static extern void Delete(
                IntPtr asyncResponce);
        }
    }
#endif
}
#endif