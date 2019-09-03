using System.Collections.Generic;
using UnicornBattle.Models;
using UnityEngine;

namespace UnicornBattle.Controllers
{

	public class ItemViewerContainerController : MonoBehaviour
	{
		public UnlockSliderController slider;
		public ItemViewerController controller;

		public Transform itemContainer;
		public Transform itemList;
		public Transform itemPrefab;

		public void Init()
		{
			slider.SetupSlider();
		}

		void EnableUnlockedItemsView(List<UBInventoryItem> unlockedItems = null)
		{
			//		this.ItemsTransform.gameObject.SetActive(true);
			//		this.ItemDescription.gameObject.SetActive(false);

		}

		void DisableUnlockedItemsView()
		{
			//		this.ItemsTransform.gameObject.SetActive(false);
			//		this.ItemDescription.gameObject.SetActive(true);
		}
	}
}