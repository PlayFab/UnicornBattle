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
    public static partial class HandAnalysis
    {
        public const int ProcessorStateCountMax = 5;
        public const int ShapePointCountMax = 512;
        public const int ShapeCountMax = 16;
        public const int ProtrusionCountMax = 8;
        public const int HandCountMax = 2;
        public const int ImageWidth = 40;
        public const int ImageHeight = 30;
    }

    public enum HandAnalysisMode
    {
        None = 0,
        Silhouette = 1,
        Image = 2,
        SilhouetteAndImage = 3,
        SilhouetteOnly = 4,
    }

    public enum HandChirality
    {
        Left = 1,
        Right = 2,
        Unknown = 3,
    }

    public enum HandFinger
    {
        Thumb = 0,
        Index = 1,
        Middle = 2,
        Ring = 3,
        Little = 4,

        Count = 5,
    }

    public enum HandTouchingFingers
    {
        IndexMiddle = 0,
        MiddleRing = 1,
        RingLittle = 2,

        Count = 3,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HandAnalysisConfig : IEquatable<HandAnalysisConfig>
    {
        public HandAnalysisMode mode;

        public override string ToString()
        {
            return string.Format("({0})",
                mode);
        }

        #region Equality
        public static bool operator ==(HandAnalysisConfig lhs, HandAnalysisConfig rhs)
        {
            return lhs.mode == rhs.mode;
        }

        public static bool operator !=(HandAnalysisConfig lhs, HandAnalysisConfig rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is HandAnalysisConfig)) { return false; }
            return Equals((HandAnalysisConfig)right);
        }

        public bool Equals(HandAnalysisConfig other)
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
    public struct Protrusion : IEquatable<Protrusion>
    {
        public int firstPointIndex;
        public int pointCount;

        public override string ToString()
        {
            return string.Format("({0} {1})",
                firstPointIndex, pointCount);
        }

        #region Equality
        public static bool operator ==(Protrusion lhs, Protrusion rhs)
        {
            return lhs.firstPointIndex == rhs.firstPointIndex && lhs.pointCount == rhs.pointCount;
        }

        public static bool operator !=(Protrusion lhs, Protrusion rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is Protrusion)) { return false; }
            return Equals((Protrusion)right);
        }

        public bool Equals(Protrusion other)
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
    public struct Finger : IEquatable<Finger>
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool isValid;
        public nn.util.Float2 tip;  
        public float tipDepthFactor;
        public nn.util.Float2 root;
        public int protrusionIndex;

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3} {4}",
                isValid, tip, tipDepthFactor, root, protrusionIndex);
        }

        #region Equality
        public static bool operator ==(Finger lhs, Finger rhs)
        {
            return lhs.isValid == rhs.isValid &&
                lhs.tip == rhs.tip &&
                lhs.tipDepthFactor == rhs.tipDepthFactor &&
                lhs.root == rhs.root &&
                lhs.protrusionIndex == rhs.protrusionIndex;
        }

        public static bool operator !=(Finger lhs, Finger rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is Finger)) { return false; }
            return Equals((Finger)right);
        }

        public bool Equals(Finger other)
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
    public struct Palm : IEquatable<Palm>
    {
        public nn.util.Float2 center;
        public float area;
        public float depthFactor;

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", center, area, depthFactor);
        }

        #region Equality
        public static bool operator ==(Palm lhs, Palm rhs)
        {
            return lhs.center == rhs.center &&
                lhs.area == rhs.area &&
                lhs.depthFactor == rhs.depthFactor;
        }

        public static bool operator !=(Palm lhs, Palm rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is Palm)) { return false; }
            return Equals((Palm)right);
        }

        public bool Equals(Palm other)
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
    public struct Arm : IEquatable<Arm>
    {
        [MarshalAs(UnmanagedType.U1)]
        public bool isValid;  
        public nn.util.Float2 wristPosition;
        public nn.util.Float2 armDirection;
        public int protrusionIndex;

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3})",
                isValid, wristPosition, armDirection, protrusionIndex);
        }

        #region Equality
        public static bool operator ==(Arm lhs, Arm rhs)
        {
            return lhs.isValid == rhs.isValid &&
                lhs.wristPosition == rhs.wristPosition &&
                lhs.armDirection == rhs.armDirection &&
                lhs.protrusionIndex == rhs.protrusionIndex;
        }

        public static bool operator !=(Arm lhs, Arm rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is Arm)) { return false; }
            return Equals((Arm)right);
        }

        public bool Equals(Arm other)
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
    public struct Hand : IEquatable<Hand>
    {
        public int shapeId;
        public int protrusionCount;
        public ProtrusionArray8 protrusions;
        public HandChirality chirality;
        public Fingers fingers;
        [MarshalAs(UnmanagedType.U1)]
        public bool areIndexMiddleFingersTouching;
        [MarshalAs(UnmanagedType.U1)]
        public bool areMiddleRingFingersTouching;
        [MarshalAs(UnmanagedType.U1)]
        public bool areRingLittleFingersTouching;
        public Palm palm;
        public Arm arm;

        #region Equality
        public static bool operator ==(Hand lhs, Hand rhs)
        {
            if (lhs.protrusions.Length != rhs.protrusions.Length) { return false; }
            for (int i = 0; i < lhs.protrusions.Length; i++)
            {
                if (lhs.protrusions[i] != rhs.protrusions[i])
                {
                    return false;
                }
            }
            if (lhs.fingers.Length != rhs.fingers.Length) { return false; }
            for (int i = 0; i < lhs.protrusions.Length; i++)
            {
                if (lhs.fingers[i] != rhs.fingers[i])
                {
                    return false;
                }
            }
            return lhs.shapeId == rhs.shapeId &&
                lhs.protrusionCount == rhs.protrusionCount &&
                lhs.chirality == rhs.chirality &&
                lhs.areIndexMiddleFingersTouching == rhs.areIndexMiddleFingersTouching &&
                lhs.areMiddleRingFingersTouching == rhs.areMiddleRingFingersTouching &&
                lhs.areRingLittleFingersTouching == rhs.areRingLittleFingersTouching &&
                lhs.palm == rhs.palm &&
                lhs.arm == rhs.arm;
        }

        public static bool operator !=(Hand lhs, Hand rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is Hand)) { return false; }
            return Equals((Hand)right);
        }

        public bool Equals(Hand other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion

        #region ProtrusionArray8
        [StructLayout(LayoutKind.Sequential)]
        public struct ProtrusionArray8 : IList<Protrusion>, ICollection<Protrusion>, IEnumerable<Protrusion>
        {
            private const int _Length = 8;
            public int Length { get { return _Length; } }

            private Protrusion _value0;
            private Protrusion _value1;
            private Protrusion _value2;
            private Protrusion _value3;
            private Protrusion _value4;
            private Protrusion _value5;
            private Protrusion _value6;
            private Protrusion _value7;

            public Protrusion this[int index]
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

            public bool Contains(Protrusion item)
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

            public int IndexOf(Protrusion item)
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
            public void CopyTo(Protrusion[] array, int arrayIndex)
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

            public IEnumerator<Protrusion> GetEnumerator()
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
            public void Add(Protrusion item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, Protrusion item) { throw new NotSupportedException(); }
            public bool Remove(Protrusion item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion

        #region Fingers
        [StructLayout(LayoutKind.Sequential)]
        public struct Fingers : IList<Finger>, ICollection<Finger>, IEnumerable<Finger>
        {
            private const int _Length = 5;
            public int Length { get { return _Length; } }

            public Finger thumb;
            public Finger index;
            public Finger middle;
            public Finger ring;
            public Finger little;

            public Finger this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return thumb;
                        case 1: return this.index;
                        case 2: return middle;
                        case 3: return ring;
                        case 4: return little;
                        default: throw new IndexOutOfRangeException();
                    }
                }
                set
                {
                    switch (index)
                    {
                        case 0:
                            thumb = value;
                            break;
                        case 1:
                            this.index = value;
                            break;
                        case 2:
                            middle = value;
                            break;
                        case 3:
                            ring = value;
                            break;
                        case 4:
                            little = value;
                            break;
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(Finger item)
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

            public int IndexOf(Finger item)
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
            public void CopyTo(Finger[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4}}}",
                    thumb, index, middle, ring, little);
            }

            public IEnumerator<Finger> GetEnumerator()
            {
                yield return thumb;
                yield return index;
                yield return middle;
                yield return ring;
                yield return little;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(Finger item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, Finger item) { throw new NotSupportedException(); }
            public bool Remove(Finger item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Shape : IEquatable<Shape>
    {
        public int firstPointIndex;
        public int pointCount;
        public float intensityAverage;
        public nn.util.Float2 intensityCentroid;

        public override string ToString()
        {
            return string.Format("({0} {1} {2} {3})",
                firstPointIndex, pointCount, intensityAverage, intensityCentroid);
        }

        #region Equality
        public static bool operator ==(Shape lhs, Shape rhs)
        {
            return lhs.firstPointIndex == rhs.firstPointIndex &&
                lhs.pointCount == rhs.pointCount &&
                lhs.intensityAverage == rhs.intensityAverage &&
                lhs.intensityCentroid == rhs.intensityCentroid;
        }

        public static bool operator !=(Shape lhs, Shape rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is Shape)) { return false; }
            return Equals((Shape)right);
        }

        public bool Equals(Shape other)
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
    public struct HandAnalysisSilhouetteState
    {
        public long samplingNumber;
        public IrCameraAmbientNoiseLevel ambientNoiseLevel;
        public int shapeCount;
        public ShapeArray16 shapes;
        public int handCount;
        public HandArray2 hands;

        #region ShapeArray16
        [StructLayout(LayoutKind.Sequential)]
        public struct ShapeArray16 : IList<Shape>, ICollection<Shape>, IEnumerable<Shape>
        {
            private const int _Length = 16;
            public int Length { get { return _Length; } }

            private Shape _value0;
            private Shape _value1;
            private Shape _value2;
            private Shape _value3;
            private Shape _value4;
            private Shape _value5;
            private Shape _value6;
            private Shape _value7;
            private Shape _value8;
            private Shape _value9;
            private Shape _value10;
            private Shape _value11;
            private Shape _value12;
            private Shape _value13;
            private Shape _value14;
            private Shape _value15;

            public Shape this[int index]
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

            public bool Contains(Shape item)
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

            public int IndexOf(Shape item)
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
            public void CopyTo(Shape[] array, int arrayIndex)
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

            public IEnumerator<Shape> GetEnumerator()
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
            public void Add(Shape item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, Shape item) { throw new NotSupportedException(); }
            public bool Remove(Shape item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion

        #region HandArray2
        [StructLayout(LayoutKind.Sequential)]
        public struct HandArray2 : IList<Hand>, ICollection<Hand>, IEnumerable<Hand>
        {
            private const int _Length = 2;
            public int Length { get { return _Length; } }

            private Hand _value0;
            private Hand _value1;

            public Hand this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return _value0;
                        case 1: return _value1;
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(Hand item)
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

            public int IndexOf(Hand item)
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
            public void CopyTo(Hand[] array, int arrayIndex)
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
                return string.Format("{{{0},{1}}}",
                    _value0, _value1);
            }

            public IEnumerator<Hand> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(Hand item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, Hand item) { throw new NotSupportedException(); }
            public bool Remove(Hand item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HandAnalysisImageState
    {
        public long samplingNumber;
        public IrCameraAmbientNoiseLevel ambientNoiseLevel;
    }
}
#endif
