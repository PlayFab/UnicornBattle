//
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#ifndef SUPERSONIC_CONFIGURATION_H
#define SUPERSONIC_CONFIGURATION_H

#import <Foundation/Foundation.h>
#import "SupersonicConfigurationProtocol.h"
#import "SupersonicGender.h"

@interface SupersonicConfiguration : NSObject<SupersonicConfigurationProtocol>

@property (nonatomic, strong)   NSString            *userId;
@property (nonatomic, strong)   NSString            *appKey;
@property (nonatomic, strong)   NSString            *segment;
@property (nonatomic, strong)   NSDictionary        *rewardedVideoCustomParameters;
@property (nonatomic, strong)   NSDictionary        *offerwallCustomParameters;
@property (nonatomic, strong)   NSString            *version;
@property (nonatomic, strong)   NSNumber            *adapterTimeOutInSeconds;
@property (nonatomic, strong)   NSNumber            *maxNumOfAdaptersToLoadOnStart;
@property (nonatomic, strong)   NSString            *plugin;
@property (nonatomic, strong)   NSString            *pluginVersion;
@property (nonatomic, strong)   NSString            *pluginFrameworkVersion;
@property (nonatomic, strong)   NSNumber            *maxVideosPerIteration;
@property (nonatomic, assign)   int                 userAge;
@property (nonatomic, assign)   SupersonicGender    userGender;
@property (nonatomic, assign)   BOOL                trackReachability;

+ (SupersonicConfiguration *)getConfiguration;

@end

#endif