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
using System.Text;
using System.Runtime.InteropServices;

namespace nn.irsensor
{
    public static partial class ClusteringProcessor
    {
        public const int StateCountMax = 5;
        public const int ObjectCountMax = 16;
        public const int ObjectPixelCountMax = 76800;
        public const int OutObjectPixelCountMax = 65535;
        public const long ExposureTimeMinNanoSeconds = 7 * 1000;
        public const long ExposureTimeMaxNanoSeconds = 600 * 1000;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ClusteringProcessorConfig
    {
        public IrCameraConfig irCameraConfig;
        public Rect windowOfInterest;
        public int objectPixelCountMin;
        public int objectPixelCountMax;
        public int objectIntensityMin;
        [MarshalAs(UnmanagedType.U1)]
        public bool isExternalLightFilterEnabled;

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3} {4} {5})",
                this.irCameraConfig.ToString(), this.windowOfInterest.ToString(),
                this.objectPixelCountMin, this.objectPixelCountMax,
                this.objectIntensityMin, this.isExternalLightFilterEnabled);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ClusteringData : IEquatable<ClusteringData>
    {
        public float averageIntensity;
        public util.Float2 centroid;
        public int pixelCount;
        public Rect bound;

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3})",
                this.averageIntensity, this.centroid.ToString(),
                this.pixelCount, this.bound.ToString());
        }

        #region Equality
        public static bool operator ==(ClusteringData lhs, ClusteringData rhs)
        {
            return lhs.averageIntensity == rhs.averageIntensity
                && lhs.centroid == rhs.centroid
                && lhs.pixelCount == rhs.pixelCount
                && lhs.bound == rhs.bound;
        }

        public static bool operator !=(ClusteringData lhs, ClusteringData rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is ClusteringData)) { return false; }
            return Equals((ClusteringData)right);
        }

        public bool Equals(ClusteringData other)
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
    public struct ClusteringProcessorState
    {
        public long samplingNumber;
        public long timeStampNanoSeconds;
        public sbyte objectCount;
        public byte _reserved0;
        public byte _reserved1;
        public byte _reserved2;
        public IrCameraAmbientNoiseLevel ambientNoiseLevel;
        public ClusteringDataArray16 objects;

        public void SetDefault()
        {
            objects = new ClusteringDataArray16();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("({0} {1} {2} {3})\n",
               this.samplingNumber, this.timeStampNanoSeconds,
               this.objectCount, this.ambientNoiseLevel.ToString());

            for (int i = 0; i < this.objectCount; i++)
            {
                builder.AppendFormat("object[{0}]:{1}\n", i, this.objects[i].ToString());
            }

            return builder.ToString();
        }
        #region ClusteringDataArray16
        [StructLayout(LayoutKind.Sequential)]
        public struct ClusteringDataArray16 : IList<ClusteringData>, ICollection<ClusteringData>, IEnumerable<ClusteringData>
        {
            private const int _Length = 16;
            public int Length { get { return _Length; } }

            private ClusteringData _value0;
            private ClusteringData _value1;
            private ClusteringData _value2;
            private ClusteringData _value3;
            private ClusteringData _value4;
            private ClusteringData _value5;
            private ClusteringData _value6;
            private ClusteringData _value7;
            private ClusteringData _value8;
            private ClusteringData _value9;
            private ClusteringData _value10;
            private ClusteringData _value11;
            private ClusteringData _value12;
            private ClusteringData _value13;
            private ClusteringData _value14;
            private ClusteringData _value15;

            public ClusteringData this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(ClusteringData item)
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

            public int IndexOf(ClusteringData item)
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
            public void CopyTo(ClusteringData[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9, _value10, _value11, _value12, _value13, _value14, _value15);
            }

            public IEnumerator<ClusteringData> GetEnumerator()
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
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(ClusteringData item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, ClusteringData item) { throw new NotSupportedException(); }
            public bool Remove(ClusteringData item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }
}
#endif
