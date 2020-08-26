using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using LWFramework.Core;
using UniRx.Async;

namespace LWFramework.Asset
{
    [ManagerClass(ManagerType.Normal)]
    public class AssetsManager : IManager
    {
        public void Init()
        {
            // throw new System.NotImplementedException();
        }

        public void LateUpdate()
        {
            //  throw new System.NotImplementedException();
        }

        public void Update()
        {
            for (var i = 0; i < _assets.Count; i++)
            {
                var item = _assets[i];
                if (item.Update() || !item.IsUnused())
                    continue;
                _unusedAssets.Add(item);
                _assets.RemoveAt(i);
                i--;
            }

            for (var i = 0; i < _unusedAssets.Count; i++)
            {
                var item = _unusedAssets[i];
                item.Unload();
                LWDebug.Log("Unload->" + item.url);
            }

            _unusedAssets.Clear();

            Bundles.Update();
        }

        private string[] _bundles = new string[0];
        private Dictionary<string, int> _bundleAssets = new Dictionary<string, int>();

        // ReSharper disable once InconsistentNaming
        private readonly List<AssetRequest> _assets = new List<AssetRequest>();

        // ReSharper disable once InconsistentNaming
        private readonly List<AssetRequest> _unusedAssets = new List<AssetRequest>();

        public Dictionary<string, int> bundleAssets
        {
            get { return _bundleAssets; }
        }

        private string updatePath { get; set; }
/**
        public void Initialize(Action onSuccess, Action<string> onError)
        {

            if (string.IsNullOrEmpty(LWUtility.dataPath)) LWUtility.dataPath = Application.streamingAssetsPath;

            LWDebug.Log(string.Format("Init->assetBundleMode: {0} | dataPath :{1}| connServer :{2}", LWUtility.GlobalConfig.assetBundleMode, LWUtility.dataPath, LWUtility.GlobalConfig.connServer), LogColor.green);

            if (LWUtility.GlobalConfig.assetBundleMode)
            {
                updatePath = LWUtility.updatePath;
                var platform = LWUtility.GetPlatform();
                //默认资源路径  
                var path = Path.Combine(LWUtility.dataPath, Path.Combine(LWUtility.AssetBundles, platform)) +
                           Path.DirectorySeparatorChar;
                Bundles.OverrideBaseDownloadingUrl += Bundles_overrideBaseDownloadingURL;

                Bundles.Initialize(path, platform, () =>
                {
                    var asset = LoadAsync(LWUtility.AssetsManifestAsset, typeof(Manifest));
                    asset.completed += obj =>
                    {
                        var manifest = obj.asset as Manifest;
                        if (manifest == null)
                        {
                            if (onError != null) onError("manifest == null");
                            return;
                        }

                        if (string.IsNullOrEmpty(LWUtility.downloadURL))  //将下载地址放在全局配置文件中
                            LWUtility.downloadURL = manifest.downloadURL;
                        Bundles.activeVariants = manifest.activeVariants;
                        _bundles = manifest.bundles;
                        var dirs = manifest.dirs;
                        _bundleAssets = new Dictionary<string, int>(manifest.assets.Length);
                        for (int i = 0, max = manifest.assets.Length; i < max; i++)
                        {
                            var item = manifest.assets[i];
                            _bundleAssets[string.Format("{0}/{1}", dirs[item.dir], item.name)] = item.bundle;
                        }

                        if (onSuccess != null)
                            onSuccess();
                        obj.Release();
                    };
                }, onError);
            }
            else
            {
                if (onSuccess != null)
                    onSuccess();
            }
        }

*/
        public async UniTask<AssetsManagerRequest> InitializeAsync()
        {
            AssetsManagerRequest assetsManagerRequest = new AssetsManagerRequest { isSuccess = true };
            

            if (string.IsNullOrEmpty(LWUtility.dataPath)) LWUtility.dataPath = Application.streamingAssetsPath;

            LWDebug.Log(string.Format("Init->assetBundleMode {0} | dataPath {1}", LWUtility.GlobalConfig.assetBundleMode, LWUtility.dataPath), LogColor.green);
            //判断当前是否为AB模式
            if (LWUtility.GlobalConfig.assetBundleMode)
            {
                updatePath = LWUtility.updatePath;
                var platform = LWUtility.GetPlatform();
                //默认资源路径  
                var path = Path.Combine(LWUtility.dataPath, Path.Combine(LWUtility.AssetBundles, platform)) + Path.DirectorySeparatorChar;
                Bundles.OverrideBaseDownloadingUrl += Bundles_overrideBaseDownloadingURL;
                //初始化Bundles
                BundleRequest bundleRequest = await Bundles.Initialize(path, platform);
                //Bundle加载出错
                if (bundleRequest.error != null)
                {
                    assetsManagerRequest.isSuccess = false;
                    assetsManagerRequest.error = bundleRequest.error;                  
                    bundleRequest.Release();
                    bundleRequest = null;
                    return assetsManagerRequest;
                }
                else {
                    //加载资源的Manifest文件
                    var assetRequest = await LoadAsyncTask<Manifest>(LWUtility.AssetsManifestAsset);
                    var manifest = assetRequest.asset as Manifest;
                    if (manifest == null)
                    {                      
                        assetsManagerRequest.isSuccess = false;
                        assetsManagerRequest.error = "manifest == null";
                        return assetsManagerRequest;
                    }
                    //设置资源的下载地址，在编辑器下会读取当前Assets下的Manifest。
                    if (string.IsNullOrEmpty(LWUtility.downloadURL)) {
                        LWUtility.downloadURL = manifest.downloadURL;
                    }                      
                    Bundles.activeVariants = manifest.activeVariants;
                    _bundles = manifest.bundles;
                    var dirs = manifest.dirs;
                    _bundleAssets = new Dictionary<string, int>(manifest.assets.Length);
                    for (int i = 0, max = manifest.assets.Length; i < max; i++)
                    {
                        var item = manifest.assets[i];
                        _bundleAssets[string.Format("{0}/{1}", dirs[item.dir], item.name)] = item.bundle;
                    }

                    assetRequest.Release();
                    bundleRequest.Release();
                    bundleRequest = null;
                    return assetsManagerRequest;
                }
            }
            else
            {
                return assetsManagerRequest;
            }
        }

        public string[] GetAllDependencies(string path)
        {
            string assetBundleName;
            return GetAssetBundleName(path, out assetBundleName) ? Bundles.GetAllDependencies(assetBundleName) : null;
        }

        public SceneAssetRequest LoadScene(string path, bool async, bool addictive)
        {
            var asset = async ? new SceneAssetAsyncRequest(path, addictive) : new SceneAssetRequest(path, addictive);
            GetAssetBundleName(path, out asset.assetBundleName);
            asset.Load();
            asset.Retain();
            _assets.Add(asset);
            return asset;
        }

        public void UnloadScene(string path)
        {
            for (int i = 0, max = _assets.Count; i < max; i++)
            {
                var item = _assets[i];
                if (!item.url.Equals(path))
                    continue;
                Unload(item);
                break;
            }
        }
        public AssetRequest Load<T>(string path)
        {
            return Load(path, typeof(T), false);
        }
        public AssetRequest Load(string path, Type type)
        {
            return Load(path, type, false);
        }
        public AssetRequest LoadAsync<T>(string path)
        {
            return Load(path, typeof(T), true);
        }
        public AssetRequest LoadAsync(string path, Type type)
        {
            return Load(path, type, true);
        }

       
        public async UniTask<AssetRequest> LoadAsyncTask<T>(string path)
        {
            AssetRequest request = LoadAsync<T>(path);
            await UniTask.WaitUntil(() =>
            {
                return request.isDone;
            });
            return request;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public void Unload(AssetRequest asset)
        {
            asset.Release();
            for (var i = 0; i < _unusedAssets.Count; i++)
            {
                var item = _unusedAssets[i];
                if (!item.url.Equals(asset.url))
                    continue;
                item.Unload();
                _unusedAssets.RemoveAt(i);
                return;
            }
        }



        private AssetRequest Load(string path, Type type, bool async)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("invalid path");
                return null;
            }

            for (int i = 0, max = _assets.Count; i < max; i++)
            {
                var item = _assets[i];
                if (!item.url.Equals(path))
                    continue;
                item.Retain();
                return item;
            }

            string assetBundleName;
            AssetRequest asset;
            if (GetAssetBundleName(path, out assetBundleName))
            {
                asset = async ? new BundleAssetAsyncRequest(assetBundleName) : new BundleAssetRequest(assetBundleName);
            }
            else
            {
                if (path.StartsWith("http://", StringComparison.Ordinal) ||
                    path.StartsWith("https://", StringComparison.Ordinal) ||
                    path.StartsWith("file://", StringComparison.Ordinal) ||
                    path.StartsWith("ftp://", StringComparison.Ordinal) ||
                    path.StartsWith("jar:file://", StringComparison.Ordinal))
                {
                    asset = new WebAssetRequest();
                }

                else
                {
                    asset = new AssetRequest();

                }

            }

            asset.url = path;
            asset.assetType = type;
            _assets.Add(asset);
            asset.Load();
            asset.Retain();

            LWDebug.Log(string.Format("Load->{0}|{1}", path, assetBundleName));
            return asset;
        }

        private bool GetAssetBundleName(string path, out string assetBundleName)
        {
            if (path.Equals(LWUtility.AssetsManifestAsset))
            {
                assetBundleName = Path.GetFileNameWithoutExtension(path).ToLower();
                return true;
            }

            assetBundleName = null;
            int bundle;
            if (!_bundleAssets.TryGetValue(path, out bundle))
                return false;
            assetBundleName = _bundles[bundle];
            return true;
        }

        private string Bundles_overrideBaseDownloadingURL(string bundleName)
        {
            return !File.Exists(Path.Combine(updatePath, bundleName)) ? null : updatePath;
        }
    }
}


