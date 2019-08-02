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

namespace nn.ec
{
#if !UNITY_SWITCH || UNITY_EDITOR
    [StructLayout(LayoutKind.Sequential)]
    public struct PurchasedItemInfo
    {
        public enum Type : int
        {
            Subscription = 0,
            Consumable = 1,
        }

        public Type type;
        public nn.account.NetworkServiceAccountId nsaId;

        public nn.ec.CourseId GetCourseId() { return new CourseId(); }

        internal nn.ec.CourseId _courseId;
    }

    public static partial class PurchasedEvent
    {
        public static void Initialize() { }

        public static bool PopPurchasedItemInfo(ref PurchasedItemInfo pOut) { return false; }
    }
#else
    [StructLayout(LayoutKind.Sequential)]
    public struct PurchasedItemInfo
    {
        public enum Type : int
        {
            Subscription = 0,
            Consumable = 1,
        }

        public Type type;
        public nn.account.NetworkServiceAccountId nsaId;

        public nn.ec.CourseId GetCourseId()
        {
            return GetCourseId(this);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_PurchasedItemInfo_GetCourseId")]
        private static extern nn.ec.CourseId GetCourseId(PurchasedItemInfo info);

        internal nn.ec.CourseId _courseId;
    }

    public static partial class PurchasedEvent
    {
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_InitializePurchasedEvent")]
        public static extern void Initialize();
        
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_PopPurchasedItemInfo")]
        public static extern bool PopPurchasedItemInfo(ref PurchasedItemInfo pOut);
    }
#endif
}
#endif