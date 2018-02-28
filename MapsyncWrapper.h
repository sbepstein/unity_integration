#import <Foundation/Foundation.h>
#import <SceneKit/SceneKit.h>
#import <MapsyncLib/MapsyncLib-Swift.h>
#import <ARKit/ARKit.h>
@class MapSession;
@class ARSession;

typedef NS_ENUM(NSInteger, Mode)
{
    ModeMapping = 0,
    ModeLocalization = 1,
};

@interface MapsyncWrapper : NSObject

+ (instancetype)sharedInstanceWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey;

+ (instancetype)sharedInstance;

+ (void)setUnityCallbackGameObject:(NSString*)objectName;
+ (void)setAssetLoadedCallbackFunction:(NSString*)functionName;
+ (void)setStatusUpdatedCallbackFunction:(NSString*)functionName;
+ (void)setStorePlacementCallbackFunction:(NSString*)functionName;

- (instancetype)initWithARSession:(ARSession *)session mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey;

- (void)uploadAssets:(NSArray*)array;

- (void)updateWithFrame:(ARFrame*)frame;

@end

