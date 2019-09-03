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

namespace nn.oe
{
    public static partial class Language
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static string GetDesired()
        {
            return string.Empty;
        }
#else
        private struct _LanguageCode
        {
#pragma warning disable 649
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            internal char[] _string;
#pragma warning restore 649
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_oe_GetDesiredLanguage")]
        private static extern _LanguageCode _GetDesired();

        public static string GetDesired()
        {
            _LanguageCode languageCode = _GetDesired();
            int length = 0;
            for (; length < 7; length++)
            {
                if (languageCode._string[length] == '\0')
                {
					break;
                }
            }
			return new string(languageCode._string, 0, length);
        }
#endif
    }
}
#endif
