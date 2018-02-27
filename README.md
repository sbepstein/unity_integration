# Getting Started With Mapsync and Unity

## Unity Project Setup

 - Create a new Unity Project.

 - Download and Import the `Unity ARKit Plugin` from the Asset Store.


## Adding Mapsync

 - Download this Unity Integration repository to your computer so you can easily copy over the files you need into your Unity project.

 - Drag the folder [MapsyncLibPlugin](https://github.com/jidomaps/unity_integration/tree/master/MapsyncLibPlugin) into the Unity project window. The `MapsyncLibPlugin` folder contains a `MapSession` prefab and a `MapSession.cs` script that exposes functionality for saving and relocalizing assets in a Unity project. [gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/Drag.gif)
 
 - Replace `UnityARKitPlugin/Plugins/iOS/UnityARKit/Nativeinterface/UnityARSessionNativeInterface.cs` with [UnityARSessionNativeInterface.cs](https://github.com/jidomaps/unity_integration/blob/master/UnityARSessionNativeInterface.cs).

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
 - Replace `ARSessionNative.mm` (also in `UnityARKitPlugin/Plugins/iOS/UnityARKit/Nativeinterface`) with [ARSessionNative.mm](https://github.com/jidomaps/unity_integration/blob/master/ARSessionNative.mm). [gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/ReplaceARSessionNative.gif)

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

 - Drag the MapSession prefab into the scene hierarchy. [gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/DragMapsyncPrefab.gif)

## Building the Unity Project
 
 - In the Unity player settings, set the iOS deployment target to version 11.0. 

 <img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/iOSVersion.png" width="500">
 
 - Build and run the Unity project for the iOS platform. 

 <img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/BuildAndRun.png" width="500">

## Building the iOS Project

 - In the newly created project directory download [Podfile](https://github.com/jidomaps/unity_integration/blob/master/Podfile) and [MapsyncLib.podspec](https://github.com/jidomaps/unity_integration/blob/master/MapsyncLib.podspec).

The content of `Podfile`:
```
target 'Unity-iPhone' do
  use_frameworks!
  pod 'MapsyncLib', :podspec => 'MapsyncLib.podspec'
end
```
(notice `Unity-iPhone` corresponds to the name of the XCode project that Unity generated.)

 - Run `pod install` in the terminal from within that same project directory. 

<img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/PodInstall.png" width="500">

 - Close the XCode project and re-open the newly created `.xcworkspace` file. [gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/OpenWorkspace.gif)

 - Copy [MapsyncWrapper.h](https://github.com/jidomaps/unity_integration/blob/master/MapsyncWrapper.h) and [MapsyncWrapper.m](https://github.com/jidomaps/unity_integration/blob/master/MapSyncWrapper.m) into the workspace `Classes` folder and add them to the XCode project. [gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/AddMapsyncWrapperh.gif)

## Notes:
 - You will need to set the iOS deployment target to 10.0 for the Pods-Unity-iPhone target in XCode.
 - The project may not build in version 9.3 beta of XCode. 
 - You will need to set Swift Versions for SwiftyJSON and Alamofire cocoapods in XCode.

<img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/SwiftLanuageVersion.png" width="500">


# MapSession API Documentation

In your C# code, access the `MapSession` component and initialize it with the `Init()` function:

```
GameObject mapSessionGameObject = GameObject.Find("MapSession");
MapSession mapSession = mapSessionGameObject.GetComponent<MapSession> ();
```

`MapSession` will not start until `MapSession.Init()` has been called. `MapSession.Init()` should only be called once and it will start an `ARKitSession` if one hasn't been started already. `MapSession.Init()` takes as argument: 

- `mapMode` (either `MapMode.MapModeMapping` or `MapMode.MapModeLocalization`)
- `userId`
- `mapId`
- `developerKey`

In `MapModeMapping`, `MapSession` is mapping the space that it sees. Assets can be saved in the space in Mapping Mode. In `MapModeLocalization`, previously saved assets are re-localized and their original saved position is resolved.

`UserId` and `MapId` are user-provided identifiers used to match Mapping and Localization session data. Assets saved during a mapping session are associated with the `UserId` and `MapId` provided and those assets would be returned during a localization session that was initialized with the same `UserId` and `MapId` values.

The developer key is a authentication key provided by Jido Maps for authenticated access to our API.

After initializing `MapSession` bind to the events:
- `AssetStoredEvent` -  Invoked after `MapSession.StorePlacements()` has been called. The bool event argument indicates whether or not the assets were saved successfully.
- `AssetLoadedEvent` - Invoked when an asset has been relocalized in a localization session. The `MapAsset` event argument is the asset that was localized. 
- `StatusChangedEvent` - Invoked on changes to `MapSesssion`'s status. The event argument is a string containing the new `MapSession` status.

To store an asset during a mapping session, call `MapSession.StorePlacements()` with a list of the `MapAsset`s to be stored on the map.