using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
	public class ConfirmationPromptController : MonoBehaviour
	{
		public Button confirmButton;
		public Button denyButton;
		public Text message;
		public Text title;

		private Action<bool> responseCallback;
		public Action runOnClose;
		// Use this for initialization
		void Start()
		{
			this.confirmButton.onClick.AddListener(() => { Confirmed(); });
			this.denyButton.onClick.AddListener(() => { Denied(); });
		}

		public void Confirmed()
		{
			HideConfirmationPrompt();
			responseCallback(true);

		}

		public void Denied()
		{
			HideConfirmationPrompt();
			responseCallback(false);
		}

		public void ShowConfirmationPrompt(string title, string message, Action<bool> callback, Action onClose)
		{
			this.message.text = message;
			this.title.text = title;
			this.responseCallback = callback;
			this.runOnClose = onClose;

			this.gameObject.SetActive(true);
		}

		public void HideConfirmationPrompt()
		{
			this.runOnClose();
			this.gameObject.SetActive(false);
		}
	}

}