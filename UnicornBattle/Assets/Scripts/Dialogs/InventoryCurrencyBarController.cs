using System.Collections.Generic;
using UnicornBattle.Managers;
using UnityEngine;

namespace UnicornBattle.Controllers
{
    public class InventoryCurrencyBarController : MonoBehaviour
    {
        public List<CurrencyDisplayItem> items = new List<CurrencyDisplayItem>();
        public Transform CurrencyDisplayItemPrefab;
        public Transform DisplayContainer;
        public FloatingInventoryController controller;

        public void Init()
        {
            for (int i = 0; i < DisplayContainer.transform.childCount; i++)
            {
                Transform l_child = DisplayContainer.transform.GetChild(i);
                Destroy(l_child.gameObject);
            }

            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            items.Clear();
            for (int l_index = 0; l_index < controller.currenciesInUse.Count; l_index++)
            {
                string l_key = controller.currenciesInUse[l_index];
                if (l_key == GlobalStrings.REAL_MONEY_CURRENCY)
                    continue;
                displayVirtualCurrency(l_key, l_inventoryMgr.GetCurrencyAmount(l_key));
            }
        }

        private void displayVirtualCurrency(string p_currencyName, int p_currencyBalance)
        {
            var l_currencyDisplayItemTransform = Instantiate(CurrencyDisplayItemPrefab);
            l_currencyDisplayItemTransform.SetParent(DisplayContainer, false);

            var l_currencyDisplayItem = l_currencyDisplayItemTransform.GetComponent<CurrencyDisplayItem>();
            l_currencyDisplayItem.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(p_currencyName, IconManager.IconTypes.Misc);
            l_currencyDisplayItem.value.text = string.Format("{0:n0}", p_currencyBalance);
            items.Add(l_currencyDisplayItem);
        }
    }
}