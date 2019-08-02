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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace nn.hid
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerSupportArg
    {
        private const int ExplainTextSize = ControllerSupport.ExplainTextMaxBufferSize * ControllerSupport.ControllerSupportPlayerCountMax;

        public byte playerCountMin;
        public byte playerCountMax;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableTakeOverConnection;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableLeftJustify;
        [MarshalAs(UnmanagedType.U1)]
        public bool enablePermitJoyDual;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableSingleMode;
        [MarshalAs(UnmanagedType.U1)]
        public bool enableIdentificationColor;
        public Color4u8Array8 identificationColor;
        [MarshalAs(UnmanagedType.I1)]
        public bool enableExplainText;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = ExplainTextSize)]
        private byte[] explainText;

        public void SetDefault()
        {
            this.playerCountMin = 0;
            this.playerCountMax = 4;
            this.enableTakeOverConnection = true;
            this.enableLeftJustify = true;
            this.enablePermitJoyDual = true;
            this.enableSingleMode = false;
            this.enableIdentificationColor = false;
            this.identificationColor = new Color4u8Array8();
            this.enableExplainText = false;
            this.explainText = new byte[ExplainTextSize];
        }

        public override string ToString()
        {
            return string.Format("Min{0} Max{1} TOC{2} LJ{3} PJD{4} SM{5} IC{6} C0{7} C1{8} C2{9} C3{10} C4{11} C5{12} C6{13} C7{14} ET{15}",
                this.playerCountMin, this.playerCountMax, this.enableTakeOverConnection, this.enableLeftJustify,
                this.enablePermitJoyDual, this.enableSingleMode, this.enableIdentificationColor,
                this.identificationColor[0], this.identificationColor[1],
                this.identificationColor[2], this.identificationColor[3],
                this.identificationColor[4], this.identificationColor[5],
                this.identificationColor[6], this.identificationColor[7],
                this.enableExplainText);
        }

        #region nn.util.Color4u8Array8
        [StructLayout(LayoutKind.Sequential)]
        public struct Color4u8Array8 : IList<nn.util.Color4u8>, ICollection<nn.util.Color4u8>, IEnumerable<nn.util.Color4u8>
        {
            private const int _Length = 8;
            public int Length { get { return _Length; } }

            private nn.util.Color4u8 _value0;
            private nn.util.Color4u8 _value1;
            private nn.util.Color4u8 _value2;
            private nn.util.Color4u8 _value3;
            private nn.util.Color4u8 _value4;
            private nn.util.Color4u8 _value5;
            private nn.util.Color4u8 _value6;
            private nn.util.Color4u8 _value7;

            public nn.util.Color4u8 this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return _value0;
                        case 1: return _value1;
                        case 2: return _value2;
                        case 3: return _value3;
                        case 4: return _value4;
                        case 5: return _value5;
                        case 6: return _value6;
                        case 7: return _value7;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            _value0 = value;
                            break;
                        case 1:
                            _value1 = value;
                            break;
                        case 2:
                            _value2 = value;
                            break;
                        case 3:
                            _value3 = value;
                            break;
                        case 4:
                            _value4 = value;
                            break;
                        case 5:
                            _value5 = value;
                            break;
                        case 6:
                            _value6 = value;
                            break;
                        case 7:
                            _value7 = value;
                            break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(nn.util.Color4u8 item)
            {
                for (int i = 0; i < Length; i++)
                {
                    if (this[i] == item)
                    {
                        return true;
                    }
                }
                return false;
            }

            public int IndexOf(nn.util.Color4u8 item)
            {
                for (int i = 0; i < Length; i++)
                {
                    if (this[i] == item)
                    {
                        return i;
                    }
                }
                return -1;
            }
            public void CopyTo(nn.util.Color4u8[] array, int arrayIndex)
            {
                if (array == null) { throw new ArgumentNullException(); }
                if (arrayIndex < 0) { throw new ArgumentOutOfRangeException(); }
                if (arrayIndex + Length < array.Length) { throw new ArgumentException(); }
                for (int i = 0; i < Length; i++)
                {
                    array[arrayIndex + i] = this[i];
                }
            }

            public override string ToString()
            {
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7);
            }

            public IEnumerator<nn.util.Color4u8> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
                yield return _value2;
                yield return _value3;
                yield return _value4;
                yield return _value5;
                yield return _value6;
                yield return _value7;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(nn.util.Color4u8 item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, nn.util.Color4u8 item) { throw new NotSupportedException(); }
            public bool Remove(nn.util.Color4u8 item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerFirmwareUpdateArg
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool enableForceUpdate;

        private byte _padding0; 
        private byte _padding1; 
        private byte _padding2;

        public void SetDefault()
        {
            this.enableForceUpdate = false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ControllerSupportResultInfo
    {
        public byte playerCount;
        public NpadId selectedId;

        private byte _padding0;
        private byte _padding1;
        private byte _padding2;

        public override string ToString()
        {
            return string.Format("{0} {1}", this.playerCount, this.selectedId);
        }
    }

    public static partial class ControllerSupport
    {
        public const int ExplainTextMaxLength = 32;
        public const int Utf8ByteSize = 4;
        public const int ExplainTextMaxBufferSize = ExplainTextMaxLength * Utf8ByteSize + 1;
        public const int ControllerSupportPlayerCountMax = 8;

#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result Show(ControllerSupportArg showControllerSupportArg)
        {
            return new Result();
        }

        public static Result Show(ref ControllerSupportResultInfo pOutValue, ControllerSupportArg showControllerSupportArg)
        {
            return new Result();
        }

        public static void SetExplainText(ref ControllerSupportArg pOutControllerSupportArg, string pStr, NpadId npadId)
        {
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ShowControllerSupport")]
        public static extern Result Show(ControllerSupportArg showControllerSupportArg);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ShowControllerSupport2")]
        public static extern Result Show(ref ControllerSupportResultInfo pOutValue, ControllerSupportArg showControllerSupportArg);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_SetExplainText")]
        public static extern void SetExplainText(ref ControllerSupportArg pOutControllerSupportArg, string pStr, NpadId npadId);
#endif

        public static Result Show(ControllerSupportArg showControllerSupportArg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = Show(showControllerSupportArg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return Show(showControllerSupportArg);
        }

        public static Result Show(
            ref ControllerSupportResultInfo pOutValue, ControllerSupportArg showControllerSupportArg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = Show(ref pOutValue, showControllerSupportArg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return Show(ref pOutValue, showControllerSupportArg);
        }
    }

    public static class ControllerStrapGuide
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result Show()
        {
            return new Result();
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ShowControllerStrapGuide")]
        public static extern Result Show();
#endif

        public static Result Show(bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = Show();
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return Show();
        }
    }

    public static partial class ControllerFirmwareUpdate
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result Show(ControllerFirmwareUpdateArg showControllerFirmwareUpdateArg)
        {
            return new Result();
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ShowControllerFirmwareUpdate")]
        public static extern Result Show(ControllerFirmwareUpdateArg showControllerFirmwareUpdateArg);
#endif

        public static Result Show(ControllerFirmwareUpdateArg showControllerFirmwareUpdateArg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = Show(showControllerFirmwareUpdateArg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return Show(showControllerFirmwareUpdateArg);
        }
    }
}
#endif
