using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InventoryCurrencyBarController : MonoBehaviour {
	
	public List<CurrencyDisplayItem> items = new List<CurrencyDisplayItem>();
	public Transform CurrencyDisplayItemPrefab;
	public Transform DisplayContainer;
	public FloatingInventoryController controller;

	public void Init(Dictionary<string, int> cd)
	{
		for(int i = 0; i < this.DisplayContainer.transform.childCount; i++)
		{
			Transform go = this.DisplayContainer.transform.GetChild(i);
			Destroy(go.gameObject);
		}
		
		this.items.Clear ();
		
		if(cd != null && cd.Count > 0)
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
					cd.TryGetValue(key, out playerBalance);
					comp.value.text = string.Format("{0:n0}", playerBalance);
					
				}
				else
				{
					comp.value.text = string.Format("{0:C2}", "0");
				}
				this.items.Add(comp);
			}
		}
	}
	
}
