namespace AppCenterEditor
{
    public class AppCenterPushPackage : AppCenterSDKPackage
    {
        private const string PushLatestDownload = "https://mobilecentersdkdev.blob.core.windows.net/sdk/AppCenterPushLatest.unitypackage";
        private const string PushDownloadFormat = "https://github.com/Microsoft/AppCenter-SDK-Unity/releases/download/{0}/AppCenterPush-v{0}.unitypackage";
      
        public static AppCenterPushPackage Instance = new AppCenterPushPackage();

        public override string TypeName { get { return "Microsoft.AppCenter.Unity.Push.Push"; } }

        public override string VersionFieldName { get { return "PushSDKVersion"; } }

        public override string Name { get { return "Push"; } }

        protected override bool IsSupportedForWSA { get { return false; } }

        public override string DownloadLatestUrl { get { return PushLatestDownload; } }

        public override string DownloadUrlFormat { get { return PushDownloadFormat; } }

        protected override bool IsSdkPackageSupported()
        {
            return true;
        }

        private AppCenterPushPackage()
        {
        }
    }
}
