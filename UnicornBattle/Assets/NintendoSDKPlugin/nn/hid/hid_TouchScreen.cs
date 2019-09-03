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
    [Flags]
    public enum TouchAttribute
    {
        Start = 0x1 << 0,
        End = 0x1 << 1,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchState : IEquatable<TouchState>
    {
        public long deltaTimeNanoSeconds;
        public TouchAttribute attributes;
        public int fingerId;
        public int x;
        public int y;
        public int diameterX;
        public int diameterY;
        public int rotationAngle;
        private int _reserved;

        public override string ToString()
        {
            return string.Format("fId:{0} pos:({1} {2}) dia:({3} {4}) rotA:{5} attr:{6} delta:{7}",
                this.fingerId, this.x, this.y, this.diameterX, this.diameterY, this.rotationAngle,
                this.attributes, this.deltaTimeNanoSeconds);
        }

        #region Equality
        public static bool operator ==(TouchState lhs, TouchState rhs)
        {
            return lhs.deltaTimeNanoSeconds == rhs.deltaTimeNanoSeconds
                && lhs.attributes == rhs.attributes
                && lhs.fingerId == rhs.fingerId
                && lhs.x == rhs.x
                && lhs.y == rhs.y
                && lhs.diameterX == rhs.diameterX
                && lhs.diameterY == rhs.diameterY
                && lhs.rotationAngle == rhs.rotationAngle;
        }

        public static bool operator !=(TouchState lhs, TouchState rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is TouchState)) { return false; }
            return Equals((TouchState)right);
        }

        public bool Equals(TouchState other)
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
    public struct TouchScreenState1
    {
        public const int TouchCount = 1;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray1 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray1();
        }

        #region TouchStateArray1
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray1 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 1;
            public int Length { get { return _Length; } }

            private TouchState _value0;

            public TouchState this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return _value0;
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0}}}",
                    _value0);
            }

            public IEnumerator<TouchState> GetEnumerator()
            {
                yield return _value0;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState2
    {
        public const int TouchCount = 2;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray2 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray2();
        }

        #region TouchStateArray2
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray2 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 2;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;

            public TouchState this[int index]
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

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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

            public IEnumerator<TouchState> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState3
    {
        public const int TouchCount = 3;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray3 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray3();
        }

        #region TouchStateArray3
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray3 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 3;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;

            public TouchState this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return _value0;
                        case 1: return _value1;
                        case 2: return _value2;
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2}}}",
                    _value0, _value1, _value2);
            }

            public IEnumerator<TouchState> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
                yield return _value2;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState4
    {
        public const int TouchCount = 4;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray4 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray4();
        }

        #region TouchStateArray4
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray4 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 4;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;

            public TouchState this[int index]
            {
                get
                {
                    switch (index)
                    {
                        case 0: return _value0;
                        case 1: return _value1;
                        case 2: return _value2;
                        case 3: return _value3;
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3}}}",
                    _value0, _value1, _value2, _value3);
            }

            public IEnumerator<TouchState> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
                yield return _value2;
                yield return _value3;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState5
    {
        public const int TouchCount = 5;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray5 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray5();
        }

        #region TouchStateArray5
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray5 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 5;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                    _value0, _value1, _value2, _value3, _value4);
            }

            public IEnumerator<TouchState> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
                yield return _value2;
                yield return _value3;
                yield return _value4;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState6
    {
        public const int TouchCount = 6;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray6 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray6();
        }

        #region TouchStateArray6
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray6 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 6;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5);
            }

            public IEnumerator<TouchState> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
                yield return _value2;
                yield return _value3;
                yield return _value4;
                yield return _value5;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState7
    {
        public const int TouchCount = 7;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray7 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray7();
        }

        #region TouchStateArray7
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray7 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 7;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6);
            }

            public IEnumerator<TouchState> GetEnumerator()
            {
                yield return _value0;
                yield return _value1;
                yield return _value2;
                yield return _value3;
                yield return _value4;
                yield return _value5;
                yield return _value6;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState8
    {
        public const int TouchCount = 8;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray8 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray8();
        }

        #region TouchStateArray8
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray8 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 8;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;

            public TouchState this[int index]
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

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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

            public IEnumerator<TouchState> GetEnumerator()
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
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState9
    {
        public const int TouchCount = 9;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray9 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray9();
        }

        #region TouchStateArray9
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray9 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 9;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;
            private TouchState _value8;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8);
            }

            public IEnumerator<TouchState> GetEnumerator()
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
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState10
    {
        public const int TouchCount = 10;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray10 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray10();
        }

        #region TouchStateArray10
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray10 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 10;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;
            private TouchState _value8;
            private TouchState _value9;

            public TouchState this[int index]
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

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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

            public IEnumerator<TouchState> GetEnumerator()
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
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState11
    {
        public const int TouchCount = 11;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray11 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray11();
        }

        #region TouchStateArray11
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray11 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 11;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;
            private TouchState _value8;
            private TouchState _value9;
            private TouchState _value10;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9, _value10);
            }

            public IEnumerator<TouchState> GetEnumerator()
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
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState12
    {
        public const int TouchCount = 12;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray12 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray12();
        }

        #region TouchStateArray12
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray12 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 12;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;
            private TouchState _value8;
            private TouchState _value9;
            private TouchState _value10;
            private TouchState _value11;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9, _value10, _value11);
            }

            public IEnumerator<TouchState> GetEnumerator()
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
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState13
    {
        public const int TouchCount = 13;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray13 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray13();
        }

        #region TouchStateArray13
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray13 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 13;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;
            private TouchState _value8;
            private TouchState _value9;
            private TouchState _value10;
            private TouchState _value11;
            private TouchState _value12;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9, _value10, _value11, _value12);
            }

            public IEnumerator<TouchState> GetEnumerator()
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
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState14
    {
        public const int TouchCount = 14;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray14 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray14();
        }

        #region TouchStateArray14
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray14 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 14;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;
            private TouchState _value8;
            private TouchState _value9;
            private TouchState _value10;
            private TouchState _value11;
            private TouchState _value12;
            private TouchState _value13;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9, _value10, _value11, _value12, _value13);
            }

            public IEnumerator<TouchState> GetEnumerator()
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
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState15
    {
        public const int TouchCount = 15;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray15 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray15();
        }

        #region TouchStateArray15
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray15 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 15;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;
            private TouchState _value8;
            private TouchState _value9;
            private TouchState _value10;
            private TouchState _value11;
            private TouchState _value12;
            private TouchState _value13;
            private TouchState _value14;

            public TouchState this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9, _value10, _value11, _value12, _value13, _value14);
            }

            public IEnumerator<TouchState> GetEnumerator()
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
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TouchScreenState16
    {
        public const int TouchCount = 16;
        public long samplingNumber;
        public int count;
        private int _reserved;
        public TouchStateArray16 touches;

        public void SetDefault()
        {
            touches = new TouchStateArray16();
        }

        #region TouchStateArray16
        [StructLayout(LayoutKind.Sequential)]
        public struct TouchStateArray16 : IList<TouchState>, ICollection<TouchState>, IEnumerable<TouchState>
        {
            private const int _Length = 16;
            public int Length { get { return _Length; } }

            private TouchState _value0;
            private TouchState _value1;
            private TouchState _value2;
            private TouchState _value3;
            private TouchState _value4;
            private TouchState _value5;
            private TouchState _value6;
            private TouchState _value7;
            private TouchState _value8;
            private TouchState _value9;
            private TouchState _value10;
            private TouchState _value11;
            private TouchState _value12;
            private TouchState _value13;
            private TouchState _value14;
            private TouchState _value15;

            public TouchState this[int index]
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

            public bool Contains(TouchState item)
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

            public int IndexOf(TouchState item)
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
            public void CopyTo(TouchState[] array, int arrayIndex)
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

            public IEnumerator<TouchState> GetEnumerator()
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
            public void Add(TouchState item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, TouchState item) { throw new NotSupportedException(); }
            public bool Remove(TouchState item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    public static partial class TouchScreen
    {
        public const int TouchCountMax = 16;
        public const int StateCountMax = 16;

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_InitializeTouchScreen")]
        public static extern void Initialize();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState1")]
        public static extern void GetState(ref TouchScreenState1 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState2")]
        public static extern void GetState(ref TouchScreenState2 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState3")]
        public static extern void GetState(ref TouchScreenState3 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState4")]
        public static extern void GetState(ref TouchScreenState4 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState5")]
        public static extern void GetState(ref TouchScreenState5 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState6")]
        public static extern void GetState(ref TouchScreenState6 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState7")]
        public static extern void GetState(ref TouchScreenState7 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState8")]
        public static extern void GetState(ref TouchScreenState8 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState9")]
        public static extern void GetState(ref TouchScreenState9 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState10")]
        public static extern void GetState(ref TouchScreenState10 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState11")]
        public static extern void GetState(ref TouchScreenState11 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState12")]
        public static extern void GetState(ref TouchScreenState12 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState13")]
        public static extern void GetState(ref TouchScreenState13 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState14")]
        public static extern void GetState(ref TouchScreenState14 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState15")]
        public static extern void GetState(ref TouchScreenState15 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenState16")]
        public static extern void GetState(ref TouchScreenState16 pOutValue);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates1")]
        public static extern int GetStates([Out] TouchScreenState1[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates2")]
        public static extern int GetStates([Out] TouchScreenState2[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates3")]
        public static extern int GetStates([Out] TouchScreenState3[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates4")]
        public static extern int GetStates([Out] TouchScreenState4[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates5")]
        public static extern int GetStates([Out] TouchScreenState5[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates6")]
        public static extern int GetStates([Out] TouchScreenState6[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates7")]
        public static extern int GetStates([Out] TouchScreenState7[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates8")]
        public static extern int GetStates([Out] TouchScreenState8[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates9")]
        public static extern int GetStates([Out] TouchScreenState9[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates10")]
        public static extern int GetStates([Out] TouchScreenState10[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates11")]
        public static extern int GetStates([Out] TouchScreenState11[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates12")]
        public static extern int GetStates([Out] TouchScreenState12[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates13")]
        public static extern int GetStates([Out] TouchScreenState13[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates14")]
        public static extern int GetStates([Out] TouchScreenState14[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates15")]
        public static extern int GetStates([Out] TouchScreenState15[] pOutValues, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetTouchScreenStates16")]
        public static extern int GetStates([Out] TouchScreenState16[] pOutValues, int count);
    }
}
#endif
