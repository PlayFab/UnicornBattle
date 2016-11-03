
namespace PlayFab
{
    public class PlayFabSettings
    {
        public const string SdkVersion = "0.40.161017";
        public const string BuildIdentifier = "pg_csharpsdk_1";
        public const string SdkVersionString = "CSharpSDK-0.40.161017";

        /// <summary> This is for PlayFab internal debugging.  Generally you shouldn't touch this </summary>
        public static bool UseDevelopmentEnvironment = false;
        /// <summary> This is for PlayFab internal debugging.  Generally you shouldn't touch this </summary>
        public static string DevelopmentEnvironmentUrl = ".playfabsandbox.com";
        /// <summary> This is only for customers running a private cluster.  Generally you shouldn't touch this </summary>
        public static string ProductionEnvironmentUrl = ".playfabapi.com";
        /// <summary> You must set this value for PlayFabSdk to work properly (Found in the Game Manager for your title, at the PlayFab Website) </summary>
        public static string DeveloperSecretKey = null;


        /// <summary> You must set this value for PlayFabSdk to work properly (Found in the Game Manager for your title, at the PlayFab Website) </summary>
        public static string TitleId;
        public static ErrorCallback GlobalErrorHandler;
        /// <summary> Set this to the appropriate AD_TYPE_X constant below </summary>
        public static string AdvertisingIdType = null;
        /// <summary> Set this to corresponding device value </summary>
        public static string AdvertisingIdValue = null;

        // DisableAdvertising is provided for completeness, but changing it is not suggested
        // Disabling this may prevent your advertising-related PlayFab marketplace partners from working correctly
        public static bool DisableAdvertising = false;
        public static readonly string AD_TYPE_IDFA = "Idfa";
        public static readonly string AD_TYPE_ANDROID_ID = "Adid";

        public static string GetFullUrl(string apiCall)
        {
            var baseUrl = UseDevelopmentEnvironment ? DevelopmentEnvironmentUrl : ProductionEnvironmentUrl;
            if (baseUrl.StartsWith("http"))
                return baseUrl;
            return "https://" + TitleId + baseUrl + apiCall;
        }
    }
}
