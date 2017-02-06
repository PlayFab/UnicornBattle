//
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#ifndef SUPERSONIC_RV_DELEGATE_H
#define SUPERSONIC_RV_DELEGATE_H

#import <Foundation/Foundation.h>

@class SupersonicPlacementInfo;

@protocol SupersonicRVDelegate <NSObject>

@required
/*!
 * @discussion Invoked when initialization of RewardedVideo ad unit has finished successfully.
 */
- (void)supersonicRVInitSuccess;

/*!
 * @discussion Invoked when RewardedVideo initialization process has failed. 
 *              
 *              NSError contains the reason for the failure.
 */
- (void)supersonicRVInitFailedWithError:(NSError *)error;

/*!
 * @discussion Invoked when there is a change in the ad availability status.
 *             
 *              hasAvailableAds - value will change to YES when rewarded videos are available.
 *              You can then show the video by calling showRV(). Value will change to NO when no videos are available.
 */
- (void)supersonicRVAdAvailabilityChanged:(BOOL)hasAvailableAds;

/*!
 * @discussion Invoked when the user completed the video and should be rewarded.
 *
 *              If using server-to-server callbacks you may ignore these events and wait for the callback from the Supersonic server.
 *              placementInfo - SupersonicPlacementInfo - an object contains the placement's reward name and amount
 */
- (void)supersonicRVAdRewarded:(SupersonicPlacementInfo*)placementInfo;

/*!
 * @discussion Invoked when an Ad failed to display.
 *              
 *          error - NSError which contains the reason for the failure.
 *          The error contains error.code and error.message.
 */
- (void)supersonicRVAdFailedWithError:(NSError *)error;

/*!
 * @discussion Invoked when the RewardedVideo ad view has opened.
 *
 */
- (void)supersonicRVAdOpened;

/*!
 * @discussion Invoked when the user is about to return to the application after closing the RewardedVideo ad.
 *
 */
- (void)supersonicRVAdClosed;

/**
 * Note: the events below are not available for all supported Rewarded Video
 * Ad Networks.
 * Check which events are available per Ad Network you choose to include in
 * your build.
 * We recommend only using events which register to ALL Ad Networks you
 * include in your build.
 */


/*!
 * @discussion Invoked when the video ad starts playing.
 *
 *             Available for: AdColony, Vungle, AppLovin, UnityAds
 */
- (void)supersonicRVAdStarted;

/*!
 * @discussion Invoked when the video ad finishes playing.
 *
 *             Available for: AdColony, Flurry, Vungle, AppLovin, UnityAds.
 */
- (void)supersonicRVAdEnded;

@end

#endif