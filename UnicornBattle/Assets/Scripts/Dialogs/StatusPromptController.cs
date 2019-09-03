using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
	public class StatusPromptController : MonoBehaviour
	{
		public Button confirmButton;
		public Text message;
		public Text title;

		public Action runOnClose;
		// Use this for initialization
		void Start()
		{
			this.confirmButton.onClick.AddListener(() => { Confirmed(); });
		}

		public void Confirmed()
		{
			HideConfirmationPrompt();
		}

		public void ShowMessagePrompt(string title, string message, Action onClose)
		{
			this.message.text = message;
			this.title.text = title;
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