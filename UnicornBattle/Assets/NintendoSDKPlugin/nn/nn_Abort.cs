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

namespace nn
{
    public static partial class Nn
    {
#if !UNITY_EDITOR
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_Abort")]
        public static extern void Abort(string message);
#else
        public static void Abort(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
#endif
        public static void Abort(string message, params object[] args)
        {
            Abort(string.Format(message, args));
        }

        public static void Abort(bool condition, string message)
        {
            if (!condition) { Abort(message); }
        }

        public static void Abort(bool condition, string message, params object[] args)
        {
            if (!condition) { Abort(string.Format(message, args)); }
        }
    }
}
#endif
