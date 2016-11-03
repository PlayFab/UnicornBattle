#if !NETFX_CORE || !XAMARIN

using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PlayFab.Json;

namespace PlayFab.Internal
{
    public class PlayFabSysHttp : IPlayFabHttp
    {
        public async Task<object> DoPost(string urlPath, PlayFabRequestCommon request, string authType, string authKey)
        {
            var fullUrl = PlayFabSettings.GetFullUrl(urlPath);
            string bodyString;

            if (request == null)
            {
                bodyString = "{}";
            }
            else
            {
                bodyString = JsonWrapper.SerializeObject(request);
            }

            var client = new HttpClient();
            HttpResponseMessage httpResponse;
            string httpResponseString;
            using (var postBody = new ByteArrayContent(Encoding.UTF8.GetBytes(bodyString)))
            {
                postBody.Headers.Add("Content-Type", "application/json");
                if (authType != null)
                    postBody.Headers.Add(authType, authKey);
                postBody.Headers.Add("X-PlayFabSDK", PlayFabSettings.SdkVersionString);

                try
                {
                    httpResponse = await client.PostAsync(fullUrl, postBody);
                    httpResponseString = await httpResponse.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    return new PlayFabError
                    {
                        Error = PlayFabErrorCode.ConnectionError,
                        ErrorMessage = e.InnerException.Message
                    };
                }
                catch (Exception e)
                {
                    return new PlayFabError
                    {
                        Error = PlayFabErrorCode.ConnectionError,
                        ErrorMessage = e.Message
                    };
                }
            }

            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = new PlayFabError();

                if (string.IsNullOrEmpty(httpResponseString) || httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    error.HttpCode = (int)httpResponse.StatusCode;
                    error.HttpStatus = httpResponse.StatusCode.ToString();
                    return error;
                }

                PlayFabJsonError errorResult;
                try
                {
                    errorResult = JsonWrapper.DeserializeObject<PlayFabJsonError>(httpResponseString);
                }
                catch (Exception e)
                {
                    error.HttpCode = (int)httpResponse.StatusCode;
                    error.HttpStatus = httpResponse.StatusCode.ToString();
                    error.Error = PlayFabErrorCode.JsonParseError;
                    error.ErrorMessage = e.Message;
                    return error;
                }

                error.HttpCode = errorResult.code;
                error.HttpStatus = errorResult.status;
                error.Error = (PlayFabErrorCode)errorResult.errorCode;
                error.ErrorMessage = errorResult.errorMessage;
                error.ErrorDetails = errorResult.errorDetails;
                return error;
            }

            if (string.IsNullOrEmpty(httpResponseString))
            {
                return new PlayFabError
                {
                    Error = PlayFabErrorCode.Unknown,
                    ErrorMessage = "Internal server error"
                };
            }

            return httpResponseString;
        }
    }
}
#endif
