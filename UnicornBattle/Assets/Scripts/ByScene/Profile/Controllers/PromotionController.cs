using PlayFab;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

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
        SupersonicEvents.OnAdRewarded += EvaluateAdState;
        InvokeRepeating("EvaluateAdState", 60, 300); //start after 1m, repeat every 5m
    }

    void OnDisable()
    {
        SupersonicEvents.OnAdRewarded -= EvaluateAdState;
        CancelInvoke("EvaluateAdState");
    }

    #region ****** PLAYFAB CALLBACKS ******
    public void CheckForPlayFabPlacement(string placement = null)
    {
        Action<PlayFabError> errorCb = error =>
        {
            if (error.GenerateErrorReport().Contains("AllAdPlacementViewsAlreadyConsumed"))
            {
                //PF_Bridge.RaiseCallbackError(string.Format("No more rewared ads can be viewed at this time. Check back in {0} minutes",  ), PlayFabAPIMethods.Generic, MessageDisplayStyle.error);
                var adpromo = promos.Find((p) => { return p.linkedAd != null; });

                if (adpromo != null)
                {
                    Debug.Log(string.IsNullOrEmpty(adpromo.assets.PromoId) ? "AdPromo was not null... removing: " + adpromo.assets.PromoId : "Unknown");
                    promos.Remove(adpromo);
                    SetAdSlotCount(promos.Count);
                    SelectBanner();
                    Debug.Log("Promo Count: " + promos.Count);
                }
                else
                {
                    Debug.Log("AdPromo not in rotation.");
                }
            }
            else
            {
                PF_Bridge.RaiseCallbackError(error.GenerateErrorReport(), PlayFabAPIMethods.Generic, MessageDisplayStyle.error);
            }
        };

        SupersonicEvents.queuedAd = null;

        if (placement != null)
        {
            PF_Advertising.GetAdPlacements(new GetAdPlacementsRequest { Identifier = new NameIdentifier() { Name = placement }, AppId = SupersonicEvents.appKey }, OnCheckForPlayFabPlacementSuccess, errorCb);
        }
        else
        {
            PF_Advertising.GetAdPlacements(new GetAdPlacementsRequest { AppId = SupersonicEvents.appKey }, OnCheckForPlayFabPlacementSuccess, errorCb);
        }
    }

    public void OnCheckForPlayFabPlacementSuccess(GetAdPlacementsResult result)
    {
        if ((result.AdPlacements != null && result.AdPlacements.Length > 0) && (result.AdPlacements[0].PlacementViewsRemaining == null || result.AdPlacements[0].PlacementViewsRemaining > 0))
        {
            var adpromo = promos.Find((p) => { return p.linkedAd != null && p.assets.PromoId == result.AdPlacements[0].PlacementId; });


            if (adpromo == null)
            {
                AddAdPromo(result.AdPlacements[0]);
                //SelectBanner(promos[0], 0);
            }
            else
            {
                adpromo.linkedAd.Details.PlacementViewsRemaining = result.AdPlacements[0].PlacementViewsRemaining;
            }

            if (activePromo.assets.PromoId == result.AdPlacements[0].PlacementId)
            {
                var qtyString = string.Format("{0}", result.AdPlacements[0].PlacementViewsRemaining == null ? "UNLIMITED rewarded ads." : result.AdPlacements[0].PlacementViewsRemaining + " more rewarded ads.");
                selectedDesc.text = result.AdPlacements[0].RewardDescription + " You may watch " + qtyString;
            }
        }
        else
        {
            var adpromo = promos.Find((p) => { return p.linkedAd != null && p.assets.PromoId == result.AdPlacements[0].PlacementId; });
            if (adpromo != null)
            {
                promos.Remove(adpromo);
                SetAdSlotCount(promos.Count);
                SelectBanner();
            }
        }
    }
    #endregion

    void EvaluateAdState(RewardAdActivityResponse result = null)
    {
        Debug.Log("EvaluateAdState Called.");
        CheckForPlayFabPlacement();
    }

    public void Init()
    {
        _timeSinceMove = Time.time;

        if (PF_GameData.PromoAssets.Count == 0)
            return;

        promos.Clear();
        foreach (var item in PF_GameData.PromoAssets)
        {
            var promo = new UB_PromoDisplay { assets = item };

            UB_EventData ev;
            if (PF_GameData.Events.TryGetValue(item.PromoId, out ev))
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
        PF_GamePlay.OutroPane(AdObject, .5f, () =>
        {
            if (callback != null)
                callback();
        });
    }

    public void FadeAdsIn(UnityAction callback = null)
    {
        PF_GamePlay.IntroPane(AdObject, .5f, () =>
        {
            if (callback != null)
                callback();
        });
    }

    public void ViewSale()
    {
        var storeId = PF_GameData.GetEventSaleStore(activePromo.EventKey);
        DialogCanvasController.RequestStore(storeId);

        Dictionary<string, object> eventData = new Dictionary<string, object> {
            { "SalePromo", activePromo.assets.PromoId },
            { "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId }
        };

        PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_SaleClicked, eventData);
    }

    public void PlayEvent()
    {
        var levelNames = PF_GameData.GetEventAssociatedLevels(activePromo.EventKey);

        UnityAction<int> playEventIndex = index => {
            _levelPicker.LevelItemClicked(levelNames[index]);
            _levelPicker.PlaySelectedLevel();
        };

        if (levelNames.Count > 1)
            DialogCanvasController.RequestSelectorPrompt(GlobalStrings.QUEST_SELECTOR_PROMPT, levelNames, playEventIndex);
        else if (levelNames.Count == 1)
            playEventIndex(0);
    }

    public void WatchAd()
    {

        if (activePromo != null && activePromo.linkedAd != null && SupersonicEvents.rewardedVideoAvailability && Time.time > _watchLastClickedAt + _watchCD)
        {
            _watchLastClickedAt = Time.time;
            SupersonicEvents.ShowRewardedVideo(activePromo.linkedAd.Details);
        }
        else
        {
            PF_Bridge.RaiseCallbackError("Rewarded ad is unavailable at this time. Please try again later.", PlayFabAPIMethods.Generic, MessageDisplayStyle.error);
        }
    }

    public void SelectBanner()
    {
        displayPromoIndex = promos.Count == 0 ? 0 : displayPromoIndex % promos.Count;
        activePromo = promos.Count == 0 ? UB_PromoDisplay.EMPTY_PROMO : promos[displayPromoIndex];
        AdjustSlotDisplay(displayPromoIndex);

        ViewSaleBtn.SetActive(!string.IsNullOrEmpty(PF_GameData.GetEventSaleStore(activePromo.EventKey)));
        PlayEventBtn.SetActive(activePromo.linkedAd == null && PF_GameData.GetEventAssociatedLevels(activePromo.EventKey).Count > 0);
        WatchAdBtn.SetActive(activePromo.linkedAd != null);

        if (activePromo.EventKey != null) // Event Type
        {
            selectedDesc.text = activePromo.Description;
            selectedTitle.text = activePromo.Title;
        }
        else if (activePromo.linkedAd != null)  // Ad Type
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
        adPromo.linkedAd = new UB_AdData { Details = details };
        adPromo.assets = new UB_UnpackedAssetBundle();
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
        EMPTY_PROMO = new UB_PromoDisplay {
            assets = new UB_UnpackedAssetBundle {
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

    public UB_UnpackedAssetBundle assets;
    public UB_AdData linkedAd;

    public string EventKey;
    public string Title;
    public string Description;
}
