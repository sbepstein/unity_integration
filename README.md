# Getting Started With Mapsync and Unity

#### 1:
Create a new Unity Project.

#### 2:
Download and Import the `Unity ARKit Plugin` from the Asset Store.

#### 3: 
Download this Unity Integration repository to your computer so you can easily copy over the files you need into your Unity project.

#### 4:
Drag the folder [MapsyncLibPlugin](https://github.com/jidomaps/unity_integration/tree/master/MapsyncLibPlugin) into the Unity project window. The `MapsyncLibPlugin` folder contains a `MapSession` prefab and a `MapSession.cs` script that exposes functionality for saving and relocalizing assets in a Unity project.

[gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/Drag.gif)

#### 5:
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

#### 6:
Replace `ARSessionNative.mm` (also in `UnityARKitPlugin/Plugins/iOS/UnityARKit/Nativeinterface`) with [ARSessionNative.mm](https://github.com/jidomaps/unity_integration/blob/master/ARSessionNative.mm).

[gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/ReplaceARSessionNative.gif)

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

#### 7: 
Drag the MapSession prefab into the scene hierarchy.

[gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/DragMapsyncPrefab.gif)

#### 8:
In the Unity player settings, set the iOS deployment target to version 11.0. Build and run the Unity project for the iOS platform.

![alt text](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/iOSVersion.png)

#### 9:
Build and run the Unity project for the iOS platform.

![alt text](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/BuildAndRun.png)

#### 10:
In the newly created project directory download [Podfile](https://github.com/jidomaps/unity_integration/blob/master/Podfile) and [MapsyncLib.podspec](https://github.com/jidomaps/unity_integration/blob/master/MapsyncLib.podspec).

The content of `Podfile`:
```
target 'Unity-iPhone' do
  use_frameworks!
  pod 'MapsyncLib', :podspec => 'MapsyncLib.podspec'
end
```
(notice `Unity-iPhone` corresponds to the name of the XCode project that Unity generated.)

#### 11:
Run `pod install` in the terminal from within that same project directory.

![alt text](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/PodInstall.png)

#### 12: 
Close the XCode project and re-open the newly created `.xcworkspace` file.

[gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/OpenWorkspace.gif)

#### 13: 
Copy [MapsyncWrapper.h](https://github.com/jidomaps/unity_integration/blob/master/MapsyncWrapper.h) and [MapsyncWrapper.m](https://github.com/jidomaps/unity_integration/blob/master/MapSyncWrapper.m) into the workspace `Classes` folder.

[gif](https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/AddMapsyncWrapperh.gif)

#### 14:
In your C# code, get the MapSession component and initialize it with the `Init()` function:

```
GameObject mapSessionGameObject = GameObject.Find("MapSession");
MapSession mapSession = mapSessionGameObject.GetComponent<MapSession> ();
```

Bind to the events: `MapSession.StatusChangedEvent`, `MapSession.AssetLoadedEvent`, and `MapSession.AssetStoredEvent`.

#### Notes:
 - You will need to set Swift Versions for SwiftyJSON and Alamofire cocoapods in XCode.
 - You will need to set the iOS deployment target to 10.0 for the Pods-Unity-iPhone target in XCode.
 - The project may not build in version 9.3 beta of XCode. 
