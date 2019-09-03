using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// STORE MANAGER
    /// =============
    /// Encapsulates and manages all the store data 
    /// </summary>
    public class StoreManager : DataManager, ITitleDataLoadable
    {

        public override void Initialize(MainManager p_manager)
        {
            m_StandardStores = new List<string>();
            base.Initialize(p_manager);
        }

        /// <summary>
        /// store the relevent title data into this manager
        /// </summary>
        /// <param name="p_titleData"></param>
        public void LoadTitleData(Dictionary<string, string> p_titleData)
        {
            ExtractJsonTitleData(p_titleData, "StandardStores", ref m_StandardStores);
        }

        /// <summary>
        /// Retrieves the store items.
        /// </summary>
        /// <param name="storeName">Store name.</param>
        /// <param name="callback">Callback.</param>
        public void RetrieveStoreItems(string storeName, System.Action<GetStoreItemsResult> callback = null)
        {
            if (string.IsNullOrEmpty(storeName))
                Debug.LogError("storeName is null or empty");

            var request = new GetStoreItemsRequest
            {
                StoreId = storeName,
                CatalogVersion = GlobalStrings.PrimaryCatalogName,
            };
            PlayFabClientAPI.GetStoreItems(
                request,
                result =>
                {
                    if (callback != null)
                        callback(result);

                    PF_Bridge.RaiseCallbackSuccess("Store Retrieved", PlayFabAPIMethods.GetStoreItems);
                },
                (PlayFabError e) =>
                {
                    PF_Bridge.RaiseCallbackError(e.ErrorMessage, PlayFabAPIMethods.GetStoreItems);
                }
            );
        }

        /// <summary>
        /// Purchase an item
        /// </summary>
        /// <param name="p_item">CatalogItem</param>
        /// <param name="p_storeId">store id</param>
        /// <param name="p_currencyKey">currency key used</param>
        /// <param name="p_currencyValue">currency amount</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void PurchaseItem(UBCatalogItem p_item,
            string p_storeId,
            string p_currencyKey,
            uint p_currencyValue,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            // using real money currency, redirect to OnePF
            if (p_currencyKey == GlobalStrings.REAL_MONEY_CURRENCY)
            {
                PF_Bridge.IAB_CurrencyCode = "US";
                PF_Bridge.IAB_Price = (int) p_currencyValue;

                if (Application.platform == RuntimePlatform.Android
                    || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    OnePF.OpenIAB.purchaseProduct(p_item.ItemId);
                }
                else
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke("Current plaform does not support IAP; cannot process transaction.");
                }

                if (null != p_onSuccessCallback)
                    p_onSuccessCallback.Invoke(string.Empty);
                return;
            }

            // normal purchase item flow
            var request = new PurchaseItemRequest
            {
                ItemId = p_item.ItemId,
                VirtualCurrency = p_currencyKey,
                Price = (int) p_currencyValue,
                StoreId = p_storeId
            };

            PlayFabClientAPI.PurchaseItem(
                request,
                (PurchaseItemResult result) =>
                {
                    Debug.Log(string.Format("{0} Items Purchased!", result.Items.Count));

                    FlagAsDirty();
                    var l_characterMgr = mainManager.getCharacterManager();
                    if (null != l_characterMgr)
                    {
                        l_characterMgr.Refresh(true, p_onSuccessCallback, p_onFailureCallback);
                    }
                    else
                    {
                        if (null != p_onSuccessCallback)
                            p_onSuccessCallback.Invoke(string.Empty);
                    }
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Get an array of strings of the Standard Stores
        /// </summary>
        /// <returns></returns>
        public string GetStandardStore(int index)
        {
            return m_StandardStores[index];
        }

        /// <summary>
        /// Redeem a Coupon
        /// </summary>
        /// <param name="p_couponCode">Coupon code</param>
        /// <param name="p_onSuccessCallback">Called if successful</param>
        /// <param name="p_onFailureCallback">Called if an error occurs</param>
        public void RedeemCoupon(string p_couponCode,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            PlayFabClientAPI.RedeemCoupon(
                new RedeemCouponRequest()
                {
                    CouponCode = p_couponCode
                },
                (RedeemCouponResult result) =>
                {
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke("Coupon (" + p_couponCode + ") Redeemed; Granted " + result.GrantedItems.Count + " items.");
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        // private local data
        private List<string> m_StandardStores;
    }
}