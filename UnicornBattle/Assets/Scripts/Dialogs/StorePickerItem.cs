using UnityEngine;

//IPointerClickHandler
namespace UnicornBattle.Controllers
{
	public class StorePickerItem : MonoBehaviour
	{

		public string storeName;
		public int storeIndex;
		public StorePicker sPicker;

		// Use this for initialization
		void Start()
		{
			this.sPicker = this.gameObject.GetComponentInParent<StorePicker>();
		}

		//	public void OnPointerClick(PointerEventData pData)
		//	{
		//		//Debug.Log("!!!!");
		//		sPicker.StorePickerItemClicked(this);
		//	}

	}
}