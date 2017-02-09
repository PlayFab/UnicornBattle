using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using PlayFab.Json;

public static class PF_GameData
{
    // General TitleData Containers
    public static Dictionary<string, UB_SpellDetail> Spells = new Dictionary<string, UB_SpellDetail>();
    public static Dictionary<string, UB_ClassDetail> Classes = new Dictionary<string, UB_ClassDetail>();
    public static Dictionary<string, UB_Achievement> Achievements = new Dictionary<string, UB_Achievement>();
    public static Dictionary<string, UB_EventData> Events = new Dictionary<string, UB_EventData>();
    public static Dictionary<string, UB_SaleData> Sales = new Dictionary<string, UB_SaleData>();
    public static Dictionary<string, UB_OfferData> Offers = new Dictionary<string, UB_OfferData>();
    public static Dictionary<string, int> CharacterLevelRamp = new Dictionary<string, int>();
    public static Dictionary<string, UB_LevelData> Levels = new Dictionary<string, UB_LevelData>();
    public static Dictionary<string, Dictionary<string, UB_EncounterData>> Encounters = new Dictionary<string, Dictionary<string, UB_EncounterData>>();

    public static List<string> StandardStores = new List<string>();
    public static int StartingCharacterSlots;
    public static float MinimumInterstitialWait;
    public static string CommunityWebsite = string.Empty;
    public static string AndroidPushSenderId = null;

    // Contents from GetTitleNews
    public static List<TitleNewsItem> RawNewsItems = new List<TitleNewsItem>();

    // all the items in our "Offers" catalog
    public static List<CatalogItem> offersCataogItems = new List<CatalogItem>();

    // all the items in our primary (CharacterClasses) catalog
    public static Dictionary<string, CatalogItem> catalogItems = new Dictionary<string, CatalogItem>();

    // these containers hold the interstitial tips, parsed from the RawNewsItems
    public static List<UB_PromotionalItem> promoItems = new List<UB_PromotionalItem>();
    public static List<UB_UnpackedAssetBundle> PromoAssets = new List<UB_UnpackedAssetBundle>();

    // these containers hold the last requested leaderboards for top 10 and friends.
    public static List<PlayerLeaderboardEntry> currentTop10LB = new List<PlayerLeaderboardEntry>();
    public static List<PlayerLeaderboardEntry> friendsLB = new List<PlayerLeaderboardEntry>();


    public static void GetTitleData()
    {
        var request = new GetTitleDataRequest { Keys = GlobalStrings.InitTitleKeys };
        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData);
        PlayFabClientAPI.GetTitleData(request, OnGetTitleDataSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void ExtractJsonTitleData<T>(Dictionary<string, string> resultData, string titleKey, ref T output)
    {
        string json;
        if (!resultData.TryGetValue(titleKey, out json))
            Debug.LogError("Failed to load titleData: " + titleKey);
        try
        {
            output = JsonWrapper.DeserializeObject<T>(resultData[titleKey]);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to load titleData: " + titleKey);
            Debug.LogException(e);
        }
    }

    private static void OnGetTitleDataSuccess(GetTitleDataResult result)
    {
        AndroidPushSenderId = GlobalStrings.DEFAULT_ANDROID_PUSH_SENDER_ID;

        ExtractJsonTitleData(result.Data, "Classes", ref Classes);
        ExtractJsonTitleData(result.Data, "Spells", ref Spells);
        ExtractJsonTitleData(result.Data, "StartingCharacterSlots", ref StartingCharacterSlots);
        ExtractJsonTitleData(result.Data, "MinimumInterstitialWait", ref MinimumInterstitialWait);
        ExtractJsonTitleData(result.Data, "CharacterLevelRamp", ref CharacterLevelRamp);
        ExtractJsonTitleData(result.Data, "Levels", ref Levels);
        ExtractJsonTitleData(result.Data, "Achievements", ref Achievements);
        ExtractJsonTitleData(result.Data, "Sales", ref Sales);
        ExtractJsonTitleData(result.Data, "Events", ref Events);
        ExtractJsonTitleData(result.Data, "Offers", ref Offers);
        ExtractJsonTitleData(result.Data, "StandardStores", ref StandardStores);
        if (result.Data.ContainsKey("CommunityWebsite"))
            CommunityWebsite = result.Data["CommunityWebsite"];
        if (result.Data.ContainsKey("AndroidPushSenderId"))
            AndroidPushSenderId = result.Data["AndroidPushSenderId"];

        DetermineSalesPromotionalTypes();
        DetermineEventPromotionalTypes();

        BuildCDNRequests();
        PF_Bridge.RaiseCallbackSuccess("Title Data Loaded", PlayFabAPIMethods.GetTitleData, MessageDisplayStyle.none);
    }


    public static void BuildCDNRequests()
    {
        List<AssetBundleHelperObject> requests = new List<AssetBundleHelperObject>();
        var mime = "application/x-gzip";
        var keyPrefix = string.Empty;

        if (Application.platform == RuntimePlatform.Android)
            keyPrefix = "Android/";
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            keyPrefix = "iOS/";

        foreach (var sale in Sales)
        {
            if (sale.Value.PromoType == PromotionType.Promoted)
            {
                requests.Add(new AssetBundleHelperObject()
                {
                    MimeType = mime,
                    ContentKey = keyPrefix + sale.Value.BundleId,
                    FileName = sale.Key,
                    Unpacked = new UB_UnpackedAssetBundle(),
                });
            }
        }

        foreach (var ev in Events)
        {
            if (ev.Value.PromoType == PromotionType.Promoted && !string.IsNullOrEmpty(ev.Value.BundleId))
            {
                requests.Add(new AssetBundleHelperObject()
                {
                    MimeType = mime,
                    ContentKey = keyPrefix + ev.Value.BundleId,
                    FileName = ev.Key,
                    Unpacked = new UB_UnpackedAssetBundle()
                });
            }
        }

        GameController.Instance.cdnController.assets = requests;

        UnityAction<bool> afterCdnRequest = response =>
        {
            if (response)
            {
                PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetCDNConent, MessageDisplayStyle.none);
                Debug.Log("CDN Retrieved!");
            }

            PromoAssets.Clear();
            foreach (var obj in requests)
                if (obj.IsUnpacked)
                    PromoAssets.Add(obj.Unpacked);
            GameController.Instance.cdnController.isInitalContentUnpacked = true;
        };

        if (GameController.Instance.cdnController.isInitalContentUnpacked == false && GameController.Instance.cdnController.useCDN)
        {
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetCDNConent);
            GameController.Instance.cdnController.KickOffCDNGet(requests, afterCdnRequest);
        }
    }

    public static void DetermineSalesPromotionalTypes()
    {
        if (Sales.Count == 0)
            return;

        var today = DateTime.Now;
        var bounds = today.AddDays(30);
        foreach (var sale in Sales)
        {
            if ((sale.Value.StartDate <= today && sale.Value.EndDate >= today))
            {
                if (sale.Value.PromoteWithCarousel || sale.Value.PromoteWithInterstitial)
                    sale.Value.PromoType = PromotionType.Promoted;
                else
                    sale.Value.PromoType = PromotionType.Active;
            }

            if (sale.Value.StartDate > today && sale.Value.StartDate < bounds)
                sale.Value.PromoType = PromotionType.Upcomming;
        }
    }

    public static void DetermineEventPromotionalTypes()
    {
        if (Events.Count == 0)
            return;

        var today = DateTime.Now;
        var bounds = today.AddDays(30);
        foreach (var each in Events)
        {
            if (each.Value.StartDate <= today && each.Value.EndDate >= today)
                each.Value.PromoType = PromotionType.Promoted;
            if (each.Value.StartDate > today && each.Value.StartDate < bounds)
                each.Value.PromoType = PromotionType.Upcomming;
        }
    }

    public static void GetEncounterLists(List<string> encounters)
    {
        var request = new GetTitleDataRequest { Keys = encounters };

        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData);
        PlayFabClientAPI.GetTitleData(request, (result) =>
        {
            //clear encounters for now (until we have reasons to merge dicts);
            PF_GamePlay.ClearQuestProgress();
            Encounters.Clear();

            foreach (var item in encounters)
                if (result.Data.ContainsKey(item))
                    Encounters.Add(item, JsonWrapper.DeserializeObject<Dictionary<string, UB_EncounterData>>(result.Data[item]));

            PF_Bridge.RaiseCallbackSuccess("Encounters Loaded!", PlayFabAPIMethods.GetTitleData, MessageDisplayStyle.none);

        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void GetOffersCatalog()
    {
        var request = new GetCatalogItemsRequest();
        request.CatalogVersion = "Offers";

        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetOffersCatalog);
        PlayFabClientAPI.GetCatalogItems(request, OnGetOffersCatalogSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void OnGetOffersCatalogSuccess(GetCatalogItemsResult result)
    {
        offersCataogItems = result.Catalog;
        PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetOffersCatalog, MessageDisplayStyle.none);
    }

    public static void GetCatalogInfo()
    {
        var request = new GetCatalogItemsRequest { CatalogVersion = GlobalStrings.PrimaryCatalogName };
        PlayFabClientAPI.GetCatalogItems(request, OnGetCatalogSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        catalogItems.Clear();
        foreach (var eachItem in result.Catalog)
            catalogItems[eachItem.ItemId] = eachItem;

        PF_PlayerData.GetUserAccountInfo();
    }

    public static void GetTitleNews()
    {
        var request = new GetTitleNewsRequest { Count = 15 };
        PlayFabClientAPI.GetTitleNews(request, OnGetTitleNewsSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    private static void OnGetTitleNewsSuccess(GetTitleNewsResult result)
    {
        // parse news "tags"
        // sort newsitems into buckets for easier use elsewhere
        // some buckets would be News / Sales / Tips / Images
        promoItems.Clear();

        foreach (var item in result.News)
        {
            int endTagsIndex = item.Title.LastIndexOf('}');

            var pItem = new UB_PromotionalItem();
            pItem.TimeStamp = item.Timestamp;
            pItem.PromoBody = item.Body;
            pItem.PromoTitle = item.Title.Substring(endTagsIndex + 1);
            pItem.PromoType = PromotionalItemTypes.Tip;

            promoItems.Add(pItem);
        }

        PF_Bridge.RaiseCallbackSuccess("Title News Loaded", PlayFabAPIMethods.GetTitleNews, MessageDisplayStyle.none);
    }


    public static void TryOpenContainer(string containerId, string characterId = null, UnityAction<UnlockContainerItemResult> callback = null)
    {
        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UnlockContainerItem);
        var request = new UnlockContainerItemRequest { ContainerItemId = containerId, CharacterId = characterId };
        PlayFabClientAPI.UnlockContainerItem(request, (UnlockContainerItemResult result) =>
        {
            if (callback != null)
                callback(result);
            PF_Bridge.RaiseCallbackSuccess("Container Unlocked", PlayFabAPIMethods.UnlockContainerItem, MessageDisplayStyle.none);
        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static CatalogItem GetCatalogItemById(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;
        CatalogItem output;
        catalogItems.TryGetValue(id, out output);
        return output;
    }

    public static string GetIconByItemById(string id, string iconDefault = "Default")
    {
        if (string.IsNullOrEmpty(id))
            return null;
        CatalogItem catalogItem;
        if (!catalogItems.TryGetValue(id, out catalogItem))
            return null;
        var iconName = iconDefault;
        try
        {
            string temp;
            var kvps = JsonWrapper.DeserializeObject<Dictionary<string, string>>(catalogItem.CustomData);
            if (kvps.TryGetValue("icon", out temp))
                iconName = temp;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        return iconName;
    }

    public static void GetPlayerLeaderboard(string stat, UnityAction callback = null)
    {
        var request = new GetLeaderboardRequest
        {
            MaxResultsCount = 10,
            StatisticName = stat
        };

        PlayFabClientAPI.GetLeaderboard(request, result =>
        {
            currentTop10LB = result.Leaderboard;
            if (callback != null)
                callback();
            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetPlayerLeaderboard, MessageDisplayStyle.none);
        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void GetFriendsLeaderboard(string stat, UnityAction callback = null)
    {
        var request = new GetFriendLeaderboardRequest
        {
            MaxResultsCount = 10,
            StatisticName = stat
        };

        PlayFabClientAPI.GetFriendLeaderboard(request, result =>
        {
            friendsLB = result.Leaderboard;
            if (callback != null)
                callback();
            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetFriendsLeaderboard, MessageDisplayStyle.none);
        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void GetMyCharacterLeaderboardRank(string stat, UnityAction<int> callback = null)
    {
        var request = new GetLeaderboardAroundCharacterRequest
        {
            CharacterId = PF_PlayerData.activeCharacter.characterDetails.CharacterId,
            StatisticName = stat,
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundCharacter(request, result =>
        {
            if (callback != null && result.Leaderboard.Count > 0)
                callback(result.Leaderboard.First().Position);
            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetLeaderboardAroundCharacter, MessageDisplayStyle.none);
        }, PF_Bridge.PlayFabErrorCallback);
    }

    public static void GetMyPlayerLeaderboardRank(string stat, UnityAction<int> callback = null)
    {
        var request = new GetLeaderboardAroundPlayerRequest { StatisticName = stat };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, result =>
        {
            if (callback != null && result.Leaderboard.Count > 0)
                callback(result.Leaderboard.First().Position);
            PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetMyPlayerRank, MessageDisplayStyle.none);
        }, PF_Bridge.PlayFabErrorCallback);
    }
}




