//
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#ifndef SUPERSONIC_OW_DELEGATE_H
#define SUPERSONIC_OW_DELEGATE_H

#import <Foundation/Foundation.h>

@protocol SupersonicOWDelegate <NSObject>

@required


/*!
 * @discussion Indicates that initiating the Offerwall was completed successfully.
 */
- (void)supersonicOWInitSuccess;

/*!
 * @discussion Called each time the Offerwall successfully loads for the user.
 */
- (void)supersonicOWShowSuccess;

/*!
 * @discussion Will be triggered in case Offerwall was not able to initiate.
 *
 *              for example: when there is no internet connection, servers are unavailable etc.
 *              error - will contain the failure code and description
 */
- (void)supersonicOWInitFailedWithError:(NSError *)error;

/*!
 * @discussion Called each time the Offerwall fails to show.
 *              
 *              error - will contain the failure code and description
 */
- (void)supersonicOWShowFailedWithError:(NSError *)error;


/*!
 * @discussion Called when the user closes the Offerwall.
 */
- (void)supersonicOWAdClosed;

/*!
 * @discussion Called each time the user completes an offer.
 * 
 *          creditInfo - A dictionary with the following key-value pairs:
 *          @"credits" - (integer) The number of credits the user has Earned since the
 *          last supersonicOWDidReceiveCredit event that returned 'YES'. Note that the
 *          credits may represent multiple completions (see return parameter).
 *          @"totalCredits" - (integer) The total number of credits ever earned by the user.
 *          @"totalCreditsFlag" - (boolean) In some cases, we won’t be able to provide
 *          the exact amount of credits since the last event(specifically if the user
 *          clears the app’s data). In this case the ‘credits’ will be equal to the
 *          @"totalCredits", and this flag will be @(YES).
 * @return The publisher should return a boolean stating if he handled this
 *      call (notified the user for example). if the return value is 'NO' the
 *      'credits' value will be added to the next call.
 */
- (BOOL)supersonicOWDidReceiveCredit:(NSDictionary *)creditInfo;


/*!
 * @discussion Called when the method ‘-getOWCredits’ failed to retrieve the users credit balance info.
 * 
 *              error - the error object with the failure info
 */
- (void)supersonicOWFailGettingCreditWithError:(NSError *)error;

@end

#endif