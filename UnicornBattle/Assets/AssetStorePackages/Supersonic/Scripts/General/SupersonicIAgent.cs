
public interface SupersonicIAgent
{
	void start();
	void reportAppStarted();

	//Base API
	void onResume();
	void onPause();
	void setAge(int age);
	void setGender(string gender);
	void setMediationSegment(string segment);
	string getAdvertiserId();
	void validateIntegration();
	void shouldTrackNetworkState(bool track);

	//RewardedVideo API
	void initRewardedVideo(string appKey,string userId);
	void showRewardedVideo();
	bool isRewardedVideoAvailable();
	void showRewardedVideo(string placementName);
	bool isRewardedVideoPlacementCapped(string placementName);

	SupersonicPlacement getPlacementInfo (string name);
		
	//Interstitial API
	void initInterstitial(string appKey,string userId);
	void loadInterstitial();
	void showInterstitial();
	void showInterstitial(string placementName);
	bool isInterstitialReady();
	bool isInterstitialPlacementCapped(string placementName);

	//Offerwall API
	void initOfferwall(string appKey,string userId);
	void showOfferwall();
	void showOfferwall(string placementName);
	bool isOfferwallAvailable();
	void getOfferwallCredits();
}
