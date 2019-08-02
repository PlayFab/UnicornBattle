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

namespace nn.irsensor
{
    public static partial class IrCamera
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_irsensor_GetIrCameraHandle")]
        public static extern IrCameraHandle GetHandle(nn.hid.NpadId npadId);

        [DllImport(Nn.DllName,
           CallingConvention = CallingConvention.Cdecl,
           EntryPoint = "nn_irsensor_Initialize")]
        public static extern void Initialize(IrCameraHandle handle);

        [DllImport(Nn.DllName,
           CallingConvention = CallingConvention.Cdecl,
           EntryPoint = "nn_irsensor_Finalize")]
        public static extern void Finalize(IrCameraHandle handle);

        [DllImport(Nn.DllName,
           CallingConvention = CallingConvention.Cdecl,
           EntryPoint = "nn_irsensor_GetIrCameraStatus")]
        public static extern IrCameraStatus GetStatus(IrCameraHandle handle);

        [DllImport(Nn.DllName,
           CallingConvention = CallingConvention.Cdecl,
           EntryPoint = "nn_irsensor_CheckFirmwareUpdateNecessity")]
        public static extern Result CheckFirmwareUpdateNecessity([MarshalAs(UnmanagedType.U1)] ref bool pOutIsUpdateNeeded, IrCameraHandle handle);
    }
}
#endif
