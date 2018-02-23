using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public class MapSession : MonoBehaviour {
	private UnityMapsyncLibNativeInterface mapsyncInterface = null;
	public MapMode Mode;

	public delegate void StatusDelegate(MapStatus status);
	public StatusDelegate StatusChangedEvent;

	public delegate void AssetDelegate(MapAsset asset);
	public AssetDelegate AssetLoadedEvent;

	public delegate void BoolDelegate(bool value);
	public BoolDelegate AssetStoredEvent;

	public void Init(MapMode mapMode, string userId, string mapId, string developerKey) {
		if (mapsyncInterface != null) {
			Debug.Log ("Warning: Mapsync has already been initialized and cannot be initialized again.");
			return;
		}

		this.Mode = mapMode;

		mapsyncInterface = new UnityMapsyncLibNativeInterface(mapId, userId, developerKey, mapMode == MapMode.MapModeMapping);
	}

	public void StorePlacement(MapAsset asset) {
		if (Mode == MapMode.MapModeMapping) {
			mapsyncInterface.SaveAsset (asset.Position, asset.AssetId, asset.Orientation);
		}
	}

	private void AssetReloaded(string assetJson) {
		MapAssets assets = JsonUtility.FromJson<MapAssets> (assetJson);
//		MapAsset mapAsset = MapAsset.FromJson (assetJson);
		foreach (MapAsset asset in assets.Assets) {
			AssetLoadedEvent (asset);
		}
	}

	private void StatusUpdated(string status) {
		int asInt = int.Parse (status);
		StatusChangedEvent ((MapStatus)asInt);
	}

	private void PlacementStored(string stored) {
		AssetStoredEvent (stored == "1");
	}
}
