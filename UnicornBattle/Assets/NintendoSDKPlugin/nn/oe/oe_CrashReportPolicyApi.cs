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
    public static partial class CrashReport
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static void Enable() { }

        public static void Disable() { }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_oe_EnableCrashReport")]
        public static extern void Enable();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_oe_DisableCrashReport")]
        public static extern void Disable();
#endif
    }
}
#endif
