using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

using PlayFab.ClientModels;
using PlayFab.Json;

public class OfferPromptController : MonoBehaviour {
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
		if(PF_PlayerData.OfferContainers.Count > 0)
		{
			ShowItemOffer(PF_PlayerData.OfferContainers[0]);
		}
		else
		{
			this.gameObject.SetActive(false);
		}
	}
	
	
	// move to PF_PlayerData?
	public void ShowItemOffer(ItemInstance item)
	{
		this.activeItem = PF_GameData.offersCataogItems.Find( (i) => { return i.ItemId == item.ItemId; });
		this.activeInstance = item;

		if(activeItem != null)
		{
			if(activeItem.Tags.IndexOf("SingleUse") > -1)
			{
				if(PF_PlayerData.RedeemedOffers.IndexOf(activeItem.ItemId) > -1)
				{
					// this is a onetime offer and it has already been redeemed for this player.
					PF_Bridge.RaiseCallbackError("This is a one-time offer and it has already redeemed for this player.", PlayFabAPIMethods.ConsumeOffer, MessageDisplayStyle.error);
					ContinueClicked(true);
					return;
				}
			}
			
			this.OfferName.text = activeItem.DisplayName;
			this.OfferDesc.text = activeItem.Description;
			
			Dictionary<string, string> customData = PlayFab.Json.JsonWrapper.DeserializeObject<Dictionary<string, string>>(activeItem.CustomData);
			
			string itemAwarded = "Random DropTable";
			customData.TryGetValue("itemAwarded", out itemAwarded);
			
			
			string awardIcon = "Default";
			if(customData.ContainsKey("itemAwarded"))
			{
				CatalogItem awardItem = PF_GameData.catalogItems.Find((i) => { return i.ItemId == itemAwarded;});
				Dictionary<string, string> awardCustomData = PlayFab.Json.JsonWrapper.DeserializeObject<Dictionary<string, string>>(awardItem.CustomData);
				
				awardCustomData.TryGetValue("icon", out awardIcon);
				this.ItemName.text = awardItem.DisplayName;
			}
			else
			{
				this.ItemName.text = "Offer reward not found.";
			}
			this.ItemIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(awardIcon);			
			this.OfferItem.gameObject.SetActive(true);

			this.OfferStore.gameObject.SetActive(false);
		}
	}
	
	
	
	public void ShowOffer(string guid)
	{
		UB_OfferData details = null;
		
		PF_GameData.Offers.TryGetValue(PF_PlayerData.pendingOffers[guid].OfferId, out details);
		
		if(details != null)
		{
			
			this.OfferName.text = details.OfferName;
			this.OfferDesc.text = details.OfferDescription;
			
			if(details.ItemToGrant != null)
			{
				PlayFab.ClientModels.CatalogItem grantItem = PF_GameData.catalogItems.Find( (i) => 
				                                                                           {
					return i.ItemId == details.ItemToGrant;
				});
				
				if(grantItem != null)
				{
					this.ItemName.text = grantItem.DisplayName;
					
					string iconString = string.Empty;
					Dictionary<string, string> customData = PlayFab.Json.JsonWrapper.DeserializeObject<Dictionary<string, string>>(grantItem.CustomData);
					customData.TryGetValue("icon", out iconString);
					
					this.ItemIcon.overrideSprite = GameController.Instance.iconManager.GetIconById(iconString);
					this.OfferItem.gameObject.SetActive(true);
				}
				else
				{
					Debug.Log("Grant Item not found in catalog");
                    this.ItemName.text = GlobalStrings.GRANT_CATALOG_ERR_MSG;
				}
			}
			else
			{
				this.OfferItem.gameObject.SetActive(false);
			}
	
	        if (details.StoreToUse != null && this.selectedDetails != null)
			{
                this.VisitStore.GetComponent<Text>().text = "Visit " + this.selectedDetails.StoreToUse;
				this.OfferStore.gameObject.SetActive(true);
			}
			else
			{
				this.OfferStore.gameObject.SetActive(false);
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
			if(!string.IsNullOrEmpty(result))
			{
				DialogCanvasController.RequestItemViewer(new List<string>() { result }, true);
			}
			
			PF_PlayerData.OfferContainers.RemoveAt(0);	
			this.Init();
		};
		
		PF_PlayerData.RedeemItemOffer(this.activeItem, this.activeInstance.ItemInstanceId, afterRedeem, wasAlreadyRedemed);
	}
}
