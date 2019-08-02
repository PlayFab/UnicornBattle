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
    public struct SixAxisSensorHandle
    {
        public int _storage;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DirectionState
    {
        public nn.util.Float3 x;
        public nn.util.Float3 y;
        public nn.util.Float3 z;

        public override string ToString()
        {
            return string.Format("X{0} Y{1} Z{2}", this.x, this.y, this.z);
        }
    }

    [Flags]
    public enum SixAxisSensorAttribute
    {
        IsConnected = 0x1 << 0,
        IsInterpolated = 0x1 << 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SixAxisSensorState
    {
        public const float AccelerometerMax = 7.0f;
        public const float AngularVelocityMax = 5.0f;

        public long deltaTimeNanoSeconds;
        public long samplingNumber;
        public nn.util.Float3 acceleration;
        public nn.util.Float3 angularVelocity;
        public nn.util.Float3 angle;
        public DirectionState direction;
        public SixAxisSensorAttribute attributes;

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6}",
                this.acceleration, this.angularVelocity, this.angle, this.direction,
                this.attributes, this.deltaTimeNanoSeconds, this.samplingNumber);
        }

        public void GetQuaternion(ref float x, ref float y, ref float z, ref float w)
        {
            SixAxisSensorState.GetQuaternion(ref this, ref x, ref y, ref z, ref w);
        }

        public void GetQuaternion(ref nn.util.Float4 quaternion)
        {
            SixAxisSensorState.GetQuaternion(ref this, ref quaternion.x, ref quaternion.y, ref quaternion.z, ref quaternion.w);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetSixAxisSensorStateQuaternion")]
        private static extern void GetQuaternion(ref SixAxisSensorState state,
            ref float pOutX, ref float pOutY, ref float pOutZ, ref float pOutW);
    }

    public enum GyroscopeZeroDriftMode
    {
        Loose,
        Standard,
        Tight,
    }

    public static class SixAxisSensor
    {
        public const int StateCountMax = 16;
        public const int HandleCountMax = 8;

        // hid_NpadSixAxisSensor.h
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetSixAxisSensorHandles")]
        public static extern int GetHandles(SixAxisSensorHandle[] pOutValues, int count, NpadId npadId, NpadStyle npadStyle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_StartSixAxisSensor")]
        public static extern void Start(SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_StopSixAxisSensor")]
        public static extern void Stop(SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_IsSixAxisSensorAtRest")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsRest(SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetSixAxisSensorState")]
        public static extern void GetState(ref SixAxisSensorState pOutValue, SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetSixAxisSensorStates")]
        public static extern int GetStates([Out] SixAxisSensorState[] pOutValues, int count, SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_IsSixAxisSensorFusionEnabled")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsFusionEnabled(SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_EnableSixAxisSensorFusion")]
        public static extern void EnableFusion(SixAxisSensorHandle handle, [MarshalAs(UnmanagedType.U1)] bool enable);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetSixAxisSensorFusionParameters")]
        public static extern void SetFusionParameters(SixAxisSensorHandle handle, float revisePower, float reviseRange);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetSixAxisSensorFusionParameters")]
        public static extern void GetFusionParameters(
            ref float pOutRevisePower, ref float pOutReviseRange, SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ResetSixAxisSensorFusionParameters")]
        public static extern void ResetFusionParameters(SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetGyroscopeZeroDriftMode")]
        public static extern void SetGyroscopeZeroDriftMode(SixAxisSensorHandle handle, GyroscopeZeroDriftMode mode);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetGyroscopeZeroDriftMode")]
        public static extern GyroscopeZeroDriftMode GetGyroscopeZeroDriftMode(SixAxisSensorHandle handle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_IsFirmwareUpdateAvailableForSixAxisSensor")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsFirmwareUpdateAvailableForSixAxisSensor(SixAxisSensorHandle handle);
    }
}
#endif
