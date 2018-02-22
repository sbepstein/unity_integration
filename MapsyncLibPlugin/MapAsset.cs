using System;
using UnityEngine;

public class MapAsset
{
	private float orientation;
	public float Orientation {
		get {
			return orientation;
		}
		private set {
			orientation = value;
		}
	}

	private float x;
	public float X {
		get {
			return x;
		}
		private set {
			x = value;
		}
	}

	private float y;
	public float Y {
		get {
			return y;
		}
		private set {
			y = value;
		}
	}

	private float z;
	public float Z {
		get {
			return z;
		}
		private set {
			z = value;
		}
	}

	private string assetId;
	public string AssetId {
		get {
			return assetId;
		}
		private set {
			assetId = value;
		}
	}

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

	public string ToJson() {
		string positionJson = string.Format ("\"position\" : {{\"x\" : {0:0.######}, \"y\" : {1:0.######}, \"z\" : {2:0.######} }}", X, Y, Z);;
		return string.Format ("{{ \"assetId\" : \"{0}\", \"orientation\" : {1}, {2} }}", assetId, orientation, positionJson);
	}

	public static MapAsset FromJson(string json) {
		string assetId = "";
		float orientation = 0, x = 0, y = 0, z = 0;
		string[] parts = json.Split (':');
		for (int i = 0; i < parts.Length; i++) {
			string part = parts [i];
			if (part.Contains ("\"orientation\"")) {
				orientation = parseFloat (parts [i + 1]);
			}

			if (part.Contains ("\"x\"")) {
				x = parseFloat (parts [i + 1]);
			}

			if (part.Contains ("\"y\"")) {
				y = parseFloat (parts [i + 1]);
			}
			if (part.Contains ("\"z\"")) {
				z = parseFloat (parts [i + 1]);
			}
			if (part.Contains ("\"assetId\"")) {
				assetId = parse(parts [i + 1]);
			}
		}

		return new MapAsset(assetId, orientation, x, y, z);
	}

	private static float parseFloat(string inspection) {
		return float.Parse(parse(inspection));
	}

	private static string parse(string inspection) {
		return inspection.Split (new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries) [0].Trim ('}').Trim ().Trim ('"');
	}
}


