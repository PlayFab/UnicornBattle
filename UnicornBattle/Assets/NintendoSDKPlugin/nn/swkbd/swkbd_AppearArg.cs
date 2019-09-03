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
using System.Runtime.InteropServices;
using System;

namespace nn.swkbd
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct KeyboardConfig
    {
        public KeyboardMode keyboardMode;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Swkbd.OkTextMaxLength + 1)]
        public string okText;
        [MarshalAs(UnmanagedType.U2)]
        public char leftOptionalSymbolKey;
        [MarshalAs(UnmanagedType.U2)]
        public char rightOptionalSymbolKey;
        [MarshalAs(UnmanagedType.U1)]
        public bool isPredictionEnabled;
        public InvalidChar invalidCharFlag;
        public InitialCursorPos initialCursorPos;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Swkbd.HeaderTextMaxLength + 1)]
        public string headerText;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Swkbd.SubTextMaxLength + 1)]
        public string subText;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Swkbd.GuideTextMaxLength + 1)]
        public string guideText;
        public int textMaxLength;
        public int textMinLength;
        public PasswordMode passwordMode;
        public InputFormMode inputFormMode;
        [MarshalAs(UnmanagedType.U1)]
        public bool isUseNewLine;
        [MarshalAs(UnmanagedType.U1)]
        public bool isUseUtf8;
        [MarshalAs(UnmanagedType.U1)]
        public bool isUseBlurBackground;

        private int _initialStringOffset;
        private int _initialStringLength;
        private int _userDictionaryOffset;
        private int _userDictionaryNum;
        [MarshalAs(UnmanagedType.U1)]
        private bool _isUseTextCheck;
        private IntPtr _textCheckCallback;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Swkbd.SepareteTextPosMax)]
        public int[] separateTextPos;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = Swkbd.CustomizedDicionarySetMax)]
        private DictionaryInfo[] _customizedDicInfoList;
        private byte _customizedDicCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        private byte[] _reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ShowKeyboardArg
    {
        public KeyboardConfig keyboardConfig;
        public IntPtr workBuf;
        public long workBufSize;
        public IntPtr textCheckWorkBuf;
        public long textCheckWorkBufSize;
        private IntPtr _customizeDicBuf;
        private long _customizeDicBufSize;
    }
}
#endif
