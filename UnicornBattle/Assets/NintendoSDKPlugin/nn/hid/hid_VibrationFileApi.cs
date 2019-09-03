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
    public struct VibrationFileInfo
    {
        public uint metaDataSize;
        public ushort formatId;
        public ushort samplingRate;
        public uint dataSize;
        public int sampleLength;
        public int isLoop;
        public uint loopStartPosition;
        public uint loopEndPosition;
        public uint loopInterval;

        public override string ToString()
        {
            return string.Format("({0} {1}) SamplingRate:{2} DataSize:{3} SampleLength:{4} Loop:{5}({6} - {7}, {8})",
                this.metaDataSize, this.formatId, this.samplingRate, this.dataSize, this.sampleLength,
                this.isLoop, this.loopStartPosition, this.loopEndPosition, this.loopInterval);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VibrationValueArrayInfo
    {
        public int sampleLength;
        [MarshalAs(UnmanagedType.U1)]
        public bool isLoop;
        public uint loopStartPosition;
        public uint loopEndPosition;
        public uint loopInterval;

        public override string ToString()
        {
            return string.Format("SampleLength:{0} Loop:{1}({2} - {3}, {4})",
                this.sampleLength, this.isLoop, this.loopStartPosition, this.loopEndPosition, this.loopInterval);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VibrationFileParserContext
    {
        public IntPtrArray10 _storage;

        #region IntPtrArray10
        [StructLayout(LayoutKind.Sequential)]
        public struct IntPtrArray10 : IList<IntPtr>, ICollection<IntPtr>, IEnumerable<IntPtr>
        {
            private const int _Length = 10;
            public int Length { get { return _Length; } }

            private IntPtr _value0;
            private IntPtr _value1;
            private IntPtr _value2;
            private IntPtr _value3;
            private IntPtr _value4;
            private IntPtr _value5;
            private IntPtr _value6;
            private IntPtr _value7;
            private IntPtr _value8;
            private IntPtr _value9;

            public IntPtr this[int index]
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
                        case 8: return _value8;
                        case 9: return _value9;
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
                        case 8:
                            _value8 = value;
                            break;
                        case 9:
                            _value9 = value;
                            break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(IntPtr item)
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

            public int IndexOf(IntPtr item)
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
            public void CopyTo(IntPtr[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9);
            }

            public IEnumerator<IntPtr> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
                yield return _value2;
                yield return _value3;
                yield return _value4;
                yield return _value5;
                yield return _value6;
                yield return _value7;
                yield return _value8;
                yield return _value9;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(IntPtr item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, IntPtr item) { throw new NotSupportedException(); }
            public bool Remove(IntPtr item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    public static partial class VibrationFile
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_ParseVibrationFile")]
        public static extern Result Parse(
            ref VibrationFileInfo pOutInfo, ref VibrationFileParserContext pOutContext, byte[] address, long fileSize);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_RetrieveVibrationValue")]
        public static extern void RetrieveValue(
            ref VibrationValue pOutValue, int position, ref VibrationFileParserContext pContext);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GenerateVibrationFile")]
        private static extern void Generate(
            ref long pOutSize, byte[] outBuffer, long bufferSize,
            VibrationValueArrayInfo info, VibrationValue[] pValues);

        public static void Generate(ref long pOutSize, byte[] outBuffer,
            VibrationValueArrayInfo info, VibrationValue[] pValues)
        {
            VibrationFile.Generate(ref pOutSize, outBuffer, outBuffer.LongLength, info, pValues);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_CalculateVibrationFileSize")]
        public static extern long CalculateSize(VibrationValueArrayInfo info);
    }
}
#endif
