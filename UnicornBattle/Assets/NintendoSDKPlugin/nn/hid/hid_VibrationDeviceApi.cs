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

namespace nn.hid
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VibrationDeviceHandle
    {
        public uint _storage;
    }

    public enum VibrationDeviceType
    {
        Unknown,
        LinearResonantActuator,
    }

    public enum VibrationDevicePosition
    {
        None,
        Left,
        Right,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VibrationDeviceInfo
    {
        public VibrationDeviceType deviceType;
        public VibrationDevicePosition position;

        public override string ToString()
        {
            return string.Format("{0} {1}", deviceType, position);
        }
    }

    public static class Vibration
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetVibrationDeviceHandles")]
        public static extern int GetDeviceHandles(
            VibrationDeviceHandle[] pOutValues, int count, NpadId npadId, NpadStyle npadStyle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetVibrationDeviceInfo")]
        public static extern void GetDeviceInfo(
            ref VibrationDeviceInfo pOutValue, VibrationDeviceHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_InitializeVibrationDevice")]
        public static extern void InitializeDevice(VibrationDeviceHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SendVibrationValue")]
        public static extern void SendValue(VibrationDeviceHandle handle, VibrationValue value);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetActualVibrationValue")]
        public static extern void GetActualValue(ref VibrationValue pOutValue, VibrationDeviceHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_IsVibrationPermitted")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsPermitted();
    }
}
#endif
