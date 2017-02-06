//
//  AppLovinUnityConfig.m
//  iOSBridge
//
//  Created by Ori  on 5/13/15.
//  Copyright (c) 2015 yossi mozgerashvily. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <Supersonic/SUSupersonicAdsConfiguration.h>
#import <Supersonic/SupersonicConfiguration.h>

@interface SupersonicAdsUnityConfig :NSObject
//Supersonic
-(void) setSupersonicUseClentSideCallbacks:(bool) useClientSideCallbacks;
-(void) setSupersonicLanguage:(NSString*) language;
-(void) setSupersonicPrivateKey:(NSString*) privateKey;
-(void) setSupersonicMaxVideoLength:(int) length;
-(void) setSupersonicItemName:(NSString*) itemName;
-(void) setSupersonicItemCount:(int) itemCount;
-(void) setSupersonicRewardedVideoCustomParams:(NSString*) rvParams;
-(void) setSupersonicOfferwallCustomParams:(NSString*) owParams;

@end

@implementation SupersonicAdsUnityConfig
//Supersonic
-(void) setSupersonicUseClentSideCallbacks:(bool)useClientSideCallbacks{
    NSNumber* ucsc = @0;
    if (useClientSideCallbacks)
        ucsc = @1;
    
    [SUSupersonicAdsConfiguration getConfiguration].useClientSideCallbacks = ucsc;
}
-(void) setSupersonicLanguage:(NSString*) language{
    [SUSupersonicAdsConfiguration getConfiguration].language = language;
}
-(void) setSupersonicPrivateKey:(NSString*) privateKey{
    [SUSupersonicAdsConfiguration getConfiguration].privateKey =privateKey;
}
-(void) setSupersonicMaxVideoLength:(int) length{
    [SUSupersonicAdsConfiguration getConfiguration].maxVideoLength =[NSNumber numberWithInt:length];
}
-(void) setSupersonicItemName:(NSString*) itemName{
    [SUSupersonicAdsConfiguration getConfiguration].itemName =itemName;
}
-(void) setSupersonicItemCount:(int) itemCount{
    [SUSupersonicAdsConfiguration getConfiguration].itemCount = [NSNumber numberWithInt:itemCount];
}
-(void) setSupersonicRewardedVideoCustomParams:(NSString*) rvParams{
    NSError *jsonError;
    NSData *objectData = [rvParams dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *json = [NSJSONSerialization JSONObjectWithData:objectData options:NSJSONReadingMutableContainers error:&jsonError];
    if (!jsonError)
        [SupersonicConfiguration getConfiguration].rewardedVideoCustomParameters = json;
}
-(void) setSupersonicOfferwallCustomParams:(NSString*) owParams{
    NSError *jsonError;
    NSData *objectData = [owParams dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary *json = [NSJSONSerialization JSONObjectWithData:objectData options:NSJSONReadingMutableContainers error:&jsonError];
    if (!jsonError)
        [SupersonicConfiguration getConfiguration].offerwallCustomParameters = json;

}



#ifdef __cplusplus
extern "C" {
#endif
    
#define ParseNSStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]
    
    
    //Supersonic
    void CFsetSupersonicUseClentSideCallbacks(bool useClientSideCallbacks){
        [[[SupersonicAdsUnityConfig alloc]init] setSupersonicUseClentSideCallbacks:useClientSideCallbacks];
    }
    void CFsetSupersonicLanguage(const char* language){
        [[[SupersonicAdsUnityConfig alloc]init] setSupersonicLanguage:ParseNSStringParam(language)];
    }
    void CFsetSupersonicPrivateKey(const char* privateKey){
        [[[SupersonicAdsUnityConfig alloc]init] setSupersonicPrivateKey:ParseNSStringParam(privateKey)];
        
    }
    void CFsetSupersonicMaxVideoLength(int length){
        [[[SupersonicAdsUnityConfig alloc]init] setSupersonicMaxVideoLength:length];
    }
    void CFsetSupersonicItemName(const char* itemName){
        [[[SupersonicAdsUnityConfig alloc]init] setSupersonicItemName:ParseNSStringParam(itemName)];
    }
    void CFsetSupersonicItemCount(int itemCount){
        [[[SupersonicAdsUnityConfig alloc]init] setSupersonicItemCount: itemCount];
    }
    void CFsetSupersonicRewardedVideoCustomParams(const char* rvParams){
        [[[SupersonicAdsUnityConfig alloc]init] setSupersonicRewardedVideoCustomParams:ParseNSStringParam(rvParams)];
    }
    void CFsetSupersonicOfferwallCustomParams(const char* owParam){
        [[[SupersonicAdsUnityConfig alloc]init] setSupersonicOfferwallCustomParams:ParseNSStringParam(owParam)];
    }
    
    
#ifdef __cplusplus
}
#endif

@end
