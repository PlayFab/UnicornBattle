//
//  iOSBridge.h
//  iOSBridge
//
//  Created by Supersonic.
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Supersonic/Supersonic.h>
#import <Supersonic/SupersonicEventsReporting.h>
#import <SUpersonic/SUSupersonicAdsConfiguration.h>


@interface iOSBridge : NSObject<SupersonicRVDelegate, SupersonicISDelegate, SupersonicOWDelegate>

+ (iOSBridge*) start;
- (void)setAge:(int)age;
- (void)setGender:(NSString *)gender;
- (void)setMediationSegment:(NSString *)segment;

- (void)reportAppStarted;

/*-----------------------------------------------*/
// Rewarded Video
/*-----------------------------------------------*/

- (void)initRewardedVideoWithAppKey:(NSString *)appKey withUserId:(NSString*)userId;
- (void)showRewardedVideo;
- (void)showRewardedVideoWithPlacementName:(NSString*) placementName;
- (BOOL)isRewardedVideoAvailable;
- (const char *) getPlacementInfo: (NSString *) placementName;
- (BOOL)isRewardedVideoPlacementCapped:(NSString *)placementName;

/*-----------------------------------------------*/
// Interstitial
/*-----------------------------------------------*/
- (void)initInterstitialWithAppKey:(NSString *)appKey withUserId:(NSString*)userId;
- (void)loadInterstitial;
- (void)showInterstitial;
- (void)showInterstitialWithPlacementName:(NSString*) placementName;
- (BOOL)isInterstitialReady;
- (BOOL)isInterstitialPlacementCapped:(NSString *)placementName;

/*-----------------------------------------------*/
// Offerwall
/*-----------------------------------------------*/
- (void)initOfferwallWithAppKey:(NSString *)appKey withUserId:(NSString*)userId;
- (void)showOfferwall;
- (void)showOfferwallWithPlacementName:(NSString*) placementName;
- (void)getOfferwallCredits;
- (BOOL)isOfferwallAvailable;

@end