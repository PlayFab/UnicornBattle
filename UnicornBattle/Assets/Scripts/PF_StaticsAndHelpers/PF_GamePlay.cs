using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class PF_GamePlay
{
    public static List<UB_GamePlayEncounter> encounters;
    public static LevelItemHelper ActiveQuest;
    public static QuestTracker QuestProgress = new QuestTracker();
    public static bool UseRaidMode = true; // TODO default to false - or make it a setting in the player's data
    public static bool isHardMode = false;

    public static List<StoreItem> mostRecentStore = new List<StoreItem>();

    // class colors used to colorize UI based on active character.
    public static Color ClassColor1;
    public static Color ClassColor2;
    public static Color ClassColor3;

    public enum ShakeEffects { None = 0, DecreaseHealth, DecreaseMana, IncreaseHealth, IncreaseMana }
    public enum PlayerSpellInputs { Null = 0, Spell1, Spell2, Spell3, Flee, UseItem }
    public enum PlayerEncounterInputs { Null = 0, Attack, UseItem, Evade, ViewStore, Rescue }
    public enum GameplayEventTypes { Null, StartQuest, EndQuest, IntroQuest, OutroQuest, IntroAct, OutroAct, IntroEncounter, OutroEncounter, PlayerTurnBegins, PlayerTurnEnds, EnemyTurnBegins, EnemyTurnEnds, StartBossBattle, PlayerDied, PlayerRespawn } // MAY need more types here for things like store, heros and bosses, perhaps even one for spelleffects
    public enum TurnStates { Null, Player, Enemy }

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
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg != null)
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
        if (cg != null)
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

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "SaveProgress",
            FunctionParameter = new { CurrentPlayerData = PF_PlayerData.activeCharacter, QuestProgress = PF_GamePlay.QuestProgress, LevelRamp = PF_GameData.CharacterLevelRamp },
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnSavePlayerDataSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    /// <summary>
    /// Callback after 
    /// </summary>
    /// <param name="result">Result.</param>
    private static void OnSavePlayerDataSuccess(ExecuteCloudScriptResult result)
    {
        if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
            return;

        PF_Bridge.RaiseCallbackSuccess("Player Info Saved", PlayFabAPIMethods.SavePlayerInfo, MessageDisplayStyle.none);
    }
    #endregion

    /// <summary>
    /// Retrieves the quest items.
    /// </summary>
    public static void RetriveQuestItems()
    {
        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.RetriveQuestItems);

        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "RetriveQuestItems",
            FunctionParameter = new { CharacterId = PF_PlayerData.activeCharacter.characterDetails.CharacterId, ItemIds = QuestProgress.ItemsFound },
        };
        PlayFabClientAPI.ExecuteCloudScript(request, OnRetrieveQuestItemsSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    /// <summary>
    /// Raises the retrieve quest items success event.
    /// </summary>
    /// <param name="result">Result.</param>
    private static void OnRetrieveQuestItemsSuccess(ExecuteCloudScriptResult result)
    {
        if (!PF_Bridge.VerifyErrorFreeCloudScriptResult(result))
            return;

        //Debug.Log(result.ToString());
        QuestProgress.ItemsGranted = PlayFab.Json.JsonWrapper.DeserializeObject<List<ItemGrantResult>>(result.FunctionResult.ToString());

        PF_GamePlay.QuestProgress.areItemsAwarded = true;

        PF_PlayerData.GetCharacterInventory(PF_PlayerData.activeCharacter.characterDetails.CharacterId);

        PF_Bridge.RaiseCallbackSuccess("Items granted", PlayFabAPIMethods.RetriveQuestItems, MessageDisplayStyle.none);
    }

    /// <summary>
    /// Retrieves the store items.
    /// </summary>
    /// <param name="storeName">Store name.</param>
    /// <param name="callback">Callback.</param>
    public static void RetrieveStoreItems(string storeName, UnityAction<List<StoreItem>> callback = null)
    {
        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GetStoreItems);
        var request = new GetStoreItemsRequest
        {
            StoreId = storeName,
            CatalogVersion = GlobalStrings.PrimaryCatalogName,
        };
        PlayFabClientAPI.GetStoreItems(request, result =>
        {
            OnRetrieveStoreItemsSuccess(result);
            if (callback != null)
            {
                callback(result.Store);
            }

        }, PF_Bridge.PlayFabErrorCallback);
    }

    /// <summary>
    /// Raises the retrieve store items success event.
    /// </summary>
    /// <param name="result">Result.</param>
    private static void OnRetrieveStoreItemsSuccess(GetStoreItemsResult result)
    {
        mostRecentStore = result.Store;
        PF_Bridge.RaiseCallbackSuccess("Store Retrieved", PlayFabAPIMethods.GetStoreItems, MessageDisplayStyle.none);
    }


    /// <summary>
    /// Makes the RM purchase.
    /// </summary>
    /// <param name="itemId">Item identifier.</param>
    public static void MakeRmPurchase(string itemId)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.MakePurchase);


            Debug.Log("RM: " + itemId);
            OnePF.OpenIAB.purchaseProduct(itemId);
        }
        else
        {
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

            MakeRmPurchase(item.ItemId);
            return;
        }

        var vcKvp = item.VirtualCurrencyPrices.First();

        // normal purchase item flow
        var request = new PurchaseItemRequest();
        request.ItemId = item.ItemId;
        request.VirtualCurrency = vcKvp.Key;
        request.Price = (int)vcKvp.Value;
        request.StoreId = storeID;
        if (PF_PlayerData.activeCharacter != null)
        {
            request.CharacterId = PF_PlayerData.activeCharacter.characterDetails.CharacterId;
        }

        DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.MakePurchase);
        PlayFabClientAPI.PurchaseItem(request, OnBuyStoreItemSuccess, PF_Bridge.PlayFabErrorCallback);
    }

    /// <summary>
    /// Raises the buy store item success event.
    /// </summary>
    /// <param name="result">Result.</param>
    private static void OnBuyStoreItemSuccess(PurchaseItemResult result)
    {
        PF_Bridge.RaiseCallbackSuccess(string.Empty, PlayFabAPIMethods.MakePurchase, MessageDisplayStyle.none);
        Debug.Log(string.Format("{0} Items Purchased!", result.Items.Count));
        GameController.CharacterSelectDataRefresh();
    }

    /// <summary>
    /// Consumes the item.
    /// </summary>
    /// <param name="id">Identifier.</param>
    public static void ConsumeItem(string id)
    {
        var request = new ConsumeItemRequest();
        request.ConsumeCount = 1;
        request.ItemInstanceId = id;
        if (PF_PlayerData.activeCharacter != null)
        {
            request.CharacterId = PF_PlayerData.activeCharacter.characterDetails.CharacterId;
        }


        //DialogCanvasController.RequestLoadingPrompt(PlayFabAPIMethods.GenericCloudScript);
        PlayFabClientAPI.ConsumeItem(request, (ConsumeItemResult result) =>
        {

            PF_Bridge.RaiseCallbackSuccess("", PlayFabAPIMethods.ConsumeItemUse, MessageDisplayStyle.none);
        }, PF_Bridge.PlayFabErrorCallback);
    }


    public static void RedeemCoupon(string code)
    {
        var request = new RedeemCouponRequest();
        request.CouponCode = code;

        PlayFabClientAPI.RedeemCoupon(request, (RedeemCouponResult result) =>
        {
            PF_Bridge.RaiseCallbackSuccess("Coupon (" + request.CouponCode + ") Redeemd: Granted " + result.GrantedItems.Count + " items.", PlayFabAPIMethods.RedeemCoupon, MessageDisplayStyle.none);
        }, PF_Bridge.PlayFabErrorCallback);
    }
}

