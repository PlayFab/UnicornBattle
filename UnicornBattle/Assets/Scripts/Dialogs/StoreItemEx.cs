using UnityEngine;
using UnityEngine.UI;

//IPointerClickHandler
namespace UnicornBattle.Controllers
{
	public class StoreItemEx : MonoBehaviour
	{
		public Image image;
		public Text text;
		public TEST_StoreItem itemData;

		//private StoreDisplayController storeController;

		// Use this for initialization
		void Start()
		{
			//this.storeController = this.gameObject.GetComponentInParent<StoreDisplayController>();
			if (this.itemData != null)
			{
				this.image.color = itemData.iconColor;
				this.text.color = itemData.textColor;
				this.text.text = itemData.displayText;
			}

		}

		//	public void OnPointerClick(PointerEventData pData)
		//	{
		//		//Debug.Log("!!!!");
		//		if(this.itemData != null)
		//		{
		//			storeController.StoreItemClicked(itemData);
		//		}
		//	}
	}
}