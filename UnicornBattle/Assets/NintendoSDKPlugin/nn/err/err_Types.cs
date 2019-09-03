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

namespace nn.err
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ErrorCode
    {
        public uint category;
        public uint number;

        public override string ToString()
        {
            return string.Format("(0x{0,0:X8} 0x{1,0:X8})", category, number);
        }

#if !UNITY_SWITCH || UNITY_EDITOR
        public bool IsValid()
        {
            return true;
        }

        public static ErrorCode GetInvalidErrorCode()
        {
            return new ErrorCode();
        }
#else
        public bool IsValid()
        {
            return ErrorCode.IsValid(this);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_err_ErrorCode_IsValid")]
        [return: MarshalAs(UnmanagedType.U1)]
        private static extern bool IsValid(ErrorCode errorCode);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_err_ErrorCode_GetInvalidErrorCode")]
        public static extern ErrorCode GetInvalidErrorCode();
#endif
    }
}
#endif
