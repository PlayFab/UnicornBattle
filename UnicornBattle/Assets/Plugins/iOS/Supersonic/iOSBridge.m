//
//  iOSBridge.m
//  iOSBridge
//
//  Created by Supersonic.
//  Copyright (c) 2015 Supersonic. All rights reserved.
//

#import "iOSBridge.h"
#import "Supersonic/SupersonicIntegrationHelper.h"
#import <UIKit/UIKit.h>


#ifdef __cplusplus
extern "C" {
#endif

extern void UnitySendMessage( const char * className, const char * methodName, const char * param );

#ifdef __cplusplus
}
#endif

@implementation iOSBridge

char* const SUPERSONIC_EVENTS = "SupersonicEvents";

+ (iOSBridge *)start{
    static iOSBridge *instance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken,
                  ^{
                      instance = [[iOSBridge alloc] init];
                  });
    
    return instance;
}

- (instancetype)init{
    self = [super init];
    if(self){
        [Supersonic sharedInstance];
        
    }
    
    return self;
}

- (NSString *)getJsonFromDic:(NSDictionary *)dict{
    
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict
                                                       options:0
                                                         error:&error];
    if (! jsonData) {
        NSLog(@"Got an error: %@", error);
        return @"";
    } else {
        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return jsonString;
    }
}

-(void)setPluginData:(NSString*)pluginType :(NSString*) pluginVersion :(NSString*) pluginFrameworkVersion{
    [SupersonicConfiguration getConfiguration].plugin = pluginType;
    [SupersonicConfiguration getConfiguration].pluginVersion = pluginVersion;
}

-(const char *)getAdvertiserId{
    char* res = NULL;
    NSString *advertiserId = [Supersonic sharedInstance].getAdvertiserId;

    const char * tmp = advertiserId.UTF8String;
    long len = strlen(tmp);
    res = malloc(len);
    memcpy(res, tmp, len + 1);
    
    return res;
}

-(void)validateIntegration{
    [SupersonicIntegrationHelper validateIntegration];
}

-(void)shouldTrackNetworkState:(BOOL)flag{
    [[Supersonic sharedInstance] setShouldTrackReachability:flag];
}

-(void)reportAppStarted{
    [SupersonicEventsReporting reportAppStarted];
}

-(void)setAge:(int)age{
    [[Supersonic sharedInstance] setAge:age];
}

-(void)setGender:(NSString *)gender{
    if([gender caseInsensitiveCompare:@"male"] == NSOrderedSame)
        [[Supersonic sharedInstance] setGender:SUPERSONIC_USER_MALE];
    
    else if([gender caseInsensitiveCompare:@"female"] == NSOrderedSame)
        [[Supersonic sharedInstance] setGender:SUPERSONIC_USER_FEMALE];
    
    else if([gender caseInsensitiveCompare:@"unknown"] == NSOrderedSame)
        [[Supersonic sharedInstance] setGender:SUPERSONIC_USER_UNKNOWN];
}

-(void)setMediationSegment:(NSString *)segment{
    [[Supersonic sharedInstance] setMediationSegment:segment];
}


#pragma mark RewardedVideo API

- (void)initRewardedVideoWithAppKey:(NSString *)appKey withUserId:(NSString*)userId{
    [[Supersonic sharedInstance] setRVDelegate:self];
    [[Supersonic sharedInstance] initRVWithAppKey:appKey withUserId:userId];
}

-(void)showRewardedVideo{
    [[Supersonic sharedInstance] showRV];
}

-(void)showRewardedVideoWithPlacementName:(NSString*) placementName{
    [[Supersonic sharedInstance] showRVWithPlacementName:placementName];
}

-(const char *) getPlacementInfo:(NSString *)placementName{
    char* res = NULL;
    
    if (placementName){
        SupersonicPlacementInfo * spi = [[Supersonic sharedInstance] getRVPlacementInfo:placementName];
        if(spi){
            NSMutableDictionary *dict = [[NSMutableDictionary alloc]init];
            [dict setObject:[spi placementName] forKey:@"placement_name"];
            [dict setObject:[spi rewardAmount] forKey:@"reward_amount"];
            [dict setObject:[spi rewardName] forKey:@"reward_name"];
            
            const char * tmp = [self getJsonFromDic:dict].UTF8String;
            long len = strlen(tmp);
            res = malloc(len);
            memcpy(res, tmp, len + 1);
        }
    }
    
    return res;
}

-(BOOL)isRewardedVideoAvailable{
    return [[Supersonic sharedInstance] isAdAvailable];
}

-(BOOL)isRewardedVideoPlacementCapped:(NSString *)placementName{
    return [[Supersonic sharedInstance] isRewardedVideoPlacementCapped:placementName];
}

#pragma mark RewardedVideoDelegate

-(void)supersonicRVInitSuccess {
    UnitySendMessage(SUPERSONIC_EVENTS, "onRewardedVideoInitSuccess", "");
}

- (void)supersonicRVInitFailedWithError:(NSError *)error{
    UnitySendMessage(SUPERSONIC_EVENTS, "onRewardedVideoInitFail", [self parseErrorToEvent:error]);
}

-(void)supersonicRVAdAvailabilityChanged:(BOOL)hasAvailableAds{
    UnitySendMessage(SUPERSONIC_EVENTS, "onVideoAvailabilityChanged", (hasAvailableAds) ? "true" : "false");
}

-(void)supersonicRVAdOpened{
    UnitySendMessage(SUPERSONIC_EVENTS, "onRewardedVideoAdOpened", "");
}

-(void)supersonicRVAdStarted{
    UnitySendMessage(SUPERSONIC_EVENTS, "onVideoStart", "");
}

-(void)supersonicRVAdEnded{
    UnitySendMessage(SUPERSONIC_EVENTS, "onVideoEnd", "");
}

-(void)supersonicRVAdClosed{
    UnitySendMessage(SUPERSONIC_EVENTS, "onRewardedVideoAdClosed", "");
}

-(void)supersonicRVAdRewarded:(SupersonicPlacementInfo*) placementInfo{
    NSMutableDictionary *dict = [[NSMutableDictionary alloc]init];
    [dict setObject: placementInfo.rewardAmount forKey:@"placement_reward_amount"];
    [dict setObject: placementInfo.rewardName forKey:@"placement_reward_name"];
    [dict setObject: placementInfo.placementName forKey:@"placement_name"];
    const char* res = [self getJsonFromDic:dict].UTF8String;
    UnitySendMessage(SUPERSONIC_EVENTS, "onRewardedVideoAdRewarded",res);
}

- (void)supersonicRVAdFailedWithError:(NSError *)error{
    UnitySendMessage(SUPERSONIC_EVENTS, "onRewardedVideoShowFail", [self parseErrorToEvent:error]);
}



#pragma mark Interstitial API

-(void)initInterstitialWithAppKey:(NSString *)appKey withUserId:(NSString*)userId{
    [[Supersonic sharedInstance] setISDelegate:self];
    [[Supersonic sharedInstance] initISWithAppKey:appKey withUserId:userId];
}

-(void)loadInterstitial{
    [[Supersonic sharedInstance] loadIS];
}

-(void)showInterstitial{
    UIViewController *viewController = [[[UIApplication sharedApplication] keyWindow] rootViewController];
    [[Supersonic sharedInstance] showISWithViewController:viewController];
}

-(void)showInterstitialWithPlacementName:(NSString*) placementName{
    UIViewController *viewController = [[[UIApplication sharedApplication] keyWindow] rootViewController];
    [[Supersonic sharedInstance] showISWithViewController:viewController placementName:placementName];
}

-(BOOL)isInterstitialReady{
    return [[Supersonic sharedInstance] isInterstitialAvailable];
}

-(BOOL)isInterstitialPlacementCapped:(NSString *)placementName{
    return [[Supersonic sharedInstance] isInterstitialPlacementCapped:placementName];
}

#pragma mark InterstitialDelegate

-(void)supersonicISInitSuccess{
    UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialInitSuccess", "");
}

-(void)supersonicISInitFailedWithError:(NSError *)error{
    if (error)
        UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialInitFailed", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialInitFailed", "");
}

-(void)supersonicISReady{
    UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialReady", "");
}

-(void)supersonicISFailed{
    UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialLoadFailed", "");
}

-(void)supersonicISShowSuccess{
    UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialShowSuccess", "");
}

-(void)supersonicISShowFailWithError:(NSError *)error{
    if (error)
        UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialShowFailed", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialShowFailed","");
}

-(void)supersonicISAdClicked{
    UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialClick", "");
}

-(void)supersonicISAdOpened{
    UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialOpen", "");
}

-(void)supersonicISAdClosed{
    UnitySendMessage(SUPERSONIC_EVENTS, "onInterstitialClose", "");
}



//public boolean onOfferwallAdCredited(int credits, int totalCredits, boolean totalCreditsFlag);
//public void onGetOfferwallCreditsFail(SupersonicError supersonicError);


#pragma mark Offerwall API

-(void)initOfferwallWithAppKey:(NSString *)appKey withUserId:(NSString*)userId{
    [[Supersonic sharedInstance] setOWDelegate:self];
    [[Supersonic sharedInstance] initOWWithAppKey:appKey withUserId:userId];
}

-(void)showOfferwall{
    [[Supersonic sharedInstance] showOW];
}

-(void)showOfferwallWithPlacementName:(NSString*) placementName{
    [[Supersonic sharedInstance] showOWWithPlacement:placementName];
}

-(void)getOfferwallCredits{
    [[Supersonic sharedInstance] getOWCredits];
}

-(BOOL)isOfferwallAvailable{
    return [[Supersonic sharedInstance] isOWAvailable];
}



#pragma mark OfferwallDelegate

-(void)supersonicOWInitSuccess{
    UnitySendMessage(SUPERSONIC_EVENTS, "onOfferwallInitSuccess", "");
}

-(void)supersonicOWShowSuccess{
    UnitySendMessage(SUPERSONIC_EVENTS, "onOfferwallOpened", "");
}

-(void)supersonicOWInitFailedWithError:(NSError *)error{
    if (error)
        UnitySendMessage(SUPERSONIC_EVENTS, "onOfferwallInitFail", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(SUPERSONIC_EVENTS, "onOfferwallInitFail", "");
}

-(void)supersonicOWShowFailedWithError:(NSError *)error{
    if (error)
        UnitySendMessage(SUPERSONIC_EVENTS, "onOfferwallShowFail", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(SUPERSONIC_EVENTS, "onOfferwallShowFail", "");
}

-(void)supersonicOWAdClosed{
    UnitySendMessage(SUPERSONIC_EVENTS, "onOfferwallClosed", "");
}

- (BOOL)supersonicOWDidReceiveCredit:(NSDictionary *)creditInfo{
    if(creditInfo)
        UnitySendMessage(SUPERSONIC_EVENTS, "onOfferwallAdCredited", [self getJsonFromDic:creditInfo].UTF8String);
    
    return YES;
}

-(void)supersonicOWFailGettingCreditWithError:(NSError *)error{
    if (error)
        UnitySendMessage(SUPERSONIC_EVENTS, "onGetOfferwallCreditsFail", [self parseErrorToEvent:error]);
    else
        UnitySendMessage(SUPERSONIC_EVENTS, "onGetOfferwallCreditsFail", "");
}

-(const char*)parseErrorToEvent:(NSError *)error{
    NSMutableDictionary *dict = [[NSMutableDictionary alloc]init];
    if (error){
        NSString* codeStr =  [NSString stringWithFormat:@"%ld", [error code]];
        [dict setObject:[error localizedDescription] forKey:@"error_description"];
        [dict setObject:codeStr forKey:@"error_code"];
    }
    const char* res = [self getJsonFromDic:dict].UTF8String;
    
    return res;
}


#pragma mark C Section


#ifdef __cplusplus
extern "C" {
#endif

#define ParseNSStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

    void CFStart(){
        [iOSBridge start];
    }
    
    void CFReportAppStarted(){
        [[iOSBridge start] reportAppStarted];
    }

    void CFSetAge(int age){
        [[iOSBridge start] setAge:age];
    }
    
    void CFSetGender(const char* gender){
        [[iOSBridge start] setGender:ParseNSStringParam(gender)];
    }
    
    void CFSetMediationSegment(const char* segment){
        [[iOSBridge start] setMediationSegment:ParseNSStringParam(segment)];
    }
    
    void CFSetPluginData(const char* pluginType, const char* pluginVersion, const char* pluginFrameworkVersion){
        [[iOSBridge start] setPluginData:ParseNSStringParam(pluginType) :ParseNSStringParam(pluginVersion) :ParseNSStringParam(pluginFrameworkVersion)];
    }
    
    void CFInitRewardedVideo(const char* appKey, const char* userId){
        [[iOSBridge start] initRewardedVideoWithAppKey:ParseNSStringParam(appKey) withUserId:ParseNSStringParam(userId)];

    }

    void CFShowRewardedVideo(){
        [[iOSBridge start] showRewardedVideo];
    }
    
    void CFShowRewardedVideoWithPlacementName(char* placementName){
        [[iOSBridge start] showRewardedVideoWithPlacementName:ParseNSStringParam(placementName)];
    }
    
    const char * CFGetPlacementInfo(char* placementName){
        return [[iOSBridge start] getPlacementInfo:ParseNSStringParam(placementName)];
    }

    bool CFIsRewardedVideoAvailable(){
        return [[iOSBridge start] isRewardedVideoAvailable];
    }
    
    bool CFIsRewardedVideoPlacementCapped(char* placementName){
        return [[iOSBridge start] isRewardedVideoPlacementCapped:ParseNSStringParam(placementName)];
    }

    void CFInitInterstitial(const char* appKey,const char* userId){
        [[iOSBridge start] initInterstitialWithAppKey:ParseNSStringParam(appKey) withUserId:ParseNSStringParam(userId)];
    }

    void CFLoadInterstitial(){
        [[iOSBridge start] loadInterstitial];
    }
    
    void CFShowInterstitial(){
        [[iOSBridge start] showInterstitial];
    }
    
    void CFShowInterstitialWithPlacementName(char* placementName){
        [[iOSBridge start] showInterstitialWithPlacementName:ParseNSStringParam(placementName)];
    }
    
    bool CFIsInterstitialReady(){
        return [[iOSBridge start] isInterstitialReady];
    }
    
    bool CFIsInterstitialPlacementCapped(char* placementName){
        return [[iOSBridge start] isInterstitialPlacementCapped:ParseNSStringParam(placementName)];
    }

    void CFInitOfferwall(const char* appKey,const char* userId){
        [[iOSBridge start] initOfferwallWithAppKey:ParseNSStringParam(appKey) withUserId:ParseNSStringParam(userId)];
    }

    void CFShowOfferwall(){
        [[iOSBridge start] showOfferwall];
    }
    
    void CFShowOfferwallWithPlacementName(char* placementName){
        [[iOSBridge start] showOfferwallWithPlacementName:ParseNSStringParam(placementName)];
    }

    void CFGetOfferwallCredits(){
        [[iOSBridge start] getOfferwallCredits];
    }
    
    bool CFIsOfferwallAvailable(){
        return [[iOSBridge start] isOfferwallAvailable];
    }
    
    const char* CFGetAdvertiserId(){
        return [[iOSBridge start] getAdvertiserId];
    }

    void CFValidateIntegration(){
        [[iOSBridge start] validateIntegration];
    }
    
    void CFShouldTrackNetworkState(bool flag){
        [[iOSBridge start] shouldTrackNetworkState:flag];
    }

#ifdef __cplusplus
}
#endif

@end