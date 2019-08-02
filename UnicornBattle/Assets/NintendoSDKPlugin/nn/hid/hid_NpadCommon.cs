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
    public enum NpadStyle
    {
        None,
        FullKey = 0x1 << 0,
        Handheld = 0x1 << 1,
        JoyDual = 0x1 << 2,
        JoyLeft = 0x1 << 3,
        JoyRight = 0x1 << 4,
        Invalid = 0x1 << 5,
    }

    // hid_NpadCommonTypes.h

    public enum NpadId
    {
        No1 = 0x00,
        No2 = 0x01,
        No3 = 0x02,
        No4 = 0x03,
        No5 = 0x04,
        No6 = 0x05,
        No7 = 0x06,
        No8 = 0x07,
        Handheld = 0x20,
        Invalid = 0x40,
    }

    [Flags]
    public enum NpadButton : long
    {
        None,
        A = 0x1 << 0,
        B = 0x1 << 1,
        X = 0x1 << 2,
        Y = 0x1 << 3,
        StickL = 0x1 << 4,
        StickR = 0x1 << 5,
        L = 0x1 << 6,
        R = 0x1 << 7,
        ZL = 0x1 << 8,
        ZR = 0x1 << 9,
        Plus = 0x1 << 10,
        Minus = 0x1 << 11,
        Left = 0x1 << 12,
        Up = 0x1 << 13,
        Right = 0x1 << 14,
        Down = 0x1 << 15,
        StickLLeft = 0x1 << 16,
        StickLUp = 0x1 << 17,
        StickLRight = 0x1 << 18,
        StickLDown = 0x1 << 19,
        StickRLeft = 0x1 << 20,
        StickRUp = 0x1 << 21,
        StickRRight = 0x1 << 22,
        StickRDown = 0x1 << 23,
        LeftSL = 0x1 << 24,
        LeftSR = 0x1 << 25,
        RightSL = 0x1 << 26,
        RightSR = 0x1 << 27,
    }

    [Flags]
    public enum NpadAttribute
    {
        None,
        IsConnected = 0x1 << 0,
        IsWired = 0x1 << 1,
        IsLeftConnected = 0x1 << 2,
        IsLeftWired = 0x1 << 3,
        IsRightConnected = 0x1 << 4,
        IsRightWired = 0x1 << 5,
    }

    // hid_NpadColor.h

    [StructLayout(LayoutKind.Sequential)]
    public struct NpadControllerColor
    {
        public nn.util.Color4u8 main;
        public nn.util.Color4u8 sub;

        public override string ToString()
        {
            return string.Format("main{0} sub{1}", this.main, this.sub);
        }
    }

    public static partial class Npad
    {
        // hid_NpadCommon.h

        public const int StateCountMax = 16;

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_InitializeNpad")]
        public static extern void Initialize();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetSupportedNpadStyleSet")]
        public static extern void SetSupportedStyleSet(NpadStyle npadStyle);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetSupportedNpadStyleSet")]
        public static extern NpadStyle GetSupportedStyleSet();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetSupportedNpadIdType")]
        public static extern void SetSupportedIdType(NpadId[] npadIds, long count);

        public static void SetSupportedIdType(NpadId[] npadIds)
        {
            SetSupportedIdType(npadIds, npadIds.LongLength);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_BindNpadStyleSetUpdateEvent")]
        public static extern void BindStyleSetUpdateEvent(NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_IsNpadStyleSetUpdated")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsStyleSetUpdated(NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_DestroyNpadStyleSetUpdateEvent")]
        public static extern void DestroyStyleSetUpdateEvent(NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetNpadStyleSet")]
        public static extern NpadStyle GetStyleSet(NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_DisconnectNpad")]
        public static extern void Disconnect(NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetPlayerLedPattern")]
        public static extern byte GetPlayerLedPattern(NpadId npadId);

        // hid_NpadColor.h

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetNpadControllerColor")]
        public static extern Result GetControllerColor(ref NpadControllerColor pOutValue, NpadId npadId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetNpadControllerColor2")]
        public static extern Result GetControllerColor(
            ref NpadControllerColor pOutLeftColor, ref NpadControllerColor pOutRightColor, NpadId npadId);

        // Utility
        public static void GetState(ref NpadState pOutValue, NpadId npadId, NpadStyle npadStyle)
        {
            NpadButton preButtons = pOutValue.buttons;
            switch (npadStyle)
            {
                case NpadStyle.FullKey:
                    NpadFullKey.GetState(ref pOutValue, npadId);
                    break;
                case NpadStyle.Handheld:
                    NpadHandheld.GetState(ref pOutValue, npadId);
                    break;
                case NpadStyle.JoyDual:
                    NpadJoyDual.GetState(ref pOutValue, npadId);
                    break;
                case NpadStyle.JoyLeft:
                    NpadJoyLeft.GetState(ref pOutValue, npadId);
                    break;
                case NpadStyle.JoyRight:
                    NpadJoyRight.GetState(ref pOutValue, npadId);
                    break;
            }
            pOutValue.preButtons = preButtons;
        }

        public static int GetStates(
            [Out] NpadStateArrayItem[] pOutValues, int count, NpadId npadId, NpadStyle npadStyle)
        {
            switch (npadStyle)
            {
                case NpadStyle.FullKey:
                    return NpadFullKey.GetStates(pOutValues, count, npadId);
                case NpadStyle.Handheld:
                    return NpadHandheld.GetStates(pOutValues, count, npadId);
                case NpadStyle.JoyDual:
                    return NpadJoyDual.GetStates(pOutValues, count, npadId);
                case NpadStyle.JoyLeft:
                    return NpadJoyLeft.GetStates(pOutValues, count, npadId);
                case NpadStyle.JoyRight:
                    return NpadJoyRight.GetStates(pOutValues, count, npadId);
            }
            return 0;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NpadState
    {
        public long samplingNumber;
        public NpadButton buttons;
        public AnalogStickState analogStickL;
        public AnalogStickState analogStickR;
        public NpadAttribute attributes;
        public NpadButton preButtons;

        public void Clear()
        {
            this.samplingNumber = 0;
            this.buttons = NpadButton.None;
            this.analogStickL.Clear();
            this.analogStickR.Clear();
            this.attributes = NpadAttribute.None;
            this.preButtons = NpadButton.None;
        }

        public bool GetButton(NpadButton button)
        {
            return (this.buttons & button) != 0;
        }

        public bool GetButtonDown(NpadButton button)
        {
            return ((this.preButtons & button) == 0) && ((this.buttons & button) != 0);
        }

        public bool GetButtonUp(NpadButton button)
        {
            return ((this.preButtons & button) != 0) && ((this.buttons & button) == 0);
        }

        public override string ToString()
        {
            return string.Format("L{0} R{1} [{2}] {3} {4}",
                this.analogStickL, this.analogStickR, this.buttons, this.attributes, this.samplingNumber);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NpadStateArrayItem
    {
        public long samplingNumber;
        public NpadButton buttons;
        public AnalogStickState analogStickL;
        public AnalogStickState analogStickR;
        public NpadAttribute attributes;

        public override string ToString()
        {
            return string.Format("L{0} R{1} [{2}] {3} {4}",
                this.analogStickL, this.analogStickR, this.buttons, this.attributes, this.samplingNumber);
        }
    }
}
#endif
