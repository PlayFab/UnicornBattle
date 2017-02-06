using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class SupersonicConfig
{
	private const string unsupportedPlatformStr = "Unsupported Platform";
	private static SupersonicConfig mInstance;

	public static SupersonicConfig Instance {
		get {
			if (mInstance == null) {
				mInstance = new SupersonicConfig ();
			}
			return mInstance;
		}
	}
	#if UNITY_ANDROID && !UNITY_EDITOR
	private static AndroidJavaObject _androidBridge;
	private readonly static string AndroidBridge = "com.supersonic.unity.androidbridge.AndroidBridge";
	
	public SupersonicConfig ()
	{
		using (var pluginClass = new AndroidJavaClass( AndroidBridge ))
			_androidBridge = pluginClass.CallStatic<AndroidJavaObject> ("getInstance");
	}

	//Setters
	public void setMaxVideoLength (int length)
	{
		_androidBridge.Call ("setSupersonicMaxVideoLength", length);
	}
	
	public void setLanguage (string language)
	{
		_androidBridge.Call ("setSupersonicLanguage", language);
	}
	
	public void setClientSideCallbacks (bool status)
	{
		_androidBridge.Call ("setSupersonicClientSideCallbacks", status);
	}
	
	public void setPrivateKey (string key)
	{
		_androidBridge.Call ("setSupersonicPrivateKey", key);
	}
	
	public void setItemName (string name)
	{
		_androidBridge.Call ("setSupersonicItemName", name);
	}
	
	public void setItemCount (int count)
	{
		_androidBridge.Call ("setSupersonicItemCount", count);
	}
	
	public void setRewardedVideoCustomParams (Dictionary<string,string> rvCustomParams)
	{ 
		string json = SupersonicJSON.Json.Serialize (rvCustomParams);
		_androidBridge.Call ("setSupersonicRewardedVideoCustomParams", json);
	}
	
	public void setOfferwallCustomParams (Dictionary<string,string> owCustomParams)
	{
		string json = SupersonicJSON.Json.Serialize (owCustomParams);
		_androidBridge.Call ("setSupersonicOfferwallCustomParams", json);
	}
	

	#elif (UNITY_IPHONE || UNITY_IOS) && !UNITY_EDITOR
	[DllImport("__Internal")]
	private static extern void CFsetSupersonicUseClentSideCallbacks (bool useClientSideCallbacks);

	[DllImport("__Internal")]
	private static extern void CFsetSupersonicLanguage (string language);

	[DllImport("__Internal")]
	private static extern void CFsetSupersonicPrivateKey (string privateKey);

	[DllImport("__Internal")]
	private static extern void CFsetSupersonicMaxVideoLength (int length);

	[DllImport("__Internal")]
	private static extern void CFsetSupersonicItemName (string itemName);

	[DllImport("__Internal")]
	private static extern void CFsetSupersonicItemCount (int itemCount);

	[DllImport("__Internal")]
	private static extern void CFsetSupersonicRewardedVideoCustomParams (string rvParams);

	[DllImport("__Internal")]
	private static extern void CFsetSupersonicOfferwallCustomParams (string owParam);
	
	public void setMaxVideoLength (int length)
	{
		CFsetSupersonicMaxVideoLength (length);
	}
	
	public void setLanguage (string language)
	{
		CFsetSupersonicLanguage (language);
	}
	
	public void setClientSideCallbacks (bool status)
	{
		CFsetSupersonicUseClentSideCallbacks (status);
	}
	
	public void setPrivateKey (string key)
	{
		CFsetSupersonicPrivateKey (key);
	}
	
	public void setItemName (string name)
	{
		CFsetSupersonicItemName (name);
	}
	
	public void setItemCount (int count)
	{
		CFsetSupersonicItemCount (count);
	}
	
	public void setRewardedVideoCustomParams (Dictionary<string,string> rvCustomParams)
	{ 
		string json = SupersonicJSON.Json.Serialize (rvCustomParams);
		CFsetSupersonicRewardedVideoCustomParams (json);
	}
	
	public void setOfferwallCustomParams (Dictionary<string,string> owCustomParams)
	{
		string json = SupersonicJSON.Json.Serialize (owCustomParams);
		CFsetSupersonicOfferwallCustomParams (json);
	}

	public SupersonicConfig ()
	{
		
	}


	#else
	public void setMaxVideoLength (int length)
	{
		Debug.Log (unsupportedPlatformStr);
	}
	
	public void setLanguage (string language)
	{
		Debug.Log (unsupportedPlatformStr);
	}
	
	public void setClientSideCallbacks (bool status)
	{
		Debug.Log (unsupportedPlatformStr);
	}
	
	public void setPrivateKey (string key)
	{
		Debug.Log (unsupportedPlatformStr);
	}
	
	public void setItemName (string name)
	{
		Debug.Log (unsupportedPlatformStr);
	}
	
	public void setItemCount (int count)
	{
		Debug.Log (unsupportedPlatformStr);
	}
	
	public void setRewardedVideoCustomParams (Dictionary<string,string> rvCustomParams)
	{ 
		Debug.Log (unsupportedPlatformStr);
	}
	
	public void setOfferwallCustomParams (Dictionary<string,string> owCustomParams)
	{
		Debug.Log (unsupportedPlatformStr);
	}

	public SupersonicConfig ()
	{
		Debug.Log (unsupportedPlatformStr);
	}
	
	#endif
}


