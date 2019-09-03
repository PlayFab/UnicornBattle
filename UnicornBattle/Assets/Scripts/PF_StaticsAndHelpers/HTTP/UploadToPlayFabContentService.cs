using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using PlayFab;
using PlayFab.ClientModels;
using UnicornBattle.Controllers;
using UnicornBattle.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// The out of the box WWW Unity3d class does not afford compatable PUT capability with AWS
/// This class provides a simple example for how to upload files to PlayFab's content service using .NET's built-in HttpWebRequest.
///
/// Scenario: I have a file (UB_Icon.png) located in my /Assets/StreamingAssets project folder that I need to upload to the PlayFab content service and download at a later time for use within my client.
/// </summary>
public class UploadToPlayFabContentService : MonoBehaviour
{
    public bool cleanCacheOnStart = false;
    public bool isInitialContentUnpacked = false;
    public bool useCDN = false;
    public Texture2D defaultBanner;
    public Texture2D defaultSplash;
    public List<AssetBundleHelperObject> assets;

    public Dictionary<string, UBUnpackedAssetBundle> unpackedAssets = new Dictionary<string, UBUnpackedAssetBundle>();

    // Use this for initialization
    void Start()
    {
        //         if (cleanCacheOnStart)
        //         {
        // #if UNITY_2017_1_OR_NEWER && (UNITY_ANDROID || UNITY_IOS || UNITY)
        //             Caching.ClearCache();
        // #else
        //             Caching.CleanCache();
        // #endif
        //         }

        foreach (var asset in assets)
        {
            if (!asset.IsFlagedForUpload)
                continue;

            StartCoroutine(GetFilePath(asset));
            GetContentUploadURL(asset);
        }
    }

    #region PUT_Methods
    /// <summary>
    /// Requests a remote endpoint for uploads from the PlaFab service.
    /// </summary>
    void GetContentUploadURL(AssetBundleHelperObject asset)
    {
        var request = new PlayFab.AdminModels.GetContentUploadUrlRequest();

        if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Android)
            request.Key = "Android/" + asset.ContentKey; // folder location & file name to use on the remote server
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
            request.Key = "iOS/" + asset.ContentKey;
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Switch)
            request.Key = "Switch/" + asset.ContentKey;
        else // stand-alone
            request.Key = asset.ContentKey;
        request.ContentType = asset.MimeType; // mime type to match the file

#if !UNITY_WEBPLAYER
        PlayFab.PlayFabAdminAPI.GetContentUploadUrl(request, result =>
        {
            asset.PutUrl = result.URL;

            byte[] fileContents = File.ReadAllBytes(asset.LocalPutPath);
            PutFile(asset, fileContents);
        }, OnPlayFabError);
#endif
    }

    /// <summary>
    /// Puts the file.
    /// </summary>
    /// <param name="postUrl">Remote URL to use (obtained from GetContentUploadUrl) </param>
    /// <param name="payload">The file to send converted to a byte[] </param>
    public void PutFile(AssetBundleHelperObject asset, byte[] payload)
    {
        if (payload == null)
        {
            Debug.LogWarning("ERROR: Byte array was empty or null");
            return;
        }

        var request = (HttpWebRequest) WebRequest.Create(asset.PutUrl);
        request.Method = "PUT";
        request.ContentType = asset.MimeType;
        using(var dataStream = request.GetRequestStream())
        dataStream.Write(payload, 0, payload.Length);
        var response = (HttpWebResponse) request.GetResponse();
        if (response.StatusCode != HttpStatusCode.OK)
            Debug.LogWarning(string.Format("ERROR: Asset:{0} -- Code:[{1}] -- Msg:{2}", asset.FileName, response.StatusCode, response.StatusDescription));
    }
    #endregion

    #region GET_Methods
    public void KickOffCDNGet(List<AssetBundleHelperObject> assets, UnityAction<bool> p_onCompleteCallback = null)
    {
        StartCoroutine(GetDownloadEndpoints(assets, p_onCompleteCallback));
    }

    public void LoadBundlesFromStreamingAssets(List<AssetBundleHelperObject> p_assetList, UnityAction<bool> p_onCompleteCallback = null)
    {
        foreach (var asset in p_assetList)
        {
            // #if UNITY_ANDROID
            // asset.BundlePlatform = AssetBundleHelperObject.BundleTypes.Android;
            // asset.GetUrl = string.Format("file:///{0}/Android/{1}", Application.streamingAssetsPath, asset.ContentKey);
            // #elif UNITY_IOS
            // asset.BundlePlatform = AssetBundleHelperObject.BundleTypes.iOS;
            // asset.GetUrl = string.Format("{0}/iOS/{1}", Application.streamingAssetsPath, asset.ContentKey);
            // #else
            // asset.BundlePlatform = AssetBundleHelperObject.BundleTypes.StandAlone:
            // asset.GetUrl = string.Format("{0}/{1}", Application.streamingAssetsPath, asset.ContentKey);
            // #endif
            Debug.Log("Loading Asset from: " + asset.GetUrl);
            try
            {
                var loadedBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, asset.ContentKey));

                if (null == loadedBundle) continue;

                asset.Error = string.Empty;
                asset.Unpacked.ContentKey = asset.ContentKey;
                asset.Unpacked.PromoId = asset.FileName;
                asset.Bundle = loadedBundle;

                var assetNames = asset.Bundle.GetAllAssetNames();
                foreach (var assetName in assetNames)
                {
                    var bannerUri = string.Empty;
                    var splashUri = string.Empty;

                    var assetNameLc = assetName.ToLower();
                    var isImage = assetNameLc.EndsWith(".jpg") || assetNameLc.EndsWith(".png");
                    if (assetName.ToLower().Contains("banner.") && isImage)
                        bannerUri = assetName;
                    else if (assetName.ToLower().Contains("splash.") && isImage)
                        splashUri = assetName;

                    if (string.IsNullOrEmpty(bannerUri) == false)
                        asset.Unpacked.Banner = asset.Bundle.LoadAsset<Texture2D>(bannerUri);
                    else if (string.IsNullOrEmpty(splashUri) == false)
                        asset.Unpacked.Splash = asset.Bundle.LoadAsset<Texture2D>(splashUri);
                    else
                        asset.Error += string.Format("[Err: Unpacking: {0} -- {1} ]", asset.FileName, assetName);
                }

                asset.Bundle.Unload(false);
                asset.IsUnpacked = true;

                if (unpackedAssets.ContainsKey(asset.FileName))
                {
                    unpackedAssets[asset.FileName] = asset.Unpacked;
                }
                else
                {
                    unpackedAssets.Add(asset.FileName, asset.Unpacked);
                }

                Debug.Log("Loaded: " + asset.FileName);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Unable to load bundle: " + asset.ContentKey + " with Error: " + ex.Message);
                asset.Error = "Unable to load";
                continue;
            }
        }

        if (p_onCompleteCallback != null)
            p_onCompleteCallback(true);
        Debug.Log("--- AllComplete ---");
    }

    public IEnumerator GetDownloadEndpoints(List<AssetBundleHelperObject> assets, UnityAction<bool> p_onCompleteCallback = null)
    {
        var endTime = Time.time + 30.0f;

        foreach (var asset in assets)
            GetContentDownloadURL(asset);

        while (DoAllAssetsHaveDownloadEndpoints(assets) == false)
        {
            if (Time.time > endTime)
            {
                Debug.LogError("Error: TimeOut");
                PF_Bridge.RaiseCallbackError("CDN Timeout: Could not obtain endpoints", PlayFabAPIMethods.GetCDNContent);
                yield break;
            }
            yield return 0;
        }

        StartCoroutine(GetAssetPackages(assets, p_onCompleteCallback));
    }

    /// <summary>
    /// Requests a remote endpoint for downloads from the PlaFab service.
    /// </summary>
    void GetContentDownloadURL(AssetBundleHelperObject asset)
    {
        var request = new GetContentDownloadUrlRequest();

        if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Android)
            request.Key = "Android/" + asset.ContentKey; // folder location & file name to use on the remote server
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
            request.Key = "iOS/" + asset.ContentKey;
        else // stand-alone
            request.Key = asset.ContentKey;

        PlayFabClientAPI.GetContentDownloadUrl(request, result =>
        {
            asset.GetUrl = result.URL;
        }, OnPlayFabError);
    }

    public IEnumerator GetAssetPackages(List<AssetBundleHelperObject> assets, UnityAction<bool> p_onCompleteCallback = null)
    {
        yield return new WaitForEndOfFrame();

        foreach (var asset in assets)
        {
            var request = UnityWebRequestAssetBundle.GetAssetBundle(asset.GetUrl, asset.Version, 0);

            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                asset.Error = request.error;
                Debug.LogError("HTTP ERROR:" + asset.ContentKey + "\n" + request.error);
                continue;
            }

            asset.Error = string.Empty;
            asset.Unpacked.ContentKey = asset.ContentKey;
            asset.Unpacked.PromoId = asset.FileName;

            asset.Bundle = DownloadHandlerAssetBundle.GetContent(request);
            var assetNames = asset.Bundle.GetAllAssetNames();
            foreach (var assetName in assetNames)
            {
                var bannerUri = string.Empty;
                var splashUri = string.Empty;

                var assetNameLc = assetName.ToLower();
                var isImage = assetNameLc.EndsWith(".jpg") || assetNameLc.EndsWith(".png");
                if (assetName.ToLower().Contains("banner.") && isImage)
                    bannerUri = assetName;
                else if (assetName.ToLower().Contains("splash.") && isImage)
                    splashUri = assetName;

                if (string.IsNullOrEmpty(bannerUri) == false)
                    asset.Unpacked.Banner = asset.Bundle.LoadAsset<Texture2D>(bannerUri);
                else if (string.IsNullOrEmpty(splashUri) == false)
                    asset.Unpacked.Splash = asset.Bundle.LoadAsset<Texture2D>(splashUri);
                else
                    asset.Error += string.Format("[Err: Unpacking: {0} -- {1} ]", asset.FileName, assetName);
            }

            asset.Bundle.Unload(false);
            asset.IsUnpacked = true;

            if (unpackedAssets.ContainsKey(asset.FileName))
            {
                unpackedAssets[asset.FileName] = asset.Unpacked;
            }
            else
            {
                unpackedAssets.Add(asset.FileName, asset.Unpacked);
            }
        }

        if (p_onCompleteCallback != null)
            p_onCompleteCallback(true);
        //Debug.Log("--- AllComplete ---");
    }
    #endregion

    #region Helper_Methods
    /// <summary>
    /// Gets the relative file path; works across Unity build targets (Web, iOS, Android, PC, Mac) 
    /// </summary>
    /// <returns> The assetPath where the file can be found (will vary depending on the platform) </returns>
    IEnumerator GetFilePath(AssetBundleHelperObject asset)
    {
        var platformPrefix = "/";
        if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Android)
            platformPrefix = "/Android/"; // folder location & file name to use on the remote server
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
            platformPrefix = "/iOS/";

        var streamingAssetPath = Application.streamingAssetsPath;
        //string filePath = System.IO.Path.Combine(streamingAssetPath, asset.FileName);
        var filePath = streamingAssetPath + platformPrefix + asset.FileName;
        //useful for uploading from crossplatform (iOS / Android) clients
        if (filePath.Contains("://"))
        {
            var uwr = new UnityWebRequest(filePath);
            yield return uwr.SendWebRequest();
            asset.LocalPutPath = uwr.downloadHandler.text;
        }
        else
        {
            asset.LocalPutPath = filePath;
        }
    }

    public bool DoAllAssetsHaveDownloadEndpoints(List<AssetBundleHelperObject> assets)
    {
        foreach (var asset in assets)
            if (string.IsNullOrEmpty(asset.GetUrl))
                return false;
        return true;
    }

    public bool AreAssetDownloadsAndUnpacksComplete(List<AssetBundleHelperObject> assets)
    {
        foreach (var asset in assets)
            if (asset.IsUnpacked == false)
                return false;
        return true;
    }

    // need a way to serve up assets by id now.
    public UBUnpackedAssetBundle GetAssetsByID(string id)
    {
        if (unpackedAssets.ContainsKey(id))
        {
            if (unpackedAssets[id].Splash == null)
                unpackedAssets[id].Splash = defaultSplash;
            if (unpackedAssets[id].Banner == null)
                unpackedAssets[id].Splash = defaultBanner;
            return unpackedAssets[id];
        }
        else
        {
            var obj = new UBUnpackedAssetBundle
            {
                ContentKey = "Default",
                PromoId = "Default",
                Splash = defaultSplash,
                Banner = defaultBanner
            };

            return obj;
        }
    }

    /// <summary>
    /// Called after a failed GetContentUploadUrl request
    /// </summary>
    /// <param name="result">Error details</param>
    void OnPlayFabError(PlayFabError error)
    {
        Debug.LogWarning(string.Format("PLAYFAB ERROR: [{0}] -- {1}", error.Error, error.ErrorMessage));
    }

    #endregion
}

[Serializable]
public class AssetBundleHelperObject
{
    public string FileName;
    public string ContentKey;
    public uint Version;
    public string MimeType;
    public BundleTypes BundlePlatform;
    public string LocalPutPath;
    public string PutUrl;
    public string GetUrl;
    public string Error;
    public float progress;
    public AssetBundle Bundle;
    public bool IsUnpacked;
    public bool IsFlagedForUpload;
    public UBUnpackedAssetBundle Unpacked;

    //public enum MimeTypes { application/x-gzip, application/octet-stream}
    public enum BundleTypes { StandAlone, iOS, Android, Switch }
}