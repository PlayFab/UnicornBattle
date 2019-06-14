#import <Foundation/Foundation.h>
#import <AppCenterCrashes/AppCenterCrashes.h>

@interface MSException : NSObject
@property(nonatomic, copy) NSString *type;
@property(nonatomic, copy) NSString *message;
@property(nonatomic, copy) NSString *stackTrace;
@property(nonatomic) NSArray<MSException *> *innerExceptions;
@property(nonatomic, copy) NSString *wrapperSdkName;
@end

@interface MSCrashes ()
+ (void)trackModelException:(MSException *)exception;
+ (void)trackModelException:(MSException *)exception withProperties:(nullable NSDictionary<NSString *, NSString *> *)properties;
@end
