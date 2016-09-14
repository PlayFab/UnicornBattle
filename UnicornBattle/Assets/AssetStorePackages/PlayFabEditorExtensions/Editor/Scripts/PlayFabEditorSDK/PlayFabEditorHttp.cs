using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using PlayFab.Editor.Json;
using PlayFab.Editor.EditorModels;

#if UNITY_5_4
    using UnityEngine.Networking;
#else
    using UnityEngine.Experimental.Networking;
#endif


namespace PlayFab.Editor
{
    public class PlayFabEditorHttp : UnityEditor.Editor
    {
        internal static void MakeDownloadCall(string url, Action<string> resultCallback)
        {
            var www = new WWW(url);
            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnHttpReq, url, PlayFabEditorHelper.MSG_SPIN_BLOCK);
            EditorCoroutine.start(PostDownload(www, (response) =>
            {
                PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnHttpRes, url);

                var fileName = string.Empty;
                if(url.IndexOf("unity-edex") > -1)
                {
                    fileName = PlayFabEditorHelper.EDEX_DOWNLOAD_PATH;
                }
                else if(url.IndexOf("unity-via-edex") > -1)
                {
                    fileName = PlayFabEditorHelper.SDK_DOWNLOAD_PATH;
                }


                string fileSaveLocation = string.Format(PlayFabEditorHelper.EDITOR_ROOT + fileName);
                System.IO.File.WriteAllBytes(fileSaveLocation, response);
                resultCallback(fileSaveLocation);

            }, PlayFabEditorHelper.SharedErrorCallback), www);
        }


        internal static void MakeApiCall<TRequestType, TResultType>(string api, string apiEndpoint, TRequestType request, Action<TResultType> resultCallback, Action<EditorModels.PlayFabError> errorCallback)
        {
            var url = apiEndpoint + api;
            var req = JsonWrapper.SerializeObject(request, PlayFabEditorUtil.ApiSerializerStrategy);
            //Set headers
            var headers = new Dictionary<string, string>
            {
                {"Content-Type", "application/json"},
                {"X-ReportErrorAsSuccess", "true"},
                {"X-PlayFabSDK", string.Format("{0}_{1}", PlayFabEditorHelper.EDEX_NAME, PlayFabEditorHelper.EDEX_VERSION)}
            };


            if(api.Contains("/Server/") || api.Contains("/Admin/"))
            {
                if(PlayFabEditorDataService.activeTitle == null || string.IsNullOrEmpty(PlayFabEditorDataService.activeTitle.SecretKey))
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, "Must have PlayFabSettings.DeveloperSecretKey set to call this method");
                    return;
                }

                headers.Add("X-SecretKey", PlayFabEditorDataService.activeTitle.SecretKey);            
            }



            //Encode Payload
            var payload = System.Text.Encoding.UTF8.GetBytes(req.Trim());

            var www = new WWW(url, payload, headers);

            PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnHttpReq, api, PlayFabEditorHelper.MSG_SPIN_BLOCK);

            EditorCoroutine.start(Post(www, (response) =>
            {
                var httpResult = JsonWrapper.DeserializeObject<HttpResponseObject>(response,
                       PlayFabEditorUtil.ApiSerializerStrategy);

                if (httpResult.code == 200)
                {

                    try
                    {
                        PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnHttpRes, api);

                        var dataJson = JsonWrapper.SerializeObject(httpResult.data,
                            PlayFabEditorUtil.ApiSerializerStrategy);
                        var result = JsonWrapper.DeserializeObject<TResultType>(dataJson,
                            PlayFabEditorUtil.ApiSerializerStrategy);

                        if (resultCallback != null)
                        {
                            resultCallback(result);
                        }
                    }
                    catch (Exception e)
                    {
                        PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, e.Message);
                    }

                }
                else
                {
                    if (errorCallback != null)
                    {
                        PlayFab.Editor.EditorModels.PlayFabError playFabError = PlayFabEditorHelper.GeneratePlayFabError(response);
                        errorCallback(playFabError);
                    }
                    else
                    {
                        PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, string.Format("ErrorCode:{0} -- {1}", httpResult.code, httpResult.status));
                    }
                }


            }, (error) =>
            {
                if (errorCallback != null)
                {
                    var playFabError = PlayFabEditorHelper.GeneratePlayFabError(error);
                    errorCallback(playFabError);
                }
                else
                {
                    PlayFabEditor.RaiseStateUpdate(PlayFabEditor.EdExStates.OnError, error);
                }
            }),www);

        }



        internal static void MakeGitHubApiCall(string url, Action<string> resultCallback)
        {
            var www = new WWW(url);

            EditorCoroutine.start(Post(www, (response) =>
            {
                List<System.Object> jsonResponse = JsonWrapper.DeserializeObject<List<System.Object>>(response);

                // list seems to come back in ascending order (oldest -> newest)
                if(jsonResponse != null && jsonResponse.Count > 0)
                {
                    JsonObject latestSdkTag = (JsonObject)jsonResponse[jsonResponse.Count -1];

                    object tag;
                    if(latestSdkTag.TryGetValue("ref", out tag))
                    {
                        if(resultCallback != null)
                        {
                            int startIndex = tag.ToString().LastIndexOf('/')+1;
                            int length = tag.ToString().Length - startIndex;   
                            resultCallback(tag.ToString().Substring(startIndex, length));
                        }
                    }
                    else
                    {
                        if(resultCallback != null)
                        {
                            resultCallback(null);
                        }  
                    }
                    return;
                }

            }, PlayFabEditorHelper.SharedErrorCallback),www);

        }


        private static IEnumerator Post(WWW www, Action<string> callBack, Action<string> errorCallback)
        {
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                errorCallback(www.error);
            }
            else
            {
                callBack(www.text);
            }
            yield break;
        }


        private static IEnumerator PostDownload(WWW www, Action<byte[]> callBack, Action<string> errorCallback)
        {
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                errorCallback(www.error);
            }
            else
            {
                callBack(www.bytes);
            }
            yield break;
        }

    }
}