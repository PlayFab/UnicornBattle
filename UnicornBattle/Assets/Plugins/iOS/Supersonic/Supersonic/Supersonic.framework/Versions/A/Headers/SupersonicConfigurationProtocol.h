//
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#ifndef SUPERSONIC_CONFIGURATION_PROTOCOL_H
#define SUPERSONIC_CONFIGURATION_PROTOCOL_H

#import <Foundation/Foundation.h>

@protocol SupersonicConfigurationProtocol <NSObject>

+ (instancetype)getConfiguration;

@end

#endif