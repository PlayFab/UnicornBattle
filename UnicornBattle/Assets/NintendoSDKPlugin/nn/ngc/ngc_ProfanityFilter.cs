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
using System.Runtime.InteropServices;

namespace nn.ngc
{
    public sealed partial class ProfanityFilter : IDisposable
    {
        public enum MaskMode
        {
            OverWrite = 0,
            ReplaceByOneCharacter = 1,
        }

        public enum SkipMode
        {
            NotSkip = 0,
            SkipAtSign = 1,
        }

        private IntPtr _profanityFilter = IntPtr.Zero;
        private IntPtr _ngcWorkBuffer = IntPtr.Zero;

        public ProfanityFilter()
        {
            nn.Result result = Initialize(ref _profanityFilter, ref _ngcWorkBuffer, true);
            result.abortUnlessSuccess();
        }

        public ProfanityFilter(bool checkDesiredLanguage)
        {
            nn.Result result = Initialize(ref _profanityFilter, ref _ngcWorkBuffer, checkDesiredLanguage);
            result.abortUnlessSuccess();
        }

        ~ProfanityFilter()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_profanityFilter != IntPtr.Zero && _ngcWorkBuffer != IntPtr.Zero)
            {
                Destroy(_profanityFilter, _ngcWorkBuffer);
                _profanityFilter = IntPtr.Zero;
                _ngcWorkBuffer = IntPtr.Zero;
            }
        }

        public uint GetContentVersion()
        {
            return GetContentVersion(_profanityFilter);
        }

        public Result CheckProfanityWords([Out] PatternList[] checkResults, PatternList patterns, string[] words)
        {
            return CheckProfanityWords(_profanityFilter, checkResults, patterns, words, words.Length);
        }

        public Result MaskProfanityWordsInText(ref int profanityWordCount, string inText, out string outText, PatternList patterns)
        {
            outText = string.Copy(inText);
            Result result = MaskProfanityWordsInText(_profanityFilter, ref profanityWordCount, outText, patterns);
            outText = outText.TrimEnd('\0');
            return result;
        }

        public void SetMaskMode(MaskMode mode)
        {
            SetMaskMode(_profanityFilter, mode);
        }

        public void SkipAtSignCheck(SkipMode skipMode)
        {
            SkipAtSignCheck(_profanityFilter, skipMode);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ngc_ProfanityFilterDestroy")]
        private static extern void Destroy(IntPtr profanityFilter, IntPtr ngcWorkBuffer);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ngc_ProfanityFilterInitialize")]
        private static extern Result Initialize(
            ref IntPtr profanityFilter, ref IntPtr ngcWorkBuffer,
            [MarshalAs(UnmanagedType.U1)] bool checkDesiredLanguage);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ngc_GetContentVersion")]
        private static extern uint GetContentVersion(IntPtr profanityFilter);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ngc_CheckProfanityWords",
            CharSet = CharSet.Unicode)]
        private static extern Result CheckProfanityWords(
            IntPtr profanityFilter, [Out] PatternList[] checkResults, PatternList patterns,
            [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr)]
            string[] words, long wordCount);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ngc_MaskProfanityWordsInText",
            CharSet = CharSet.Unicode)]
        private static extern Result MaskProfanityWordsInText(
            IntPtr profanityFilter, ref int profanityWordCount,
            string text, PatternList patterns);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ngc_SetMaskMode")]
        private static extern void SetMaskMode(IntPtr profanityFilter, MaskMode mode);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ngc_SkipAtSignCheck")]
        private static extern void SkipAtSignCheck(IntPtr profanityFilter, SkipMode skipMode);
    }
}
#endif
