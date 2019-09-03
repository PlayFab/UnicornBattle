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

namespace nn.account
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UserSelectionSettings : IEquatable<UserSelectionSettings>
    {
        public UidArray8 invalidUidList;

        [MarshalAs(UnmanagedType.U1)]
        public bool isSkipEnabled;
        [MarshalAs(UnmanagedType.U1)]
        public bool isNetworkServiceAccountRequired;
        [MarshalAs(UnmanagedType.U1)]
        public bool showSkipButton;
        [MarshalAs(UnmanagedType.U1)]
        public bool additionalSelect;
        [MarshalAs(UnmanagedType.U1)]
        public bool isUnqualifiedUserSelectable;
   
        public void SetDefault()
        {
            invalidUidList = new UidArray8();
            isSkipEnabled = false;
            isNetworkServiceAccountRequired = false;
            showSkipButton = false;
            additionalSelect = false;
            isUnqualifiedUserSelectable = false;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("skip:{0} netAccount:{1} skipButton:{2} addSelect:{3} unqualifiedUserSelectable:{4}  ignore:[ ",
                isSkipEnabled, isNetworkServiceAccountRequired, showSkipButton, additionalSelect, isUnqualifiedUserSelectable);
            for (int i = 0; i < Account.UserCountMax; i++)
            {
                if (invalidUidList[i] != Uid.Invalid)
                {
                    builder.Append(invalidUidList[i].ToString());
                    builder.Append(" ");
                }
                builder.Append("]");
            }
            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UserSelectionSettings)) { return false; }
            return Equals((UserSelectionSettings)obj);
        }
        public bool Equals(UserSelectionSettings other) { return this == other; }
        public override int GetHashCode() { return base.GetHashCode(); }
        public static bool operator ==(UserSelectionSettings lhs, UserSelectionSettings rhs)
        {
            if (lhs.invalidUidList.Length != rhs.invalidUidList.Length)
            {
                return false;
            }
            for (int i = 0; i < lhs.invalidUidList.Length; i++)
            {
                if (lhs.invalidUidList[i] != rhs.invalidUidList[i])
                {
                    return false;
                }
            }
            return lhs.isSkipEnabled == rhs.isSkipEnabled
                && lhs.isNetworkServiceAccountRequired == rhs.isNetworkServiceAccountRequired
                && lhs.showSkipButton == rhs.showSkipButton
                && lhs.additionalSelect == rhs.additionalSelect
                && lhs.isUnqualifiedUserSelectable == rhs.isUnqualifiedUserSelectable;
        }
        public static bool operator !=(UserSelectionSettings lhs, UserSelectionSettings rhs) { return !(lhs == rhs); }

        #region UidArray8
        [StructLayout(LayoutKind.Sequential)]
        public struct UidArray8 : IList<Uid>, ICollection<Uid>, IEnumerable<Uid>
        {
            private const int _Length = 8;
            public int Length { get { return _Length; } }

            private Uid _value0;
            private Uid _value1;
            private Uid _value2;
            private Uid _value3;
            private Uid _value4;
            private Uid _value5;
            private Uid _value6;
            private Uid _value7;

            public Uid this[int index]
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

            public bool Contains(Uid item)
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

            public int IndexOf(Uid item)
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
            public void CopyTo(Uid[] array, int arrayIndex)
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

            public IEnumerator<Uid> GetEnumerator()
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
            public void Add(Uid item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, Uid item) { throw new NotSupportedException(); }
            public bool Remove(Uid item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }

    public static partial class Account
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result ShowUserSelector(ref Uid pOut, UserSelectionSettings arg)
        {
            return new Result();
        }

        public static Result ShowUserSelector(ref Uid pOut)
        {
            return new Result();
        }

        public static Result ShowUserCreator()
        {
            return new Result();
        }

#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ShowUserSelector0")]
        public static extern Result ShowUserSelector(ref Uid pOut, UserSelectionSettings arg);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ShowUserSelector1")]
        public static extern Result ShowUserSelector(ref Uid pOut);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_account_ShowUserCreator")]
        public static extern Result ShowUserCreator();
#endif

        public static Result ShowUserSelector(
            ref Uid pOut, UserSelectionSettings arg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = ShowUserSelector(ref pOut, arg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return ShowUserSelector(ref pOut, arg);
        }

        public static Result ShowUserSelector(ref Uid pOut, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = ShowUserSelector(ref pOut);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return ShowUserSelector(ref pOut);
        }

        public static Result ShowUserCreator(bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = ShowUserCreator();
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return ShowUserCreator();
        }
    }
}
#endif
