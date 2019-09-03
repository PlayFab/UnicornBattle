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

namespace nn.irsensor
{
    public static partial class MomentProcessor
    {
        public const int StateCountMax = 5;
        public const int BlockColumnCount = 8;
        public const int BlockRowCount = 6;
        public const int BlockCount = BlockColumnCount * BlockRowCount;
    }

    public enum MomentProcessorPreprocess
    {
        Binarize,
        Cutoff,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MomentProcessorConfig : IEquatable<MomentProcessorConfig>
    {
        public IrCameraConfig irCameraConfig;
        public Rect windowOfInterest;
        public MomentProcessorPreprocess preprocess;
        public int preprocessIntensityThreshold;

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3}",
                irCameraConfig, windowOfInterest, preprocess, preprocessIntensityThreshold);
        }

        #region Equality
        public static bool operator ==(MomentProcessorConfig lhs, MomentProcessorConfig rhs)
        {
            return lhs.irCameraConfig == rhs.irCameraConfig &&
                lhs.windowOfInterest == rhs.windowOfInterest &&
                lhs.preprocess == rhs.preprocess &&
                lhs.preprocessIntensityThreshold == rhs.preprocessIntensityThreshold;
        }

        public static bool operator !=(MomentProcessorConfig lhs, MomentProcessorConfig rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is MomentProcessorConfig)) { return false; }
            return Equals((MomentProcessorConfig)right);
        }

        public bool Equals(MomentProcessorConfig other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MomentStatistic : IEquatable<MomentStatistic>
    {
        public float averageIntensity;
        public nn.util.Float2 centroid;

        public override string ToString()
        {
            return string.Format("({0} {1}",
                averageIntensity, centroid);
        }

        #region Equality
        public static bool operator ==(MomentStatistic lhs, MomentStatistic rhs)
        {
            return lhs.averageIntensity == rhs.averageIntensity &&
                lhs.centroid == rhs.centroid;
        }

        public static bool operator !=(MomentStatistic lhs, MomentStatistic rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is MomentStatistic)) { return false; }
            return Equals((MomentStatistic)right);
        }

        public bool Equals(MomentStatistic other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MomentProcessorState
    {
        public long samplingNumber;
        public long deltaTimeNanoSeconds;
        public IrCameraAmbientNoiseLevel ambientNoiseLevel;
        private byte _reserved0;
        private byte _reserved1;
        private byte _reserved2;
        private byte _reserved3;
        public MomentStatisticArray48 blocks;

        #region MomentStatisticArray48
        [StructLayout(LayoutKind.Sequential)]
        public struct MomentStatisticArray48 : IList<MomentStatistic>, ICollection<MomentStatistic>, IEnumerable<MomentStatistic>
        {
            private const int _Length = 48;
            public int Length { get { return _Length; } }

            private MomentStatistic _value0;
            private MomentStatistic _value1;
            private MomentStatistic _value2;
            private MomentStatistic _value3;
            private MomentStatistic _value4;
            private MomentStatistic _value5;
            private MomentStatistic _value6;
            private MomentStatistic _value7;
            private MomentStatistic _value8;
            private MomentStatistic _value9;
            private MomentStatistic _value10;
            private MomentStatistic _value11;
            private MomentStatistic _value12;
            private MomentStatistic _value13;
            private MomentStatistic _value14;
            private MomentStatistic _value15;
            private MomentStatistic _value16;
            private MomentStatistic _value17;
            private MomentStatistic _value18;
            private MomentStatistic _value19;
            private MomentStatistic _value20;
            private MomentStatistic _value21;
            private MomentStatistic _value22;
            private MomentStatistic _value23;
            private MomentStatistic _value24;
            private MomentStatistic _value25;
            private MomentStatistic _value26;
            private MomentStatistic _value27;
            private MomentStatistic _value28;
            private MomentStatistic _value29;
            private MomentStatistic _value30;
            private MomentStatistic _value31;
            private MomentStatistic _value32;
            private MomentStatistic _value33;
            private MomentStatistic _value34;
            private MomentStatistic _value35;
            private MomentStatistic _value36;
            private MomentStatistic _value37;
            private MomentStatistic _value38;
            private MomentStatistic _value39;
            private MomentStatistic _value40;
            private MomentStatistic _value41;
            private MomentStatistic _value42;
            private MomentStatistic _value43;
            private MomentStatistic _value44;
            private MomentStatistic _value45;
            private MomentStatistic _value46;
            private MomentStatistic _value47;

            public MomentStatistic this[int index]
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
                        case 10: return _value10;
                        case 11: return _value11;
                        case 12: return _value12;
                        case 13: return _value13;
                        case 14: return _value14;
                        case 15: return _value15;
                        case 16: return _value16;
                        case 17: return _value17;
                        case 18: return _value18;
                        case 19: return _value19;
                        case 20: return _value20;
                        case 21: return _value21;
                        case 22: return _value22;
                        case 23: return _value23;
                        case 24: return _value24;
                        case 25: return _value25;
                        case 26: return _value26;
                        case 27: return _value27;
                        case 28: return _value28;
                        case 29: return _value29;
                        case 30: return _value30;
                        case 31: return _value31;
                        case 32: return _value32;
                        case 33: return _value33;
                        case 34: return _value34;
                        case 35: return _value35;
                        case 36: return _value36;
                        case 37: return _value37;
                        case 38: return _value38;
                        case 39: return _value39;
                        case 40: return _value40;
                        case 41: return _value41;
                        case 42: return _value42;
                        case 43: return _value43;
                        case 44: return _value44;
                        case 45: return _value45;
                        case 46: return _value46;
                        case 47: return _value47;
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
                        case 10:
                            _value10 = value;
                            break;
                        case 11:
                            _value11 = value;
                            break;
                        case 12:
                            _value12 = value;
                            break;
                        case 13:
                            _value13 = value;
                            break;
                        case 14:
                            _value14 = value;
                            break;
                        case 15:
                            _value15 = value;
                            break;
                        case 16:
                            _value16 = value;
                            break;
                        case 17:
                            _value17 = value;
                            break;
                        case 18:
                            _value18 = value;
                            break;
                        case 19:
                            _value19 = value;
                            break;
                        case 20:
                            _value20 = value;
                            break;
                        case 21:
                            _value21 = value;
                            break;
                        case 22:
                            _value22 = value;
                            break;
                        case 23:
                            _value23 = value;
                            break;
                        case 24:
                            _value24 = value;
                            break;
                        case 25:
                            _value25 = value;
                            break;
                        case 26:
                            _value26 = value;
                            break;
                        case 27:
                            _value27 = value;
                            break;
                        case 28:
                            _value28 = value;
                            break;
                        case 29:
                            _value29 = value;
                            break;
                        case 30:
                            _value30 = value;
                            break;
                        case 31:
                            _value31 = value;
                            break;
                        case 32:
                            _value32 = value;
                            break;
                        case 33:
                            _value33 = value;
                            break;
                        case 34:
                            _value34 = value;
                            break;
                        case 35:
                            _value35 = value;
                            break;
                        case 36:
                            _value36 = value;
                            break;
                        case 37:
                            _value37 = value;
                            break;
                        case 38:
                            _value38 = value;
                            break;
                        case 39:
                            _value39 = value;
                            break;
                        case 40:
                            _value40 = value;
                            break;
                        case 41:
                            _value41 = value;
                            break;
                        case 42:
                            _value42 = value;
                            break;
                        case 43:
                            _value43 = value;
                            break;
                        case 44:
                            _value44 = value;
                            break;
                        case 45:
                            _value45 = value;
                            break;
                        case 46:
                            _value46 = value;
                            break;
                        case 47:
                            _value47 = value;
                            break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(MomentStatistic item)
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

            public int IndexOf(MomentStatistic item)
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
            public void CopyTo(MomentStatistic[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9, _value10, _value11, _value12, _value13, _value14, _value15, _value16, _value17, _value18, _value19, _value20, _value21, _value22, _value23,
                    _value24, _value25, _value26, _value27, _value28, _value29, _value30, _value31, _value32, _value33, _value34, _value35, _value36, _value37, _value38, _value39, _value40, _value41, _value42, _value43, _value44, _value45, _value46, _value47);
            }

            public IEnumerator<MomentStatistic> GetEnumerator()
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
                yield return _value10;
                yield return _value11;
                yield return _value12;
                yield return _value13;
                yield return _value14;
                yield return _value15;
                yield return _value16;
                yield return _value17;
                yield return _value18;
                yield return _value19;
                yield return _value20;
                yield return _value21;
                yield return _value22;
                yield return _value23;
                yield return _value24;
                yield return _value25;
                yield return _value26;
                yield return _value27;
                yield return _value28;
                yield return _value29;
                yield return _value30;
                yield return _value31;
                yield return _value32;
                yield return _value33;
                yield return _value34;
                yield return _value35;
                yield return _value36;
                yield return _value37;
                yield return _value38;
                yield return _value39;
                yield return _value40;
                yield return _value41;
                yield return _value42;
                yield return _value43;
                yield return _value44;
                yield return _value45;
                yield return _value46;
                yield return _value47;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(MomentStatistic item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, MomentStatistic item) { throw new NotSupportedException(); }
            public bool Remove(MomentStatistic item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }
}
#endif
