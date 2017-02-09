using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SupersonicEvents : MonoBehaviour
{
    private const string ERROR_CODE = "error_code";
    private const string ERROR_DESCRIPTION = "error_description";

    public static string uniqueUserId = "ZB_PF_DEV";
    public static string appKey = "5689cd1d";

    public static bool trackNetworkState = true;

    public static event Action<RewardAdActivityResponse> OnAdRewarded;
    public static AdPlacementDetails queuedAd;

    public static bool isRewardedVideoPlacementCapped(string placement)
    {
        return Supersonic.Agent.isRewardedVideoPlacementCapped(placement);
    }

    public static bool rewardedVideoAvailability
    {
        get
        {
            return Supersonic.Agent.isRewardedVideoAvailable();
        }
    }

    #region ****** UNITY EVENTS & METHODS *****

    void Awake()
    {
        gameObject.name = "SupersonicEvents";               //Change the GameObject name to SupersonicEvents.
        DontDestroyOnLoad(gameObject);
        //Supersonic.m			//Makes the object not be destroyed automatically when loading a new scene.
    }

    // general show ad feature?

    public static void ShowRewardedVideo(AdPlacementDetails ad)
    {
        //if (rewardedVideoAvailability) {
        //Supersonic.Agent.showRewardedVideo("placementName");
        queuedAd = ad;
        Supersonic.Agent.showRewardedVideo(ad.PlacementName);
        //		} else {
        //			Debug.Log ("RewardedVideo was not available.");
        //			queuedAd = null;
        //		}
    }

    void OnEnable()
    {
        SupersonicEvents.onRewardedVideoInitSuccessEvent += RewardedVideoInitSuccessEvent;
        SupersonicEvents.onRewardedVideoInitFailEvent += RewardedVideoInitFailEvent;
        SupersonicEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        SupersonicEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
        SupersonicEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
        SupersonicEvents.onVideoAvailabilityChangedEvent += VideoAvailabilityChangedEvent;
        SupersonicEvents.onVideoStartEvent += VideoStartEvent;
        SupersonicEvents.onVideoEndEvent += VideoEndEvent;
    }

    void OnDisable()
    {
        SupersonicEvents.onRewardedVideoInitSuccessEvent -= RewardedVideoInitSuccessEvent;
        SupersonicEvents.onRewardedVideoInitFailEvent -= RewardedVideoInitFailEvent;
        SupersonicEvents.onRewardedVideoAdOpenedEvent -= RewardedVideoAdOpenedEvent;
        SupersonicEvents.onRewardedVideoAdRewardedEvent -= RewardedVideoAdRewardedEvent;
        SupersonicEvents.onRewardedVideoAdClosedEvent -= RewardedVideoAdClosedEvent;
        SupersonicEvents.onVideoAvailabilityChangedEvent -= VideoAvailabilityChangedEvent;
        SupersonicEvents.onVideoStartEvent -= VideoStartEvent;
        SupersonicEvents.onVideoEndEvent -= VideoEndEvent;
    }

    void Start()
    {
        Supersonic.Agent.start();
        //Set the unique id of your end user.
        //Init Rewarded Video

        Supersonic.Agent.initRewardedVideo(SupersonicEvents.appKey, SupersonicEvents.uniqueUserId);

        //track the network state:
        Supersonic.Agent.shouldTrackNetworkState(trackNetworkState);
    }



    #endregion

    #region ****** PLAYFAB CALLBACKS ******
    public static void RouteActivityToPlayFab(string placement, string reward)
    {
        PF_Advertising.RewardAdActivity(new RewardAdActivityRequest() { PlacementId = placement, RewardId = reward }, OnReportAdActivitySuccess, PF_Bridge.PlayFabErrorCallback);
    }

    public static void OnReportAdActivitySuccess(RewardAdActivityResponse result)
    {
        Debug.Log("OnReportAdActivitySuccess!");
        Debug.Log(string.Format("Retrieved {0} items and {1} VCs.", result.RewardResults.GrantedItems.Count, result.RewardResults.GrantedVirtualCurrencies.Count));

        var gotItems = false;
        var gotVc = false;

        var output = "Congratulations! ";
        List<string> itemGifts = new List<string>();

        if (result.RewardResults.GrantedItems != null && result.RewardResults.GrantedItems.Count > 0)
        {
            gotItems = true;
            output += "You received: " + result.RewardResults.GrantedItems.Count + " new items";

            foreach (var item in result.RewardResults.GrantedItems)
            {
                itemGifts.Add(item.ItemId);
            }
        }

        if (result.RewardResults.GrantedVirtualCurrencies != null && result.RewardResults.GrantedVirtualCurrencies.Count > 0)
        {
            gotVc = true;

            var count = 0;
            foreach (var grant in result.RewardResults.GrantedVirtualCurrencies)
            {
                if (gotItems || count > 0)
                {
                    output += "; ";
                }
                else
                {
                    output += "You received: ";
                }

                output += string.Format("{1}({0})", grant.Key, grant.Value);

                count++;
            }

            output += " in Virtual Currencies";

        }

        if (result.RewardResults.IncrementedStatistics != null && result.RewardResults.IncrementedStatistics.Count > 0)
        {
            var count = 0;
            foreach (var stat in result.RewardResults.IncrementedStatistics)
            {
                if (gotItems || gotVc || count > 0)
                {
                    output += "; ";
                }

                output += string.Format(" Your \"{0}\" increased by {1}", stat.Key, stat.Value);

                count++;
            }
        }

        if (gotItems)
            output += ".\n Click the check mark to view your new items.";

        if (OnAdRewarded != null)
            OnAdRewarded(result);

        DialogCanvasController.RequestConfirmationPrompt("You were granted a gift!", output, response =>
        {
            if (response && gotItems)
                DialogCanvasController.RequestItemViewer(itemGifts, true);
        });


    }
    #endregion



    #region ******* SUPERSONIC CALLBACKS *******

    //Invoked when initialization of RewardedVideo has finished successfully.
    public void RewardedVideoInitSuccessEvent()
    {
        Debug.Log("RewardedVideoInitSuccessEvent");
        //PF_Advertising.ReportAdActivity(new ReportAdActivityRequest(){ PlacementId = SupersonicEvents.queuedAd.PlacementId, RewardId = SupersonicEvents.queuedAd.RewardId, Activity = AdActivity }, null, PF_Bridge.PlayFabErrorCallback);
    }

    //Invoked when RewardedVideo initialization process has failed.
    //SupersonicError contains the reason for the failure.
    public void RewardedVideoInitFailEvent(SupersonicError error)
    {

        Debug.Log("Init rewarded video error ");
        //PF_Advertising.ReportAdActivity(new ReportAdActivityRequest(){ PlacementId = SupersonicEvents.queuedAd.PlacementId, RewardId = SupersonicEvents.queuedAd.RewardId, Activity = AdAc }, null, PF_Bridge.PlayFabErrorCallback);

    }

    //Invoked when the RewardedVideo ad view has opened.
    //Your Activity will lose focus. Please avoid performing heavy
    //tasks till the video ad will be closed.
    public void RewardedVideoAdOpenedEvent()
    {
        Debug.Log("RewardedVideoAdOpenedEvent");
        PF_Advertising.ReportAdActivity(new ReportAdActivityRequest() { PlacementId = SupersonicEvents.queuedAd.PlacementId, RewardId = SupersonicEvents.queuedAd.RewardId, Activity = AdActivity.Opened }, null, PF_Bridge.PlayFabErrorCallback);
    }

    //Invoked when the RewardedVideo ad view is about to be closed.
    //Your activity will now regain its focus.
    public void RewardedVideoAdClosedEvent()
    {
        Debug.Log("RewardedVideoAdClosedEvent");
        PF_Advertising.ReportAdActivity(new ReportAdActivityRequest() { PlacementId = SupersonicEvents.queuedAd.PlacementId, RewardId = SupersonicEvents.queuedAd.RewardId, Activity = AdActivity.Closed }, null, PF_Bridge.PlayFabErrorCallback);
        queuedAd = null;
    }

    //Invoked when there is a change in the ad availability status.
    //@param - available - value will change to true when rewarded videos are available.
    //You can then show the video by calling showRewardedVideo().
    //Value will change to false when no videos are available.
    public void VideoAvailabilityChangedEvent(bool available)
    {
        //Change the in-app 'Traffic Driver' state according to availability.
        // rewardedVideoAvailability = available; // Actually does nothing
    }

    //Invoked when the video ad starts playing.
    public void VideoStartEvent()
    {
        Debug.Log("VideoStartEvent");
        PF_Advertising.ReportAdActivity(new ReportAdActivityRequest() { PlacementId = SupersonicEvents.queuedAd.PlacementId, RewardId = SupersonicEvents.queuedAd.RewardId, Activity = AdActivity.Start }, null, PF_Bridge.PlayFabErrorCallback);

    }

    //Invoked when the video ad finishes playing.
    public void VideoEndEvent()
    {
        Debug.Log("VideoEndEvent");
        PF_Advertising.ReportAdActivity(new ReportAdActivityRequest() { PlacementId = SupersonicEvents.queuedAd.PlacementId, RewardId = SupersonicEvents.queuedAd.RewardId, Activity = AdActivity.End }, null, PF_Bridge.PlayFabErrorCallback);

    }

    //Invoked when the user completed the video and should be rewarded.
    //If using server-to-server callbacks you may ignore this events and wait for
    //the callback from the Supersonic server.
    //@param - placement - placement object which contains the reward data
    public void RewardedVideoAdRewardedEvent(SupersonicPlacement ssp)
    {
        //TODO - here you can reward the user according to the reward name and amount
        Debug.Log("RewardedVideoAdRewardedEvent");
        //string adResult = string.Format ("Ad Complete: {0} -- {1} -- {2}", ssp.getPlacementName (), ssp.getRewardName (), ssp.getRewardAmount ());
        //Debug.Log (adResult);

        //var grantable = PF_GameData.catalogItems.Find((item) => { });
        //PF_Bridge.RaiseCallbackError(string.Format("Completed {0}, attempted to grant ({1}x){2}, but this functionality must be performed on the Server.", ssp.getPlacementName(), ssp.getRewardAmount(), ssp.getRewardName()) , PlayFabAPIMethods.Generic, MessageDisplayStyle.error);

        Debug.Log(string.Format("PlacementId:{0} -- RewardId:{1}", queuedAd.PlacementId, queuedAd.RewardId));
        RouteActivityToPlayFab(queuedAd.PlacementId, queuedAd.RewardId);

    }


    #endregion

    #region ******************************* RewardedVideoEvents *******************************

    private static event Action _onRewardedVideoInitSuccessEvent;

    public static event Action onRewardedVideoInitSuccessEvent
    {
        add
        {
            if (_onRewardedVideoInitSuccessEvent == null || !_onRewardedVideoInitSuccessEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoInitSuccessEvent += value;
            }
        }

        remove
        {
            if (_onRewardedVideoInitSuccessEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoInitSuccessEvent -= value;
            }
        }
    }

    public void onRewardedVideoInitSuccess(string empty)
    {
        if (_onRewardedVideoInitSuccessEvent != null)
        {
            _onRewardedVideoInitSuccessEvent();
        }
    }

    private static event Action<SupersonicError> _onRewardedVideoInitFailEvent;

    public static event Action<SupersonicError> onRewardedVideoInitFailEvent
    {
        add
        {
            if (_onRewardedVideoInitFailEvent == null || !_onRewardedVideoInitFailEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoInitFailEvent += value;
            }
        }

        remove
        {
            if (_onRewardedVideoInitFailEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoInitFailEvent -= value;
            }
        }
    }

    public void onRewardedVideoInitFail(string description)
    {
        if (_onRewardedVideoInitFailEvent != null)
        {
            SupersonicError sse = getErrorFromErrorString(description);
            _onRewardedVideoInitFailEvent(sse);
        }
    }

    private static event Action<SupersonicError> _onRewardedVideoShowFailEvent;

    public static event Action<SupersonicError> onRewardedVideoShowFailEvent
    {
        add
        {
            if (_onRewardedVideoShowFailEvent == null || !_onRewardedVideoShowFailEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoShowFailEvent += value;
            }
        }

        remove
        {
            if (_onRewardedVideoShowFailEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoShowFailEvent -= value;
            }
        }
    }

    public void onRewardedVideoShowFail(string description)
    {
        if (_onRewardedVideoShowFailEvent != null)
        {
            SupersonicError sse = getErrorFromErrorString(description);
            _onRewardedVideoShowFailEvent(sse);
        }
    }

    private static event Action _onRewardedVideoAdOpenedEvent;

    public static event Action onRewardedVideoAdOpenedEvent
    {
        add
        {
            if (_onRewardedVideoAdOpenedEvent == null || !_onRewardedVideoAdOpenedEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoAdOpenedEvent += value;
            }
        }

        remove
        {
            if (_onRewardedVideoAdOpenedEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoAdOpenedEvent -= value;
            }
        }
    }

    public void onRewardedVideoAdOpened(string empty)
    {
        if (_onRewardedVideoAdOpenedEvent != null)
        {
            _onRewardedVideoAdOpenedEvent();
        }
    }

    private static event Action _onRewardedVideoAdClosedEvent;

    public static event Action onRewardedVideoAdClosedEvent
    {
        add
        {
            if (_onRewardedVideoAdClosedEvent == null || !_onRewardedVideoAdClosedEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoAdClosedEvent += value;
            }
        }

        remove
        {
            if (_onRewardedVideoAdClosedEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoAdClosedEvent -= value;
            }
        }
    }

    public void onRewardedVideoAdClosed(string empty)
    {
        if (_onRewardedVideoAdClosedEvent != null)
        {
            _onRewardedVideoAdClosedEvent();
        }
    }

    private static event Action _onVideoStartEvent;

    public static event Action onVideoStartEvent
    {
        add
        {
            if (_onVideoStartEvent == null || !_onVideoStartEvent.GetInvocationList().Contains(value))
            {
                _onVideoStartEvent += value;
            }
        }

        remove
        {
            if (_onVideoStartEvent.GetInvocationList().Contains(value))
            {
                _onVideoStartEvent -= value;
            }
        }
    }

    public void onVideoStart(string empty)
    {
        if (_onVideoStartEvent != null)
        {
            _onVideoStartEvent();
        }
    }

    private static event Action _onVideoEndEvent;

    public static event Action onVideoEndEvent
    {
        add
        {
            if (_onVideoEndEvent == null || !_onVideoEndEvent.GetInvocationList().Contains(value))
            {
                _onVideoEndEvent += value;
            }
        }

        remove
        {
            if (_onVideoEndEvent.GetInvocationList().Contains(value))
            {
                _onVideoEndEvent -= value;
            }
        }
    }

    public void onVideoEnd(string empty)
    {
        if (_onVideoEndEvent != null)
        {
            _onVideoEndEvent();
        }
    }

    private static event Action<SupersonicPlacement> _onRewardedVideoAdRewardedEvent;

    public static event Action<SupersonicPlacement> onRewardedVideoAdRewardedEvent
    {
        add
        {
            if (_onRewardedVideoAdRewardedEvent == null || !_onRewardedVideoAdRewardedEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoAdRewardedEvent += value;
            }
        }

        remove
        {
            if (_onRewardedVideoAdRewardedEvent.GetInvocationList().Contains(value))
            {
                _onRewardedVideoAdRewardedEvent -= value;
            }
        }
    }

    public void onRewardedVideoAdRewarded(string description)
    {
        if (_onRewardedVideoAdRewardedEvent != null)
        {
            SupersonicPlacement ssp = getPlacementFromString(description);
            _onRewardedVideoAdRewardedEvent(ssp);
        }
    }

    private static event Action<bool> _onVideoAvailabilityChangedEvent;

    public static event Action<bool> onVideoAvailabilityChangedEvent
    {
        add
        {
            if (_onVideoAvailabilityChangedEvent == null || !_onVideoAvailabilityChangedEvent.GetInvocationList().Contains(value))
            {
                _onVideoAvailabilityChangedEvent += value;
            }
        }

        remove
        {
            if (_onVideoAvailabilityChangedEvent.GetInvocationList().Contains(value))
            {
                _onVideoAvailabilityChangedEvent -= value;
            }
        }
    }

    public void onVideoAvailabilityChanged(string stringAvailable)
    {
        bool isAvailable = (stringAvailable == "true") ? true : false;
        if (_onVideoAvailabilityChangedEvent != null)
            _onVideoAvailabilityChangedEvent(isAvailable);
    }

    #endregion

    #region ******************************* InterstitialEvents *******************************

    private static event Action _onInterstitialInitSuccessEvent;

    public static event Action onInterstitialInitSuccessEvent
    {
        add
        {
            if (_onInterstitialInitSuccessEvent == null || !_onInterstitialInitSuccessEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialInitSuccessEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialInitSuccessEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialInitSuccessEvent -= value;
            }
        }
    }

    public void onInterstitialInitSuccess(string empty)
    {
        if (_onInterstitialInitSuccessEvent != null)
        {
            _onInterstitialInitSuccessEvent();
        }
    }

    private static event Action<SupersonicError> _onInterstitialInitFailedEvent;

    public static event Action<SupersonicError> onInterstitialInitFailedEvent
    {
        add
        {
            if (_onInterstitialInitFailedEvent == null || !_onInterstitialInitFailedEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialInitFailedEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialInitFailedEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialInitFailedEvent -= value;
            }
        }
    }

    public void onInterstitialInitFailed(string description)
    {
        if (_onInterstitialInitFailedEvent != null)
        {
            SupersonicError sse = getErrorFromErrorString(description);
            _onInterstitialInitFailedEvent(sse);
        }

    }

    private static event Action _onInterstitialReadyEvent;

    public static event Action onInterstitialReadyEvent
    {
        add
        {
            if (_onInterstitialReadyEvent == null || !_onInterstitialReadyEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialReadyEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialReadyEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialReadyEvent -= value;
            }
        }
    }

    public void onInterstitialReady()
    {
        if (_onInterstitialReadyEvent != null)
            _onInterstitialReadyEvent();
    }

    private static event Action<SupersonicError> _onInterstitialLoadFailedEvent;

    public static event Action<SupersonicError> onInterstitialLoadFailedEvent
    {
        add
        {
            if (_onInterstitialLoadFailedEvent == null || !_onInterstitialLoadFailedEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialLoadFailedEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialLoadFailedEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialLoadFailedEvent -= value;
            }
        }
    }

    public void onInterstitialLoadFailed(string description)
    {
        if (_onInterstitialLoadFailedEvent != null)
        {
            SupersonicError sse = getErrorFromErrorString(description);
            _onInterstitialLoadFailedEvent(sse);
        }

    }

    private static event Action _onInterstitialOpenEvent;

    public static event Action onInterstitialOpenEvent
    {
        add
        {
            if (_onInterstitialOpenEvent == null || !_onInterstitialOpenEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialOpenEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialOpenEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialOpenEvent -= value;
            }
        }
    }

    public void onInterstitialOpen(string empty)
    {
        if (_onInterstitialOpenEvent != null)
        {
            _onInterstitialOpenEvent();
        }
    }

    private static event Action _onInterstitialCloseEvent;

    public static event Action onInterstitialCloseEvent
    {
        add
        {
            if (_onInterstitialCloseEvent == null || !_onInterstitialCloseEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialCloseEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialCloseEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialCloseEvent -= value;
            }
        }
    }

    public void onInterstitialClose(string empty)
    {
        if (_onInterstitialCloseEvent != null)
        {
            _onInterstitialCloseEvent();
        }
    }

    private static event Action _onInterstitialShowSuccessEvent;

    public static event Action onInterstitialShowSuccessEvent
    {
        add
        {
            if (_onInterstitialShowSuccessEvent == null || !_onInterstitialShowSuccessEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialShowSuccessEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialShowSuccessEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialShowSuccessEvent -= value;
            }
        }
    }

    public void onInterstitialShowSuccess(string empty)
    {
        if (_onInterstitialShowSuccessEvent != null)
        {
            _onInterstitialShowSuccessEvent();
        }
    }

    private static event Action<SupersonicError> _onInterstitialShowFailedEvent;

    public static event Action<SupersonicError> onInterstitialShowFailedEvent
    {
        add
        {
            if (_onInterstitialShowFailedEvent == null || !_onInterstitialShowFailedEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialShowFailedEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialShowFailedEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialShowFailedEvent -= value;
            }
        }
    }

    public void onInterstitialShowFailed(string description)
    {
        if (_onInterstitialShowFailedEvent != null)
        {
            SupersonicError sse = getErrorFromErrorString(description);
            _onInterstitialShowFailedEvent(sse);
        }

    }

    private static event Action _onInterstitialClickEvent;

    public static event Action onInterstitialClickEvent
    {
        add
        {
            if (_onInterstitialClickEvent == null || !_onInterstitialClickEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialClickEvent += value;
            }
        }

        remove
        {
            if (_onInterstitialClickEvent.GetInvocationList().Contains(value))
            {
                _onInterstitialClickEvent -= value;
            }
        }
    }

    public void onInterstitialClick(string empty)
    {
        if (_onInterstitialClickEvent != null)
        {
            _onInterstitialClickEvent();
        }
    }

    #endregion

    #region ******************************* OfferwallEvents *******************************

    private static event Action _onOfferwallInitSuccessEvent;

    public static event Action onOfferwallInitSuccessEvent
    {
        add
        {
            if (_onOfferwallInitSuccessEvent == null || !_onOfferwallInitSuccessEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallInitSuccessEvent += value;
            }
        }

        remove
        {
            if (_onOfferwallInitSuccessEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallInitSuccessEvent -= value;
            }
        }
    }

    public void onOfferwallInitSuccess(string empty)
    {
        if (_onOfferwallInitSuccessEvent != null)
        {
            _onOfferwallInitSuccessEvent();
        }
    }

    private static event Action<SupersonicError> _onOfferwallInitFailEvent;

    public static event Action<SupersonicError> onOfferwallInitFailEvent
    {
        add
        {
            if (_onOfferwallInitFailEvent == null || !_onOfferwallInitFailEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallInitFailEvent += value;
            }
        }

        remove
        {
            if (_onOfferwallInitFailEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallInitFailEvent -= value;
            }
        }
    }

    public void onOfferwallInitFail(string description)
    {
        if (_onOfferwallInitFailEvent != null)
        {
            SupersonicError sse = getErrorFromErrorString(description);
            _onOfferwallInitFailEvent(sse);
        }
    }

    private static event Action _onOfferwallOpenedEvent;

    public static event Action onOfferwallOpenedEvent
    {
        add
        {
            if (_onOfferwallOpenedEvent == null || !_onOfferwallOpenedEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallOpenedEvent += value;
            }
        }

        remove
        {
            if (_onOfferwallOpenedEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallOpenedEvent -= value;
            }
        }
    }

    public void onOfferwallOpened(string empty)
    {
        if (_onOfferwallOpenedEvent != null)
        {
            _onOfferwallOpenedEvent();
        }
    }

    private static event Action<SupersonicError> _onOfferwallShowFailEvent;

    public static event Action<SupersonicError> onOfferwallShowFailEvent
    {
        add
        {
            if (_onOfferwallShowFailEvent == null || !_onOfferwallShowFailEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallShowFailEvent += value;
            }
        }

        remove
        {
            if (_onOfferwallShowFailEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallShowFailEvent -= value;
            }
        }
    }

    public void onOfferwallShowFail(string description)
    {
        if (_onOfferwallShowFailEvent != null)
        {
            SupersonicError sse = getErrorFromErrorString(description);
            _onOfferwallShowFailEvent(sse);
        }
    }

    private static event Action _onOfferwallClosedEvent;

    public static event Action onOfferwallClosedEvent
    {
        add
        {
            if (_onOfferwallClosedEvent == null || !_onOfferwallClosedEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallClosedEvent += value;
            }
        }

        remove
        {
            if (_onOfferwallClosedEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallClosedEvent -= value;
            }
        }
    }

    public void onOfferwallClosed(string empty)
    {
        if (_onOfferwallClosedEvent != null)
        {
            _onOfferwallClosedEvent();
        }
    }

    private static event Action<SupersonicError> _onGetOfferwallCreditsFailEvent;

    public static event Action<SupersonicError> onGetOfferwallCreditsFailEvent
    {
        add
        {
            if (_onGetOfferwallCreditsFailEvent == null || !_onGetOfferwallCreditsFailEvent.GetInvocationList().Contains(value))
            {
                _onGetOfferwallCreditsFailEvent += value;
            }
        }

        remove
        {
            if (_onGetOfferwallCreditsFailEvent.GetInvocationList().Contains(value))
            {
                _onGetOfferwallCreditsFailEvent -= value;
            }
        }
    }

    public void onGetOfferwallCreditsFail(string description)
    {
        if (_onGetOfferwallCreditsFailEvent != null)
        {
            SupersonicError sse = getErrorFromErrorString(description);
            _onGetOfferwallCreditsFailEvent(sse);

        }
    }

    private static event Action<Dictionary<string, object>> _onOfferwallAdCreditedEvent;

    public static event Action<Dictionary<string, object>> onOfferwallAdCreditedEvent
    {
        add
        {
            if (_onOfferwallAdCreditedEvent == null || !_onOfferwallAdCreditedEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallAdCreditedEvent += value;
            }
        }

        remove
        {
            if (_onOfferwallAdCreditedEvent.GetInvocationList().Contains(value))
            {
                _onOfferwallAdCreditedEvent -= value;
            }
        }
    }

    public void onOfferwallAdCredited(string json)
    {
        if (_onOfferwallAdCreditedEvent != null)
            _onOfferwallAdCreditedEvent(SupersonicJSON.Json.Deserialize(json) as Dictionary<string, object>);
    }

    #endregion

    public SupersonicError getErrorFromErrorString(string description)
    {
        SupersonicError sse;
        if (!String.IsNullOrEmpty(description))
        {
            Dictionary<string, object> error = SupersonicJSON.Json.Deserialize(description) as Dictionary<string, object>;
            // if there is a supersonic error
            if (error != null)
            {
                int eCode = Convert.ToInt32(error[ERROR_CODE].ToString());
                string eDescription = error[ERROR_DESCRIPTION].ToString();
                sse = new SupersonicError(eCode, eDescription);
            }
            // else create an empty one
            else
            {
                sse = new SupersonicError(-1, "");
            }
        }
        else
        {
            sse = new SupersonicError(-1, "");
        }

        return sse;
    }

    public SupersonicPlacement getPlacementFromString(string jsonPlacement)
    {
        Dictionary<string, object> placementJSON = SupersonicJSON.Json.Deserialize(jsonPlacement) as Dictionary<string, object>;
        SupersonicPlacement ssp;
        int rewardAmount = Convert.ToInt32(placementJSON["placement_reward_amount"].ToString());
        string rewardName = placementJSON["placement_reward_name"].ToString();
        string placementName = placementJSON["placement_name"].ToString();

        ssp = new SupersonicPlacement(placementName, rewardName, rewardAmount);
        return ssp;
    }
}
