# Getting Started with Jido and Unity

## Unity Project Setup

 - Create a new Unity Project.

 - Download and Import the `Unity ARKit Plugin` from the Asset Store.


## Adding Mapsync

 - Download this Unity Integration repository to your computer so you can easily copy over the files you need into your Unity project. This repo contains all the files you will need to integrate Jido into your Unity project.

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

Also, this change configures the ARSession to start with [z-axis set to true-north](https://developer.apple.com/documentation/arkit/arconfiguration.worldalignment/2873776-gravityandheading).

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
 - To build this project in version 9.3 beta of XCode, you will need to change: `s.source = { :git => 'https://github.com/jidomaps/jido_pods.git', :tag => 'v0.1.2' }` to:
 `s.source = { :git => 'https://github.com/jidomaps/jido_pods.git', :tag => 'v0.1.2-beta' }` in `MapsyncLib.podspec`.
 - You will need to set Swift Versions for SwiftyJSON and Alamofire cocoapods in XCode.

<img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/SwiftLanuageVersion.png" width="500">


# How to Save AR Sessions with Jido 

## Initialization

Jido runs in the background during an `ARKit` ARSession to handle data management and synchronization with the API. To start a MapSession, you must access the Jido `MapSession` component as follows:

```
GameObject mapSessionGameObject = GameObject.Find("MapSession");
MapSession mapSession = mapSessionGameObject.GetComponent<MapSession> ();
```

Lastly, you must initialize a unique map session (this should only be done only once for each `ARKit` ARSession):

`MapSession.Init (MapMode mapMode, string userId, string mapId, string developerKey)`

MapMode determines if you are attempting to create a new session or are relocalizing assets on a map from a previously stored session. Your userID, and mapID are set to uniquely identify your session. Your developer key is used for authentication and is unique to your Jido account. 

## `MapSession` Events

After initializing `MapSession` you can bind to the following events:
- `AssetStoredEvent` -  Invoked after `MapSession.StorePlacements()` has been called. The `bool` event argument indicates whether or not the assets were saved successfully.
- `AssetLoadedEvent` - Invoked when an asset has been relocalized in a localization session. The `MapAsset` event argument is the asset that has been localized. 
- `StatusChangedEvent` - Invoked on changes to `MapSesssion`'s status. The event argument is a `string` containing the new `MapSession` status.

## Saving an AR Session

When in `MapMode.MapModeMapping`, assets can be saved  by calling `MapSession.StorePlacements(List<MapAsset>)` with a list of the `MapAsset`s to be stored on the map.

`StorePlacement()` will store a `List` of `MapAsset`s which are used to reference assets or waypoints in your user's AR Session (more information about `MapAsset` below). Shortly calling `StorePlacement()` the `AssetStoredEvent` will be invoked with a `bool` indicating whether or not the placement assets were stored successfully.

## Reloading an AR Session

To reload an AR Session, `MapSession` must be initialized with `MapMode.MapModeLocalization`. When an asset has been re-localized in a localization session, the `AssetLoadedEvent` will be invoked with the newly localized `MapAsset` that was saved by the user in a prior mapping session and transformed to the current session's coordinate frame. 

To use the `MapAssets` in your Unity project, simply create GameObjects that correspond to the saved AssetIds and place them in your scene using the position and orientation that was returned. Now the assets will be back in the session where you had left them! 

## `MapAsset` Class

The `MapAsset` stores the information necessary for each asset that is saved or reloaded from the Jido API. Each `MapAsset` has three member variables that are set when initialized.

`public MapAsset(string assetId, float orientation, float x, float y, float z)`

`assetId` is a custom identifier that can be set to any value. The position coordinates are relative to the global coordinate system of the asset in the saved session. The orientation is the y-axis orientation of the asset in the saved session.

## Notes

 - Jido configures ARSession to run with [z-axis set to true-north](https://developer.apple.com/documentation/arkit/arconfiguration.worldalignment/2873776-gravityandheading). This is necessary, and should not be manually overridden. 
 - The phone must have an internet connection. 
 - For an ideal relocalization, the user should be encouraged to move around so that they will cover an "interesting" part of the saved session.

<img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/media-20180227.png" width="500">

<img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/relocalization_explanation_V2_bad.png" width="500">
