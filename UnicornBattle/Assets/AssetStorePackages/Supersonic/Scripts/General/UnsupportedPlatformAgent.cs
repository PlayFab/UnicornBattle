#if (!UNITY_IPHONE && !UNITY_IOS && !UNITY_ANDROID) || (UNITY_EDITOR)
using UnityEngine;
using System.Collections;

public class UnsupportedPlatformAgent : SupersonicIAgent
{
    public UnsupportedPlatformAgent()
    {
        // Debug.Log("Unsupported Platform");
    }

    #region SupersonicAgent implementation

    public void start()
    {
       // Debug.Log("Unsupported Platform");
    }

    public void reportAppStarted()
    {
       // Debug.Log("Unsupported Platform");
    }

    public void onResume()
    {
    }

    public void onPause()
    {
    }

    public void setAge(int age)
    {
       // Debug.Log("Unsupported Platform");
    }

    public void setGender(string gender)
    {
       // Debug.Log("Unsupported Platform");
    }

    public void setMediationSegment(string segment)
    {
       // Debug.Log("Unsupported Platform");
    }

    public void initRewardedVideo(string appKey, string userId)
    {
       // Debug.Log("Unsupported Platform");
    }

    public void showRewardedVideo()
    {
       // Debug.Log("Unsupported Platform");
    }

    public void showRewardedVideo(string placementName)
    {
       // Debug.Log("Unsupported Platform");
    }

    public bool isRewardedVideoAvailable()
    {
       // Debug.Log("Unsupported Platform");
        return false;
    }

    public bool isRewardedVideoPlacementCapped(string placementName)
    {
       // Debug.Log("Unsupported Platform");
        return true;
    }

    public SupersonicPlacement getPlacementInfo(string placementName)
    {
       // Debug.Log("Unsupported Platform");
        return null;
    }

    public void initInterstitial(string appKey, string userId)
    {
       // Debug.Log("Unsupported Platform");
    }

    public void loadInterstitial()
    {
       // Debug.Log("Unsupported Platform");
    }

    public void showInterstitial()
    {
       // Debug.Log("Unsupported Platform");
    }

    public void showInterstitial(string placementName)
    {
       // Debug.Log("Unsupported Platform");
    }

    public bool isInterstitialReady()
    {
       // Debug.Log("Unsupported Platform");
        return false;
    }

    public bool isInterstitialPlacementCapped(string placementName)
    {
       // Debug.Log("Unsupported Platform");
        return true;
    }

    public void initOfferwall(string appKey, string userId)
    {
       // Debug.Log("Unsupported Platform");
    }

    public void showOfferwall()
    {
       // Debug.Log("Unsupported Platform");
    }

    public void showOfferwall(string placementName)
    {
       // Debug.Log("Unsupported Platform");
    }

    public void getOfferwallCredits()
    {
       // Debug.Log("Unsupported Platform");
    }

    public bool isOfferwallAvailable()
    {
       // Debug.Log("Unsupported Platform");
        return false;
    }

    public string getAdvertiserId()
    {
       // Debug.Log("Unsupported Platform");
        return "";
    }

    public void validateIntegration()
    {
       // Debug.Log("Unsupported Platform");
    }

    public void shouldTrackNetworkState(bool track)
    {
       // Debug.Log("Unsupported Platform");
    }

    #endregion
}

#endif
