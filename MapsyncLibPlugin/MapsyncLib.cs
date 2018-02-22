using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class MapsyncLib : MonoBehaviour {
	private Action<MapAsset> assetFoundCallback;
	private Action<MapStatus> statusCallback;
	private Action<bool> storedAssetCallback;
	private UnityMapsyncLibNativeInterface mapsyncInterface;
	public MapMode MapMode;

	public void Init(MapMode mapMode, string userId, string mapId, string developerKey, Action<MapAsset> assetFoundCallback, Action<MapStatus> statusCallback) {
		this.assetFoundCallback = assetFoundCallback;
		this.statusCallback = statusCallback;
		this.MapMode = mapMode;
		mapsyncInterface = new UnityMapsyncLibNativeInterface(mapId, userId, developerKey, mapMode == MapMode.MapModeMapping);
	}

	public void StorePlacement(MapAsset asset, Action<bool> storedAssetCallback) {
		this.storedAssetCallback = storedAssetCallback; 
		if (MapMode == MapMode.MapModeMapping) {
			mapsyncInterface.SaveAsset (asset.Position, asset.AssetId, asset.Orientation);
		}
	}

	private void AssetReloaded(string assetJson) {
		MapAsset mapAsset = MapAsset.FromJson (assetJson);
		assetFoundCallback (mapAsset);
	}

	private void StatusUpdated(string status) {
		int asInt = int.Parse (status);
		statusCallback ((MapStatus)asInt);
	}

	private void PlacementStored(string stored) {
		storedAssetCallback (stored == "1");
	}
}
