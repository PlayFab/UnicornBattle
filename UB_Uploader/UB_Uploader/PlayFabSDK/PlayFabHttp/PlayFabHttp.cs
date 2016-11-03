using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlayFab.Internal
{
    /// <summary>
    /// This is a base-class for all Api-request objects.
    /// It is currently unfinished, but we will add result-specific properties,
    ///   and add template where-conditions to make some code easier to follow
    /// </summary>
    public class PlayFabRequestCommon
    {
    }

    /// <summary>
    /// This is a base-class for all Api-result objects.
    /// It is currently unfinished, but we will add result-specific properties,
    ///   and add template where-conditions to make some code easier to follow
    /// </summary>
    public class PlayFabResultCommon
    {
    }

    public class PlayFabJsonError
    {
        public int code;
        public string status;
        public string error;
        public int errorCode;
        public string errorMessage;
        public Dictionary<string, string[]> errorDetails = null;
    }

    public class PlayFabJsonSuccess<TResult> where TResult : PlayFabResultCommon
    {
        public int code;
        public string status;
        public TResult data;
    }

    public static class PlayFabHttp
    {
        private static IPlayFabHttp _http;

        static PlayFabHttp()
        {
            var httpInterfaceType = typeof(IPlayFabHttp);
            var types = typeof(PlayFabHttp).GetAssembly().GetTypes();
            foreach (var eachType in types)
            {
                if (httpInterfaceType.IsAssignableFrom(eachType) && !eachType.IsAbstract)
                {
                    _http = (IPlayFabHttp)Activator.CreateInstance(eachType.AsType());
                    return;
                }
            }
            throw new Exception("Cannot find a valid IPlayFabHttp type");
        }

        public static async Task<object> DoPost(string urlPath, PlayFabRequestCommon request, string authType, string authKey)
        {
            if (PlayFabSettings.TitleId == null)
                throw new Exception("You must set your titleId before making an api call");
            return await _http.DoPost(urlPath, request, authType, authKey);
        }
    }
}
