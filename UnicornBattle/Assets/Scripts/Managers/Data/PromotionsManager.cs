using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Controllers;
using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Managers
{
    /// <summary>
    /// PROMOTIONS MANAGER
    /// ==================
    /// - manages the promotion data for the game (Events, Promo Items, etc)
    /// - manages the News
    /// </summary>
    public class PromotionsManager : DataManager, IPlayerDataRefreshable
    {

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="p_manager"></param>
        public override void Initialize(MainManager p_manager)
        {
            m_ActiveEventKeys = new List<string>();
            base.Initialize(p_manager);
        }

        /// <summary>
        /// Refresh the local data
        /// </summary>
        /// <param name="p_forceRefresh">Force the data to be refreshed from the server</param>
        /// <param name="p_onSuccessCallback">Called if successfully refreshed or if no refresh was necessary</param>
        /// <param name="p_onFailureCallback">Called if an error occurred</param>
        public void Refresh(bool p_forceRefresh,
            System.Action<string> p_onSuccessCallback,
            System.Action<string> p_onFailureCallback)
        {
            //Debug.Log(GetType().ToString() + "::Refresh Called");
            if (IsInitialized == false)
            {
                Initialize(MainManager.Instance);
            }

            if (p_forceRefresh == false)
            {
                if (IsDataCleanAndFresh)
                {
                    if (null != p_onSuccessCallback)
                        p_onSuccessCallback.Invoke("Data Fresh");
                    return;
                }
            }

            // first get list of events --- these are stored in a special catalog
            PlayFabClientAPI.GetCatalogItems(
                new GetCatalogItemsRequest { CatalogVersion = "Events" },
                (GetCatalogItemsResult result) =>
                {
                    m_Events.Clear();
                    foreach (var eachItem in result.Catalog)
                    {
                        var newEvent = new UBEventData
                        {
                            EventName = eachItem.DisplayName,
                            EventDescription = eachItem.Description,
                            StoreToUse = eachItem.ItemClass,
                            BundleId = eachItem.ItemImageUrl,
                            EventKey = eachItem.ItemId
                        };

                        m_Events[eachItem.ItemId] = newEvent;
                    }

                    PlayFabClientAPI.GetStoreItems(
                        new GetStoreItemsRequest { CatalogVersion = "Events", StoreId = "events" },
                        (GetStoreItemsResult result2) =>
                        {
                            // now process the results of the store to determine what events are active
                            m_ActiveEventKeys.Clear();
                            foreach (var eachItem in result2.Store)
                            {
                                m_ActiveEventKeys.Add(eachItem.ItemId);
                            }

                            BuildCDNRequests();
                            RefreshTitleNews(15, p_onSuccessCallback, p_onFailureCallback);
                        },
                        (PlayFabError error) =>
                        {
                            if (null != p_onFailureCallback)
                                p_onFailureCallback.Invoke(error.ErrorMessage);
                        }
                    );
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Refresh just the title News
        /// </summary>
        /// <param name="p_entriesCount"></param>
        /// <param name="p_onSuccessCallback"></param>
        /// <param name="p_onFailureCallback"></param>
        public void RefreshTitleNews(int p_entriesCount,
            System.Action<string> p_onSuccessCallback = null,
            System.Action<string> p_onFailureCallback = null)
        {
            var request = new GetTitleNewsRequest { Count = p_entriesCount };
            PlayFabClientAPI.GetTitleNews(request,
                (GetTitleNewsResult result) =>
                {
                    m_promoItems.Clear();

                    foreach (var item in result.News)
                    {
                        var endTagsIndex = item.Title.LastIndexOf('}');
                        m_promoItems.Add(new UBPromotionalItem
                        {
                            TimeStamp = item.Timestamp,
                                PromoBody = item.Body,
                                PromoTitle = item.Title.Substring(endTagsIndex + 1),
                                PromoType = PromotionalItemTypes.Tip
                        });
                    }

                    DataRefreshed();
                    if (p_onSuccessCallback != null)
                        p_onSuccessCallback.Invoke("Title News Loaded");
                },
                (PlayFabError e) =>
                {
                    if (null != p_onFailureCallback)
                        p_onFailureCallback.Invoke(e.ErrorMessage);
                }
            );
        }

        /// <summary>
        /// Get the Active Event data as an array
        /// </summary>
        /// <returns>a string array of event keys</returns>
        private string[] GetActiveEventData()
        {
            return m_ActiveEventKeys.ToArray();
        }

        /// <summary>
        /// What is the status of the event?
        /// </summary>
        /// <param name="eventKey">The event's id/key</param>
        /// <returns>Active, Inactive, or ActivePromoted</returns>
        public PromotionStatus IsEventActive(string eventKey)
        {
            if (string.IsNullOrEmpty(eventKey))
                return PromotionStatus.Active;
            foreach (var eachKey in m_ActiveEventKeys)
                if (eventKey == eachKey)
                    return PromotionStatus.Active;
            return PromotionStatus.Inactive;
        }

        /// <summary>
        /// Get the event sale store
        /// </summary>
        /// <param name="eventKey">event key</param>
        /// <returns>string ID of the store</returns>
        public string GetEventSaleStore(string eventKey)
        {
            UBEventData eventData;
            if (string.IsNullOrEmpty(eventKey) || IsEventActive(eventKey) != PromotionStatus.Active || !m_Events.TryGetValue(eventKey, out eventData))
                return null;

            // To match previous usage, this just returns the first one - Later we should allow multiple
            return string.IsNullOrEmpty(eventData.StoreToUse) ? null : eventData.StoreToUse;
        }

        /// <summary>
        /// Is there any active promotions?
        /// </summary>
        /// <returns></returns>
        public bool HasAnyPromotions()
        {
            return (m_PromoAssets.Count > 0);
        }

        /// <summary>
        /// Gets all promotional assets as an array
        /// </summary>
        /// <returns>UBUnpackedAssetBundle array</returns>
        public UBUnpackedAssetBundle[] GetAllPromoAssets()
        {
            return m_PromoAssets.ToArray();
        }

        /// <summary>
        /// Gets event data by ID
        /// </summary>
        /// <param name="p_id">event ID</param>
        /// <returns>UBEventData object</returns>
        public UBEventData GetUBEventData(string p_id)
        {
            if (m_Events.ContainsKey(p_id))
                return m_Events[p_id];
            else
                return null;
        }

        /// <summary>
        /// Get a list of Promotional Items by type
        /// </summary>
        /// <param name="p_filter">Type</param>
        /// <returns>List of UBPromotionalItems</returns>
        public List<UBPromotionalItem> GetUBPromotionalItems(PromotionalItemTypes p_filter)
        {
            if (p_filter == PromotionalItemTypes.All)
            {
                return m_promoItems.ToList();
            }
            return m_promoItems.FindAll((i) => { return i.PromoType == p_filter; }).ToList();
        }

        /// <summary>
        /// Builds the CDN request
        /// </summary>
        private void BuildCDNRequests()
        {

            List<AssetBundleHelperObject> requests = new List<AssetBundleHelperObject>();
            var mime = "application/x-gzip";
            var keyPrefix = string.Empty;

            if (Application.platform == RuntimePlatform.Android)
                keyPrefix = "Android/";
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                keyPrefix = "iOS/";
            else if (Application.platform == RuntimePlatform.Switch)
                keyPrefix = "Switch/";

            foreach (var ev in m_Events)
            {
                if (IsEventActive(ev.Key) == PromotionStatus.Active && !string.IsNullOrEmpty(ev.Value.BundleId))
                {
                    var l_abho = new AssetBundleHelperObject
                    {
                    MimeType = mime,
                    ContentKey = keyPrefix + ev.Value.BundleId,
                    FileName = ev.Key,
                    Unpacked = new UBUnpackedAssetBundle()
                    };

                    requests.Add(l_abho);
                }
                else
                {
                    //Debug.LogWarning("Rejected: " + ev.Key + ", " + ev.Value.EventName);
                }
            }

            GameController.Instance.cdnController.assets = requests;

            UnityEngine.Events.UnityAction<bool> afterCdnRequest = response =>
            {
                if (response)
                {
                    PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetCDNContent);
                    //Debug.Log("Assets Retrieved! From CDN? " + GameController.Instance.cdnController.useCDN);
                }

                m_PromoAssets.Clear();
                foreach (var obj in requests)
                    if (obj.IsUnpacked)
                    {
                        m_PromoAssets.Add(obj.Unpacked);
                    }

                //Debug.Log("requests:\n" + requests.WriteToString());
                //Debug.Log("m_PromoAssets:\n" + m_PromoAssets.WriteToString());
                GameController.Instance.cdnController.isInitialContentUnpacked = true;
            };

            if (GameController.Instance.cdnController.isInitialContentUnpacked == false && GameController.Instance.cdnController.useCDN)
            {
                //Debug.Log("Getting assets from CDN");
                DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetCDNContent);
                GameController.Instance.cdnController.KickOffCDNGet(requests, afterCdnRequest);
            }
            else
            {
                //Debug.Log(GetType().ToString() + " -> Getting Assets from Streaming Folder");
                GameController.Instance.cdnController.LoadBundlesFromStreamingAssets(requests, afterCdnRequest);
            }

        }

        /// DATA
        private List<string> m_ActiveEventKeys;
        private List<UBPromotionalItem> m_promoItems = new List<UBPromotionalItem>();
        private List<UBUnpackedAssetBundle> m_PromoAssets = new List<UBUnpackedAssetBundle>();
        private Dictionary<string, UBEventData> m_Events = new Dictionary<string, UBEventData>();
        private List<CatalogItem> m_offersCatalogItems = new List<CatalogItem>();
    }
}