using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
	public class ErrorPromptController : MonoBehaviour
	{
		public Image banner;
		public Text title;
		public Text body;
		public Button close;

		public void RaiseErrorDialog(string txt)
		{
			this.body.text = txt;
			this.gameObject.SetActive(true);
		}

		public void CloseErrorDialog()
		{
			this.gameObject.SetActive(false);
		}

	}
}