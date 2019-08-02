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
using System.Text;

namespace nn.swkbd
{
    public static partial class Swkbd
    {
        public const int TextMaxLength = 500;
        public const int SeparateModeTextMaxLength = 24;
        public const int HeaderTextMaxLength = 64;
        public const int SubTextMaxLength = 128;
        public const int GuideTextMaxLength = 256;
        public const int OkTextMaxLength = 8;
        public const int UnfixedStringLengthMax = 24;
        public const int UserWordMax = 5000;
        public const int DialogTextMaxLength = 500;
        public const int SepareteTextPosMax = 8;
        public const int CustomizedDicionarySetMax = 24;
    }

    public enum Preset
    {
        Default,
        Password,
        UserName,
        DownloadCode,

        Max,
    }

    public enum KeyboardMode
    {
        LanguageSet1,
        Numeric,
        ASCII,
        LanguageSet1Latin,
        Alphabet,
        SimplifiedChinese,
        TraditionalChinese,
        Korean,
        LanguageSet2,

        Max,

        Full = LanguageSet1,
        FullLatin = LanguageSet1Latin,
    }

    [Flags]
    public enum InvalidChar
    {
        Space = 1 << 1,
        AtMark = 1 << 2,
        Percent = 1 << 3,
        Slash = 1 << 4,
        BackSlash = 1 << 5,
        Numeric = 1 << 6,
        OutsideOfDownloadCode = 1 << 7,
        OutsideOfMiiNickName = 1 << 8,
        Force32 = -1,
    }

    public enum PasswordMode
    {
        Show,
        Hide,

        Max,
    }

    public enum InputFormMode
    {
        OneLine,
        MultiLine,
        Separate,

        Max,
    }

    public enum InitialCursorPos
    {
        First,
        Last,

        Max,
    }

    public enum TextCheckResult
    {
        Success,
        ShowFailureDialog,
        ShowConfirmDialog,

        Max,
    }

    public enum DictionaryLang : ushort
    {
        Japanese,
        AmericanEnglish,
        CanadianFrench,
        LatinAmericanSpanish,
        Reserved1,
        BritishEnglish,
        French,
        German,
        Spanish,
        Italian,
        Dutch,
        Portuguese,
        Russian,
        Reserved2,
        SimplifiedChinesePinyin,
        TraditionalChineseCangjie,
        TraditionalChineseSimplifiedCangjie,
        TraditionalChineseZhuyin,
        Korean,

        Max,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct String
    {
        public IntPtr ptr;
        public long bufSize;
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate TextCheckResult TextCheckCallback(IntPtr pOutDialogTextBuf, ref long pOutDialogTextLengthSize, ref nn.swkbd.String pStr);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct UserWord
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Swkbd.UnfixedStringLengthMax + 1)]
        public string reading;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Swkbd.UnfixedStringLengthMax + 1)]
        public string word;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DictionaryInfo
    {
        public uint offset;
        public ushort size;
        public DictionaryLang lang;

        #region Equality
        public static bool operator ==(DictionaryInfo lhs, DictionaryInfo rhs)
        {
            return lhs.offset == rhs.offset && lhs.size == rhs.size && lhs.lang == rhs.lang;
        }

        public static bool operator !=(DictionaryInfo lhs, DictionaryInfo rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object right)
        {
            if (!(right is DictionaryInfo)) { return false; }
            return Equals((DictionaryInfo)right);
        }

        public bool Equals(DictionaryInfo other)
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
    public struct CustomizedDictionarySet
    {
        public IntPtr pDictionaries;
        public uint dictionariesSize;
        public DictionaryInfoArray24 dicInfoList;
        public ushort count;

        #region DictionaryInfoArray24
        [StructLayout(LayoutKind.Sequential)]
        public struct DictionaryInfoArray24 : IList<DictionaryInfo>, ICollection<DictionaryInfo>, IEnumerable<DictionaryInfo>
        {
            private const int _Length = 24;
            public int Length { get { return _Length; } }

            private DictionaryInfo _value0;
            private DictionaryInfo _value1;
            private DictionaryInfo _value2;
            private DictionaryInfo _value3;
            private DictionaryInfo _value4;
            private DictionaryInfo _value5;
            private DictionaryInfo _value6;
            private DictionaryInfo _value7;
            private DictionaryInfo _value8;
            private DictionaryInfo _value9;
            private DictionaryInfo _value10;
            private DictionaryInfo _value11;
            private DictionaryInfo _value12;
            private DictionaryInfo _value13;
            private DictionaryInfo _value14;
            private DictionaryInfo _value15;
            private DictionaryInfo _value16;
            private DictionaryInfo _value17;
            private DictionaryInfo _value18;
            private DictionaryInfo _value19;
            private DictionaryInfo _value20;
            private DictionaryInfo _value21;
            private DictionaryInfo _value22;
            private DictionaryInfo _value23;

            public DictionaryInfo this[int index]
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
                        default: throw new IndexOutOfRangeException();
                    }
                }
            }

            public int Count { get { return Length; } }

            public bool IsReadOnly { get { return true; } }

            public bool Contains(DictionaryInfo item)
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

            public int IndexOf(DictionaryInfo item)
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
            public void CopyTo(DictionaryInfo[] array, int arrayIndex)
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
                return string.Format("{{{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23}}}",
                    _value0, _value1, _value2, _value3, _value4, _value5, _value6, _value7,
                    _value8, _value9, _value10, _value11, _value12, _value13, _value14, _value15,
                    _value16, _value17, _value18, _value19, _value20, _value21, _value22, _value23);
            }

            public IEnumerator<DictionaryInfo> GetEnumerator()
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
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region NotSupported
            public void Add(DictionaryInfo item) { throw new NotSupportedException(); }
            public void Clear() { throw new NotSupportedException(); }
            public void Insert(int index, DictionaryInfo item) { throw new NotSupportedException(); }
            public bool Remove(DictionaryInfo item) { throw new NotSupportedException(); }
            public void RemoveAt(int index) { throw new NotSupportedException(); }
            #endregion
        }
        #endregion
    }
}
#endif
