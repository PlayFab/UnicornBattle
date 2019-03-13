namespace AppCenterEditor
{
    public class AppCenterCrashesPackage : AppCenterSDKPackage
    {
        private const string CrashesLatestDownload = "https://mobilecentersdkdev.blob.core.windows.net/sdk/AppCenterCrashesLatest.unitypackage";
        private const string CrashesDownloadFormat = "https://github.com/Microsoft/AppCenter-SDK-Unity/releases/download/{0}/AppCenterCrashes-v{0}.unitypackage";

        public static AppCenterCrashesPackage Instance = new AppCenterCrashesPackage();

        public override string Name { get { return "Crashes"; } }

        protected override bool IsSupportedForWSA { get { return false; } }

        public override string TypeName { get { return "Microsoft.AppCenter.Unity.Crashes.Crashes"; } }

        public override string VersionFieldName { get { return "CrashesSDKVersion"; } }

        public override string DownloadLatestUrl { get { return CrashesLatestDownload; } }

        public override string DownloadUrlFormat { get { return CrashesDownloadFormat; } }

        protected override bool IsSdkPackageSupported()
        {
            return true;
        }

        private AppCenterCrashesPackage()
        {
        }
    }
}
