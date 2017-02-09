using System.Collections.Generic;
using UnityEngine;

public class StoreCurrencyBarController : MonoBehaviour
{
    public List<CurrencyDisplayItem> items = new List<CurrencyDisplayItem>();
    public Transform CurrencyDisplayItemPrefab;
    public Transform DisplayContainer;
    public FloatingStoreController controller;

    public void Init()
    {
        for (var i = 0; i < DisplayContainer.transform.childCount; i++)
        {
            var go = DisplayContainer.transform.GetChild(i);
            Destroy(go.gameObject);
        }

        items.Clear();
        if (PF_PlayerData.virtualCurrency != null && PF_PlayerData.virtualCurrency.Count > 0)
            for (var z = 0; z < controller.currenciesInUse.Count; z++)
                DisplayVc(z, PF_PlayerData.virtualCurrency);
    }

    private void DisplayVc(int z, Dictionary<string, int> vcBalances)
    {
        var key = controller.currenciesInUse[z];
        if (key == GlobalStrings.REAL_MONEY_CURRENCY)
            return;

        var go = Instantiate(CurrencyDisplayItemPrefab);
        go.SetParent(DisplayContainer, false);
        var comp = go.GetComponent<CurrencyDisplayItem>();
        comp.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(key);
        int playerBalance;
        vcBalances.TryGetValue(key, out playerBalance);
        comp.value.text = string.Format("{0:n0}", playerBalance);
        items.Add(comp);
    }
}
