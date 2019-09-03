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
    [Flags]
    public enum DebugPadButton
    {
        A = 0x1 << 0,
        B = 0x1 << 1,
        X = 0x1 << 2,
        Y = 0x1 << 3,
        L = 0x1 << 4,
        R = 0x1 << 5,
        ZL = 0x1 << 6,
        ZR = 0x1 << 7,
        Start = 0x1 << 8,
        Select = 0x1 << 9,
        Left = 0x1 << 10,
        Up = 0x1 << 11,
        Right = 0x1 << 12,
        Down = 0x1 << 13,
    }

    [Flags]
    public enum DebugPadAttribute
    {
        IsConnected = 0x1 << 0,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DebugPadState
    {
        public long samplingNumber;
        public DebugPadAttribute attributes;
        public DebugPadButton buttons;
        public AnalogStickState analogStickR;
        public AnalogStickState analogStickL;

        public override string ToString()
        {
            return string.Format("L{0} R{1} [{2}] {3} {4}",
                this.analogStickL, this.analogStickR, this.buttons, this.attributes, this.samplingNumber);
        }
    }

    public static class DebugPad
    {
#if DEVELOPMENT_BUILD || NN_HID_DEBUG_PAD_ENABLE
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_InitializeDebugPad")]
        public static extern void Initialize();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetDebugPadState")]
        public static extern void GetState(ref DebugPadState pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetDebugPadStates")]
        public static extern int GetStates([Out] DebugPadState[] pOutValues, int count);
#else
        public static void Initialize()
        {
        }
        public static void GetState(ref DebugPadState pOutValue)
        {
        }
        public static int GetStates([Out] DebugPadState[] pOutValues, int count)
        {
            return 0;
        }
#endif
        public const int StateCountMax = 16;
    }
}
#endif
