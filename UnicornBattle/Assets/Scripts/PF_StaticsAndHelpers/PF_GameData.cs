using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
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
	
	// Contents from GetTitleNews
	public static List<TitleNewsItem> RawNewsItems = new List<TitleNewsItem>();

	// all the items in our "Offers" catalog
	public static List<CatalogItem> offersCataogItems = new List<CatalogItem>(); 
	
	// all the items in our primary (CharacterClasses) catalog
	public static List<CatalogItem> catalogItems = new List<CatalogItem>(); 
	
	// these containers hold the interstitial tips, parsed from the RawNewsItems
	public static List<UB_PromotionalItem> promoItems = new List<UB_PromotionalItem>();
	public static List<UB_UnpackedAssetBundle> PromoAssets = new List<UB_UnpackedAssetBundle>();
	
	// these containers hold the last requested leaderboards for top 10 and friends.
	public static List<PlayerLeaderboardEntry> currentTop10LB = new List<PlayerLeaderboardEntry>();
	public static List<PlayerLeaderboardEntry> friendsLB = new List<PlayerLeaderboardEntry>();


	public static void GetTitleData()
	{
		GetTitleDataRequest request = new GetTitleDataRequest()
		{
			Keys = new List<string>() { "Classes", "Spells", "StartingCharacterSlots", "MinimumInterstitialWait", "CharacterLevelRamp", "Levels", "Achievements", "Sales", "Events", "Offers", "CommunityWebsite", "StandardStores"} 
		};
		DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData);
		PlayFabClientAPI.GetTitleData(request, OnGetTitleDataSuccess, PF_Bridge.PlayFabErrorCallback);
		
	}
	
	
	public static void OnGetTitleDataSuccess(GetTitleDataResult result)
	{
		Debug.Log ("OnGetTitleDataSuccess");
		
		Debug.Log ("OnGetTitleDataSuccess -- Classes");
		if(result.Data.ContainsKey("Classes"))
		{
			Spells = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, UB_SpellDetail>>(result.Data["Spells"]);
		}
		
		Debug.Log ("OnGetTitleDataSuccess -- Spells");
		if(result.Data.ContainsKey("Spells"))
		{
			Classes = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, UB_ClassDetail>>(result.Data["Classes"]);
		}
		
		Debug.Log ("OnGetTitleDataSuccess -- StartingCharacterSlots");
		if(result.Data.ContainsKey("StartingCharacterSlots"))
		{
			StartingCharacterSlots = Int32.Parse(result.Data["StartingCharacterSlots"]);
		}
		
		Debug.Log ("OnGetTitleDataSuccess -- MinimumInterstitialWait");
		if(result.Data.ContainsKey("MinimumInterstitialWait"))
		{
			MinimumInterstitialWait = float.Parse(result.Data["MinimumInterstitialWait"]);
		}
		
		Debug.Log ("OnGetTitleDataSuccess -- CharacterLevelRamp");
		if(result.Data.ContainsKey("CharacterLevelRamp"))
		{
			CharacterLevelRamp = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, int>>(result.Data["CharacterLevelRamp"]);
		}
		
		Debug.Log ("OnGetTitleDataSuccess -- Levels");
		if(result.Data.ContainsKey("Levels"))
		{
			Levels = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, UB_LevelData>>(result.Data["Levels"]);
		}

		if(result.Data.ContainsKey("Achievements"))
		{
			Achievements = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, UB_Achievement>>(result.Data["Achievements"]);
		}
		
		if(result.Data.ContainsKey("Sales"))
		{
			Sales = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, UB_SaleData>>(result.Data["Sales"], PlayFab.Internal.Util.ApiSerializerStrategy);
			Debug.Log ("Sale Data Retrieved");

			if(Sales.Count > 0)
			{
				DetermineSalesPromotionalTypes();
			}
		}
		
		if(result.Data.ContainsKey("Events"))
		{
			Events = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, UB_EventData>>(result.Data["Events"], PlayFab.Internal.Util.ApiSerializerStrategy);
			Debug.Log ("Event Data Retrieved");
			if(Events.Count > 0)
			{
				DetermineEventPromotionalTypes();
			}
		}

		if(result.Data.ContainsKey("Offers"))
		{
			Offers = PlayFab.SimpleJson.DeserializeObject<Dictionary<string, UB_OfferData>>(result.Data["Offers"], PlayFab.Internal.Util.ApiSerializerStrategy);
			Debug.Log ("Offer Data Retrieved");

		}

		if(result.Data.ContainsKey("CommunityWebsite"))
		{
			CommunityWebsite = result.Data["CommunityWebsite"];
			Debug.Log ("Community Website URL Retrieved");
			
		}
		if(result.Data.ContainsKey("StandardStores"))
		{
			StandardStores = PlayFab.SimpleJson.DeserializeObject<List<string>>(result.Data["StandardStores"]);
			Debug.Log ("Standard Stores Retrieved");
		}
		



		BuildCDNRequests ();
		PF_Bridge.RaiseCallbackSuccess("Title Data Loaded", PlayFabAPIMethods.GetTitleData, MessageDisplayStyle.none);
	}


	public static void BuildCDNRequests()
	{
		List<AssetBundleHelperObject> requests = new List<AssetBundleHelperObject>();
		string mime = "application/x-gzip";
		
		string keyPrefix = string.Empty;
		
		if(Application.platform == RuntimePlatform.Android)
		{
			keyPrefix = "Android/";
		}
		else if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			keyPrefix = "iOS/";
		}


		foreach(var sale in Sales)
		{
			if(sale.Value.PromoType == PromotionType.Promoted)
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

		foreach(var ev in Events)
		{
			if(ev.Value.PromoType == PromotionType.Promoted && !string.IsNullOrEmpty(ev.Value.BundleId))
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

	

		UnityAction<bool> afterCDNRequest = (bool response) => 
		{
			if(response == true)
			{
				PF_Bridge.RaiseCallbackSuccess (string.Empty, PlayFabAPIMethods.GetCDNConent, MessageDisplayStyle.none);
				Debug.Log ("CDN Retrieved!");
			}
			
			PF_GameData.PromoAssets.Clear();
			foreach(var obj in requests)
			{
				if(obj.IsUnpacked == true)
				{
					PF_GameData.PromoAssets.Add(obj.Unpacked);
				}
			}
			GameController.Instance.cdnController.isInitalContentUnpacked = true;
		};

		if(GameController.Instance.cdnController.isInitalContentUnpacked == false && GameController.Instance.cdnController.useCDN == true)
		{
			DialogCanvasController.RequestLoadingPrompt (PlayFabAPIMethods.GetCDNConent);
			GameController.Instance.cdnController.KickOffCDNGet(requests, afterCDNRequest);
		}
	}


	public static void DetermineSalesPromotionalTypes()
	{
		DateTime today = DateTime.Now;
		DateTime bounds = today.AddDays (30);
		foreach (var sale in PF_GameData.Sales) 
		{
			if((sale.Value.StartDate <= today && sale.Value.EndDate  >= today))
			{
				// active sale
				
				if(sale.Value.PromoteWithCarousel == true || sale.Value.PromoteWithInterstitial == true)
				{
					sale.Value.PromoType = PromotionType.Promoted;
				}
				else
				{
					sale.Value.PromoType = PromotionType.Active;
				}
			}

			if( sale.Value.StartDate > today && sale.Value.StartDate  < bounds)
			{
				// upcomming sales
				sale.Value.PromoType = PromotionType.Upcomming;
				// should be included because we will want to promtote it in our chanels

			}
		}
	}

	public static void DetermineEventPromotionalTypes()
	{
		DateTime today = DateTime.Now;
		DateTime bounds = today.AddDays (30);

		foreach (var each in PF_GameData.Events) 
		{
			
			if((each.Value.StartDate <= today && each.Value.EndDate  >= today))
			{
				each.Value.PromoType = PromotionType.Promoted;
			}
			
			if( each.Value.StartDate > today && each.Value.StartDate  < bounds)
			{
				each.Value.PromoType = PromotionType.Upcomming;		
			}
		}
	}

	public static void GetEncounterLists(List<string> encounters)
	{
		GetTitleDataRequest request = new GetTitleDataRequest();
		request.Keys = encounters;
		
		DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetTitleData);
		PlayFabClientAPI.GetTitleData(request, (result) => 
		{ 
			//clear encounters for now (until we have reasons to merge dicts);
			PF_GamePlay.ClearQuestProgress();
			Encounters.Clear();
			
			foreach(var item in encounters)
			{
				if(result.Data.ContainsKey(item))
				{
					Encounters.Add(item, PlayFab.SimpleJson.DeserializeObject<Dictionary<string, UB_EncounterData>>(result.Data[item], PlayFab.Internal.Util.ApiSerializerStrategy));
				}	
			}
			
			PF_Bridge.RaiseCallbackSuccess("Encounters Loaded!", PlayFabAPIMethods.GetTitleData, MessageDisplayStyle.none);
			
		}, PF_Bridge.PlayFabErrorCallback);
	}

	
	public static void GetOffersCatalog()
	{
		GetCatalogItemsRequest request = new GetCatalogItemsRequest();
		request.CatalogVersion = "Offers";
		
		DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetOffersCatalog);
		PlayFabClientAPI.GetCatalogItems(request, OnGetOffersCatalogSuccess, PF_Bridge.PlayFabErrorCallback);
	}
	
	public static void OnGetOffersCatalogSuccess(GetCatalogItemsResult result)
	{
		offersCataogItems = result.Catalog;
		PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetOffersCatalog, MessageDisplayStyle.none);
		
	}
	
	public static void GetCatalogInfo()
	{
		GetCatalogItemsRequest request = new GetCatalogItemsRequest();
		request.CatalogVersion = "CharacterClasses";
		PlayFabClientAPI.GetCatalogItems(request, OnGetCatalogSuccess, PF_Bridge.PlayFabErrorCallback);
	}
	
	public static void OnGetCatalogSuccess(GetCatalogItemsResult result)
	{
		catalogItems = result.Catalog;
		
		PF_PlayerData.GetUserAccountInfo();
	}
	
	
	public static void GetTitleNews()
	{
		GetTitleNewsRequest request = new GetTitleNewsRequest();
		request.Count = 15;
		PlayFabClientAPI.GetTitleNews(request, OnGetTitleNewsSuccess, PF_Bridge.PlayFabErrorCallback);
	}
	
	public static void OnGetTitleNewsSuccess(GetTitleNewsResult result)
	{
		// parse news "tags"
		// sort newsitems into buckets for easier use elsewhere
		// some buckets would be News / Sales / Tips / Images
		promoItems.Clear();

		foreach(var item in result.News)
		{
			int endTagsIndex = item.Title.LastIndexOf('}');

			UB_PromotionalItem pItem = new UB_PromotionalItem();
			pItem.TimeStamp = item.Timestamp;
			pItem.PromoBody = item.Body;
			pItem.PromoTitle = item.Title.Substring(endTagsIndex+1);
			pItem.PromoType = PromotionalItemTypes.Tip;

			promoItems.Add(pItem);
		}
		
		PF_Bridge.RaiseCallbackSuccess("Title News Loaded", PlayFabAPIMethods.GetTitleNews, MessageDisplayStyle.none);
	}
	
	
	public static void TryOpenContainer(string containerId, string characterId = null, UnityAction<UnlockContainerItemResult> callback = null)
	{
		DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.UnlockContainerItem);
		UnlockContainerItemRequest request = new UnlockContainerItemRequest();
		request.ContainerItemId = containerId;
		
		if(characterId != null)
		{
			request.CharacterId = characterId;
		}
		
		PlayFabClientAPI.UnlockContainerItem(request, (UnlockContainerItemResult result) =>
		{	
			if(callback != null)
			{
				callback(result);
			}
		
			PF_Bridge.RaiseCallbackSuccess("Container Unlocked", PlayFabAPIMethods.UnlockContainerItem, MessageDisplayStyle.none);
		} , PF_Bridge.PlayFabErrorCallback);
		
	}
	
	
	public static CatalogItem GetCatalogItemById(string id)
	{
		return PF_GameData.catalogItems.Find( (item) => { return item.ItemId == id; });
	}

	
	public static CatalogItem ConvertStoreItemToCatalogItem(StoreItem si)
	{
		CatalogItem ci = new CatalogItem();
		CatalogItem reference = PF_GameData.catalogItems.Find( (item) => { return item.ItemId == si.ItemId; });
		
		if(reference == null)
		{
			return new CatalogItem() { 
				ItemId = "ITEM ERROR", 
				DisplayName = "ITEM ERROR",
				VirtualCurrencyPrices = new Dictionary<string, uint>()
				};
		}
		
		ci.Bundle = reference.Bundle;
		ci.CanBecomeCharacter = reference.CanBecomeCharacter;
		ci.CatalogVersion = reference.CatalogVersion;
		ci.Consumable = reference.Consumable;
		ci.Container = reference.Container;
		ci.CustomData = reference.CustomData;
		ci.Description = reference.Description;
		ci.DisplayName = reference.DisplayName;
		ci.IsStackable = reference.IsStackable;
		ci.ItemClass = reference.ItemClass;
		ci.Tags = reference.Tags;
		
		ci.RealCurrencyPrices = si.RealCurrencyPrices;
		ci.VirtualCurrencyPrices = si.VirtualCurrencyPrices;
		ci.ItemId = si.ItemId;
		
		return ci;
	}
	
	public static void GetPlayerLeaderboard(string stat, UnityAction callback = null)
	{
		GetLeaderboardRequest request = new GetLeaderboardRequest();
		request.MaxResultsCount = 10;
		request.StatisticName = stat;

		PlayFabClientAPI.GetLeaderboard(request, (GetLeaderboardResult result) => 
		                                         {
			currentTop10LB = result.Leaderboard;
			if(callback != null)
			{
				callback();
			}
			PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetPlayerLeaderboard, MessageDisplayStyle.none);
		}, PF_Bridge.PlayFabErrorCallback);
	}
	
	public static void GetFriendsLeaderboard(string stat, UnityAction callback = null)
	{
		GetFriendLeaderboardRequest request = new GetFriendLeaderboardRequest();
		request.MaxResultsCount = 10;
		request.StatisticName = stat;


		PlayFabClientAPI.GetFriendLeaderboard(request, (GetLeaderboardResult result) => 
		{
			friendsLB = result.Leaderboard;
			if(callback != null)
			{
				callback();
			}
			PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetFriendsLeaderboard, MessageDisplayStyle.none);
		}, PF_Bridge.PlayFabErrorCallback);
	}
	
	public static void GetMyCharacterLeaderboardRank(string stat, UnityAction<int> callback = null)
	{
		GetLeaderboardAroundCharacterRequest request = new GetLeaderboardAroundCharacterRequest();
		request.CharacterId = PF_PlayerData.activeCharacter.characterDetails.CharacterId;
		request.StatisticName = stat;
		request.MaxResultsCount = 1;

		PlayFabClientAPI.GetLeaderboardAroundCharacter(request, (GetLeaderboardAroundCharacterResult result) => 
		{
			if(callback != null && result.Leaderboard.Count > 0)
			{
				callback(result.Leaderboard.First().Position);
			}
			PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetLeaderboardAroundCharacter, MessageDisplayStyle.none);
		}, PF_Bridge.PlayFabErrorCallback);
	}

	public static void GetMyPlayerLeaderboardRank(string stat, UnityAction<int> callback = null)
	{
		GetLeaderboardAroundCurrentUserRequest request = new GetLeaderboardAroundCurrentUserRequest();
		request.StatisticName = stat;

		PlayFabClientAPI.GetLeaderboardAroundCurrentUser(request, (GetLeaderboardAroundCurrentUserResult result) => 
		{
			if(callback != null && result.Leaderboard.Count > 0)
			{
				callback(result.Leaderboard.First().Position);
			}
			PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.GetMyPlayerRank, MessageDisplayStyle.none);
		}, PF_Bridge.PlayFabErrorCallback);
	}
}




