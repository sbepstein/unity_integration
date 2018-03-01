//Based on: http://educoelho.com/unity/2015/06/15/automating-unity-builds-with-cocoapods/

using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.XcodeAPI;

public class XcodePostprocess 
{   
	#region PostProcessBuild

	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path) {

		if (buildTarget == BuildTarget.iOS) {

			string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
			PBXProject proj = new PBXProject();
			proj.ReadFromString (File.ReadAllText (projPath));
			string target = proj.TargetGuidByName ("Unity-iPhone");

			// 1. CocoaPods support.
			proj.AddBuildProperty (target, "HEADER_SEARCH_PATHS", "$(inherited)");
			proj.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
			proj.AddBuildProperty (target, "OTHER_CFLAGS", "$(inherited)");
			proj.AddBuildProperty (target, "OTHER_LDFLAGS", "$(inherited)");

			// 2. Include the final AppController file.
			foreach (string appFilePath in AppControllerFilePaths)
				CopyAndReplaceFile (appFilePath, Path.Combine (Path.Combine (path, "Classes/"), Path.GetFileName (appFilePath)));

			// 3. Include Podfile into the project root folder.
			foreach (string podFilePath in PodFilePaths)
				CopyAndReplaceFile (podFilePath, Path.Combine (path, Path.GetFileName (podFilePath)));

			File.WriteAllText (projPath, proj.WriteToString ());
		}
	}

	#endregion

	#region Private methods

	internal static void CopyAndReplaceFile (string srcPath, string dstPath)
	{
		if (File.Exists (dstPath))
			File.Delete (dstPath);

		File.Copy (srcPath, dstPath);
	}

	internal static void CopyAndReplaceDirectory (string srcPath, string dstPath)
	{
		if (Directory.Exists (dstPath))
			Directory.Delete (dstPath);

		if (File.Exists (dstPath))
			File.Delete (dstPath);

		Directory.CreateDirectory (dstPath);

		foreach (var file in Directory.GetFiles (srcPath))
			File.Copy (file, Path.Combine (dstPath, Path.GetFileName (file)));

		foreach (var dir in Directory.GetDirectories (srcPath))
			CopyAndReplaceDirectory (dir, Path.Combine (dstPath, Path.GetFileName (dir)));
	}

	internal static void CopyDirectory (string srcPath, string dstPath)
	{
		if (!Directory.Exists (dstPath))
			Directory.CreateDirectory (dstPath);

		foreach (var file in Directory.GetFiles (srcPath))
			File.Copy (file, Path.Combine (dstPath, Path.GetFileName (file)));

		foreach (var dir in Directory.GetDirectories (srcPath))
			CopyAndReplaceDirectory (dir, Path.Combine (dstPath, Path.GetFileName (dir)));
	}

	#endregion

	#region Paths

	static string[] PodFilePaths {
		get {
			return new [] {
				Path.Combine (PodFolderPath, "Podfile"),
				Path.Combine (PodFolderPath, "MapsyncLib.podspec")
			};
		}
	}

	static string[] AppControllerFilePaths {
		get {
			return new [] {
				Path.Combine (XCodeFilesFolderPath, "MapsyncWrapper.h"),
				Path.Combine (XCodeFilesFolderPath, "MapsyncWrapper.m")
			};
		}
	}

	static string PodFolderPath {
		get {
			return Path.Combine (XCodeFilesFolderPath, "Pod/");
		}
	}

	static string XCodeFilesFolderPath {
		get {
			return Path.Combine (UnityProjectRootFolder, "Assets/MapsyncLibPlugin/XCodeFiles/");
		}
	}

	static string UnityProjectRootFolder {
		get {
			return ".";
		}
	}

	#endregion
}

