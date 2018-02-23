# Getting Started With Mapsync and Unity

#### 1:
Create a new Unity Project.

#### 2:
Download and Import the `Unity ARKit Plugin` from the Asset Store.

#### 3:
Download the folder [MapsyncLibPlugin](https://github.com/jidomaps/unity_integration/tree/master/MapsyncLibPlugin) and drag it into the project window.

#### 4:
Replace `UnityARKitPlugin/Plugins/iOS/UnityARKit/Nativeinterface/UnityARSessionNativeInterface.cs` with [UnityARSessionNativeInterface.cs](https://github.com/jidomaps/unity_integration/blob/master/UnityARSessionNativeInterface.cs).

The new `UnityARSessionNativeInterface.cs` adds the following method: 
```
public IntPtr GetSession() 
{
    #if !UNITY_EDITOR
        return m_NativeARSession;
    #else
        return IntPtr.Zero;
    #endif
}
```

#### 5:
Replace `ARSessionNative.mm` (also in `UnityARKitPlugin/Plugins/iOS/UnityARKit/Nativeinterface`) with [ARSessionNative.mm](https://github.com/jidomaps/unity_integration/blob/master/ARSessionNative.mm).

The new `ARSession.mm` adds:

 - `#import "MapsyncWrapper.h"` 
 - After line 669 add the line:
  `[[MapsyncWrapper sharedInstance] updateWithFrame:frame];`
 Line 669 is the last line in the function: 
 `- (void)session:(ARSession *)session didUpdateFrame:(ARFrame *)frame`
  - The following functions at the bottom of the file:

```

extern "C" void* _CreateMapsyncSession(void* nativeSession, char* mapId, char* userId, char* developerKey, BOOL isMappingMode)
{
    Mode mode = isMappingMode ? ModeMapping : ModeLocalization;
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    
    [MapsyncWrapper sharedInstanceWithARSession:session->_session mapMode:mode mapId:[NSString stringWithUTF8String:mapId] userId:[NSString stringWithUTF8String:userId] developerKey:[NSString stringWithUTF8String:developerKey]];
    
    return (__bridge_retained void*) [MapsyncWrapper sharedInstance];
}

extern "C" void _SaveAssets(char* json)
{
    NSError *error = nil;
    
    NSData* data = [[NSString stringWithUTF8String:json] dataUsingEncoding:NSUTF8StringEncoding];
    id objects = [NSJSONSerialization
                 JSONObjectWithData:data
                 options:0
                 error:&error];
    
    if(error)
    {
        return;
    }
    
    NSMutableArray* assets = [[NSMutableArray alloc] init];
    
    for (id object in objects) {
        NSString *assetId = object[@"AssetId"];
        SCNVector3 position = SCNVector3Make([object[@"X"] floatValue], [object[@"Y"] floatValue], [object[@"Z"] floatValue]);
        CGFloat orientation = [object[@"Orientation"] floatValue];
        
        MapAsset* asset = [[MapAsset alloc] init:assetId :position :orientation];
        [assets addObject:asset];
    }
    
    [[MapsyncWrapper sharedInstance] uploadAssets:assets];
}

extern "C" void _RegisterUnityCallbacks(char* callbackGameObject, char* assetLoadedCallback, char* statusUpdatedCallback, char* storePlacementCallback)
{
    [MapsyncWrapper setUnityCallbackGameObject:[NSString stringWithUTF8String:callbackGameObject]];
    [MapsyncWrapper setAssetLoadedCallbackFunction:[NSString stringWithUTF8String:assetLoadedCallback]];
    [MapsyncWrapper setStatusUpdatedCallbackFunction:[NSString stringWithUTF8String:statusUpdatedCallback]];
    [MapsyncWrapper setStorePlacementCallbackFunction:[NSString stringWithUTF8String:storePlacementCallback]];
}
```

#### 6: 
Drag the MapsyncSession prefab into the scene hierarchy.

#### 7:
In the Unity player settings, set the iOS deployment target to version 11.0. Build and run the Unity project for the iOS platform.

#### 8:
Build and run the Unity project for the iOS platform.

#### 9:
In the newly created project directory download [Podfile](https://github.com/jidomaps/unity_integration/blob/master/Podfile) and [MapsyncLib.podspec](https://github.com/jidomaps/unity_integration/blob/master/MapsyncLib.podspec).

The content of `Podfile`:
```
target 'Unity-iPhone' do
  use_frameworks!
  pod 'MapsyncLib', :podspec => 'MapsyncLib.podspec'
end
```
(notice `Unity-iPhone` corresponds to the name of the XCode project that Unity generated.)

#### 10:
Run `pod install` in the terminal from within that same project directory.

#### 11: 
Close the XCode project and re-open the newly created `.xcworkspace` file.

#### 12: 
Download [MapsyncWrapper.h](https://github.com/jidomaps/unity_integration/blob/master/MapsyncWrapper.h) and [MapsyncWrapper.m](https://github.com/jidomaps/unity_integration/blob/master/MapSyncWrapper.m) into the workspace `Classes` folder.

#### 13:
In your C# code, get the Mapsync component and initialize it with the `Init()` function:

```
GameObject mapsyncGameObject = GameObject.Find("MapsyncSession");
MapsyncSession mapsync = mapsyncGameObject.GetComponent<MapsyncSession> ();
```

#### Notes:
 - You will need to set Swift Versions for SwiftyJSON and Alamofire cocoapods in XCode.
 - You will need to set the iOS deployment target to 10.0 for the Pods-Unity-iPhone target in XCode.
 - The project may not build in version 9.3 beta of XCode. 
