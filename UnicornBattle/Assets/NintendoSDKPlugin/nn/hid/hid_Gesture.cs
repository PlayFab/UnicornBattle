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
    public enum GestureType
    {
        Idle,
        Complete,
        Cancel,
        Touch,
        Press,
        Tap,
        Pan,
        Swipe,
        Pinch,
        Rotate,
    }

    public enum GestureDirection
    {
        None,
        Left,
        Up,
        Right,
        Down,
    }

    [Flags]
    public enum GestureAttribute
    {
        IsNewTouch = 0x1 << 4,
        IsDoubleTap = 0x1 << 8,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct GesturePoint : IEquatable<GesturePoint>
    {
        public int x;
        public int y;

        public override string ToString()
        {
            return string.Format("({0} {1})", this.x, this.y);
        }

        #region Equality
        public static bool operator ==(GesturePoint lhs, GesturePoint rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(GesturePoint lhs, GesturePoint rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is GesturePoint)) { return false; }
            return Equals((GesturePoint)right);
        }

        public bool Equals(GesturePoint other)
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
    public struct GestureState
    {
        public long eventNumber;
        public long contextNumber;
        public int _type;
        public int _direction;
        public int x;
        public int y;
        public int deltaX;
        public int deltaY;
        public nn.util.Float2 velocity;
        public GestureAttribute attributes;
        public float scale;
        public float rotationAngle;
        public int pointCount;
        public GesturePointArray4 points;

        public void SetDefault()
        {
            points = new GesturePointArray4();
        }

        public GestureType type { get { return (GestureType)this._type; } }

        public GestureDirection direction { get { return (GestureDirection)_direction; } }

        public bool isDoubleTap
        {
            get
            {
                return ((attributes & GestureAttribute.IsDoubleTap) == GestureAttribute.IsDoubleTap);
            }
        }

        public override string ToString()
        {
            return string.Format(
                "event:{0} con:{1} type:{2} dir:{3} pos:({4} {5}) delta:({6} {7}) vel:{8} attr:{9} scale:{10} rotA:{11} count:{12} p0:{13} p1:{14} p2:{15} p3:{16}",
                this.eventNumber, this.contextNumber, this.type, this.direction,
                this.x, this.y, this.deltaX, this.deltaY, this.velocity,
                this.attributes, this.scale, this.rotationAngle,
                this.pointCount, this.points[0], this.points[1], this.points[2], this.points[3]);
        }
        
        #region GesturePointArray4
        [StructLayout(LayoutKind.Sequential)]
        public struct GesturePointArray4 : IList<GesturePoint>, ICollection<GesturePoint>, IEnumerable<GesturePoint>
        {
            private const int _Length = 4;
            public int Length { get { return _Length; } }

            private GesturePoint _value0;
            private GesturePoint _value1;
            private GesturePoint _value2;
            private GesturePoint _value3;

            public GesturePoint this[int index]
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

            public bool Contains(GesturePoint item)
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

            public int IndexOf(GesturePoint item)
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
            public void CopyTo(GesturePoint[] array, int arrayIndex)
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

            public IEnumerator<GesturePoint> GetEnumerator()
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
            public void Add(GesturePoint item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, GesturePoint item) { throw new NotSupportedException(); }
            public bool Remove(GesturePoint item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    public static partial class Gesture
    {
        public const int PointCountMax = 4;
        public const int StateCountMax = 16;

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_InitializeGesture")]
        public static extern void Initialize();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_hid_GetGestureStates")]
        public static extern int GetStates([Out] GestureState[] pOutValues, int count);
    }
}
#endif
