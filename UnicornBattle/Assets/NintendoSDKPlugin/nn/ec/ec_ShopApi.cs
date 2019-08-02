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
    public static partial class Shop
    {
#if !UNITY_SWITCH || UNITY_EDITOR
        public static void ShowApplicationInformation(ulong applicationId) { }
        public static void ShowApplicationInformation(ulong applicationId, nn.account.UserHandle selectedUser) { }
        public static void ShowAddOnContentList(ulong applicationId) { }
        public static void ShowAddOnContentList(ulong applicationId, nn.account.UserHandle selectedUser) { }
        public static void ShowSubscriptionList(ulong applicationId) { }
        public static void ShowSubscriptionList(ulong applicationId, nn.account.UserHandle selectedUser) { }
        public static void ShowSubscriptionList(ulong applicationId, CourseId courseId) { }
        public static void ShowSubscriptionList(ulong applicationId, CourseId courseId, nn.account.UserHandle selectedUser) { }
        public static void ShowConsumableItemList(ulong applicationId) { }
        public static void ShowConsumableItemList(ulong applicationId, nn.account.UserHandle selectedUser) { }
        public static void ShowConsumableItemDetail(ulong applicationId, ConsumableId consumableId, NsUid nsUid) { }
        public static void ShowConsumableItemDetail(ulong applicationId, ConsumableId consumableId, NsUid nsUid, nn.account.UserHandle selectedUser) { }
        public static void ShowEnterCodeScene() { }
        public static void ShowEnterCodeScene(nn.account.UserHandle selectedUser) { }
        public static void ShowShopProductDetails(nn.ec.NsUid nsuid) { }
        public static void ShowShopProductDetails(nn.ec.NsUid nsuid, nn.account.UserHandle selectedUser) { }
        public static void ShowShopProductList(nn.ec.NsUid[] nsuidList, string listName) { }
        public static void ShowShopProductList(nn.ec.NsUid[] nsuidList, string listName, nn.account.UserHandle selectedUser) { }
#else
        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopApplicationInformation1")]
        public static extern void ShowApplicationInformation(
            ulong applicationId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopApplicationInformation2")]
        public static extern void ShowApplicationInformation(
            ulong applicationId, nn.account.UserHandle selectedUser);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopAddOnContentList1")]
        public static extern void ShowAddOnContentList(
            ulong applicationId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopAddOnContentList2")]
        public static extern void ShowAddOnContentList(
            ulong applicationId, nn.account.UserHandle selectedUser);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopSubscriptionList1")]
        public static extern void ShowSubscriptionList(
            ulong applicationId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopSubscriptionList2")]
        public static extern void ShowSubscriptionList(
            ulong applicationId, nn.account.UserHandle selectedUser);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
         EntryPoint = "nn_ec_ShowShopSubscriptionList3")]
        public static extern void ShowSubscriptionList(
            ulong applicationId, CourseId courseId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopSubscriptionList4")]
        public static extern void ShowSubscriptionList(
            ulong applicationId, CourseId courseId,
            nn.account.UserHandle selectedUser);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopConsumableItemList1")]
        public static extern void ShowConsumableItemList(
            ulong applicationId);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopConsumableItemList2")]
        public static extern void ShowConsumableItemList(
            ulong applicationId, nn.account.UserHandle selectedUser);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopConsumableItemDetail1")]
        public static extern void ShowConsumableItemDetail(
            ulong applicationId, ConsumableId consumableId, NsUid nsUid);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopConsumableItemDetail2")]
        public static extern void ShowConsumableItemDetail(
            ulong applicationId, ConsumableId consumableId,
            NsUid nsUid, nn.account.UserHandle selectedUser);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowEnterCodeScene1")]
        public static extern void ShowEnterCodeScene();

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowEnterCodeScene2")]
        public static extern void ShowEnterCodeScene(nn.account.UserHandle selectedUser);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopProductDetails1")]
        public static extern void ShowShopProductDetails(nn.ec.NsUid nsuid);

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopProductDetails2")]
        public static extern void ShowShopProductDetails(
            nn.ec.NsUid nsuid, nn.account.UserHandle selectedUser);

        public static void ShowShopProductList(nn.ec.NsUid[] nsuidList, string listName)
        {
            ShowShopProductList(nsuidList, nsuidList.Length, listName);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopProductList1")]
        private static extern void ShowShopProductList(nn.ec.NsUid[] nsuidList, int nsuidCount, string listName);

        public static void ShowShopProductList(nn.ec.NsUid[] nsuidList, string listName, nn.account.UserHandle selectedUser)
        {
            ShowShopProductList(nsuidList, nsuidList.Length, listName, selectedUser);
        }

        [DllImport(Nn.DllName,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "nn_ec_ShowShopProductList2")]
        private static extern void ShowShopProductList(nn.ec.NsUid[] nsuidList, int nsuidCount, string listName, nn.account.UserHandle selectedUser);
#endif
    }
}
#endif