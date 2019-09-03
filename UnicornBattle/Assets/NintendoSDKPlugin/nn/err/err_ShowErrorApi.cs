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

namespace nn.err
{
    public static partial class Error
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static void Show(nn.Result result)
        {
        }

        public static void Show(ErrorCode errorCode)
        {
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_err_ShowError_Result")]
        public static extern void Show(nn.Result result);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_err_ShowError_ErrorCode")]
        public static extern void Show(ErrorCode errorCode);
#endif

        public static void Show(nn.Result result, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Show(result);
                UnityEngine.Switch.Applet.End();
                return;
            }
#endif
            Show(result);
        }

        public static void Show(ErrorCode errorCode, bool suspendUnityThreads)
        {
#if UNITY_SWITCH && ENABLE_IL2CPP
            if (suspendUnityThreads)
            {
                UnityEngine.Switch.Applet.Begin();
                Show(errorCode);
                UnityEngine.Switch.Applet.End();
                return;
            }
#endif
            Show(errorCode);
        }
    }
}
#endif
