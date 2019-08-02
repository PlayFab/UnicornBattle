using UnityEngine;

namespace UnicornBattle.Controllers
{
	public class StoreDisplayController : MonoBehaviour
	{
		public SelectedStoreItemController selectedItemController;
		//public StoreItem selectedItem;
		public TEST_StoreItem selectedItem;
		public StoreDisplayPageController pageController;

		// Use this for initialization
		void Start()
		{
			HideSelected();
		}

		public void HideSelected()
		{
			this.selectedItem = null;
			this.selectedItemController.gameObject.SetActive(false);
		}

		public void ShowSelected()
		{
			this.selectedItemController.gameObject.SetActive(true);
		}

		public void StoreItemClicked(TEST_StoreItem item)
		{
			ShowSelected();

			this.selectedItem = item;

			this.selectedItemController.itemArt.color = item.iconColor;
			this.selectedItemController.displayName.text = item.displayName;
			this.selectedItemController.displayText.text = item.displayText;

		}
	}
}