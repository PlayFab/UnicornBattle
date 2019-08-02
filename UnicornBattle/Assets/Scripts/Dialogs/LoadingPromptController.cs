using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
	public class LoadingPromptController : MonoBehaviour
	{
		public Image spinner;

		public void RaiseLoadingPrompt()
		{
			this.gameObject.SetActive(true);
		}

		public void CloseLoadingPrompt()
		{
			this.gameObject.SetActive(false);
		}
	}
}