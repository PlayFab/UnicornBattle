//
//  Copyright (c) 2015 Supersonic. All rights reserved.
//


@interface SUSupersonicAdsConfiguration : NSObject

@property (nonatomic, assign)   NSNumber        *useClientSideCallbacks;
@property (nonatomic, strong)   NSString        *language;
@property (nonatomic, strong)   NSString        *minimumOfferCommission;
@property (nonatomic, strong)   NSDictionary    *controllerConfig;
@property (nonatomic, strong)   NSString        *itemName;
@property (nonatomic, strong)   NSString        *controllerUrl;
@property (strong)              NSNumber        *itemCount;
@property (strong)              NSNumber        *maxVideoLength;
@property (nonatomic, strong)   NSString        *privateKey;
@property (nonatomic)           BOOL            debugMode;
@property (nonatomic)           NSInteger       debugLevel;
//@property (nonatomic, strong)   NSNumber        *storeKitTimeout;

+ (SUSupersonicAdsConfiguration *)getConfiguration;

@end
