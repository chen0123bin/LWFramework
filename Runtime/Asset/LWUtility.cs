using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LWFramework.Asset
{
    public delegate Object LoadDelegate(string path, Type type);

    public delegate string GetPlatformDelegate();

    public static class LWUtility
    {
        public static string AssetBundles { get; private set; } = "AssetBundles";
        /// <summary>
        ///Manifest位置
        /// </summary>
        public static string AssetsManifestAsset { get; private set; } = "Assets/Manifest.asset";

        /// <summary>
        /// 热更dll的名称
        /// </summary>
        public static string HotfixFileName { get; private set; }= "hotfix.dll";

        public static LoadDelegate loadDelegate = null;
        public static GetPlatformDelegate getPlatformDelegate = null;
        /// <summary>
        /// 项目根目录
        /// </summary>
        public static string ProjectRoot { get; private set; } = Application.dataPath.Replace("/Assets", "");
        /// <summary>
        /// Library
        /// </summary>
        public static string Library { get; private set; } = ProjectRoot + "/Library";
        /// <summary>
        /// 资源路径 运行时一般为StreamingAssets 编辑器下测试非AB模式则为System.Environment.CurrentDirectory便于使用 AssetDatabase.LoadAssetAtPath模拟AB加载
        /// </summary>
        public static string dataPath { get; set; }
        /// <summary>
        /// 下载路径 编辑器下读取Assets/Manifest.asset，如果为空读取ab中的Manifest   
        /// </summary>
        public static string downloadURL { get; set; }

        private static LWGlobalConfig _lwGlobalConfig;

        /// <summary>
        /// Resources下的全局配置文件
        /// </summary>
        public static LWGlobalConfig GlobalConfig { get {
                if (_lwGlobalConfig == null) {
                    _lwGlobalConfig = Resources.Load<LWGlobalConfig>("LWGlobalConfig");

#if UNITY_EDITOR
                    if (_lwGlobalConfig == null) {
                        FileTool.CheckCreateDirectory(Application.dataPath + "/Resources");
                        var asset = ScriptableObject.CreateInstance(typeof(LWGlobalConfig));
                        UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Resources/LWGlobalConfig.asset");
                        UnityEditor.AssetDatabase.Refresh();
                    }
#endif
#if !UNITY_EDITOR
                    _lwGlobalConfig.assetBundleMode = true;
#endif
                }
                return _lwGlobalConfig;
            } 
        }
        /// <summary>
        /// 获取当前平台的名称
        /// </summary>
        /// <returns></returns>
        public static string GetPlatform()
        {
            return getPlatformDelegate != null
                ? getPlatformDelegate()
                : GetPlatformForAssetBundles(Application.platform);
        }

        private static string GetPlatformForAssetBundles(RuntimePlatform platform)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (platform)
            {
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "iOS";
                case RuntimePlatform.WebGLPlayer:
                    return "WebGL";
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    return "Windows";
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    return "OSX";
                default:
                    return null;
            }
        }
        /// <summary>
        /// persistentDataPath+平台名称 +/
        /// </summary>
        public static string updatePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, Path.Combine(AssetBundles, GetPlatform())) +
                       Path.DirectorySeparatorChar;
            }
        }
        /// <summary>
        /// persistentDataPath  persistentData+平台名称+path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetRelativePath4Update(string path)
        {
            return updatePath + path;
        }
        /// <summary>
        /// 获取文件下载地址   服务器地址+平台+filename
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetDownloadURL(string filename)
        {
            return Path.Combine(Path.Combine(LWUtility.downloadURL, GetPlatform()), filename);
        }
        /// <summary>
        /// 获取各个平台的资源路径，运行时一般为StreamingAssets+AssetBundles+平台+文件名
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetWebUrlFromDataPath(string filename)
        {
            var path = Path.Combine(dataPath, Path.Combine(AssetBundles, GetPlatform())) + Path.DirectorySeparatorChar + filename;
#if UNITY_IOS || UNITY_EDITOR
            path = "file://" + path;
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            path = "file:///" + path;
#endif
            return path;
        }

        public static string GetWebUrlFromStreamingAssets(string filename)
        {
            var path = updatePath + filename;
            if (!File.Exists(path))
            {
                path = Application.streamingAssetsPath + "/" + filename;
            }
#if UNITY_IOS || UNITY_EDITOR
			path = "file://" + path;
#elif UNITY_STANDALONE_WIN
            path = "file:///" + path;
#endif
            return path;
        }
    }
}