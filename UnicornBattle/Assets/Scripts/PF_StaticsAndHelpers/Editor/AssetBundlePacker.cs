using UnityEditor;

public class CreateAssetBundles
{
	//x
	[MenuItem ("Assets/Build StandAlone AssetBundles")]
	static void BuildAllAssetBundles ()
	{
		BuildPipeline.BuildAssetBundles ("Assets/StreamingAssets/",BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXUniversal);
	}
	
	[MenuItem ("Assets/Build Android AssetBundles")]
	static void BuildAndroidAssetBundles ()
	{
		BuildPipeline.BuildAssetBundles ("Assets/StreamingAssets/Android",BuildAssetBundleOptions.None, BuildTarget.Android);
	}
	
	[MenuItem ("Assets/Build iOS AssetBundles")]
	static void BuildiOSAssetBundles ()
	{
		BuildPipeline.BuildAssetBundles ("Assets/StreamingAssets/iOS",BuildAssetBundleOptions.None, BuildTarget.iOS);
	}
}