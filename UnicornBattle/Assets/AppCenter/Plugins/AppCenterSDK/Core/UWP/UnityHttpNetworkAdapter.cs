// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

#if UNITY_WSA_10_0 && ENABLE_IL2CPP && !UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AppCenter.Ingestion.Http;
using UnityEngine.Networking;

namespace Microsoft.AppCenter.Unity.Internal.Utils
{
    public class UnityHttpNetworkAdapter : IHttpNetworkAdapter
    {
        public Task<string> SendAsync(string uri, string method, IDictionary<string, string> headers, string jsonContent,
            CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<string>();
            UnityCoroutineHelper.StartCoroutine(() => SendAsync(uri, method, headers, jsonContent, request =>
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    tcs.SetCanceled();
                    return;
                }
                if (request.isNetworkError)
                {
                    tcs.SetException(new NetworkIngestionException());
                }
                else if (request.isHttpError)
                {
                    tcs.SetException(new HttpIngestionException($"Operation returned an invalid status code '{request.responseCode}'")
                    {
                        Method = request.method,
                        RequestUri = new Uri(request.url),
                        StatusCode = (int)request.responseCode,
                        RequestContent = jsonContent,
                        ResponseContent = request.downloadHandler.text
                    });
                }
                else
                {
                    var responseContent = request.downloadHandler.text;
                    AppCenterLog.Verbose(AppCenterLog.LogTag, $"HTTP response status={(int)request.responseCode} payload={responseContent}");
                    tcs.SetResult(responseContent);
                }
            }));
            return tcs.Task;
        }

        public void Dispose()
        {
        }

        private IEnumerator SendAsync(string uri, string method, IDictionary<string, string> headers, string jsonContent, Action<UnityWebRequest> callback)
        {
            using (var request = new UnityWebRequest(uri, method))
            {
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonContent))
                {
                    contentType = "application/json; charset=utf-8"
                };
                request.downloadHandler = new DownloadHandlerBuffer();
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
                yield return request.Send();
                callback(request);
            }
        }
    }
}
#endif