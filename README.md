# Getting Started With Mapsync and Unity

#### 1:
Create a new Unity Project.

#### 2:
Download and Import the `Unity ARKit Plugin` from the Asset Store.

#### 3:
In the "Project" window, navigate to the folder: `UnityARKitPlugin/Plugins/iOS/UnityARKit/Nativeinterface` and add the C# script file: [UnityMapsyncLibNativeInterface.cs](https://bitbucket.org/Amichai/mapsync-and-unity/raw/b2b3e4549c69224582d8ac7109a591a6248ccb51/UnityMapsyncLibNativeInterface.cs).

#### 4:
Replace `UnityARSessionNativeInterface.cs` with [UnityARSessionNativeInterface.cs](https://bitbucket.org/Amichai/mapsync-and-unity/raw/b2b3e4549c69224582d8ac7109a591a6248ccb51/UnityARSessionNativeInterface.cs).

The new `UnityARSessionNativeInterface.cs` adds the following line below line 417: 
```
    UnityMapsyncLibNativeInterface.Initialize (m_NativeARSession);
```
    The previous line should be: 
    `m_NativeARSession = unity_CreateNativeARSession();`


#### 5:
Replace `ARSessionNative.mm` (also in `UnityARKitPlugin/Plugins/iOS/UnityARKit/Nativeinterface`) with [ARSessionNative.mm](https://bitbucket.org/Amichai/mapsync-and-unity/raw/b2b3e4549c69224582d8ac7109a591a6248ccb51/ARSessionNative.mm).

The new `ARSession.mm` adds:

 - `#import "MapsyncWrapper.h"` 
 - After line 669 add the line:
  `[[MapsyncWrapper sharedInstance] updateWithFrame:frame];`
 Line 669 is the last line in the function: 
 `- (void)session:(ARSession *)session didUpdateFrame:(ARFrame *)frame`
  - The following functions at the bottom of the file:
```
extern "C" void* _CreateMapsyncSession(void* nativeSession, char* mapId, char* appId, char* userId, char* developerKey, BOOL isMappingMode, char* unityAssetLoadedCallbackGameObject, char* unityAssetLoadedCallbackFunction)
{
    Mode mode = isMappingMode ? ModeMapping : ModeLocalization;
    UnityARSession* session = (__bridge UnityARSession*)nativeSession;
    
    [MapsyncWrapper sharedInstanceWithARSession:session->_session mapMode:mode appId:[NSString stringWithUTF8String:appId] mapId:[NSString stringWithUTF8String:mapId] userId:[NSString stringWithUTF8String:userId] developerKey:[NSString stringWithUTF8String:developerKey] unityAssetLoadedCallbackGameObject:[NSString stringWithUTF8String:unityAssetLoadedCallbackGameObject] unityAssetLoadedCallbackFunction:[NSString stringWithUTF8String:unityAssetLoadedCallbackFunction]];
    
    return (__bridge_retained void*) [MapsyncWrapper sharedInstance];
}

extern "C" void _SaveAsset(char* json)
{
    NSError *error = nil;
    
    NSData* data = [[NSString stringWithUTF8String:json] dataUsingEncoding:NSUTF8StringEncoding];
    id object = [NSJSONSerialization
                 JSONObjectWithData:data
                 options:0
                 error:&error];
    
    if(error)
    {
        return;
    }
    
    NSString *assetId = object[@"assetId"];
    NSDictionary *positionVals = object[@"position"];
    SCNVector3 position = SCNVector3Make([positionVals[@"x"] floatValue], [positionVals[@"y"] floatValue], [positionVals[@"z"] floatValue]);
    CGFloat orientation = [object[@"orientation"] floatValue];
    [[MapsyncWrapper sharedInstance] uploadAssetWithID:assetId position:position orientation:orientation];
}
```

#### 6:
In the Unity player settings, set the iOS deployment target to version 11.0. Build and run the Unity project for the iOS platform.

#### 7:
Add the file [AssetModel.cs](https://bitbucket.org/Amichai/mapsync-and-unity/raw/b2b3e4549c69224582d8ac7109a591a6248ccb51/AssetModel.cs) to the Unity project folder.

#### 8:
Update the developer key placeholder value with a real developer key in `UnityMapsyncLibNativeInterface.cs` line 28.

#### 9:
Add a C# script with a C# class that inherits from type `MonoBehavior`. In this new class add the following function:
```
public void AssetRelocalized(string assetJson) {
AssetModel assetModel = AssetModel.FromJson (assetJson);
    //Handle the relocalized asset here.
}
```
Attach this MonoBehavior to a Unity object and then update the variable `unityAssetLoadedCallbackGameObject` value on line 29 of `UnityMapsyncLibNativeInterface.cs` with that object name.


#### 10:
Build and run the Unity project for the iOS platform

#### 11:
In the newly created project directory download [Podfile](https://bitbucket.org/Amichai/mapsync-and-unity/raw/b2b3e4549c69224582d8ac7109a591a6248ccb51/Podfile) and [MapsyncLib.podspec](https://bitbucket.org/Amichai/mapsync-and-unity/raw/b2b3e4549c69224582d8ac7109a591a6248ccb51/MapsyncLib.podspec).

The content of `Podfile`:
```
target 'Unity-iPhone' do
  use_frameworks!
  pod 'MapsyncLib'
end
```
(notice `Unity-iPhone` corresponds to the name of the XCode project that Unity generated.)

#### 12:
Run `pod install` in the terminal from within that same project directory.

#### 13: 
Close XCode and re-open the newly created `.xcworkspace` file.

#### 14: 
Download [MapsyncWrapper.h](https://bitbucket.org/Amichai/mapsync-and-unity/raw/b2b3e4549c69224582d8ac7109a591a6248ccb51/MapsyncWrapper.h) and [MapsyncWrapper.m](https://bitbucket.org/Amichai/mapsync-and-unity/raw/b2b3e4549c69224582d8ac7109a591a6248ccb51/MapSyncWrapper.m) into the workspace `Classes` folder.

#### Notes:
 - You will need to set Swift Versions for SwiftyJSON and Alamofire cocoapods in XCode.
 - You will need to set the iOS deployment target to 10.0 for the Pods-Unity-iPhone target in XCode.
 - The project may not build in version 9.3 beta of XCode. 

