using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
    public class TextInputPromptController : MonoBehaviour
    {
        public Button confirmButton;
        public Button denyButton;
        public Text message;
        public Text title;
        public InputField userInput;

        private Action<string> responseCallback;

        public Action runOnClose; // will generally be used for hiding the overlay tint.

        // Use this for initialization
        private void Start()
        {
            this.confirmButton.onClick.AddListener(Confirmed);
            this.denyButton.onClick.AddListener(Denied);
        }

        public void Confirmed()
        {
            HideConfirmationPrompt();
            responseCallback(userInput.text);
        }

        public void Denied()
        {
            HideConfirmationPrompt();
            responseCallback(null);
        }

        public void ShowTextInputPrompt(
            string titleText,
            string messageText,
            Action<string> callback,
            string defaultValue,
            int maxLength
        )
        {
            this.message.text = messageText;
            this.title.text = titleText;
            this.responseCallback = callback;
            this.userInput.text = string.IsNullOrEmpty(defaultValue) ? string.Empty : defaultValue;
            this.userInput.characterLimit = maxLength;
            this.gameObject.SetActive(true);
        }

        public void HideConfirmationPrompt()
        {
            this.gameObject.SetActive(false);
        }
    }
}