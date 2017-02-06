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
        {
            Caching.CleanCache();
        }

        foreach (var asset in this.assets)
        {
            if (asset.IsFlagedForUpload)
            {
                StartCoroutine(GetFilePath(asset));
                GetContentUploadURL(asset);
            }
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
        {
            request.Key = "Android/" + asset.ContentKey;        // folder location & file name to use on the remote server
        }
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
        {
            request.Key = "iOS/" + asset.ContentKey;
        }
        else // stand-alone
        {
            request.Key = asset.ContentKey;
        }
        request.ContentType = asset.MimeType;       // mime type to match the file

        Debug.Log("Requesting url for PUT: " + asset.FileName);

#if UNITY_WEBPLAYER
			//UnityEngine.Deubg.Log("Webplayer does not support uploading files.");
#else
        PlayFabAdminAPI.GetContentUploadUrl(request, (GetContentUploadUrlResult result) =>
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
        var request = (HttpWebRequest)WebRequest.Create(asset.PutUrl);
        request.Method = "PUT";
        request.ContentType = asset.MimeType;

        if (payload != null)
        {
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(payload, 0, payload.Length);
            dataStream.Close();
        }
        else
        {
            Debug.LogWarning(string.Format("ERROR: Byte arrry was empty or null"));
            return;
        }

        Debug.Log("Starting HTTP PUT: " + asset.FileName);
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Debug.Log("HTTP PUT Successful:" + asset.FileName);
        }
        else
        {
            Debug.LogWarning(string.Format("ERROR: Asset:{0} -- Code:[{1}] -- Msg:{2}", asset.FileName, response.StatusCode, response.StatusDescription));
        }
    }

    #endregion

    #region GET_Methods

    public void KickOffCDNGet(List<AssetBundleHelperObject> assets, UnityAction<bool> callback = null)
    {
        StartCoroutine(GetDownloadEndpoints(assets, callback));
    }


    public IEnumerator GetDownloadEndpoints(List<AssetBundleHelperObject> assets, UnityAction<bool> callback = null)
    {
        float stTime = Time.time;
        float timeOut = 30;

        foreach (var asset in assets)
        {
            //StartCoroutine(DownloadAndUnpackAsset(asset));
            Debug.Log("Requesting url for GET: " + asset.FileName);
            GetContentDownloadURL(asset);
        }

        while (DoAllAssetsHaveDownloadEndpoints(assets) == false)
        {
            if (Time.time > stTime + timeOut)
            {
                Debug.Log("Error: TimeOut");
                PF_Bridge.RaiseCallbackError("CDN Timeout: Could not obtain endpoints", PlayFabAPIMethods.GetCDNConent, MessageDisplayStyle.error);
                yield break;
            }
            yield return 0;
        }

        Debug.Log("--- DownloadEndpoint Complete ---");
        StartCoroutine(GetAssetPackages(this.assets, callback));
        yield break;
    }



    /// <summary>
    /// Requests a remote endpoint for downloads from the PlaFab service.
    /// </summary>
    void GetContentDownloadURL(AssetBundleHelperObject asset)
    {
        GetContentDownloadUrlRequest request = new GetContentDownloadUrlRequest();

        if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Android)
        {
            request.Key = "Android/" + asset.ContentKey;        // folder location & file name to use on the remote server
        }
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
        {
            request.Key = "iOS/" + asset.ContentKey;
        }
        else // stand-alone
        {
            request.Key = asset.ContentKey;
        }
        //request.ThruCDN = this.useCDN;
        //request.ThruCDN = false;

        PlayFabClientAPI.GetContentDownloadUrl(request, (GetContentDownloadUrlResult result) =>
                                               {
                                                   asset.GetUrl = result.URL;
                                               }, OnPlayFabError);
    }

    public IEnumerator GetAssetPackages(List<AssetBundleHelperObject> assets, UnityAction<bool> callback = null)
    {
        float stTime = Time.time;
        float timeOut = 30;

        foreach (var asset in assets)
        {
            StartCoroutine(DownloadAndUnpackAsset(asset));
        }

        while (AreAssetDownloadsAndUnpacksComplete(assets) == false)
        {
            if (Time.time > stTime + timeOut)
            {
                //Debug.Log ("Error: TimeOut");
                PF_Bridge.RaiseCallbackError("CDN Timeout: Could not obtain AssetBundles", PlayFabAPIMethods.GetCDNConent, MessageDisplayStyle.error);

                if (callback != null)
                {
                    callback(false);
                }

                yield break;
            }

            yield return 0;
        }

        foreach (var asset in assets)
        {
            unpackedAssets.Add(asset.FileName, asset.Unpacked);
        }

        if (callback != null)
        {
            callback(true);
        }
        Debug.Log("--- AllComplete ---");
        yield break;
    }




    public IEnumerator DownloadAndUnpackAsset(AssetBundleHelperObject asset)
    {
        Debug.Log(Caching.IsVersionCached(asset.GetUrl, asset.Version));

        // Start a download of the given URL
        Debug.Log("HTTP GET:" + asset.ContentKey);
        var www = WWW.LoadFromCacheOrDownload(asset.GetUrl, asset.Version);

        // wait until the download is done
        while (www.progress < 1)
        {
            asset.progress = www.progress;
            yield return www;
        }

        if (string.IsNullOrEmpty(www.error))
        {

            asset.Error = "";
            asset.Unpacked.ContentKey = asset.ContentKey;
            asset.Unpacked.PromoId = asset.FileName;

            asset.Bundle = www.assetBundle;

            string[] names = asset.Bundle.GetAllAssetNames();
            //Debug.Log(names.ToString());

            foreach (var name in names)
            {
                string bannerURI = string.Empty;
                string splashURI = string.Empty;

                Debug.Log("Unpacking:" + name);
                if (name.Contains("banner.png") || name.Contains("Banner.png") || name.Contains("banner.jpg") || name.Contains("banner.jpg"))
                {
                    bannerURI = name;
                }
                else if (name.Contains("splash.png") || name.Contains("Splash.png") || name.Contains("splash.jpg") || name.Contains("Splash.jpg"))
                {
                    splashURI = name;
                }

                if (string.IsNullOrEmpty(bannerURI) == false)
                {
                    Texture2D banner = asset.Bundle.LoadAsset<Texture2D>(bannerURI);
                    asset.Unpacked.Banner = banner;
                }
                else if (string.IsNullOrEmpty(splashURI) == false)
                {
                    Texture2D splash = asset.Bundle.LoadAsset<Texture2D>(splashURI);
                    asset.Unpacked.Splash = splash;
                }
                else
                {
                    asset.Error += string.Format("[Err: Unplacking: {0} -- {1} ]", asset.FileName, name);
                }
            }

            asset.Bundle.Unload(false);
            asset.IsUnpacked = true;
            //this.isImageDownloaded = true;
            yield break;
        }
        else
        {
            asset.Error = www.error;
            Debug.Log("HTTP ERROR:" + asset.ContentKey);
        }

        yield break;
    }



    #endregion

    #region Helper_Methods
    /// <summary>
    /// Gets the reletive file path; works acrocss Unity build targets (Web, iOS, Android, PC, Mac) 
    /// </summary>
    /// <returns> The assetPath where the file can be found (will varry depending on the platform) </returns>
    IEnumerator GetFilePath(AssetBundleHelperObject asset)
    {
        string platformPrefix = "/";
        if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.Android)
        {
            platformPrefix = "/Android/";       // folder location & file name to use on the remote server
        }
        else if (asset.BundlePlatform == AssetBundleHelperObject.BundleTypes.iOS)
        {
            platformPrefix = "/iOS/";
        }


        string streamingAssetPath = Application.streamingAssetsPath;
        //string filePath = System.IO.Path.Combine(streamingAssetPath, asset.FileName);
        string filePath = streamingAssetPath + platformPrefix + asset.FileName;
        //useful for uploading from crossplatform (iOS / Android) clients
        if (filePath.Contains("://"))
        {
            WWW www = new WWW(filePath);
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
        {
            if (string.IsNullOrEmpty(asset.GetUrl))
            {
                return false;
            }
        }
        return true;
    }

    public bool AreAssetDownloadsAndUnpacksComplete(List<AssetBundleHelperObject> assets)
    {
        foreach (var asset in assets)
        {
            if (asset.IsUnpacked == false)
            {
                return false;
            }
        }
        return true;
    }

    // need a way to serve up assets by id now.
    public UB_UnpackedAssetBundle GetAssetsByID(string id)
    {
        if (unpackedAssets.ContainsKey(id))
        {

            if (unpackedAssets[id].Splash == null)
            {
                unpackedAssets[id].Splash = defaultSplash;
            }

            if (unpackedAssets[id].Banner == null)
            {
                unpackedAssets[id].Splash = defaultBanner;
            }

            return unpackedAssets[id];
        }
        else
        {

            UB_UnpackedAssetBundle obj = new UB_UnpackedAssetBundle()
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

public class UB_AssetPack
{
    public Texture2D Banner;
    public Texture2D Splash;
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
