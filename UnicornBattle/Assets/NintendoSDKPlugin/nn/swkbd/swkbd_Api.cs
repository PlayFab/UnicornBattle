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
using System.Text;

namespace nn.swkbd
{
    public static partial class Swkbd
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static Result ShowKeyboard(StringBuilder outResultString, ShowKeyboardArg showKeyboardArg) { return new Result(); }
        public static Result ShowKeyboard(StringBuilder outResultString, ShowKeyboardArg showKeyboardArg, bool suspendUnityThreads) { return new Result(); }
        public static Result ShowKeyboard(byte[] outResultString, ShowKeyboardArg showKeyboardArg) { return new Result(); }
        public static Result ShowKeyboard(byte[] outResultString, ShowKeyboardArg showKeyboardArg, bool suspendUnityThreads) { return new Result(); }
        public static void InitializeKeyboardConfig(ref KeyboardConfig pOutKeyboardConfig) { }
        public static void MakePreset(ref KeyboardConfig pOutKeyboardConfig, Preset preset) { }
        public static long GetRequiredStringBufferSize() { return 0L; }
        public static void SetLeftOptionalSymbolKey(ref KeyboardConfig pOutKeyboardConfig, char code) { }
        public static void SetLeftOptionalSymbolKeyUtf8(ref KeyboardConfig pOutKeyboardConfig, byte[] code) { }
        public static void SetRightOptionalSymbolKey(ref KeyboardConfig pOutKeyboardConfig, char code) { }
        public static void SetRightOptionalSymbolKeyUtf8(ref KeyboardConfig pOutKeyboardConfig, byte[] code) { }
        public static void SetOkText(ref KeyboardConfig pOutKeyboardConfig, string pStr) { }
        public static void SetOkTextUtf8(ref KeyboardConfig pOutKeyboardConfig, byte[] pStr) { }
        public static void SetHeaderText(ref KeyboardConfig pOutKeyboardConfig, string pStr) { }
        public static void SetHeaderTextUtf8(ref KeyboardConfig pOutKeyboardConfig, byte[] pStr) { }
        public static void SetSubText(ref KeyboardConfig pOutKeyboardConfig, string pStr) { }
        public static void SetSubTextUtf8(ref KeyboardConfig pOutKeyboardConfig, byte[] pStr) { }
        public static void SetGuideText(ref KeyboardConfig pOutKeyboardConfig, string pStr) { }
        public static void SetGuideTextUtf8(ref KeyboardConfig pOutKeyboardConfig, byte[] pStr) { }
        public static void SetInitialText(ref ShowKeyboardArg pOutShowKeyboardArg, string pStr) { }
        public static void SetInitialTextUtf8(ref ShowKeyboardArg pOutShowKeyboardArg, byte[] pStr) { }
        public static void SetUserWordList(ref ShowKeyboardArg pOutShowKeyboardArg, UserWord[] pUserWord, int userWordNum) { }
        public static void SetTextCheckCallback(ref ShowKeyboardArg pOutShowKeyboardArg, TextCheckCallback pCallback) { }
        public static void SetCustomizedDictionaries(ref ShowKeyboardArg pOutShowKeyboardArg, CustomizedDictionarySet dicSet) { }
        public static void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg) { }
        public static void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg, bool useDirectory) { }
        public static void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg, bool useDirectory, bool useTextCheck) { }
        public static void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg, int userWordNum) { }
        public static void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg, int userWordNum, bool useTextCheck) { }
        public static void Destroy(ref ShowKeyboardArg pOutShowKeyboardArg) { }
#else
        public static Result ShowKeyboard(StringBuilder pOutResultString, ShowKeyboardArg showKeyboardArg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = ShowKeyboard(pOutResultString, GetByteSize(pOutResultString), showKeyboardArg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return ShowKeyboard(pOutResultString, GetByteSize(pOutResultString), showKeyboardArg);
        }
        public static Result ShowKeyboard(byte[] pOutResultString, ShowKeyboardArg showKeyboardArg, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Result result = ShowKeyboard(pOutResultString, pOutResultString.LongLength, showKeyboardArg);
                UnityEngine.Switch.Applet.End();
                return result;
            }
#endif
            return ShowKeyboard(pOutResultString, pOutResultString.LongLength, showKeyboardArg);
        }
        public static Result ShowKeyboard(StringBuilder pOutResultString, ShowKeyboardArg showKeyboardArg)
        {
            return ShowKeyboard(pOutResultString, GetByteSize(pOutResultString), showKeyboardArg);
        }
        public static Result ShowKeyboard(byte[] pOutResultString, ShowKeyboardArg showKeyboardArg)
        {
            return ShowKeyboard(pOutResultString, pOutResultString.LongLength, showKeyboardArg);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_ShowKeyboard", CharSet = CharSet.Unicode)]
        private static extern Result ShowKeyboard([In, Out] StringBuilder pOutResultString, long bufSize, ShowKeyboardArg showKeyboardArg);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_ShowKeyboard", CharSet = CharSet.Unicode)]
        private static extern Result ShowKeyboard([In, Out] byte[] pOutResultString, long bufSize, ShowKeyboardArg showKeyboardArg);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_InitializeKeyboardConfig")]
        public static extern void InitializeKeyboardConfig(ref KeyboardConfig pOutKeyboardConfig);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_MakePreset")]
        public static extern void MakePreset(ref KeyboardConfig pOutKeyboardConfig, Preset preset);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_GetRequiredStringBufferSize")]
        public static extern long GetRequiredStringBufferSize();
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetLeftOptionalSymbolKey", CharSet = CharSet.Unicode)]
        public static extern void SetLeftOptionalSymbolKey(ref KeyboardConfig pOutKeyboardConfig, char code);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetLeftOptionalSymbolKeyUtf8")]
        public static extern void SetLeftOptionalSymbolKeyUtf8(ref KeyboardConfig pOutKeyboardConfig, [In] byte[] code);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetRightOptionalSymbolKey", CharSet = CharSet.Unicode)]
        public static extern void SetRightOptionalSymbolKey(ref KeyboardConfig pOutKeyboardConfig, char code);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetRightOptionalSymbolKeyUtf8")]
        public static extern void SetRightOptionalSymbolKeyUtf8(ref KeyboardConfig pOutKeyboardConfig, [In] byte[] code);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetOkText", CharSet = CharSet.Unicode)]
        public static extern void SetOkText(ref KeyboardConfig pOutKeyboardConfig, [In] string pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetOkTextUtf8")]
        public static extern void SetOkTextUtf8(ref KeyboardConfig pOutKeyboardConfig, [In] byte[] pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetHeaderText", CharSet = CharSet.Unicode)]
        public static extern void SetHeaderText(ref KeyboardConfig pOutKeyboardConfig, [In] string pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetHeaderTextUtf8")]
        public static extern void SetHeaderTextUtf8(ref KeyboardConfig pOutKeyboardConfig, [In] byte[] pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetSubText", CharSet = CharSet.Unicode)]
        public static extern void SetSubText(ref KeyboardConfig pOutKeyboardConfig, [In] string pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetSubTextUtf8")]
        public static extern void SetSubTextUtf8(ref KeyboardConfig pOutKeyboardConfig, [In] byte[] pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetGuideText", CharSet = CharSet.Unicode)]
        public static extern void SetGuideText(ref KeyboardConfig pOutKeyboardConfig, [In] string pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetGuideTextUtf8")]
        public static extern void SetGuideTextUtf8(ref KeyboardConfig pOutKeyboardConfig, [In] byte[] pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetInitialText", CharSet = CharSet.Unicode)]
        public static extern void SetInitialText(ref ShowKeyboardArg pOutShowKeyboardArg, [In] string pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetInitialTextUtf8")]
        public static extern void SetInitialTextUtf8(ref ShowKeyboardArg pOutShowKeyboardArg, [In] byte[] pStr);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetUserWordList")]
        public static extern void SetUserWordList(ref ShowKeyboardArg pOutShowKeyboardArg, [In] UserWord[] pUserWord, int userWordNum);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetTextCheckCallback")]
        public static extern void SetTextCheckCallback(ref ShowKeyboardArg pOutShowKeyboardArg, [In] TextCheckCallback pCallback);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_SetCustomizedDictionaries")]
        public static extern void SetCustomizedDictionaries(ref ShowKeyboardArg pOutShowKeyboardArg, CustomizedDictionarySet dicSet);
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_Initialize")]
        public static extern void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg, [MarshalAs(UnmanagedType.U1)] bool useDirectory, [MarshalAs(UnmanagedType.U1)] bool useTextCheck);
        public static void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg, bool useDirectory) { Initialize(ref pOutShowKeyboardArg, useDirectory, false); }
        public static void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg) { Initialize(ref pOutShowKeyboardArg, false, false); }
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_Initialize2")]
        public static extern void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg, int userWordNum, [MarshalAs(UnmanagedType.U1)] bool useTextCheck);
        public static void Initialize(ref ShowKeyboardArg pOutShowKeyboardArg, int userWordNum) { Initialize(ref pOutShowKeyboardArg, userWordNum, false); }
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_swkbd_Destroy")]
        public static extern void Destroy(ref ShowKeyboardArg pOutShowKeyboardArg);

        private static int GetByteSize(StringBuilder sb)
        {
            return sb.Capacity * sizeof(char);
        }
#endif
    }
}
#endif
