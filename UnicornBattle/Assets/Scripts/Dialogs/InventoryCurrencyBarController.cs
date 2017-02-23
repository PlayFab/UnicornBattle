using System.Collections.Generic;
using UnityEngine;

public class InventoryCurrencyBarController : MonoBehaviour
{
    public List<CurrencyDisplayItem> items = new List<CurrencyDisplayItem>();
    public Transform CurrencyDisplayItemPrefab;
    public Transform DisplayContainer;
    public FloatingInventoryController controller;

    public void Init(Dictionary<string, int> vcBalances)
    {
        for (var i = 0; i < DisplayContainer.transform.childCount; i++)
        {
            var go = DisplayContainer.transform.GetChild(i);
            Destroy(go.gameObject);
        }

        items.Clear();

        if (vcBalances == null || vcBalances.Count == 0)
            return;

        for (var z = 0; z < controller.currenciesInUse.Count; z++)
        {
            var key = controller.currenciesInUse[z];
            if (key == GlobalStrings.REAL_MONEY_CURRENCY)
                continue;
            DisplayVc(z, vcBalances);
        }
    }

    private void DisplayVc(int z, Dictionary<string, int> vcBalances)
    {
        var key = controller.currenciesInUse[z];
        if (key == GlobalStrings.REAL_MONEY_CURRENCY)
            return;

        var go = Instantiate(CurrencyDisplayItemPrefab);
        go.SetParent(DisplayContainer, false);
        var comp = go.GetComponent<CurrencyDisplayItem>();
        comp.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(key, IconManager.IconTypes.Misc);
        int playerBalance;
        vcBalances.TryGetValue(key, out playerBalance);
        comp.value.text = string.Format("{0:n0}", playerBalance);
        items.Add(comp);
    }
}
