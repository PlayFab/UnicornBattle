#if UNITY_ANDROID
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AndroidAgent : SupersonicIAgent
{
	private static AndroidJavaObject _androidBridge;
	private readonly static string AndroidBridge = "com.supersonic.unity.androidbridge.AndroidBridge";
	private const string REWARD_AMOUNT = "reward_amount";
	private const string REWARD_NAME = "reward_name";
	private const string PLACEMENT_NAME = "placement_name";

	public AndroidAgent ()
	{
		Debug.Log ("AndroidAgent ctr");
	}
	
	#region SupersonicAgent implementation
	private AndroidJavaObject getBridge ()
	{
		if (_androidBridge == null)
			using (var pluginClass = new AndroidJavaClass( AndroidBridge ))
				_androidBridge = pluginClass.CallStatic<AndroidJavaObject> ("getInstance");
		
		return _androidBridge;
	}

	public void start ()
	{
		Debug.Log ("Android started");

		getBridge ().Call ("setPluginData", "Unity", Supersonic.pluginVersion (), Supersonic.unityVersion ());
		Debug.Log ("Android started - ended");		
	}

	public void reportAppStarted ()
	{
		getBridge ().Call ("reportAppStarted");
	}

	public void onResume ()
	{
		getBridge ().Call ("onResume");
	}

	public void onPause ()
	{
		getBridge ().Call ("onPause");
	}

	public void setAge (int age)
	{
		getBridge ().Call ("setAge", age);
	}

	public void setGender (string gender)
	{
		getBridge ().Call ("setGender", gender);
	}

	public void setMediationSegment (string segment)
	{
		getBridge ().Call ("setMediationSegment", segment);
	}

	public void initRewardedVideo (string appKey, string userId)
	{
		getBridge ().Call ("initRewardedVideo", appKey, userId);
	}

	public void showRewardedVideo ()
	{
		getBridge ().Call ("showRewardedVideo");
	}

	public void showRewardedVideo (string placementName)
	{
		getBridge ().Call ("showRewardedVideo", placementName);
	}

	public bool isRewardedVideoAvailable ()
	{
		bool available = getBridge ().Call<bool> ("isRewardedVideoAvailable");
		return available;
	}

	public bool isRewardedVideoPlacementCapped (string placementName)
	{
		bool capped = getBridge ().Call<bool> ("isRewardedVideoPlacementCapped", placementName);
		return capped;
	}

	public string getAdvertiserId ()
	{
		string id = getBridge ().Call<string> ("getAdvertiserId");
		return id;
	}

	public void shouldTrackNetworkState (bool track)
	{
		getBridge ().Call ("shouldTrackNetworkState", track);
	}

	public void validateIntegration ()
	{
		getBridge ().Call ("validateIntegration");
	}

	public SupersonicPlacement getPlacementInfo (string placementName)
	{
		string placementInfo = getBridge ().Call<string> ("getPlacementInfo", placementName);
		SupersonicPlacement pInfo = null;
		if (placementInfo != null) {
			Dictionary<string,object> pInfoDic = SupersonicJSON.Json.Deserialize (placementInfo) as Dictionary<string,object>;
			string pName = pInfoDic [PLACEMENT_NAME].ToString ();
			string rName = pInfoDic [REWARD_NAME].ToString ();
			int rAmount = Convert.ToInt32 (pInfoDic [REWARD_AMOUNT].ToString ());

			pInfo = new SupersonicPlacement (pName, rName, rAmount);		
		}
		return pInfo;
	}

	public void initInterstitial (string appKey, string userId)
	{
		getBridge ().Call ("initInterstitial", appKey, userId);
	}

	public void loadInterstitial ()
	{
		getBridge ().Call ("loadInterstitial");
	}

	public void showInterstitial ()
	{
		getBridge ().Call ("showInterstitial");
	}

	public void showInterstitial (string placementName)
	{
		getBridge ().Call ("showInterstitial", placementName);
	}

	public bool isInterstitialReady ()
	{
		bool available = getBridge ().Call<bool> ("isInterstitialReady");
		return available;
	}

	public bool isInterstitialPlacementCapped (string placementName)
	{
		bool capped = getBridge ().Call<bool> ("isInterstitialPlacementCapped", placementName);
		return capped;
	}

	public void initOfferwall (string appKey, string userId)
	{
		getBridge ().Call ("initOfferwall", appKey, userId);
	}

	public void showOfferwall ()
	{
		getBridge ().Call ("showOfferwall");
	}

	public void showOfferwall (string placementName)
	{
		getBridge ().Call ("showOfferwall", placementName);
	}

	public void getOfferwallCredits ()
	{
		getBridge ().Call ("getOfferwallCredits");
	}

	public bool isOfferwallAvailable ()
	{
		bool available = getBridge ().Call<bool> ("isOfferwallAvailable");
		return available;
	}

	#endregion
}

#endif

