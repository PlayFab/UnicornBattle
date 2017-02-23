using PlayFab;
using PlayFab.AdminModels;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The out of the box WWW Unity3d class does not afford compatable PUT capability with AWS
/// This class provides a simple example for how to upload files to PlayFab's content service using .NET's built-in HttpWebRequest.
///
/// Scenario: I have a file (UB_Icon.png) located in my /Assets/StreamingAssets project folder that I need to upload to the PlayFab content service and download at a later time for use within my client.
/// </summary>
public class UploadToPlayFabContentService : MonoBehaviour
{
    public bool cleanCacheOnStart = false;
    public bool isInitalContentUnpacked = false;
    public bool useCDN = true;
    public Texture2D defaultBanner;
    public Texture2D defaultSplash;
    public List<AssetBundleHelperObject> assets;

    public Dictionary<string, UB_UnpackedAssetBundle> unpackedAssets = new Dictionary<string, UB_UnpackedAssetBundle>();

    // Use this for initialization
    void Start()
    {
        if (cleanCacheOnStart)
            Caching.CleanCache();

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
        var request = new GetContentUploadUrlRequest();

        if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Android)
            request.Key = "Android/" + asset.ContentKey;        // folder location & file name to use on the remote server
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
            request.Key = "iOS/" + asset.ContentKey;
        else // stand-alone
            request.Key = asset.ContentKey;
        request.ContentType = asset.MimeType;       // mime type to match the file

#if UNITY_WEBPLAYER
		//UnityEngine.Deubg.Log("Webplayer does not support uploading files.");
#else
        PlayFabAdminAPI.GetContentUploadUrl(request, result =>
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
            Debug.LogWarning("ERROR: Byte arrry was empty or null");
            return;
        }

        var request = (HttpWebRequest)WebRequest.Create(asset.PutUrl);
        request.Method = "PUT";
        request.ContentType = asset.MimeType;
        using (var dataStream = request.GetRequestStream())
            dataStream.Write(payload, 0, payload.Length);
        var response = (HttpWebResponse)request.GetResponse();
        if (response.StatusCode != HttpStatusCode.OK)
            Debug.LogWarning(string.Format("ERROR: Asset:{0} -- Code:[{1}] -- Msg:{2}", asset.FileName, response.StatusCode, response.StatusDescription));
    }
    #endregion

    #region GET_Methods
    public void KickOffCDNGet(List<AssetBundleHelperObject> assets, UnityAction<bool> callback = null)
    {
        StartCoroutine(GetDownloadEndpoints(assets, callback));
    }

    public IEnumerator GetDownloadEndpoints(List<AssetBundleHelperObject> assets, UnityAction<bool> callback = null)
    {
        var endTime = Time.time + 30.0f;

        foreach (var asset in assets)
            GetContentDownloadURL(asset);

        while (DoAllAssetsHaveDownloadEndpoints(assets) == false)
        {
            if (Time.time > endTime)
            {
                Debug.Log("Error: TimeOut");
                PF_Bridge.RaiseCallbackError("CDN Timeout: Could not obtain endpoints", PlayFabAPIMethods.GetCDNConent, MessageDisplayStyle.error);
                yield break;
            }
            yield return 0;
        }

        StartCoroutine(GetAssetPackages(assets, callback));
    }

    /// <summary>
    /// Requests a remote endpoint for downloads from the PlaFab service.
    /// </summary>
    void GetContentDownloadURL(AssetBundleHelperObject asset)
    {
        var request = new GetContentDownloadUrlRequest();

        if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Android)
            request.Key = "Android/" + asset.ContentKey;        // folder location & file name to use on the remote server
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
            request.Key = "iOS/" + asset.ContentKey;
        else // stand-alone
            request.Key = asset.ContentKey;

        PlayFabClientAPI.GetContentDownloadUrl(request, result =>
        {
            asset.GetUrl = result.URL;
        }, OnPlayFabError);
    }

    public IEnumerator GetAssetPackages(List<AssetBundleHelperObject> assets, UnityAction<bool> callback = null)
    {
        var stTime = Time.time;
        var timeOut = 30.0f;

        foreach (var asset in assets)
            StartCoroutine(DownloadAndUnpackAsset(asset));

        while (AreAssetDownloadsAndUnpacksComplete(assets) == false)
        {
            if (Time.time > stTime + timeOut)
            {
                PF_Bridge.RaiseCallbackError("CDN Timeout: Could not obtain AssetBundles", PlayFabAPIMethods.GetCDNConent, MessageDisplayStyle.error);
                if (callback != null)
                    callback(false);
                yield break;
            }

            yield return 0;
        }

        foreach (var asset in assets)
            unpackedAssets.Add(asset.FileName, asset.Unpacked);

        if (callback != null)
            callback(true);
        Debug.Log("--- AllComplete ---");
    }

    public IEnumerator DownloadAndUnpackAsset(AssetBundleHelperObject asset)
    {
        // Caching.IsVersionCached(asset.GetUrl, asset.Version)
        // Start a download of the given URL
        var www = WWW.LoadFromCacheOrDownload(asset.GetUrl, asset.Version);

        // wait until the download is done
        while (www.progress < 1)
        {
            asset.progress = www.progress;
            yield return www;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            asset.Error = www.error;
            Debug.LogError("HTTP ERROR:" + asset.ContentKey + "\n" + www.error);
            yield break;
        }

        asset.Error = "";
        asset.Unpacked.ContentKey = asset.ContentKey;
        asset.Unpacked.PromoId = asset.FileName;

        asset.Bundle = www.assetBundle;
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
    }
    #endregion

    #region Helper_Methods
    /// <summary>
    /// Gets the reletive file path; works acrocss Unity build targets (Web, iOS, Android, PC, Mac) 
    /// </summary>
    /// <returns> The assetPath where the file can be found (will varry depending on the platform) </returns>
    IEnumerator GetFilePath(AssetBundleHelperObject asset)
    {
        var platformPrefix = "/";
        if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Android)
            platformPrefix = "/Android/";       // folder location & file name to use on the remote server
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
            platformPrefix = "/iOS/";

        var streamingAssetPath = Application.streamingAssetsPath;
        //string filePath = System.IO.Path.Combine(streamingAssetPath, asset.FileName);
        var filePath = streamingAssetPath + platformPrefix + asset.FileName;
        //useful for uploading from crossplatform (iOS / Android) clients
        if (filePath.Contains("://"))
        {
            var www = new WWW(filePath);
            yield return www;
            asset.LocalPutPath = www.text;
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
    public UB_UnpackedAssetBundle GetAssetsByID(string id)
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
            var obj = new UB_UnpackedAssetBundle
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
    public int Version;
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
    public UB_UnpackedAssetBundle Unpacked;

    //public enum MimeTypes { application/x-gzip, application/octet-stream}
    public enum BundleTypes { StandAlone, iOS, Android }
}

[Serializable]
public class UB_UnpackedAssetBundle
{
    public string PromoId;
    public string ContentKey;
    public Texture2D Banner;
    public Texture2D Splash;
}
