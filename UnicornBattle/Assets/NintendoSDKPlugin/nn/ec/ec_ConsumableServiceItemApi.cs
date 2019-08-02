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
#if !UNITY_SWITCH || UNITY_EDITOR
    //----------------------------------------------------------------------------
    // ConsumableServiceItemManager
    //----------------------------------------------------------------------------
    public sealed class ConsumableServiceItemManager : IDisposable
    {
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public Result Initialize(ShopServiceAccessor accessor, account.Uid user) { return new Result(); }
        public Result Initialize(ShopServiceAccessor accessor, account.Uid user, byte[] buffer) { return new Result(); }
        public bool IsInitialized() { return false; }
        public bool CanQueryNewData() { return false; }
        public bool IsConsumptionRequired() { return true; }
        public bool IsRecoveryRequired() { return true; }

        public void SetupAsyncRequest(AsyncGetConsumableRightDataRequest outRequest) { }
        public void SetupAsyncRequest(AsyncConsumeRightDataRequest outRequest) { }
        public void SetupAsyncRequest(AsyncRecoverRightDataRequest outRequest) { }

        public int GetRequiredBufferSizeToExportSerializedRightData() { return 0; }
        public int ExportSerializedRightData(byte[] buffer)
        {
            buffer = new byte[0];
            return 0;
        }
        public byte[] ExportSerializedRightData()
        {
            return new byte[0];
        }

        public int GetProvidableItemIdCount() { return 0; }
        public Result GetProvidableItemIds(ref int outItemIdCount, ref ConsumableServiceItemId[] outItemIdArray) { return new Result(); }
        public Result GetProvidableItemIds(out ConsumableServiceItemId[] outItemIdArray)
        {
            outItemIdArray = new ConsumableServiceItemId[0];
            return new Result();
        }

        public Result MarkServiceProvided(ConsumableServiceItemId itemId) { return new Result(); }
        public Result MarkServiceProvided(ref ulong outSeed, ConsumableServiceItemId itemId) { return new Result(); }
    }

    //----------------------------------------------------------------------------
    // ConsumableServiceItemAsyncRequestBase
    //----------------------------------------------------------------------------
    public abstract class ConsumableServiceItemAsyncRequestBase : IDisposable
    {
        public ConsumableServiceItemAsyncRequestBase() { }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Cancel() { }
        public void Wait() { }
        public bool TryWait() { return false; }

        public abstract Result Begin();
        public abstract Result End();

        public nn.err.ErrorCode GetErrorCode()
        {
            return nn.err.ErrorCode.GetInvalidErrorCode();
        }
    }

    //----------------------------------------------------------------------------
    // AsyncGetConsumableRightDataRequest
    //----------------------------------------------------------------------------
    public sealed class AsyncGetConsumableRightDataRequest : ConsumableServiceItemAsyncRequestBase
    {
        public override Result Begin() { return new Result(); }
        public override Result End() { return new Result(); }
    }

    //----------------------------------------------------------------------------
    // AsyncConsumeRightDataRequest
    //----------------------------------------------------------------------------
    public sealed class AsyncConsumeRightDataRequest : ConsumableServiceItemAsyncRequestBase
    {
        public override Result Begin() { return new Result(); }
        public override Result End() { return new Result(); }
    }

    //----------------------------------------------------------------------------
    // AsyncRecoverRightDataRequest
    //----------------------------------------------------------------------------
    public sealed class AsyncRecoverRightDataRequest : ConsumableServiceItemAsyncRequestBase
    {
        public override Result Begin() { return new Result(); }
        public override Result End() { return new Result(); }
    }

#else
    //----------------------------------------------------------------------------
    // ConsumableServiceItemManager
    //----------------------------------------------------------------------------
    public sealed class ConsumableServiceItemManager : IDisposable
    {
        private IntPtr _consumableServiceItemManager = IntPtr.Zero;
        private IntPtr _workMemory = IntPtr.Zero;
        private bool isDisposed = false;

        public ConsumableServiceItemManager()
        {
            ConsumableServiceItemManager.New(ref _consumableServiceItemManager);
        }

        ~ConsumableServiceItemManager()
        {
            _Dispose();
        }
        
        public Result Initialize(ShopServiceAccessor accessor, account.Uid user)
        {
            return ConsumableServiceItemManager.Initialize(_consumableServiceItemManager, ref _workMemory, accessor.Ptr, user);
        }

        public Result Initialize(ShopServiceAccessor accessor, account.Uid user, byte[] buffer)
        {
            return ConsumableServiceItemManager.Initialize(_consumableServiceItemManager, ref _workMemory, accessor.Ptr, user, buffer, buffer.LongLength);
        }

        public bool IsInitialized()
        {
            return ConsumableServiceItemManager.IsInitialized(_consumableServiceItemManager);
        }

        public bool CanQueryNewData()
        {
            return ConsumableServiceItemManager.CanQueryNewData(_consumableServiceItemManager);
        }

        public bool IsConsumptionRequired()
        {
            return ConsumableServiceItemManager.IsConsumptionRequired(_consumableServiceItemManager);
        }

        public bool IsRecoveryRequired()
        {
            return ConsumableServiceItemManager.IsRecoveryRequired(_consumableServiceItemManager);
        }

        public void SetupAsyncRequest(AsyncGetConsumableRightDataRequest outRequest)
        {
            IntPtr asyncRequest = outRequest.Ptr;
            ConsumableServiceItemManager.SetupAsyncGetConsumableRightDataRequest(_consumableServiceItemManager, ref asyncRequest);
        }

        public void SetupAsyncRequest(AsyncConsumeRightDataRequest outRequest)
        {
            IntPtr asyncRequest = outRequest.Ptr;
            ConsumableServiceItemManager.SetupAsyncConsumeRightDataRequest(_consumableServiceItemManager, ref asyncRequest);
        }

        public void SetupAsyncRequest(AsyncRecoverRightDataRequest outRequest)
        {
            IntPtr asyncRequest = outRequest.Ptr;
            ConsumableServiceItemManager.SetupAsyncRecoverRightDataRequest(_consumableServiceItemManager, ref asyncRequest);
        }

        public long GetRequiredBufferSizeToExportSerializedRightData()
        {
            return ConsumableServiceItemManager.GetRequiredBufferSizeToExportSerializedRightData(_consumableServiceItemManager);
        }

        public long ExportSerializedRightData(byte[] buffer)
        {
            return ConsumableServiceItemManager.ExportSerializedRightData(_consumableServiceItemManager, buffer, buffer.LongLength);
        }

        public byte[] ExportSerializedRightData()
        {
            long bufferSize = ConsumableServiceItemManager.GetRequiredBufferSizeToExportSerializedRightData(_consumableServiceItemManager);
            byte[] buffer = new byte[bufferSize];

            ConsumableServiceItemManager.ExportSerializedRightData(_consumableServiceItemManager, buffer, bufferSize);
            return buffer;
        }

        public int GetProvidableItemIdCount()
        {
            return ConsumableServiceItemManager.GetProvidableItemIdCount(_consumableServiceItemManager);
        }

        public Result GetProvidableItemIds(ref int outItemIdCount, ref ConsumableServiceItemId[] outItemIdArray)
        {
            return ConsumableServiceItemManager.GetProvidableItemIds(
                _consumableServiceItemManager,
                ref outItemIdCount,
                outItemIdArray,
                outItemIdArray.Length);
        }

        public Result GetProvidableItemIds(out ConsumableServiceItemId[] outItemIdArray)
        {
            int itemIdArrayCount = GetProvidableItemIdCount();
            outItemIdArray = new ConsumableServiceItemId[itemIdArrayCount];
            int itemIdCount = 0;

            return GetProvidableItemIds(ref itemIdCount, ref outItemIdArray);
        }

        public Result MarkServiceProvided(ConsumableServiceItemId itemId)
        {
            return ConsumableServiceItemManager.MarkServiceProvided(_consumableServiceItemManager, itemId);
        }

        public Result MarkServiceProvided(ref ulong outSeed, ConsumableServiceItemId itemId)
        {
            return ConsumableServiceItemManager.MarkServiceProvided(_consumableServiceItemManager, ref outSeed, itemId);
        }

        public void Dispose()
        {
            _Dispose();
            GC.SuppressFinalize(this);
        }
        private void _Dispose()
        {
            if (isDisposed) { return; }

            if (_consumableServiceItemManager != IntPtr.Zero || _workMemory != IntPtr.Zero)
            {
                Delete(_consumableServiceItemManager, _workMemory);
                _consumableServiceItemManager = IntPtr.Zero;
                _workMemory = IntPtr.Zero;
            }

            isDisposed = true;
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_Initialize1")]
        private static extern Result Initialize(
            IntPtr consumableServiceItemManager,
            ref IntPtr workMemory,
            IntPtr shopServiceAccessor,
            nn.account.Uid user);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_Initialize2")]
        private static extern Result Initialize(
            IntPtr consumableServiceItemManager,
            ref IntPtr workMemory,
            IntPtr shopServiceAccessor,
            nn.account.Uid user,
            byte[] buffer,
            long bufferSize);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_IsInitialized")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IsInitialized(IntPtr consumableServiceItemManager);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_CanQueryNewData")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool CanQueryNewData(IntPtr consumableServiceItemManager);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_IsConsumptionRequired")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IsConsumptionRequired(IntPtr consumableServiceItemManager);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_IsRecoveryRequired")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IsRecoveryRequired(IntPtr consumableServiceItemManager);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_SetupAsyncRequest1")]
        private static extern void SetupAsyncGetConsumableRightDataRequest(
            IntPtr consumableServiceItemManager,
            ref IntPtr asyncGetConsumableRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_SetupAsyncRequest2")]
        private static extern void SetupAsyncConsumeRightDataRequest(
            IntPtr consumableServiceItemManager,
            ref IntPtr asyncConsumeRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_SetupAsyncRequest3")]
        private static extern void SetupAsyncRecoverRightDataRequest(
            IntPtr consumableServiceItemManager,
            ref IntPtr asyncRecoverRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_GetRequiredBufferSizeToExportSerializedRightData")]
        private static extern long GetRequiredBufferSizeToExportSerializedRightData(IntPtr consumableServiceItemManager);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_ExportSerializedRightData")]
        private static extern long ExportSerializedRightData(
            IntPtr consumableServiceItemManager,
            byte[] buffer,
            long bufferSize);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_GetProvidableItemIdCount")]
        private static extern int GetProvidableItemIdCount(IntPtr consumableServiceItemManager);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_GetProvidableItemIds")]
        private static extern Result GetProvidableItemIds(
            IntPtr consumableServiceItemManager,
            ref int outItemIdCount,
            [Out] ConsumableServiceItemId[] outItemIdArray,
            int itemIdArrayCount);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_MarkServiceProvided1")]
        private static extern Result MarkServiceProvided(
            IntPtr consumableServiceItemManager,
            ConsumableServiceItemId itemId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_MarkServiceProvided2")]
        private static extern Result MarkServiceProvided(
            IntPtr consumableServiceItemManager,
            ref ulong outSeed,
            ConsumableServiceItemId itemId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_New")]
        private static extern void New(ref IntPtr consumableServiceItemManager);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemManager_Delete")]
        private static extern void Delete(IntPtr consumableServiceItemManager, IntPtr workBuffer);
    }

    //----------------------------------------------------------------------------
    // ConsumableServiceItemAsyncRequestBase
    //----------------------------------------------------------------------------
    public abstract class ConsumableServiceItemAsyncRequestBase : IDisposable
    {
        protected IntPtr _consumableServiceItemAsyncRequestBase = IntPtr.Zero;
        protected bool isDisposed = false;

        public ConsumableServiceItemAsyncRequestBase() { }

        internal IntPtr Ptr
        {
            get { return _consumableServiceItemAsyncRequestBase; }
        }

        ~ConsumableServiceItemAsyncRequestBase()
        {
            _Dispose();
        }

        public void Cancel()
        {
            if (_consumableServiceItemAsyncRequestBase == IntPtr.Zero) { return; }
            ConsumableServiceItemAsyncRequestBase.Cancel(_consumableServiceItemAsyncRequestBase);
        }

        public void Wait()
        {
            if (_consumableServiceItemAsyncRequestBase == IntPtr.Zero) { return; }
            ConsumableServiceItemAsyncRequestBase.Wait(_consumableServiceItemAsyncRequestBase);
        }

        public bool TryWait()
        {
            if (_consumableServiceItemAsyncRequestBase == IntPtr.Zero) { return false; }
            return ConsumableServiceItemAsyncRequestBase.TryWait(_consumableServiceItemAsyncRequestBase);
        }

        public nn.err.ErrorCode GetErrorCode()
        {
            if (_consumableServiceItemAsyncRequestBase == IntPtr.Zero) { return nn.err.ErrorCode.GetInvalidErrorCode(); }
            return ConsumableServiceItemAsyncRequestBase.GetErrorCode(_consumableServiceItemAsyncRequestBase);
        }

        public abstract Result Begin();

        public abstract Result End();

        public void Dispose()
        {
            _Dispose();
            GC.SuppressFinalize(this);
        }

        private void _Dispose()
        {
            if (isDisposed) { return; }

            Free(_consumableServiceItemAsyncRequestBase);

            isDisposed = true;
        }

        protected abstract void Free(IntPtr asyncRequestPtr);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemAsyncRequestBase_Cancel")]
        private static extern void Cancel(IntPtr consumableServiceItemAsyncRequestBase);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemAsyncRequestBase_Wait")]
        private static extern void Wait(IntPtr consumableServiceItemAsyncRequestBase);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemAsyncRequestBase_TryWait")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool TryWait(IntPtr consumableServiceItemAsyncRequestBase);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ConsumableServiceItemAsyncRequestBase_GetErrorCode")]
        private static extern nn.err.ErrorCode GetErrorCode(IntPtr consumableServiceItemAsyncRequestBase);
    }

    //----------------------------------------------------------------------------
    // AsyncGetConsumableRightDataRequest
    //----------------------------------------------------------------------------
    public sealed class AsyncGetConsumableRightDataRequest : ConsumableServiceItemAsyncRequestBase
    {
        public AsyncGetConsumableRightDataRequest()
        {
            AsyncGetConsumableRightDataRequest.New(ref _consumableServiceItemAsyncRequestBase);
        }

        protected override void Free(IntPtr asyncRequestPtr)
        {
            AsyncGetConsumableRightDataRequest.Delete(asyncRequestPtr);
        }

        public override Result Begin()
        {
            return AsyncGetConsumableRightDataRequest.Begin(_consumableServiceItemAsyncRequestBase);
        }

        public override Result End()
        {
            return AsyncGetConsumableRightDataRequest.End(_consumableServiceItemAsyncRequestBase);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncGetConsumableRightDataRequest_Begin")]
        private static extern Result Begin(IntPtr asyncGetConsumableRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncGetConsumableRightDataRequest_End")]
        private static extern Result End(IntPtr asyncGetConsumableRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncGetConsumableRightDataRequest_New")]
        private static extern void New(ref IntPtr asyncGetConsumableRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncGetConsumableRightDataRequest_Delete")]
        private static extern void Delete(IntPtr asyncGetConsumableRightDataRequest);
    }

    //----------------------------------------------------------------------------
    // AsyncConsumeRightDataRequest
    //----------------------------------------------------------------------------
    public sealed class AsyncConsumeRightDataRequest : ConsumableServiceItemAsyncRequestBase
    {
        public AsyncConsumeRightDataRequest()
        {
            AsyncConsumeRightDataRequest.New(ref _consumableServiceItemAsyncRequestBase);
        }

        protected override void Free(IntPtr asyncRequestPtr)
        {
            AsyncConsumeRightDataRequest.Delete(asyncRequestPtr);
        }

        public override Result Begin()
        {
            return AsyncConsumeRightDataRequest.Begin(_consumableServiceItemAsyncRequestBase);
        }

        public override Result End()
        {
            return AsyncConsumeRightDataRequest.End(_consumableServiceItemAsyncRequestBase);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncConsumeRightDataRequest_Begin")]
        private static extern Result Begin(IntPtr asyncConsumeRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncConsumeRightDataRequest_End")]
        private static extern Result End(IntPtr asyncConsumeRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncConsumeRightDataRequest_New")]
        private static extern void New(ref IntPtr asyncConsumeRightDataRequest);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncConsumeRightDataRequest_Delete")]
        private static extern void Delete(IntPtr asyncConsumeRightDataRequest);
    }

    //----------------------------------------------------------------------------
    // AsyncRecoverRightDataRequest
    //----------------------------------------------------------------------------
    public sealed class AsyncRecoverRightDataRequest : ConsumableServiceItemAsyncRequestBase
    {
        public AsyncRecoverRightDataRequest()
        {
            AsyncRecoverRightDataRequest.New(ref _consumableServiceItemAsyncRequestBase);
        }

        protected override void Free(IntPtr asyncRequestPtr)
        {
            AsyncRecoverRightDataRequest.Delete(asyncRequestPtr);
        }

        public override Result Begin()
        {
            return AsyncRecoverRightDataRequest.Begin(_consumableServiceItemAsyncRequestBase);
        }

        public override Result End()
        {
            return AsyncRecoverRightDataRequest.End(_consumableServiceItemAsyncRequestBase);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncGetConsumableRightDataRequest_Begin")]
        private static extern Result Begin(IntPtr asyncGetConsumableRightDataRequestBase);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncGetConsumableRightDataRequest_End")]
        private static extern Result End(IntPtr asyncGetConsumableRightDataRequestBase);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncConsumeRightDataRequest_New")]
        private static extern Result New(ref IntPtr asyncGetConsumableRightDataRequestBase);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_AsyncConsumeRightDataRequest_Delete")]
        private static extern Result Delete(IntPtr asyncGetConsumableRightDataRequestBase);
    }

#endif
}
#endif