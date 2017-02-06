//
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#ifndef SUPERSONIC_LOG_DELEGATE_H
#define SUPERSONIC_LOG_DELEGATE_H

#import <Foundation/Foundation.h>

typedef enum LogLevelValues
{
    SU_LOG_NONE = -1,
    SU_LOG_INTERNAL = 0,
    SU_LOG_INFO = 1,
    SU_LOG_WARNING = 2,
    SU_LOG_ERROR = 3,
    SU_LOG_CRITICAL = 4,
    
} LogLevel;

typedef enum LogTagValue
{
    TAG_API,
    TAG_DELEGATE,
    TAG_ADAPTER_API,
    TAG_ADAPTER_DELEGATE,
    TAG_NETWORK,
    TAG_NATIVE,
    TAG_INTERNAL,
    
} LogTag;

@protocol SupersonicLogDelegate <NSObject>

@required

- (void)sendLog:(NSString *)log level:(LogLevel)level tag:(LogTag)tag;

@end

#endif