#import "MapsyncWrapper.h"
#import <MapsyncLib/MapsyncLib-Swift.h>
#import <SceneKit/SceneKit.h>
#import <ARKit/ARKit.h>

@interface MapsyncWrapper()

@property (nonatomic, strong) MapSession *mapSession;
@property (nonatomic, strong) ARSession *session;
@property (nonatomic) Mode mode;
@property (nonatomic, strong) NSString *userId;
@property (nonatomic, strong) NSString *mapId;

@end

@implementation MapsyncWrapper

static MapsyncWrapper *instance = nil;
static NSString* unityCallbackGameObject = @"";
static NSString* assetLoadedCallback = @"";
static NSString* statusUpdatedCallback = @"";
static NSString* storePlacementCallback = @"";

+ (void)setUnityCallbackGameObject:(NSString *)objectName {
    unityCallbackGameObject = objectName;
}

+ (void)setStatusUpdatedCallbackFunction:(NSString *)functionName {
    statusUpdatedCallback = functionName;
}

+ (void)setAssetLoadedCallbackFunction:(NSString *)functionName {
    assetLoadedCallback = functionName;
}

+ (void)setStorePlacementCallbackFunction:(NSString*)functionName {
    storePlacementCallback = functionName;
}

+ (instancetype)sharedInstanceWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey;
{
    dispatch_async(dispatch_get_main_queue(), ^{
        instance = [[self alloc] initWithARSession:session mapMode:mode mapId:mapId userId:userId developerKey:developerKey];
    });
    
    return instance;
}

+ (instancetype)sharedInstance
{
    if (instance == nil)
    {
        NSLog(@"error: shared called before setup");
    }
    
    return instance;
}

- (instancetype)initWithARSession:(ARSession *)session mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey;
{
    return [self initWithARSession:session mapMode:ModeMapping mapId:mapId userId:userId developerKey:developerKey];
}

- (instancetype)initWithARSession:(ARSession *)session mapMode:(Mode)mode mapId: (NSString*) mapId userId:(NSString*) userId developerKey: (NSString*) developerKey;
{
    self = [super init];
    if (self)
    {
        self.session = session;
        self.mode = mode;
        self.mapId = mapId;
        self.userId = userId;
    
        self.mapSession = [[MapSession alloc] initWithArSession:self.session mapMode:mode userID:self.userId mapID:self.mapId developerKey:developerKey assetsFoundCallback:^(NSArray<MapAsset *> * assets) {
            
            NSMutableArray *assetData = [[NSMutableArray alloc] init];
            for (MapAsset *asset in assets)
            {
                NSDictionary* dict = [NSMutableDictionary dictionary];
                [dict setValue:asset.assetID forKey:@"AssetId"];
                [dict setValue:@(asset.position.x) forKey:@"X"];
                [dict setValue:@(asset.position.y) forKey:@"Y"];
                [dict setValue:@(asset.position.z) forKey:@"Z"];
                [dict setValue:@(asset.orientation) forKey:@"Orientation"];
                
                [assetData addObject:dict];
                
            }
            
            NSDictionary* assetsDict = [NSMutableDictionary dictionary];
            [assetsDict setValue:assetData forKey:@"Assets"];
            
            NSError* error;
            
            NSData* jsonData = [NSJSONSerialization dataWithJSONObject:assetsDict
                                                               options:NSJSONWritingPrettyPrinted error:&error];
            
            NSString* json = [[NSString alloc] initWithData:jsonData encoding:NSASCIIStringEncoding];
            
            NSLog(@"%@", json);
            UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [assetLoadedCallback cStringUsingEncoding:NSASCIIStringEncoding], [json cStringUsingEncoding:NSASCIIStringEncoding]);
            
        } statusCallback:^(enum MapStatus mapStatus) {
            NSLog(@"mapStatus: %li", mapStatus);
            UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [statusUpdatedCallback cStringUsingEncoding:NSASCIIStringEncoding], [[NSString stringWithFormat:@"%ld", (long)mapStatus] cStringUsingEncoding:NSASCIIStringEncoding]);

        }];
    }
    
     return self;
}

- (void)uploadAssets:(NSArray*)array {
    if (self.mode != ModeMapping)
    {
        NSLog(@"Error, not in mapping mode");
        return;
    }
    
    BOOL result = [self.mapSession storePlacementWithAssets:array callback:^(BOOL stored)
    {
       NSLog(@"model stored: %i", stored);
       UnitySendMessage([unityCallbackGameObject cStringUsingEncoding:NSASCIIStringEncoding], [storePlacementCallback cStringUsingEncoding:NSASCIIStringEncoding], [[NSString stringWithFormat:@"%d", stored] cStringUsingEncoding:NSASCIIStringEncoding]);
    }];
    
}

- (void) updateWithFrame:(ARFrame*)frame
{
    [self.mapSession updateWithFrame:frame];
}

@end