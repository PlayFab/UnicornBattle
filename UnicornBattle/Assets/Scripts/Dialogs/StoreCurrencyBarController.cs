using System.Collections.Generic;
using UnicornBattle.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class StoreCurrencyBarController : MonoBehaviour
    {
        public List<CurrencyDisplayItem> items = new List<CurrencyDisplayItem>();
        public Transform CurrencyDisplayItemPrefab;
        public Transform DisplayContainer;
        public FloatingStoreController controller;

        public void Init()
        {
            for (int i = 0; i < DisplayContainer.transform.childCount; i++)
            {
                var go = DisplayContainer.transform.GetChild(i);
                Destroy(go.gameObject);
            }

            var l_inventoryMgr = MainManager.Instance.getInventoryManager();
            if (null == l_inventoryMgr) return;

            items.Clear();

            for (int j = 0; j < controller.currenciesInUse.Count; j++)
            {
                string l_currencyName = controller.currenciesInUse[j];
                displayVirtualCurrency(l_currencyName, l_inventoryMgr.GetCurrencyAmount(l_currencyName));
            }
        }

        private void displayVirtualCurrency(string p_currencyName, int p_currencyAmount)
        {
            var l_cdiTransform = Instantiate(CurrencyDisplayItemPrefab);
            l_cdiTransform.SetParent(DisplayContainer, false);

            var l_cdi = l_cdiTransform.GetComponent<CurrencyDisplayItem>();
            l_cdi.icon.overrideSprite = GameController.Instance.iconManager.GetIconById(p_currencyName, IconManager.IconTypes.Misc);

            l_cdi.value.text = string.Format("{0:n0}", p_currencyAmount);

            l_cdiTransform.GetComponent<LayoutElement>().minWidth = 200f;

            items.Add(l_cdi);
        }
    }
}