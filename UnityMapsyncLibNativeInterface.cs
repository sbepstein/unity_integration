using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS {
	public class UnityMapsyncLibNativeInterface {
		private bool isMappingMode = true;

		private static IntPtr s_UnityMapsyncLibNativeInterface = IntPtr.Zero;
		private static UnityMapsyncLibNativeInterface instance = null;

		[DllImport("__Internal")]
		private static extern IntPtr _CreateMapsyncSession(IntPtr arSession, string mapId, string appId, string userId, string developerKey, 
			bool isMappingMode, string unityAssetLoadedCallbackGameObject, string unityAssetLoadedCallbackFunction);

		[DllImport("__Internal")]
		private static extern void _SaveAsset(string assetJson);

		private UnityMapsyncLibNativeInterface(IntPtr arSession) 
		{
			Debug.Log ("UnityMapsyncLibNativeInterface()");
			this.isMappingMode = PlayerPrefs.GetInt ("IsMappingMode") == 1;
			string mapId = PlayerPrefs.GetString ("MapId");
			string userId = PlayerPrefs.GetString ("UserId");
			string appId = PlayerPrefs.GetString ("AppId");
			string developerKey = @"UPDATE_ME";
			string unityAssetLoadedCallbackGameObject = "UPDATE_ME";
			string unityAssetLoadedCallbackFunction = "AssetRelocalized";

			Debug.Log(string.Format("UnityMapsyncLibNativeInterface: {0}, {1}, {2}, {3}", mapId, appId, userId, developerKey, isMappingMode));
			s_UnityMapsyncLibNativeInterface = _CreateMapsyncSession(arSession, mapId, appId, userId, developerKey, this.isMappingMode, unityAssetLoadedCallbackGameObject, unityAssetLoadedCallbackFunction);
		}

		public static void Initialize(IntPtr arSession)
		{
			instance = new UnityMapsyncLibNativeInterface(arSession);
		}

		public static UnityMapsyncLibNativeInterface GetInstance() 
		{
			return instance;
		}

		public bool IsMappingMode()
		{
			return isMappingMode;
		}

		public void SaveAsset(Vector3 position, string assetId, float orientation)
		{
			AssetModel asset = new AssetModel (assetId, orientation, position.x, position.y, position.z);
			string assetJson = asset.ToJson ();

			Debug.Log ("Asset json: " + assetJson);
			_SaveAsset(assetJson);
		}
	}
}
