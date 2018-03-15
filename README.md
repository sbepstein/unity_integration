# Getting Started with Jido and Unity

## Unity Project Setup

 - Create a new Unity Project.

 - Download the JidoMaps plugin for Unity [here](https://github.com/jidomaps/unity_integration/blob/NewUnityPluginRefactor/JidoMaps.unitypackage)

 - In the Unity menu, go to Assets > Import Package > Custom Package and select this package file to import.

 - Drag the MapSession prefab, located in the `JidoMaps` folder, into the scene hierarchy.

## Building the Unity Project
 
 - In the Unity player settings, set the iOS deployment target to version 11.0. 

<img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/iOSVersion.png" width="500">
 
 - Build the Unity project for the iOS platform. 

<img src="https://s3-us-west-2.amazonaws.com/unity-integration-screenshots/BuildAndRun.png" width="500">

## Building the iOS Project

 - Set the provisioning profile for your XCode project

 - Close the XCode project and then double click on `pods.command` in the newly created project directory.

## Notes:
 - To build this project in version 9.3 beta of XCode, you will need to append the `-beta` suffix to the package version. For example: `s.source = { :git => 'https://github.com/jidomaps/jido_pods.git', :tag => 'v0.1.6' }` to:
 `s.source = { :git => 'https://github.com/jidomaps/jido_pods.git', :tag => 'v0.1.6-beta' }` in `MapsyncLib.podspec`.

# [Jido With Unity Demo Project](https://github.com/jidomaps/unity_demo/)

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

[Swift API Documentation](https://docs.google.com/document/d/1GyMbAHtKBAUwj8a8tuu4RE6XwrOGbop8OTIluU-16b8/)
