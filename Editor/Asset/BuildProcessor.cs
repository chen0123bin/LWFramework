using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LWFramework.Asset.Editor
{
    [Obsolete]
    public class BuildProcessor : IPreprocessBuild, IPostprocessBuild
    {
        public void OnPostprocessBuild(BuildTarget target, string path)
        {
            if (target != BuildTarget.iOS || Environment.OSVersion.Platform != PlatformID.MacOSX) return;
            var searchPath = Path.Combine(Application.dataPath, "Assets/LWFramework/Asset/Editor/Shells"); 
            var shells = Directory.GetFiles(searchPath, "*.sh", SearchOption.AllDirectories);
            foreach (var item in shells)
            {
                if (item == null) continue;
                var newPath = Path.Combine(path, Path.GetFileName(item));
                File.Copy(item, newPath, true);
            }

            var bundleIdentifiers = PlayerSettings.applicationIdentifier.Split('.');
            var appName = bundleIdentifiers[bundleIdentifiers.Length - 1];
            var ipaType = EditorUserBuildSettings.development ? "develop" : "release";
            var ipaName = string.Format("{0}-{1}-{2}",
                appName,
                PlayerSettings.bundleVersion,
                ipaType);

            var configType = EditorUserBuildSettings.iOSBuildConfigType.ToString();
            var openTerminalBash = Path.Combine(path, "OpenTerminal.sh");
            var args = openTerminalBash + " " + path + " " + ipaName + " " + ipaType + " " + appName + " " + configType;
            Process.Start("/bin/bash", args);
        }

        public void OnPreprocessBuild(BuildTarget target, string path)
        {
            BuildABScript.CopyAssetBundlesTo(Path.Combine(Application.streamingAssetsPath, LWUtility.AssetBundles));
            var platformName = BuildABScript.GetPlatformName();
            var searchPath = Path.Combine(Path.Combine(Application.streamingAssetsPath, LWUtility.AssetBundles),
                platformName);
            if (!Directory.Exists(searchPath)) return;
            var files = Directory.GetFiles(searchPath, "*.manifest", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var info = new FileInfo(file);
                if (info.Exists) info.Delete();
            }

            files = Directory.GetFiles(searchPath, "*.meta", SearchOption.AllDirectories);
            foreach (var item in files)
            {
                var info = new FileInfo(item);
                info.Delete();
            }
        }

        public int callbackOrder
        {
            get { return 0; }
        }
    }
}