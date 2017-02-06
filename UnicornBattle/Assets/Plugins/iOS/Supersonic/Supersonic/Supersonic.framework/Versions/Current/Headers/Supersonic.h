//
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#ifndef SUPERSONIC_H
#define SUPERSONIC_H

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

#import "SupersonicConfiguration.h"
#import "SupersonicConfigurationProtocol.h"
#import "SupersonicGender.h"
#import "SupersonicRVDelegate.h"
#import "SupersonicOWDelegate.h"
#import "SupersonicISDelegate.h"
#import "SupersonicLogDelegate.h"
#import "SupersonicPlacementInfo.h"
#import "SupersonicIntegrationHelper.h"
#import "SupersonicEventsReporting.h"
#import "SUSupersonicAdsConfiguration.h"

NS_ASSUME_NONNULL_BEGIN

@interface Supersonic : NSObject

/*!
 * @discussion Create an instance of Supersonic SDK main class.
 * @return instance of Supersonic SDK.
 */
+ (Supersonic *)sharedInstance;

/*!
 * @discussion Get Supersonic SDK version by using the below function.
 * @return NSString representing the current version.
 */
- (NSString *)getVersion;


/*!
 * @discussion Set the age of your user by using the below function.
 * @param age is the user's age represented by int type.
 */
- (void)setAge:(int)age;

/*!
 * @discussion Set the gender of your user by using the below function.
 * @param gender is the user's gender reperesented by SupersonicGender enum type.
 */
- (void)setGender:(SupersonicGender)gender;

/*!
 * @discussion Set this value to 'YES' if you want Supersonic SDK to track and notify
 * about changes in network reachability.
 */
- (void)setShouldTrackReachability:(BOOL)flag;

- (void)setMediationSegment:(NSString *)segment;

/*-----------------------------------------------*/
// Rewarded Video
/*-----------------------------------------------*/

/*!
 * @discussion Initialize the Rewarded Video Ad Unit.
 *              You can call this function only once per app session, (We recommend to init the Rewarded Video Ad Unit when the application is being loaded for the first time).
 * @param appKey is the unique ID of your Application in your Supersonic account.
 * @param withUserId is the unique ID of your end user. We support NSString from 1 to 64 characters. Common practice is to use the Apple Advertising ID (IDFA).
 */
- (void)initRVWithAppKey:(NSString *)appKey withUserId:(NSString *)userId;

/*!
 * @discussion Set the delegate which the Supersonic SDK will call, to inform the application of Rewarded Video Ad Availability. Make sure to set your delegate before calling initRVWithAppKey.
 * @param rvDelegete is an instance which implements the SupersonicRVDelegate protocol.
 */
- (void)setRVDelegate:(id<SupersonicRVDelegate>)rvDelegate;

/*!
 * @discussion Once an Ad Network has an available video you can show this video Ad by using the below function.
 */
- (void)showRV;

/*!
 * @discussion  Show Rewarded Video with a specific Placement ID.
 *
 *              You can present video ads to your users from several Placements depending on what you want to Reward them with.
 *              Once an Ad Network has an available video, you can use the below function to define the exact Placement you want to show an ad from.
 *              The Reward settings of this Placement will be pulled from the Supersonic server:
 */
- (void)showRVWithPlacementName:(nullable NSString *)placementName;


- (void)showRVWithViewController:(nullable UIViewController*)viewController;

- (void)showRVWithViewController:(nullable UIViewController *)viewController
                   placementName:(nullable NSString *)placementName;



/*!
 * @discussion  Check for Rewarded Video Ad availability directly.
 *
 *              There are two ways to get the RewardedVideo Ad availability status.
 *              We recommend to set the RewardedVideo delegate before calling initRVWithAppKey. You will be notified with the delegate function (supersonicRVAdAvailabilityChanged:
 *              (BOOL)hasAvailableAds;) upon ad availability change.
 *              Another way is to ask for Ad availbility directly by using the below function.
 * @return YES when Rewarded Video Ad is available, else NO.
 */
- (BOOL)isAdAvailable;

/*!
 * @discussion To ask for RewardedVideo capping directly, you can use the below function.
 * @return YES for capped RewardedVideo, else NO.
 */
- (BOOL)isRewardedVideoPlacementCapped:(NSString *)placementName;

/*!
 * @discussion To get details about the specific Reward associated with each Ad Placement, you can use the below function.
 * @param placementName is the name of the placement in your Supersonic account.
 * @return SupersonicPlacementInfo instance with three properties: placementName, rewardName and rewardAmount (These properties are configured per placement in your Supersonic account).
 */
- (SupersonicPlacementInfo*)getRVPlacementInfo:(NSString *)placementName;

/*-----------------------------------------------*/
// Interstitial
/*-----------------------------------------------*/


/*!
 * @discussion Initialize the Interstitial Ad Unit.
 *
 *              You can call this function only once per app session, (We recommend to init the Interstitial Ad Unit when the application is being loaded for the first time).
 * @param appKey is the unique ID of your Application in your Supersonic account.
 * @param withUserId is the unique ID of your end user. We support NSString from 1 to 64 characters. Common practice is to use the Apple Advertising ID (IDFA).
 */
- (void)initISWithAppKey:(NSString *)appKey
              withUserId:(NSString *)userId;

/*!
 * @discussion Set the delegate which the Supersonic SDK will call to inform the application of Interstitial Ad Availability. Make sure to set your delegate before calling initISWithAppKey.
 * @param isDelegate is an instance which implements SupersonicISDelegate protocol.
 */
- (void)setISDelegate:(id<SupersonicISDelegate>)isDelegate;

- (void)loadIS;

/*!
 * @discussion  Once you have received the supersonicISAdAvailable callback with a YES value you are ready to show the Interstitial to your users. 
 *              You can do so by using the below function.
 *
 *              You will receive notification when the ad is loaded and ready to be shown with the supersonicISAdAvailable:(BOOL)available delegate.
 */
- (void)showISWithViewController:(UIViewController *)viewController;

- (void)showISWithViewController:(UIViewController *)viewController
                   placementName:(nullable NSString *)placementName;

/*!
 * @discussion To ask for Interstitial availbility directly, you can use the below function.
 * @return YES for available Interstitial, else NO.
 */
- (BOOL)isInterstitialAvailable;

/*!
 * @discussion To ask for Interstitial capping directly, you can use the below function.
 * @return YES for capped Interstitial, else NO.
 */
- (BOOL)isInterstitialPlacementCapped:(NSString *)placementName;


/*-----------------------------------------------*/
// Offerwall
/*-----------------------------------------------*/

/*!
 * @discussion Initialize the Offerwall Ad Unit.
 *
 *              You can call this function only once per app session, (We recommend to init the Offerwall Ad Unit when the application is being loaded for the first time).
 * @param appKey is the unique ID of your Application in your Supersonic account.
 * @param withUserId is the unique ID of your end user. We support NSString from 1 to 64 characters. Common practice is to use the Apple Advertising ID (IDFA).
 */
- (void)initOWWithAppKey:(NSString *)appKey
              withUserId:(NSString *)userId;

/*!
 * @discussion Set the delegate which the Supersonic SDK will call to inform the application of Offerwall Availability. Make sure to set your delegate before calling initOWWithAppKey.
 * @param owDelegate is an instance which implements SupersonicOWDelegate protocol.
 */
- (void)setOWDelegate:(id<SupersonicOWDelegate>)owDelegate;

/*!
 * @discussion Once you receive the supersonicOWInitSuccess delegate you are ready to show the Offerwall to your user.
 *             You do so by using the below function.
 */
- (void)showOW;

- (void)showOWWithViewController:(nullable UIViewController *)viewController;

- (void)showOWWithPlacement:(NSString *)placementName;

- (void)showOWWithViewController:(nullable UIViewController *)viewController
                       placement:(NSString *)placementName;

/*!
 * @discussion When using client-side callbacks, at any point during the user engagement with the app, you can receive the users total credits and any new credits the user has earned.
 *              you do so by using the below function.
 */
- (void)getOWCredits;


/*!
 * @discussion To ask for offerwal availbility directly, you can use the below function.
 * @return YES for available Offerwall, else NO.
 */
- (BOOL)isOWAvailable;

/*-----------------------------------------------*/
// Logging
/*-----------------------------------------------*/

- (void)setLogDelegate:(id<SupersonicLogDelegate>) logDelegate;

- (NSString *)getAdvertiserId;

@end

NS_ASSUME_NONNULL_END

#endif