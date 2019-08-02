using System;
using System.Collections.Generic;
using PlayFab;
using UnicornBattle.Managers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class PromotionController : MonoBehaviour
    {
        public Transform AdSlotCounterBar;

        private List<LayoutElement> adSlots = new List<LayoutElement>();
        public List<UB_PromoDisplay> promos = new List<UB_PromoDisplay>();
        public UB_PromoDisplay activePromo = null;

        public GameObject AdObject, PrvAd, NextAd, PlayEventBtn, ViewSaleBtn, WatchAdBtn;

        public LayoutElement SlotEmpty;
        public Sprite SlotSelected;
        public Image PromoBanner;
        public Texture2D defaultVideoBanner;
        public Text selectedTitle, selectedDesc;

        private float _timeSinceMove = 99999999f;
        private float rotateDelay = 8f;
        private float _watchCD = 0.5f;
        private float _watchLastClickedAt = 0;
        private int displayPromoIndex = 0;

        private LevelPicker _levelPicker;

        [System.NonSerialized] private PromotionsManager m_PromotionsManager;
        [System.NonSerialized] private GameDataManager m_gameDataManager;

        // Use this for initialization
        void Start()
        {
            // Set up the default display
            SetAdSlotCount(1);
            SelectBanner();
            _levelPicker = FindObjectOfType<LevelPicker>();
        }

        void OnEnable()
        {
            // SupersonicEvents.OnAdRewarded += EvaluateAdState;
            InvokeRepeating("EvaluateAdState", 60, 300); //start after 1m, repeat every 5m
        }

        void OnDisable()
        {
            // SupersonicEvents.OnAdRewarded -= EvaluateAdState;
            CancelInvoke("EvaluateAdState");
        }

        #region ****** PLAYFAB CALLBACKS ******
        public void CheckForPlayFabPlacement(string placement = null)
        {
            Action<PlayFabError> errorCb = error =>
            {
                if (error.GenerateErrorReport().Contains("AllAdPlacementViewsAlreadyConsumed"))
                {
                    //PF_Bridge.RaiseCallbackError(string.Format("No more rewarded ads can be viewed at this time. Check back in {0} minutes",  ), PlayFabAPIMethods.Generic, MessageDisplayStyle.error);
                    var l_adPromo = promos.Find((p) => { return p.linkedAd != null; });

                    if (l_adPromo != null)
                    {
                        Debug.LogError(string.IsNullOrEmpty(l_adPromo.assets.PromoId) ? "AdPromo was not null... removing: " + l_adPromo.assets.PromoId : "Unknown");
                        promos.Remove(l_adPromo);
                        SetAdSlotCount(promos.Count);
                        SelectBanner();
                        Debug.LogError("Promo Count: " + promos.Count);
                    }
                    else
                    {
                        Debug.LogError("AdPromo not in rotation.");
                    }
                }
                else
                {
                    PF_Bridge.RaiseCallbackError(error.GenerateErrorReport(), PlayFabAPIMethods.Generic);
                }
            };

            // SupersonicEvents.queuedAd = null;

            if (placement != null)
            {
                // PF_Advertising.GetAdPlacements(new GetAdPlacementsRequest { Identifier = new NameIdentifier() { Name = placement }, AppId = SupersonicEvents.appKey }, OnCheckForPlayFabPlacementSuccess, errorCb);
                PF_Advertising.GetAdPlacements(new GetAdPlacementsRequest { Identifier = new NameIdentifier() { Name = placement }, AppId = "0" }, OnCheckForPlayFabPlacementSuccess, errorCb);
            }
            else
            {
                // PF_Advertising.GetAdPlacements(new GetAdPlacementsRequest { AppId = SupersonicEvents.appKey }, OnCheckForPlayFabPlacementSuccess, errorCb);
                PF_Advertising.GetAdPlacements(new GetAdPlacementsRequest { AppId = "0" }, OnCheckForPlayFabPlacementSuccess, errorCb);
            }
        }

        public void OnCheckForPlayFabPlacementSuccess(GetAdPlacementsResult result)
        {
            if ((result.AdPlacements != null && result.AdPlacements.Length > 0) && (result.AdPlacements[0].PlacementViewsRemaining == null || result.AdPlacements[0].PlacementViewsRemaining > 0))
            {
                var l_adPromo = promos.Find((p) => { return p.linkedAd != null && p.assets.PromoId == result.AdPlacements[0].PlacementId; });

                if (l_adPromo == null)
                {
                    AddAdPromo(result.AdPlacements[0]);
                    //SelectBanner(promos[0], 0);
                }
                else
                {
                    l_adPromo.linkedAd.Details.PlacementViewsRemaining = result.AdPlacements[0].PlacementViewsRemaining;
                }

                if (activePromo.assets.PromoId == result.AdPlacements[0].PlacementId)
                {
                    var qtyString = string.Format("{0}", result.AdPlacements[0].PlacementViewsRemaining == null ? "UNLIMITED rewarded ads." : result.AdPlacements[0].PlacementViewsRemaining + " more rewarded ads.");
                    selectedDesc.text = result.AdPlacements[0].RewardDescription + " You may watch " + qtyString;
                }
            }
            else
            {
                var l_adPromo = promos.Find((p) => { return p.linkedAd != null && p.assets.PromoId == result.AdPlacements[0].PlacementId; });
                if (l_adPromo != null)
                {
                    promos.Remove(l_adPromo);
                    SetAdSlotCount(promos.Count);
                    SelectBanner();
                }
            }
        }
        #endregion

        void EvaluateAdState(RewardAdActivityResponse result = null)
        {
            //Debug.Log("EvaluateAdState Called.");
            CheckForPlayFabPlacement();
        }

        public void Init()
        {
            if (null == m_PromotionsManager)
                m_PromotionsManager = MainManager.Instance.getPromotionsManager();

            if (null == m_gameDataManager)
                m_gameDataManager = MainManager.Instance.getGameDataManager();

            _timeSinceMove = Time.time;

            m_PromotionsManager.Refresh(false,
                (s) =>
                {
                    if (m_PromotionsManager.HasAnyPromotions())
                    {
                        promos.Clear();
                        foreach (var item in m_PromotionsManager.GetAllPromoAssets())
                        {
                            var promo = new UB_PromoDisplay { assets = item };

                            UBEventData ev = m_PromotionsManager.GetUBEventData(item.PromoId);
                            if (null != ev)
                            {
                                promo.EventKey = ev.EventKey;
                                promo.Title = ev.EventName;
                                promo.Description = ev.EventDescription;
                                promos.Add(promo);
                            }
                        }

                        SetAdSlotCount(promos.Count);
                        SelectBanner();
                        CheckForPlayFabPlacement();
                    }
                },
                null);

        }

        // Update is called once per frame
        void Update()
        {
            if (_timeSinceMove + rotateDelay < Time.time && promos.Count > 1)
                NextBanner();
        }

        public void NextBanner()
        {
            _timeSinceMove = Time.time;
            displayPromoIndex += 1;

            UnityAction afterFadeOut = () =>
            {
                SelectBanner();
                FadeAdsIn();
            };
            FadeAdsOut(afterFadeOut);
        }

        public void PrevBanner()
        {
            _timeSinceMove = Time.time;
            displayPromoIndex += promos.Count - 1;

            UnityAction afterFadeOut = () =>
            {
                SelectBanner();
                FadeAdsIn();
            };
            FadeAdsOut(afterFadeOut);
        }

        public void FadeAdsOut(UnityAction callback = null)
        {
            UBAnimator.OutroPane(AdObject, .5f, () =>
            {
                if (callback != null)
                    callback();
            });
        }

        public void FadeAdsIn(UnityAction callback = null)
        {
            UBAnimator.IntroPane(AdObject, .5f, () =>
            {
                if (callback != null)
                    callback();
            });
        }

        public void ViewSale()
        {
            if (null == m_PromotionsManager) return;
            var storeId = m_PromotionsManager.GetEventSaleStore(activePromo.EventKey);
            DialogCanvasController.RequestStore(storeId);

            Dictionary<string, object> eventData = new Dictionary<string, object>
                { { "SalePromo", activePromo.assets.PromoId },
                    { "Character_ID", GameController.Instance.ActiveCharacter.CharacterId }
                };

            TelemetryManager.RecordPlayerEvent(TelemetryEvent.Client_SaleClicked, eventData);
        }

        public void PlayEvent()
        {
            if (null == m_gameDataManager) return;
            var l_levelNames = m_gameDataManager.GetEventAssociatedLevelNames(activePromo.EventKey);

            UnityAction<int> playEventIndex = index =>
            {
                _levelPicker.LevelItemClicked(l_levelNames[index]);
                _levelPicker.PlaySelectedLevel();
            };

            if (l_levelNames.Length > 1)
                DialogCanvasController.RequestSelectorPrompt(GlobalStrings.QUEST_SELECTOR_PROMPT, l_levelNames, playEventIndex);
            else if (l_levelNames.Length == 1)
                playEventIndex(0);
        }

        public void WatchAd()
        {

            //if (activePromo != null && activePromo.linkedAd != null && SupersonicEvents.rewardedVideoAvailability && Time.time > _watchLastClickedAt + _watchCD)
            if (activePromo != null && activePromo.linkedAd != null && false && Time.time > _watchLastClickedAt + _watchCD)
            {
                _watchLastClickedAt = Time.time;
                // SupersonicEvents.ShowRewardedVideo(activePromo.linkedAd.Details);
            }
            else
            {
                PF_Bridge.RaiseCallbackError("Rewarded ad is unavailable at this time. Please try again later.", PlayFabAPIMethods.Generic);
            }
        }

        public void SelectBanner()
        {
            if (null == m_gameDataManager) return;

            displayPromoIndex = promos.Count == 0 ? 0 : displayPromoIndex % promos.Count;
            activePromo = promos.Count == 0 ? UB_PromoDisplay.EMPTY_PROMO : promos[displayPromoIndex];
            AdjustSlotDisplay(displayPromoIndex);

            ViewSaleBtn.SetActive(!string.IsNullOrEmpty(m_PromotionsManager.GetEventSaleStore(activePromo.EventKey)));
            PlayEventBtn.SetActive(activePromo.linkedAd == null && m_gameDataManager.HasAnyEventAssociatedLevels(activePromo.EventKey));
            WatchAdBtn.SetActive(activePromo.linkedAd != null);

            if (activePromo.EventKey != null) // Event Type
            {
                selectedDesc.text = activePromo.Description;
                selectedTitle.text = activePromo.Title;
            }
            else if (activePromo.linkedAd != null) // Ad Type
            {
                var qtyString = string.Format("{0}", activePromo.linkedAd.Details.PlacementViewsRemaining == null ? "UNLIMITED rewarded ads." : activePromo.linkedAd.Details.PlacementViewsRemaining + " more rewarded ads.");
                selectedDesc.text = activePromo.linkedAd.Details.RewardDescription + " You may watch " + qtyString;
                selectedTitle.text = activePromo.linkedAd.Details.RewardName;
            }
            else
            {
                selectedDesc.text = GlobalStrings.NO_EVENTS_MSG;
                selectedTitle.text = "";
            }

            if (activePromo.assets.Banner != null)
                PromoBanner.overrideSprite = Sprite.Create(activePromo.assets.Banner, new Rect(0, 0, activePromo.assets.Banner.width, activePromo.assets.Banner.height), new Vector2(0.5f, 0.5f));
        }

        public void AddAdPromo(AdPlacementDetails details)
        {
            var adPromo = new UB_PromoDisplay();
            adPromo.linkedAd = new UBAdData { Details = details };
            adPromo.assets = new UBUnpackedAssetBundle();
            adPromo.assets.Banner = defaultVideoBanner;
            adPromo.assets.PromoId = details.PlacementId;
            promos.Add(adPromo);

            SetAdSlotCount(promos.Count);
            displayPromoIndex = promos.Count - 1;
            SelectBanner();
        }

        public void AdjustSlotDisplay(int selected)
        {
            for (var z = 0; z < adSlots.Count; z++)
                adSlots[z].GetComponent<Image>().overrideSprite = (z == selected ? SlotSelected : null);
        }

        //TODO bug check this and update this code to be more dynamic.
        private void SetAdSlotCount(int adCount)
        {
            PrvAd.SetActive(adCount > 1);
            NextAd.SetActive(adCount > 1);

            if (adSlots.Count == 0) // Initialize if unset
                adSlots.AddRange(AdSlotCounterBar.GetComponentsInChildren<LayoutElement>());

            if (adSlots.Remove(SlotEmpty)) // Never remove this one as we're going to use it as a template
                adCount -= 1; // Adjust the target to match 

            // Add/Remove children as needed
            while (adSlots.Count > adCount)
            {
                var temp = adSlots[adSlots.Count - 1];
                adSlots.RemoveAt(adSlots.Count - 1);
                Destroy(temp.gameObject);
            }

            if (adSlots.Count < adCount)
            {
                for (int z = 0; z <= adCount - adSlots.Count; z++)
                {
                    var newSlot = Instantiate(SlotEmpty.transform);
                    newSlot.SetParent(AdSlotCounterBar, false);
                    adSlots.Add(newSlot.GetComponent<LayoutElement>()); // Never remove this one as we're going to use it as a template
                }
            }
            // We just changed the ad count, reset this list
            adSlots.Add(SlotEmpty); // Never remove this one as we're going to use it as a template
        }
    }

    public class UB_PromoDisplay
    {
        public static UB_PromoDisplay EMPTY_PROMO;

        static UB_PromoDisplay()
        {
            EMPTY_PROMO = new UB_PromoDisplay
            {
                assets = new UBUnpackedAssetBundle
                {
                Banner = null,
                ContentKey = "none",
                PromoId = "none",
                Splash = null
                },
                linkedAd = null,
                EventKey = null,
                Title = null,
                Description = null
            };
        }

        public UBUnpackedAssetBundle assets;
        public UBAdData linkedAd;

        public string EventKey;
        public string Title;
        public string Description;
    }
}