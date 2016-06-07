using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class StoreCurrencyBarController : MonoBehaviour {

	public List<CurrencyDisplayItem> items = new List<CurrencyDisplayItem>();
	public Transform CurrencyDisplayItemPrefab;
	public Transform DisplayContainer;
	public FloatingStoreController controller;

	public void Init()
	{
		for(int i = 0; i < this.DisplayContainer.transform.childCount; i++)
		{
			Transform go = this.DisplayContainer.transform.GetChild(i);
			Destroy(go.gameObject);
		}

		this.items.Clear ();
		
		if(PF_PlayerData.activeCharacter != null)
		{
			if(PF_PlayerData.characterVirtualCurrency != null && PF_PlayerData.characterVirtualCurrency.Count > 0)
			{
				for(int z = 0; z < controller.currenciesInUse.Count; z++)
				{
					string key = controller.currenciesInUse[z];
	
					// no need to show RM balances
	
						Transform go = Instantiate(this.CurrencyDisplayItemPrefab);
						go.SetParent(DisplayContainer, false);
						CurrencyDisplayItem comp = go.GetComponent<CurrencyDisplayItem>();
						comp.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(key);
						if(key != "RM")
						{
							int playerBalance;
							PF_PlayerData.characterVirtualCurrency.TryGetValue(key, out playerBalance);
							comp.value.text = string.Format("{0:n0}", playerBalance);
							
						}
						else
						{
							comp.value.text = string.Format("${0}.00", "0");
						}
						this.items.Add(comp);
				}
			}
		}
		else
		{
			// need to show player balances, not character
			if(PF_PlayerData.virtualCurrency != null && PF_PlayerData.virtualCurrency.Count > 0)
			{
				for(int z = 0; z < controller.currenciesInUse.Count; z++)
				{
					string key = controller.currenciesInUse[z];
					
					// no need to show RM balances
					
					Transform go = Instantiate(this.CurrencyDisplayItemPrefab);
					go.SetParent(DisplayContainer, false);
					CurrencyDisplayItem comp = go.GetComponent<CurrencyDisplayItem>();
					comp.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(key);
					if(key != "RM")
					{
						int playerBalance;
						PF_PlayerData.virtualCurrency.TryGetValue(key, out playerBalance);
						comp.value.text = string.Format("{0:n0}", playerBalance);
						
					}
					else
					{
						comp.value.text = string.Format("${0}.00", "0");
					}
					this.items.Add(comp);
				}
			}
		}
	}

}
