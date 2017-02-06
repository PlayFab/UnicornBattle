//
//  Copyright (c) 2015 Supersonic. All rights reserved.
//
#import <Foundation/Foundation.h>

@interface SupersonicPlacementInfo : NSObject

- (instancetype)init NS_UNAVAILABLE;
- (instancetype)initWithPlacementName:(NSString *)placementName
                       withRewardName:(NSString *)name
                     withRewardAmount:(NSNumber*)amount NS_DESIGNATED_INITIALIZER;

@property (readonly) NSString *placementName;
@property (readonly) NSString *rewardName;
@property (readonly) NSNumber *rewardAmount;


@end
