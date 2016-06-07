using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab.ClientModels;
using PlayFab;
using PlayFab.Json;

public static class PF_GamePlay
{
	public static List<UB_GamePlayEncounter> encounters;
	public static LevelItemHelper ActiveQuest; 
	public static QuestTracker QuestProgress = new QuestTracker();
	public static bool UseRaidMode = true; // TODO default to false - or make it a setting in the player's data
	public static bool isHardMode = false;
	
	public static List<PlayFab.ClientModels.StoreItem> mostRecentStore = new List<PlayFab.ClientModels.StoreItem>();
	
	// class colors used to colorize UI based on active character.
	public static Color ClassColor1;
	public static Color ClassColor2;
	public static Color ClassColor3;
	
	public enum ShakeEffects { None = 0, DecreaseHealth, DecreaseMana, IncreaseHealth, IncreaseMana }
	public enum PlayerSpellInputs { Null = 0, Spell1, Spell2, Spell3, Flee, UseItem }
	public enum PlayerEncounterInputs { Null = 0, Attack, UseItem, Evade, ViewStore, Rescue }
	public enum GameplayEventTypes { Null, StartQuest, EndQuest, IntroQuest, OutroQuest, IntroAct, OutroAct, IntroEncounter, OutroEncounter, PlayerTurnBegins, PlayerTurnEnds, EnemyTurnBegins, EnemyTurnEnds, StartBossBattle, PlayerDied, PlayerRespawn} // MAY need more types here for things like store, heros and bosses, perhaps even one for spelleffects
	public enum TurnStates { Null, Player, Enemy}
	
	#region UI Animation
	/// <summary>
	/// Wait the specified time and callback to method.
	/// </summary>
	/// <param name="time">Time.</param>
	/// <param name="callback">Callback.</param>
	public static IEnumerator Wait(float time, UnityAction callback)
	{
		yield return new WaitForSeconds(time);
		callback();
	}
	
	/// <summary>
	/// Intros the pane.
	/// </summary>
	/// <param name="obj">Object - object to animate in</param>
	/// <param name="duration">Duration -   how long is the transition</param>
	/// <param name="callback">Callback - method to call after the animation is complete </param>
	public static void IntroPane(GameObject obj, float duration, UnityAction callback = null)
	{
		CanvasGroup cg = obj.GetComponent<CanvasGroup>();
		if(cg != null)
		{
			cg.blocksRaycasts = true;
			TweenCGAlpha.Tween(obj, duration, 1, callback);
		}
		else
		{
			// will add a cg automatically
			TweenCGAlpha.Tween(obj, duration, 1, callback);
		}
	}
	
	/// <summary>
	/// Outros the pane.
	/// </summary>
	/// <param name="obj">Object - object to animate out</param>
	/// <param name="duration">Duration -   how long is the transition</param>
	/// <param name="callback">Callback - method to call after the animation is complete </param>
	public static void OutroPane(GameObject obj, float duration, UnityAction callback = null)
	{
		CanvasGroup cg = obj.GetComponent<CanvasGroup>();
		if(cg != null)
		{
			cg.blocksRaycasts = false;
			TweenCGAlpha.Tween(obj, duration, 0, callback);
		}
		else
		{
			// will add a cg automatically
			TweenCGAlpha.Tween(obj, duration, 0, callback);
		}
	}
	#endregion
	
	
	#region misc helpers
	/// <summary>
	/// Reset the quest tracker
	/// </summary>
	public static void ClearQuestProgress()
	{
		encounters = null;
		ActiveQuest = null;
		QuestProgress = new QuestTracker();
	}
	

	/// <summary>
	/// write back the quest progress to playfab
	/// </summary>
	public static void SavePlayerData()
	{
		DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.SavePlayerInfo);

		ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest();
		request.FunctionName = "SaveProgress";
		request.FunctionParameter = new { CurrentPlayerData = PF_PlayerData.activeCharacter, QuestProgress = PF_GamePlay.QuestProgress, LevelRamp = PF_GameData.CharacterLevelRamp};


		PlayFabClientAPI.ExecuteCloudScript(request, OnSavePlayerDataSuccess, PF_Bridge.PlayFabErrorCallback);
		
	}
	#endregion
	 
	/// <summary>
	/// Callback after 
	/// </summary>
	/// <param name="result">Result.</param>
	public static void OnSavePlayerDataSuccess(ExecuteCloudScriptResult result)
	{
		if(!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
			return;
	
		PF_Bridge.RaiseCallbackSuccess("Player Info Saved", PlayFabAPIMethods.SavePlayerInfo, MessageDisplayStyle.none);
	}

	/// <summary>
	/// Retrives the quest items.
	/// </summary>
	public static void RetriveQuestItems()
	{
		DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.RetriveQuestItems);

		ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest();

		request.FunctionName = "RetriveQuestItems";
		request.FunctionParameter = new { CharacterId = PF_PlayerData.activeCharacter.characterDetails.CharacterId, ItemIds = PF_GamePlay.QuestProgress.ItemsFound};
		PlayFabClientAPI.ExecuteCloudScript(request, OnRetriveQuestItemsSuccess, PF_Bridge.PlayFabErrorCallback);
	}
	
	/// <summary>
	/// Raises the retrive quest items success event.
	/// </summary>
	/// <param name="result">Result.</param>
	public static void OnRetriveQuestItemsSuccess(ExecuteCloudScriptResult result)
	{
		if(!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
			return;

		Debug.Log(result.ToString());
		QuestProgress.ItemsGranted = PlayFab.SimpleJson.DeserializeObject<List<ItemGrantResult>>(result.FunctionResult.ToString());
		
		PF_GamePlay.QuestProgress.areItemsAwarded = true;
		
		PF_PlayerData.GetCharacterInventory(PF_PlayerData.activeCharacter.characterDetails.CharacterId);
		
		PF_Bridge.RaiseCallbackSuccess("Items granted", PlayFabAPIMethods.RetriveQuestItems, MessageDisplayStyle.none);
	}
	
	/// <summary>
	/// Retrives the store items.
	/// </summary>
	/// <param name="storeName">Store name.</param>
	/// <param name="callback">Callback.</param>
	public static void RetriveStoreItems(string storeName, UnityAction<List<StoreItem>> callback = null)
	{
		DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetStoreItems);
		GetStoreItemsRequest request = new GetStoreItemsRequest();
		request.StoreId = storeName;
		request.CatalogVersion = "CharacterClasses";
		PlayFabClientAPI.GetStoreItems(request, (GetStoreItemsResult result) =>
		{
			OnRetriveStoreItemsSuccess(result);
			if(callback != null)
			{
				callback(result.Store);
			}

		}, PF_Bridge.PlayFabErrorCallback);
		
	}
	
	/// <summary>
	/// Raises the retrive store items success event.
	/// </summary>
	/// <param name="result">Result.</param>
	public static void OnRetriveStoreItemsSuccess(GetStoreItemsResult result)
	{		
		mostRecentStore = result.Store;
		PF_Bridge.RaiseCallbackSuccess("Store Retrieved", PlayFabAPIMethods.GetStoreItems, MessageDisplayStyle.none);
	}
	
	
	/// <summary>
	/// Makes the RM purchase.
	/// </summary>
	/// <param name="itemId">Item identifier.</param>
	public static void MakeRMPurchase(string itemId)
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) 
		{
			DialogCanvasController.RequestLoadingPrompt (PlayFabAPIMethods.MakePurchase);
			
			
			Debug.Log ("RM: " + itemId);
			OnePF.OpenIAB.purchaseProduct (itemId);
		} else {
			PF_Bridge.RaiseCallbackError("Current plaform does not support IAP; cannot process transaction.", PlayFabAPIMethods.MakePurchase, MessageDisplayStyle.error);		
		}
	}
	
	
	/// <summary>
	/// Starts the buy store item.
	/// </summary>
	/// <param name="item">Item.</param>
	/// <param name="storeID">Store I.</param>
	public static void StartBuyStoreItem(CatalogItem item, string storeID)
	{
        if (item.VirtualCurrencyPrices.ContainsKey("RM"))
		{
			PF_Bridge.IAB_CurrencyCode = "US";
			PF_Bridge.IAB_Price = (int)item.VirtualCurrencyPrices["RM"];
			
			MakeRMPurchase(item.ItemId);
            return;
		}

        string characterId = PF_PlayerData.activeCharacter == null ? null : PF_PlayerData.activeCharacter.characterDetails.CharacterId;
		var vcKVP = item.VirtualCurrencyPrices.First();

        if (characterId != null)
		{
			DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.MakePurchase);

			ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest();
			request.FunctionName = "PurchaseItem";
			request.FunctionParameter = new { ItemPrice = (int)vcKVP.Value, CurrencyCode = vcKVP.Key, CharacterId = PF_PlayerData.activeCharacter.characterDetails.CharacterId, ItemId = item.ItemId };

			PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) => 
			{
				if(!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
					return;

				if((bool)result.FunctionResult == true)
				{
					PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.MakePurchase, MessageDisplayStyle.none);	
				}
				else
				{
					PF_Bridge.RaiseCallbackError("Could not process request due to insufficient VC.", PlayFabAPIMethods.MakePurchase, MessageDisplayStyle.error);
				}
					
			}, PF_Bridge.PlayFabErrorCallback);
		}
        else if (characterId == null)
        {
            // normal purchase item flow
            PurchaseItemRequest request = new PurchaseItemRequest();
            request.ItemId = item.ItemId;
            request.VirtualCurrency = vcKVP.Key;
            request.Price = (int)vcKVP.Value;
            request.StoreId = storeID;
			DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.MakePurchase);
            PlayFabClientAPI.PurchaseItem(request, OnBuyStoreItemSuccess, PF_Bridge.PlayFabErrorCallback);
        }
        else
        {
            Debug.LogWarning("Store purchase failed: " + characterId);
        }
	}

	/// <summary>
	/// Raises the buy store item success event.
	/// </summary>
	/// <param name="result">Result.</param>
	public static void OnBuyStoreItemSuccess(PurchaseItemResult result)
	{		
		PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.MakePurchase, MessageDisplayStyle.none);
		Debug.Log (string.Format("{0} Items Purchased!", result.Items.Count));
        GameController.CharacterSelectDataRefresh();
	}

	/// <summary>
	/// Consumes the item.
	/// </summary>
	/// <param name="id">Identifier.</param>
	public static void ConsumeItem(string id)
	{
		//DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericCloudScript);
		ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest();
		request.FunctionName = "ConsumeItem";
		request.FunctionParameter = new { CharacterId = PF_PlayerData.activeCharacter.characterDetails.CharacterId, ItemId = id };

		PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) => 
		{
			if(!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
				return;

			PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.ExecuteCloudScript, MessageDisplayStyle.none);	
		}, PF_Bridge.PlayFabErrorCallback);
	}


	public static void RedeemCoupon(string code)
	{
		RedeemCouponRequest request = new RedeemCouponRequest();
		request.CouponCode = code;

		PlayFabClientAPI.RedeemCoupon(request, (RedeemCouponResult result) => 
		{
			PF_Bridge.RaiseCallbackSuccess("Coupon (" + request.CouponCode + ") Redeemd: Granted " + result.GrantedItems.Count + " items.", PlayFabAPIMethods.RedeemCoupon, MessageDisplayStyle.none);
		}, PF_Bridge.PlayFabErrorCallback);
	}
}

