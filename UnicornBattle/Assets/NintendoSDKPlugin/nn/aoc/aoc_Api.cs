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

namespace nn.aoc
{
    public static partial class Aoc
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static int CountAddOnContent()
        {
            return 0;
        }

        public static int ListAddOnContent(int[] outIndices, int offset, int count)
        {
            return 0;
        }

        public static void GetAddOnContentListChangedEvent()
        {
        }

        public static bool IsAddOnContentListChanged()
        {
            return false;
        }

        public static void DestroyAddOnContentListChangedEvent()
        {
        }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_aoc_CountAddOnContent")]
        public static extern int CountAddOnContent();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_aoc_ListAddOnContent")]
        public static extern int ListAddOnContent(int[] outIndices, int offset, int count);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_aoc_GetAddOnContentListChangedEvent")]
        public static extern void GetAddOnContentListChangedEvent();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_aoc_IsAddOnContentListChanged")]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool IsAddOnContentListChanged();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_aoc_DestroyAddOnContentListChangedEvent")]
        public static extern void DestroyAddOnContentListChangedEvent();
#endif
    }
}
#endif
