using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogCanvasController : Singleton<DialogCanvasController>
{
    protected DialogCanvasController() { } // guarantee this will be always a singleton only - can't use the constructor!

    public Button openDialogsButton;
    public Button closeDialogsButton;

    public Transform overlayTint;
    public ErrorPromptController errorPrompt;
    // public Transform contextPrompt;
    public ConfirmationPromptController confirmPrompt;
    public LoadingPromptController loadingPrompt;
    public TextInputPrompController textInputPrompt;
    public InterstitialController interstitialPrompt;
    public SelectorPromptController selectorPrompt;

    public ItemViewerController itemViewerPrompt;
    public FloatingStoreController floatingStorePrompt;
    public FloatingInventoryController floatingInvPrompt;

    public OfferPromptController offerPrompt;
    public LeaderboardPaneController socialPrompt;
    public AccountStatusController accountSettingsPrompt;

    public enum InventoryFilters { AllItems, UsableInCombat, Keys, Containers }

    public bool showOpenCloseButton = true;


    public delegate void LoadingPromptHandler(PlayFabAPIMethods method);
    public static event LoadingPromptHandler RaiseLoadingPromptRequest;

    public delegate void ConfirmationPromptHandler(string title, string message, Action<bool> responseCallback);
    public static event ConfirmationPromptHandler RaiseConfirmationPromptRequest;

    public delegate void TextInputPromptHandler(string title, string message, Action<string> responseCallback, string defaultValue = null);
    public static event TextInputPromptHandler RaiseTextInputPromptRequest;

    public delegate void SelectorPromptHandler(string title, List<string> options, UnityAction<int> responseCallback);
    public static event SelectorPromptHandler RaiseSelectorPromptRequest;

    public delegate void InterstitialRequestHandler();
    public static event InterstitialRequestHandler RaiseInterstitialRequest;

    public delegate void StoreRequestHandler(string storeID);
    public static event StoreRequestHandler RaiseStoreRequest;

    public delegate void ItemViewRequestHandler(List<string> items, bool unpackToPlayer);
    public static event ItemViewRequestHandler RaiseItemViewRequest;

    public delegate void InventoryPromptHandler(Action<string> responseCallback, InventoryFilters filter, bool enableTransUi = false, FloatingInventoryController.InventoryMode displayMode = FloatingInventoryController.InventoryMode.Character);
    public static event InventoryPromptHandler RaiseInventoryPromptRequest;

    public delegate void RequestAccountSettingsHandler();
    public static event RequestAccountSettingsHandler RaiseAccountSettingsRequest;

    public delegate void RequestSocialHandler();
    public static event RequestSocialHandler RaiseSocialRequest;

    public delegate void RequestOfferPromptHandler();
    public static event RequestOfferPromptHandler RaiseOfferRequest;

    private List<OutgoingAPICounter> waitingOnRequests = new List<OutgoingAPICounter>();

    //Coroutine to manage the 10 second timeout.
    private Coroutine timeOutCallback;
    private float timeOutLength = 10f;


    //public str

    void OnEnable()
    {
        PF_Bridge.OnPlayFabCallbackError += HandleCallbackError;
        PF_Bridge.OnPlayfabCallbackSuccess += HandleCallbackSuccess;

        PF_Authentication.OnLoginFail += HandleOnLoginFail;
        PF_Authentication.OnLoginSuccess += HandleOnLoginSuccess;

        RaiseLoadingPromptRequest += HandleLoadingPromptRequest;
        RaiseConfirmationPromptRequest += HandleConfirmationPromptRequest;
        RaiseTextInputPromptRequest += HandleTextInputRequest;
        RaiseInterstitialRequest += HandleInterstitialRequest;
        RaiseStoreRequest += HandleStoreRequest;
        RaiseItemViewRequest += HandleItemViewerRequest;
        RaiseInventoryPromptRequest += HandleInventoryRequest;
        RaiseAccountSettingsRequest += HandleRaiseAccountSettingsRequest;
        RaiseSelectorPromptRequest += HandleSelectorPromptRequest;
        RaiseSocialRequest += HandleSocialRequest;
        RaiseOfferRequest += HandleOfferPromptRequest;

    }

    void HandleRaiseOfferRequest()
    {

    }

    void OnDisable()
    {
        PF_Bridge.OnPlayFabCallbackError -= HandleCallbackError;
        PF_Bridge.OnPlayfabCallbackSuccess -= HandleCallbackSuccess;

        PF_Authentication.OnLoginFail -= HandleOnLoginFail;
        PF_Authentication.OnLoginSuccess -= HandleOnLoginSuccess;

        RaiseLoadingPromptRequest -= HandleLoadingPromptRequest;
        RaiseConfirmationPromptRequest -= HandleConfirmationPromptRequest;
        RaiseTextInputPromptRequest -= HandleTextInputRequest;
        RaiseInterstitialRequest -= HandleInterstitialRequest;
        RaiseStoreRequest -= HandleStoreRequest;
        RaiseItemViewRequest -= HandleItemViewerRequest;
        RaiseInventoryPromptRequest -= HandleInventoryRequest;
        RaiseAccountSettingsRequest -= HandleRaiseAccountSettingsRequest;
        RaiseSelectorPromptRequest -= HandleSelectorPromptRequest;
        RaiseSocialRequest -= HandleSocialRequest;
        RaiseOfferRequest -= HandleOfferPromptRequest;
    }

    void HandleOfferPromptRequest()
    {
        this.offerPrompt.gameObject.SetActive(true);
        this.offerPrompt.Init();
    }

    public static void RequestOfferPrompt()
    {
        if (RaiseOfferRequest != null)
        {
            RaiseOfferRequest();
        }
    }


    void HandleSocialRequest()
    {
        this.socialPrompt.gameObject.SetActive(true);
        this.socialPrompt.Init();
    }

    public static void RequestSocialPrompt()
    {
        if (RaiseSocialRequest != null)
        {
            RaiseSocialRequest();
        }
    }

    void HandleSelectorPromptRequest(string title, List<string> options, UnityAction<int> responseCallback)
    {
        this.selectorPrompt.gameObject.SetActive(true);
        this.selectorPrompt.InitSelector(title, options, responseCallback);
    }

    public static void RequestSelectorPrompt(string title, List<string> options, UnityAction<int> responseCallback)
    {
        if (RaiseSelectorPromptRequest != null)
        {
            RaiseSelectorPromptRequest(title, options, responseCallback);
        }
    }

    public static void RequestAccountSettings()
    {
        if (RaiseAccountSettingsRequest != null)
        {
            RaiseAccountSettingsRequest();
        }
    }

    void HandleRaiseAccountSettingsRequest()
    {
        this.accountSettingsPrompt.gameObject.SetActive(true);
        this.accountSettingsPrompt.Init();
    }


    public static void RequestInventoryPrompt(Action<string> callback = null, InventoryFilters filter = InventoryFilters.AllItems, bool enableTransUi = true, FloatingInventoryController.InventoryMode displayMode = FloatingInventoryController.InventoryMode.Character)
    {
        if (RaiseInventoryPromptRequest != null)
        {
            RaiseInventoryPromptRequest(callback, filter, enableTransUi, displayMode);
        }
    }

    void HandleInventoryRequest(Action<string> callback = null, InventoryFilters filter = InventoryFilters.AllItems, bool enableTransUi = true, FloatingInventoryController.InventoryMode displayMode = FloatingInventoryController.InventoryMode.Character)
    {

        Action afterGetInventory = () =>
        {
            // ENABLE THIS AFTER WE HAVE A CONSISTENT WAY TO HIDE TINTS
            //ShowTint();
            this.floatingInvPrompt.Init(callback, filter, enableTransUi, displayMode);
        };

        if (displayMode == FloatingInventoryController.InventoryMode.Character)
        {
            PF_PlayerData.GetCharacterInventory(PF_PlayerData.activeCharacter.characterDetails.CharacterId, afterGetInventory);
        }
        else
        {
            PF_PlayerData.GetUserInventory(afterGetInventory);
        }

    }


    public static void RequestItemViewer(List<string> items, bool unpackToPlayer = false)
    {
        if (RaiseItemViewRequest != null)
        {
            RaiseItemViewRequest(items, unpackToPlayer);
        }
    }

    void HandleItemViewerRequest(List<string> items, bool unpackToPlayer)
    {
        //		UnityAction<List<StoreItem>> afterGetStoreItems = (List<StoreItem> resultSet) => 
        //		{
        //			ShowTint();
        //			this.floatingStorePrompt.InitiateStore(storeID, resultSet);
        //		};
        //		PF_GamePlay.RetrieveStoreItems (storeID, afterGetStoreItems);

        this.itemViewerPrompt.InitiateViewer(items, unpackToPlayer);

    }

    public static void RequestStore(string storeId)
    {
        if (RaiseStoreRequest != null)
        {
            RaiseStoreRequest(storeId);
        }
    }

    void HandleStoreRequest(string storeId)
    {
        UnityAction<List<StoreItem>> afterGetStoreItems = (List<StoreItem> resultSet) =>
        {
            // ENABLE THIS AFTER WE HAVE A CONSISTENT WAY TO HIDE TINTS
            //ShowTint();
            this.floatingStorePrompt.InitiateStore(storeId, resultSet);
        };
        PF_GamePlay.RetrieveStoreItems(storeId, afterGetStoreItems);

    }

    void OnGUI()
    {
        /*		if (GUI.Button (new Rect(5, 200, 150, 50), "Test Selector")) 
                {
                    UnityAction<int> afterSelect = ( int  response) => 
                    {
                        Debug.Log("" + response);
                    };
                    List<string> options = new List<string>()
                    {
                        "Gold",
                        "Gems",
                        "Potions",
                        "Sale Items"
                    };
                    DialogCanvasController.RaiseSelectorPromptRequest(options, afterSelect);
                } */

    }

    public static void RequestInterstitial()
    {
        if (RaiseInterstitialRequest != null)
        {
            RaiseInterstitialRequest();
        }
    }

    public void HandleInterstitialRequest()
    {
        this.interstitialPrompt.ShowInterstitial();
    }


    public static void RequestTextInputPrompt(string title, string message, Action<string> responseCallback, string defaultValue = null)
    {
        if (RaiseTextInputPromptRequest != null)
        {
            RaiseTextInputPromptRequest(title, message, responseCallback, defaultValue);
        }
    }

    public void HandleTextInputRequest(string title, string message, Action<string> responseCallback, string defaultValue)
    {
        //this.ShowTint();
        this.textInputPrompt.ShowTextInputPrompt(title, message, responseCallback, defaultValue);
    }


    public static void RequestConfirmationPrompt(string title, string message, Action<bool> responseCallback)
    {
        if (RaiseConfirmationPromptRequest != null)
        {
            RaiseConfirmationPromptRequest(title, message, responseCallback);
        }
    }

    public void HandleConfirmationPromptRequest(string title, string message, Action<bool> responseCallback)
    {
        //this.ShowTint();
        this.confirmPrompt.ShowConfirmationPrompt(title, message, responseCallback, this.HideTint);
    }


    public static void RequestLoadingPrompt(PlayFabAPIMethods method)
    {
        if (RaiseLoadingPromptRequest != null)
        {
            RaiseLoadingPromptRequest(method);
        }
    }

    public void HandleLoadingPromptRequest(PlayFabAPIMethods method)
    {
        if (this.waitingOnRequests.Count == 0)
        {
            //ShowTint();
            this.loadingPrompt.RaiseLoadingPrompt();
        }
        this.waitingOnRequests.Add(new OutgoingAPICounter() { method = method, outgoingGameTime = Time.time });

        if (this.timeOutCallback == null)
        {
            this.timeOutCallback = StartCoroutine(OutgoingApiTimeoutCallback());
        }
    }

    public void CloseLoadingPrompt(PlayFabAPIMethods method)
    {
        List<OutgoingAPICounter> waiting = this.waitingOnRequests.FindAll((i) => { return i.method == method; });

        OutgoingAPICounter itemToRemove = null;

        for (int z = 0; z < waiting.Count; z++)
        {
            // in absence of a true GUID request system, we will get the oldest requests to prevent timeouts
            if (itemToRemove != null && waiting[z].outgoingGameTime > itemToRemove.outgoingGameTime)
            {
                // shouldnt be too many times where there are multiple requests of the same type.
                itemToRemove = waiting[z];
            }
            else if (itemToRemove == null)
            {
                //first and likly only match
                itemToRemove = waiting[z];
            }
        }

        if (itemToRemove != null)
        {
            this.waitingOnRequests.Remove(itemToRemove);
            HideTint();
            this.loadingPrompt.CloseLoadingPrompt();
        }
    }

    public void CloseLoadingPromptAfterError()
    {
        this.waitingOnRequests.Clear();
        this.loadingPrompt.CloseLoadingPrompt();

        //		if(this.timeOutCallback != null)
        //		{
        //			StopCoroutine(this.timeOutCallback);
        //		}
    }



    //	public delegate void PlayFabErrorHandler(string details, PlayFabAPIMethods method);
    //	public static event PlayFabErrorHandler OnPlayFabCallbackError;
    //	
    //	// called after a successful API callback (useful for stopping the spinner)
    //	public delegate void CallbackSuccess(string details, PlayFabAPIMethods method);
    //	public static event CallbackSuccess OnPlayfabCallbackSuccess;

    void HandleOnLoginSuccess(string message, MessageDisplayStyle style)
    {
        HandleCallbackSuccess(message, PlayFabAPIMethods.GenericLogin, style);
    }

    void HandleOnLoginFail(string message, MessageDisplayStyle style)
    {
        HandleCallbackError(message, PlayFabAPIMethods.GenericLogin, style);
    }


    public void HandleCallbackError(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
    {
        switch (style)
        {
            case MessageDisplayStyle.error:
                string errorMessage = string.Format("CALLBACK ERROR: {0}: {1}", method, details);
                //ShowTint();
                this.errorPrompt.RaiseErrorDialog(errorMessage);
                CloseLoadingPromptAfterError();
                break;

            default:
                CloseLoadingPrompt(method);
                Debug.Log(string.Format("CALLBACK ERROR: {0}: {1}", method, details));
                break;

        }

    }

    public void HandleCallbackSuccess(string details, PlayFabAPIMethods method, MessageDisplayStyle style)
    {
        CloseLoadingPrompt(method);
        //Debug.Log(string.Format("{0} completed successfully.", method.ToString()));
    }


    public void ShowTint()
    {
        overlayTint.gameObject.SetActive(true);
    }

    public void HideTint()
    {
        overlayTint.gameObject.SetActive(false);
    }


    public void OpenDialogsMenu()
    {
        // THIS BREAKS IF THE GO IS DISABLED!!!

        //Tween.Tween(this.menuOverlayPanel.gameObject, .001f, Quaternion.Euler(0,0,0) , Quaternion.Euler(0,0,15f), TweenMain.Style.PingPong, TweenMain.Method.EaseIn, null);


        //new Vector3(Screen.width/2, Screen.height/2, 0)
        //ShowTint();
        //TweenPos.Tween(this.menuOverlayPanel.gameObject, .001f, this.menuOverlayPanel.transform.position, new Vector3(0, Screen.height, 0), TweenMain.Style.Once, TweenMain.Method.Linear, null, Space.World);
        //TweenScale.Tween(this.menuOverlayPanel.gameObject, .001f, new Vector3(1,1,1), new Vector3(0,0,0), TweenMain.Style.Once, TweenMain.Method.EaseIn, null);
        //TweenPos.Tween(this.menuOverlayPanel.gameObject, .5f, new Vector3(0, Screen.height, 0), new Vector3(Screen.width/2, Screen.height/2, 0), TweenMain.Style.Once, TweenMain.Method.EaseIn, null, Space.World);
        //TweenScale.Tween(this.menuOverlayPanel.gameObject, .5f, new Vector3(0,0,0), new Vector3(1,1,1), TweenMain.Style.Once, TweenMain.Method.EaseIn, null);

        ToggleOpenCloseButtons();
    }

    // NEED TO MAKE THIS A COROUTINE
    public void CloseDialogsMenu()
    {
        //TweenPos.Tween(this.menuOverlayPanel.gameObject, .5f, new Vector3(Screen.width/2, Screen.height/2, 0), new Vector3(0, Screen.height, 0), TweenMain.Style.Once, TweenMain.Method.EaseIn, null, Space.World);
        //TweenScale.Tween(this.menuOverlayPanel.gameObject, .5f, new Vector3(1,1,1), new Vector3(0,0,0), TweenMain.Style.Once, TweenMain.Method.EaseIn, null);

        HideTint();
        ToggleOpenCloseButtons();

    }

    public void CloseErrorDialog()
    {
        this.errorPrompt.CloseErrorDialog();
        HideTint();

    }

    public void ToggleOpenCloseButtons()
    {
        if (this.showOpenCloseButton)
        {
            if (this.openDialogsButton.gameObject.activeSelf)
            {
                this.openDialogsButton.gameObject.SetActive(false);
                this.closeDialogsButton.gameObject.SetActive(true);
            }
            else
            {
                this.openDialogsButton.gameObject.SetActive(true);
                this.closeDialogsButton.gameObject.SetActive(false);
            }
        }
    }



    //Loading time-out co-routine management code
    private IEnumerator OutgoingApiTimeoutCallback()
    {
        while (this.waitingOnRequests.Count > 0)
        {
            for (var z = 0; z < this.waitingOnRequests.Count; z++)
            {
                if (Time.time > (this.waitingOnRequests[z].outgoingGameTime + this.timeOutLength))
                {
                    // time has elapsed for this request, until we can handle this more specifically, we can only reload the scene, and hope for the best.
                    var capturedDetails = this.waitingOnRequests[z].method;
                    PF_Bridge.RaiseCallbackError(string.Format("API Call: {0} Timed out after {1} seconds.", capturedDetails, this.timeOutLength), this.waitingOnRequests[z].method, MessageDisplayStyle.error);

                    Action<bool> afterConfirmation = response =>
                    {
                        if (response == false)
                        {
                            // user clicked cancel (to reload);
                            Debug.LogErrorFormat("Reloading scene due {0} API timing out.", capturedDetails);
                            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
                        }
                    };

                    RequestConfirmationPrompt("Caution! Bravery Required!", string.Format("API Call: {0} Timed out. \n\tACCEPT: To proceed, may cause client instability. \n\tCANCEL: To reload this scene and hope for the best.", capturedDetails), afterConfirmation);
                }
            }

            // tick once per second while we have outbound requests. (keep enabled while debugging this feature)
            Debug.Log(string.Format("{0}", (int)Time.time % 2 == 0 ? "Tick" : "Tock"));
            yield return new WaitForSeconds(1f);
        }

        // outgoing request queue empty
        this.timeOutCallback = null;
        yield break;
    }
}
