using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

public class PromotionController : MonoBehaviour
{
    public Transform AdSlotCounterBar;
    private List<LayoutElement> adSlots = new List<LayoutElement>();
    public GameObject AdObject, PrvAd, NextAd, PlayEventBtn, ViewSaleBtn;
    public LayoutElement SlotEmpty;
    public Sprite SlotSelected;
    public Image PromoBanner;
    public Text selectedTitle, selectedDesc;

    public UB_PromoDisplay activePromo = null;
    public List<UB_PromoDisplay> promos = new List<UB_PromoDisplay>();

    private float timeSinceMove = 99999999f;
    private float rotateDelay = 8f;

    public enum PromotionFilter { All = 0, Sales = 1, Events = 2, News = 3 }
    //public PromotionFilter activeFilter = PromotionFilter.All;
    //public Text titleText;

    // Use this for initialization
    void Start()
    {
        // Set up the default display
        SetAdSlotCount(1);
        SelectBanner(UB_PromoDisplay.EMPTY_PROMO, 0);
    }

    public void Init()
    {
        // load sales or load default banners
        // set up ad circles
        timeSinceMove = Time.time;

        if (PF_GameData.PromoAssets.Count > 0)
        {
            promos.Clear();
            foreach (var item in PF_GameData.PromoAssets)
            {
                UB_PromoDisplay promo = new UB_PromoDisplay() { assets = item };

                if (item.ContentKey.Contains("events"))
                {
                    UB_EventData ev = null;
                    PF_GameData.Events.TryGetValue(item.PromoId, out ev);
                    promo.linkedEvent = ev;
                    promo.linkedSale = null;
                }
                else
                {
                    UB_SaleData sl = null;
                    PF_GameData.Sales.TryGetValue(item.PromoId, out sl);
                    promo.linkedSale = sl;
                    promo.linkedEvent = null;
                }

                promos.Add(promo);
            }

            SetAdSlotCount(promos.Count);
            SelectBanner(promos[0], 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (timeSinceMove + rotateDelay < Time.time && promos.Count > 1)
            NextBanner();
    }

    public void NextBanner()
    {
        timeSinceMove = Time.time;
        int index = promos.FindIndex((z) => { return z.assets.PromoId == activePromo.assets.PromoId; });
        index = (index + 1) % promos.Count;
        var newPromo = index < promos.Count ? promos[index] : UB_PromoDisplay.EMPTY_PROMO;

        UnityAction afterFadeOut = () =>
        {
            SelectBanner(newPromo, index);
            FadeAdsIn();
        };
        FadeAdsOut(afterFadeOut);
    }

    public void PrevBanner()
    {
        timeSinceMove = Time.time;
        int index = promos.FindIndex((z) => { return z.assets.PromoId == activePromo.assets.PromoId; });
        index = (index + promos.Count - 1) % promos.Count;
        var newPromo = index < promos.Count ? promos[index] : UB_PromoDisplay.EMPTY_PROMO;

        UnityAction afterFadeOut = () =>
        {
            SelectBanner(newPromo, index);
            FadeAdsIn();
        };
        FadeAdsOut(afterFadeOut);
    }

    public void FadeAdsOut(UnityAction callback = null)
    {
        PF_GamePlay.OutroPane(AdObject, .5f, () =>
        {
            if (callback != null)
            {
                callback();
            }
        });
    }

    public void FadeAdsIn(UnityAction callback = null)
    {
        PF_GamePlay.IntroPane(AdObject, .5f, () =>
        {
            if (callback != null)
            {
                callback();
            }
        });
    }

    public void ViewSale()
    {
        string storeId = (activePromo.linkedEvent != null) ? activePromo.linkedEvent.StoreToUse : activePromo.linkedSale.StoreToUse;
        DialogCanvasController.RequestStore(storeId);

        Dictionary<string, object> eventData = new Dictionary<string, object>()
		{
			{ "SalePromo", activePromo.assets.PromoId },
			{ "Character_ID", PF_PlayerData.activeCharacter.characterDetails.CharacterId }
		};

        PF_Bridge.LogCustomEvent(PF_Bridge.CustomEventTypes.Client_LevelUp, eventData);
    }

    public void PlayEvent()
    {
        UnityAction<int> afterSelect = (int response) =>
        {
            Debug.Log("Starting Event #" + response + "...");
        };

        if (activePromo.linkedEvent.AssociatedLevels.Count > 1)
        {
            DialogCanvasController.RequestSelectorPrompt(GlobalStrings.QUEST_SELECTOR_PROMPT, activePromo.linkedEvent.AssociatedLevels, afterSelect);

        }
        else
        {
            afterSelect(0);
        }
    }

    public void SelectBanner(UB_PromoDisplay newPromo, int index)
    {
        activePromo = newPromo;
        AdjustSlotDisplay(index);

        PlayEventBtn.SetActive(activePromo.linkedEvent != null);
        ViewSaleBtn.SetActive(activePromo.linkedSale != null || (activePromo.linkedEvent != null && !string.IsNullOrEmpty(activePromo.linkedEvent.StoreToUse)));

        if (activePromo.linkedSale != null) // Sale Type
        {
            selectedDesc.text = activePromo.linkedSale.SaleDescription;
            selectedTitle.text = activePromo.linkedSale.SaleName;
        }
        else if (activePromo.linkedEvent != null) // Event Type
        {
            selectedDesc.text = activePromo.linkedEvent.EventDescription;
            selectedTitle.text = activePromo.linkedEvent.EventName;
        }
        else
        {
            selectedDesc.text = GlobalStrings.NO_EVENTS_MSG;
            selectedTitle.text = "";
        }

        if (activePromo.assets.Banner != null)
            PromoBanner.sprite = Sprite.Create(activePromo.assets.Banner, new Rect(0, 0, activePromo.assets.Banner.width, activePromo.assets.Banner.height), new Vector2(0.5f, 0.5f));
        else
            PromoBanner.sprite = null;
    }

    public void AdjustSlotDisplay(int selected)
    {
        for (int z = 0; z < adSlots.Count; z++)
            adSlots[z].GetComponent<Image>().overrideSprite = (z == selected ? SlotSelected : null);
    }

    private void SetAdSlotCount(int adCount)
    {
        PrvAd.SetActive(adCount > 1);
        NextAd.SetActive(adCount > 1);

        if (adSlots.Count == 0) // Initialize if unset
            adSlots.AddRange(AdSlotCounterBar.GetComponentsInChildren<LayoutElement>());
        adSlots.Remove(SlotEmpty); // Never remove this one as we're going to use it as a template
        adCount -= 1; // Adjust the target to match 

        // Add/Remove children as needed
        while (adSlots.Count > adCount && adSlots.Count > 0)
        {
            var temp = adSlots[adSlots.Count - 1];
            adSlots.RemoveAt(adSlots.Count - 1);
            GameObject.Destroy(temp.gameObject);
        }
        for (int z = 0; z < adCount; z++)
        {
            var newSlot = Instantiate(SlotEmpty.transform);
            newSlot.SetParent(AdSlotCounterBar, false);
            adSlots.Add(newSlot.GetComponent<LayoutElement>()); // Never remove this one as we're going to use it as a template
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
        EMPTY_PROMO = new UB_PromoDisplay();
        EMPTY_PROMO.assets = new UB_UnpackedAssetBundle();
        EMPTY_PROMO.assets.Banner = null;
        EMPTY_PROMO.assets.ContentKey = "none";
        EMPTY_PROMO.assets.PromoId = "none";
        EMPTY_PROMO.assets.Splash = null;
        EMPTY_PROMO.linkedEvent = null;
        EMPTY_PROMO.linkedSale = null;
    }

    public UB_UnpackedAssetBundle assets;
    public UB_EventData linkedEvent;
    public UB_SaleData linkedSale;
}
