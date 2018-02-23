using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class MapAssets {
	public List<MapAsset> Assets;
}

[Serializable]
public class MapAsset
{
	public float Orientation;
	public float X;
	public float Y;
	public float Z;
	public string AssetId;

	public Vector3 Position {
		get {
			return new Vector3 (X, Y, Z);
		}
	}

	public MapAsset(string assetId, float orientation, float x, float y, float z) {
		this.AssetId = assetId;
		this.Orientation = orientation;
		this.X = x;
		this.Y = y;
		this.Z = z;
	}

	public MapAsset(string assetId, float orientation, Vector3 position) : this(assetId, orientation, position.x, position.y, position.z) {
	}
}