using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using PlayFab.ClientModels;

public class RegistrationController : MonoBehaviour {
	public InputField userName;
	public InputField email;
	public InputField pass1;
	public InputField pass2;

	public Button register;
	public Button exit;
	public AccountStatusController controller;

	private UnityAction<AddUsernamePasswordResult> callback;

	public void Init(UnityAction<AddUsernamePasswordResult> cb = null)
	{
		if (cb != null) 
		{
			this.callback = cb;
		}
	}


	public void CloseRegistration()
	{
		this.gameObject.SetActive (false);
	}

	public void RegisterClicked()
	{
		if (!string.IsNullOrEmpty(pass1.text) && !string.IsNullOrEmpty(pass2.text) && string.Equals (this.pass1.text, this.pass2.text)) {
			PF_Authentication.AddUserNameAndPassword (this.userName.text, this.pass1.text, this.email.text, (AddUsernamePasswordResult result) =>
			{
				this.callback (result);
				CloseRegistration ();
			});
		} else 
		{
            PF_Bridge.RaiseCallbackError(GlobalStrings.REGISTER_FAIL_MSG, PlayFabAPIMethods.AddUsernamePassword, MessageDisplayStyle.error);
		}
	}
}
