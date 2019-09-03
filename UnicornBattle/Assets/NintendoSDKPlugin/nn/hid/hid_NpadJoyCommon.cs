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

namespace nn.hid
{
    // hid_NpadJoyCommon.h

    public enum NpadJoyAssignmentMode
    {
        Dual,
        Single,
    }

    public enum NpadHandheldActivationMode
    {
        Dual,
        Single,
        None,
    }

    public enum NpadJoyDeviceType
    {
        Left,
        Right,
    }

    public enum NpadJoyHoldType
    {
        Vertical,
        Horizontal,
    }

    public enum NpadCommunicationMode
    {
        Mode5ms,
        Mode10ms,
        Mode15ms,
        ModeDefault,
    }

    public static partial class NpadJoy
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetNpadJoyAssignment")]
        public static extern NpadJoyAssignmentMode GetAssignment(NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetNpadJoyAssignmentModeSingle")]
        public static extern void SetAssignmentModeSingle(NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetNpadJoyAssignmentModeSingle2")]
        public static extern void SetAssignmentModeSingle(NpadId npadId, NpadJoyDeviceType deviceType);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetNpadJoyAssignmentModeSingle3")]
        public static extern void SetAssignmentModeSingle(ref NpadId pOutValue, NpadId npadId, NpadJoyDeviceType deviceType);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetNpadJoyAssignmentModeDual")]
        public static extern void SetAssignmentModeDual(NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_MergeSingleJoyAsDualJoy")]
        public static extern Result MergeSingleAsDual(NpadId npadId1, NpadId npadId2);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SwapNpadAssignment")]
        public static extern void SwapAssignment(NpadId npadId1, NpadId npadId2);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetNpadJoyHoldType")]
        public static extern void SetHoldType(NpadJoyHoldType holdType);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetNpadJoyHoldType")]
        public static extern NpadJoyHoldType GetHoldType();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_StartLrAssignmentMode")]
        public static extern void StartLrAssignmentMode();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_StopLrAssignmentMode")]
        public static extern void StopLrAssignmentMode();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetNpadHandheldActivationMode")]
        public static extern void SetHandheldActivationMode(NpadHandheldActivationMode activationMode);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetNpadHandheldActivationMode")]
        public static extern NpadHandheldActivationMode GetHandheldActivationMode();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetNpadCommunicationMode")]
        public static extern void SetCommunicationMode(NpadCommunicationMode mode);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetNpadCommunicationMode")]
        public static extern NpadCommunicationMode GetCommunicationMode();
    }
}
#endif
