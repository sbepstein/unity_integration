using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;

public class XcodePostprocess 
{   
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path) {

		if (buildTarget == BuildTarget.iOS) {
			foreach (string appFilePath in AppControllerFilePaths)
				CopyAndReplaceFile (appFilePath, Path.Combine (Path.Combine (path, "Classes/"), Path.GetFileName (appFilePath)));
			
			foreach (string podFilePath in PodFilePaths)
				CopyAndReplaceFile (podFilePath, Path.Combine (path, Path.GetFileName (podFilePath)));
		}
	}
		
	internal static void CopyAndReplaceFile (string srcPath, string dstPath)
	{
		if (File.Exists (dstPath))
			File.Delete (dstPath);

		File.Copy (srcPath, dstPath);
	}

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
}
