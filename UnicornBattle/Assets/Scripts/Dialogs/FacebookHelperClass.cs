using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using PlayFab.Json;

public class FacebookHelperClass
{
    public static string meQueryString = "/v2.0/me?fields=id,first_name,friends.limit(100).fields(first_name,id,picture.width(128).height(128)),invitable_friends.limit(100).fields(first_name,id,picture.width(128).height(128))";
    public delegate void LoadPictureCallback(Texture texture);
    public delegate void WebFetchHandle(string url, UnityAction<Texture2D> callback);

    #region facebook Util
    public static string GetPictureURL(string facebookID, int? width = null, int? height = null, string type = null)
    {
        var url = string.Format("/{0}/picture", facebookID);
        var query = width != null ? "&width=" + width.ToString() : "";
        query += height != null ? "&height=" + height.ToString() : "";
        query += type != null ? "&type=" + type : "";
        query += "&redirect=false";
        if (query != "") url += ("?g" + query);
        return url;
    }

    public static string DeserializePictureURLString(string response)
    {
        Dictionary<string, FB_PhotoResponse> result = JsonWrapper.DeserializeObject<Dictionary<string, FB_PhotoResponse>>(response);
        FB_PhotoResponse urlH;
        result.TryGetValue("data", out urlH);
        return urlH == null ? null : urlH.url;
    }

    public static string DeserializePictureURLObject(object pictureObj)
    {
        var picture = (Dictionary<string, object>)(((Dictionary<string, object>)pictureObj)["data"]);
        object urlH;
        picture.TryGetValue("url", out urlH);
        return urlH as string;
    }
    #endregion

    public static IEnumerator GetPlayerProfilePhoto(WebFetchHandle webRequest, UnityAction<Texture2D> callback = null)
    {
        if (FB.IsInitialized && FB.IsLoggedIn)
        {
            LoadPictureAPI(GetPictureURL("me", 128, 128), webRequest, tx =>
            {
                if (tx == null)
                {
                    // Let's just try again
                    LoadPictureAPI(GetPictureURL("me", 128, 128), webRequest, callback);
                    return;
                }
                else
                {
                    if (callback != null)
                        callback(tx);
                }
            });
        }
        yield break;
    }

    public static IEnumerator GetFriendProfilePhoto(string id, WebFetchHandle webRequest, UnityAction<Texture2D> callback = null)
    {
        if (FB.IsInitialized && FB.IsLoggedIn)
        {
            LoadPictureAPI(GetPictureURL(id, 128, 128), webRequest, tx =>
            {
                if (tx == null)
                {
                    // Let's just try again
                    LoadPictureAPI(GetPictureURL(id, 128, 128), webRequest, callback);
                    return;
                }
                else
                {
                    if (callback != null)
                        callback(tx);
                }
            });
        }
        yield break;
    }

    public static void LoadPictureAPI(string url, WebFetchHandle wwwFetch, UnityAction<Texture2D> callback = null)
    {
        FB.API(url, HttpMethod.GET, result =>
        {
            if (result.Error != null)
            {
                Debug.LogError(result.Error);
                return;
            }

            var imageUrl = DeserializePictureURLString(result.RawResult);

            wwwFetch(imageUrl, callback);
        });
    }


    public static void GetFBUserName(UnityAction<string> callback = null)
    {
        FB.API("me?fields=name", HttpMethod.GET, result =>
        {
            if (result.Error != null)
            {
                Debug.LogWarning("Facebook API Error: " + result.Error);
                if (callback != null)
                    callback(null);
            }
            else
            {
                var dict = result.ResultDictionary;
                if (callback != null && dict.ContainsKey("name"))
                    callback(dict["name"].ToString());
            }
        });
    }
}

public class FB_PhotoResponse
{
    public int height { get; set; }
    public int width { get; set; }
    public bool is_silhouette { get; set; }
    public string url { get; set; }
}
