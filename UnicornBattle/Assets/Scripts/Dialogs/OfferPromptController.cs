using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PlayFab.ClientModels;
using PlayFab.Json;

public class OfferPromptController : MonoBehaviour
{
    public Text Title;
    public Text OfferName;
    public Text OfferDesc;
    public Button Redeem;
    public Transform OfferItem;
    public Transform OfferStore;
    public Button VisitStore;
    public Image ItemIcon;
    public Text ItemName;
    private UB_OfferData selectedDetails = null;

    private CatalogItem activeItem;
    private ItemInstance activeInstance;

    public void Init()
    {
        if (PF_PlayerData.OfferContainers.Count > 0)
        {
            ShowItemOffer(PF_PlayerData.OfferContainers[0]);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // move to PF_PlayerData?
    public void ShowItemOffer(ItemInstance item)
    {
        activeItem = PF_GameData.offersCataogItems.Find((i) => { return i.ItemId == item.ItemId; });
        activeInstance = item;

        if (activeItem != null)
        {
            if (activeItem.Tags.IndexOf("SingleUse") > -1)
            {
                if (PF_PlayerData.RedeemedOffers.IndexOf(activeItem.ItemId) > -1)
                {
                    // this is a onetime offer and it has already been redeemed for this player.
                    PF_Bridge.RaiseCallbackError("This is a one-time offer and it has already redeemed for this player.", PlayFabAPIMethods.ConsumeOffer, MessageDisplayStyle.error);
                    ContinueClicked(true);
                    return;
                }
            }

            OfferName.text = activeItem.DisplayName;
            OfferDesc.text = activeItem.Description;

            var customData = JsonWrapper.DeserializeObject<Dictionary<string, string>>(activeItem.CustomData);

            string itemAwarded;
            var awardIcon = "Default";
            customData.TryGetValue("itemAwarded", out itemAwarded);
            if (customData.ContainsKey("itemAwarded"))
            {
                var awardItem = PF_GameData.GetCatalogItemById(itemAwarded);
                awardIcon = PF_GameData.GetIconByItemById(itemAwarded);
                ItemName.text = awardItem.DisplayName;
            }
            else
            {
                ItemName.text = "Offer reward not found.";
            }
            ItemIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(awardIcon);
            OfferItem.gameObject.SetActive(true);

            OfferStore.gameObject.SetActive(false);
        }
    }

    public void ShowOffer(string guid)
    {
        UB_OfferData details = null;

        PF_GameData.Offers.TryGetValue(PF_PlayerData.pendingOffers[guid].OfferId, out details);

        if (details != null)
        {

            OfferName.text = details.OfferName;
            OfferDesc.text = details.OfferDescription;

            if (details.ItemToGrant != null)
            {
                var grantItem = PF_GameData.GetCatalogItemById(details.ItemToGrant);
                if (grantItem != null)
                {
                    ItemName.text = grantItem.DisplayName;
                    var iconString = PF_GameData.GetIconByItemById(grantItem.ItemId);
                    ItemIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(iconString);
                    OfferItem.gameObject.SetActive(true);
                }
                else
                {
                    Debug.Log("Grant Item not found in catalog");
                    ItemName.text = GlobalStrings.GRANT_CATALOG_ERR_MSG;
                }
            }
            else
            {
                OfferItem.gameObject.SetActive(false);
            }

            if (details.StoreToUse != null && selectedDetails != null)
            {
                VisitStore.GetComponent<Text>().text = "Visit " + selectedDetails.StoreToUse;
                OfferStore.gameObject.SetActive(true);
            }
            else
            {
                OfferStore.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Could not locate a corresponding offer on the server.");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wasAlreadyRedemed">If set to <c>true</c>  if the player has already redeemed this offer.</param>
    public void ContinueClicked(bool wasAlreadyRedemed = false)
    {
        UnityAction<string> afterRedeem = (string result) =>
        {
            if (!string.IsNullOrEmpty(result))
                DialogCanvasController.RequestItemViewer(new List<string>() { result }, true);

            PF_PlayerData.OfferContainers.RemoveAt(0);
            Init();
        };

        PF_PlayerData.RedeemItemOffer(activeItem, activeInstance.ItemInstanceId, afterRedeem, wasAlreadyRedemed);
    }
}
