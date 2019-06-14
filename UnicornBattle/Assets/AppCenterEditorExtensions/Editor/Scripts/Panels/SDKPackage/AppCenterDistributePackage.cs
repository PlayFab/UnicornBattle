namespace AppCenterEditor
{
    public class AppCenterDistributePackage : AppCenterSDKPackage
    {
        private const string DistributeLatestDownload = "https://mobilecentersdkdev.blob.core.windows.net/sdk/AppCenterDistributeLatest.unitypackage";
        private const string DistributeDownloadFormat = "https://github.com/Microsoft/AppCenter-SDK-Unity/releases/download/{0}/AppCenterDistribute-v{0}.unitypackage";
      
        public static AppCenterDistributePackage Instance = new AppCenterDistributePackage();

        public override string TypeName { get { return "Microsoft.AppCenter.Unity.Distribute.Distribute"; } }

        public override string VersionFieldName { get { return "DistributeSDKVersion"; } }

        public override string Name { get { return "Distribute"; } }

        protected override bool IsSupportedForWSA { get { return false; } }

        public override string DownloadLatestUrl { get { return DistributeLatestDownload; } }

        public override string DownloadUrlFormat { get { return DistributeDownloadFormat; } }

        protected override bool IsSdkPackageSupported()
        {
            return true;
        }

        private AppCenterDistributePackage()
        {
        }
    }
}
