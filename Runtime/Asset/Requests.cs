using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace LWFramework.Asset
{
    public enum LoadState
    {
        Init,
        LoadAssetBundle,
        LoadAsset,
        Loaded,
        Unload,
    }

    public class AssetRequest : Reference, IEnumerator
    {
        private List<Object> _requires;
        public Type assetType;
        public string url;
        public LoadState loadState { get; protected set; }

        public AssetRequest()
        {
            asset = null;
            loadState = LoadState.Init;
        }

        public virtual bool isDone
        {
            get { return true; }
        }

        public virtual float progress
        {
            get { return 1; }
        }

        public virtual string error { get; protected set; }

        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string text { get; protected set; }

        // ReSharper disable once MemberCanBeProtected.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public byte[] bytes { get; protected set; }

        public Object asset { get; internal set; }

        private bool checkRequires
        {
            get { return _requires != null; }
        }

        public void Require(Object obj)
        {
            if (_requires == null)
                _requires = new List<Object>();

            _requires.Add(obj);
            Retain();
        }

        // ReSharper disable once IdentifierTypo
        public void Dequire(Object obj)
        {
            if (_requires == null)
                return;

            if (_requires.Remove(obj))
                Release();
        }

        private void UpdateRequires()
        {
            for (var i = 0; i < _requires.Count; i++)
            {
                var item = _requires[i];
                if (item != null)
                    continue;
                Release();
                _requires.RemoveAt(i);
                i--;
            }

            if (_requires.Count == 0)
                _requires = null;
        }

        internal virtual void Load()
        {
            if (LWUtility.loadDelegate != null)
                asset = LWUtility.loadDelegate(url, assetType);
        }



        internal virtual void Unload()
        {
            if (asset == null)
                return;
            if (!(asset is GameObject))
                Resources.UnloadAsset(asset);

            asset = null;
        }

        internal bool Update()
        {
            if (checkRequires)
                UpdateRequires();
            if (!isDone)
                return true;
            if (completed == null)
                return false;
            try
            {
                completed.Invoke(this);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            completed = null;
            return false;
        }

        // ReSharper disable once InconsistentNaming
        public event Action<AssetRequest> completed;

        #region IEnumerator implementation

        public bool MoveNext()
        {
            return !isDone;
        }

        public void Reset()
        {
        }

        public object Current
        {
            get { return null; }
        }

        #endregion
    }
    public class AssetsManagerRequest 
    {
        public string error;
        public bool isSuccess;
    }
    /**  示例
    public class AssetsInitRequest : AssetRequest
    {
        public override float progress
        {
            get
            {
                switch (loadState)
                {
                    case LoadState.LoadAsset:
                        return 0.5f;

                    case LoadState.Loaded:
                        return 1f;
                    default:
                        break;
                }
                return string.IsNullOrEmpty(error) ? 1f : 0f;
            }
        }

        public override bool isDone
        {
            get
            {
                if (!string.IsNullOrEmpty(error))
                {
                    return true;
                }
                return loadState == LoadState.Loaded;
            }
        }

        internal override void Load()
        {
            if (LWUtility.GlobalConfig.assetBundleMode)
            {
                var bundleRequest = Bundles.Load(Assets.platform, true, true);
                loadState = LoadState.LoadAssetBundle;
                bundleRequest.completed += BundleRequest_completed;
            }
            else
            {
                loadState = LoadState.Loaded;
            }
        }

        private void BundleRequest_completed(AssetRequest request)
        {
            var bundleRequest = request as BundleRequest;
            if (!string.IsNullOrEmpty(bundleRequest.error))
            {
                error = bundleRequest.error;
                bundleRequest.Release();
                return;
            }
            var manifest = bundleRequest.assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (manifest == null)
            {
                error = "AssetBundleManifest == null";
                bundleRequest.Release();
                return;
            }
            else
            {
                Bundles.manifest = manifest;
                bundleRequest.assetBundle.Unload(false);
                bundleRequest.assetBundle = null;
                var assetRequest = Assets.LoadAssetAsync(Assets.AssetsManifestAsset, typeof(Manifest));
                assetRequest.completed = AssetRequest_completed;
                loadState = LoadState.LoadAsset;
                bundleRequest.Release();
            }
        }

        private void AssetRequest_completed(AssetRequest request)
        {
            var manifest = request.asset as Manifest;
            if (manifest == null)
            {
                error = "manifest == null";
            }
            else
            {
                Assets.Init(manifest);
            }
            request.Release();
            loadState = LoadState.Loaded;
        }

        internal override void Unload()
        {

        }
    }
    */
    public class BundleAssetRequest : AssetRequest
    {
        protected readonly string assetBundleName;
        protected BundleRequest bundle;

        public BundleAssetRequest(string bundle)
        {
            assetBundleName = bundle;
        }

        internal override void Load()
        {
            bundle = Bundles.Load(assetBundleName);
            var assetName = Path.GetFileName(url);
            asset = bundle.assetBundle.LoadAsset(assetName, assetType);
        }

        internal override void Unload()
        {
            if (bundle != null)
            {
                bundle.Release();
                bundle = null;
            }

            asset = null;
        }
    }

    public class BundleAssetAsyncRequest : BundleAssetRequest
    {
        private AssetBundleRequest _request;

        public BundleAssetAsyncRequest(string bundle)
            : base(bundle)
        {
        }

        public override bool isDone
        {
            get
            {
                
                if (error != null || bundle.error != null)
                    return true;

                for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                {
                    var item = bundle.dependencies[i];
                    if (item.error != null)
                        return true;
                }

                switch (loadState)
                {
                    case LoadState.Init:
                        return false;
                    case LoadState.Loaded:
                        return true;
                    case LoadState.LoadAssetBundle:
                        {
                            if (!bundle.isDone)
                                return false;

                            for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                            {
                                var item = bundle.dependencies[i];
                                if (!item.isDone)
                                    return false;
                            }

                            if (bundle.assetBundle == null)
                            {
                                error = "assetBundle == null";
                                return true;
                            }

                            var assetName = Path.GetFileName(url);
                            bundle.assetBundle.LoadAssetAsync(assetName);
                            _request = bundle.assetBundle.LoadAssetAsync(assetName, assetType);
                            loadState = LoadState.LoadAsset;
                            break;
                        }
                    case LoadState.Unload:
                        break;
                    case LoadState.LoadAsset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
               
                if (loadState != LoadState.LoadAsset)
                    return false;

                if (_request.isDone)
                {
                    asset = _request.asset;
                    loadState = LoadState.Loaded;
                    return true;
                }
                else {
                    return false;
                }
                
               

            }
        }

        public override float progress
        {
            get
            {
                var bundleProgress = bundle.progress;
                if (bundle.dependencies.Count <= 0)
                    return bundleProgress * 0.3f + (_request != null ? _request.progress * 0.7f : 0);
                for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                {
                    var item = bundle.dependencies[i];
                    bundleProgress += item.progress;
                }

                return bundleProgress / (bundle.dependencies.Count + 1) * 0.3f +
                       (_request != null ? _request.progress * 0.7f : 0);
            }
        }

        internal override void Load()
        {
            bundle = Bundles.LoadAsync(assetBundleName);
            loadState = LoadState.LoadAssetBundle;
        }

        internal override void Unload()
        {
            _request = null;
            loadState = LoadState.Unload;
            base.Unload();
        }
    }

    public class SceneAssetRequest : AssetRequest
    {
        protected readonly LoadSceneMode loadSceneMode;
        protected readonly string sceneName;
        public string assetBundleName;
        protected BundleRequest bundle;

        public SceneAssetRequest(string path, bool addictive)
        {
            url = path;
            sceneName = Path.GetFileNameWithoutExtension(url);
            loadSceneMode = addictive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        }

        public override float progress
        {
            get { return 1; }
        }

        internal override void Load()
        {
            if (!string.IsNullOrEmpty(assetBundleName))
            {
                bundle = Bundles.Load(assetBundleName);
                if (bundle != null)
                    SceneManager.LoadScene(sceneName, loadSceneMode);
            }
            else
            {
                SceneManager.LoadScene(sceneName, loadSceneMode);
            }
        }

        internal override void Unload()
        {
            if (bundle != null)
                bundle.Release();

            if (SceneManager.GetSceneByName(sceneName).isLoaded)
                SceneManager.UnloadSceneAsync(sceneName);

            bundle = null;
        }
    }

    public class SceneAssetAsyncRequest : SceneAssetRequest
    {
        private AsyncOperation _request;

        public SceneAssetAsyncRequest(string path, bool addictive)
            : base(path, addictive)
        {
        }

        public override float progress
        {
            get
            {
                if (bundle == null)
                    return _request == null ? 0 : _request.progress;

                var bundleProgress = bundle.progress;
                if (bundle.dependencies.Count <= 0)
                    return bundleProgress * 0.3f + (_request != null ? _request.progress * 0.7f : 0);
                for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                {
                    var item = bundle.dependencies[i];
                    bundleProgress += item.progress;
                }

                return bundleProgress / (bundle.dependencies.Count + 1) * 0.3f +
                       (_request != null ? _request.progress * 0.7f : 0);
            }
        }

        public override bool isDone
        {
            get
            {
                switch (loadState)
                {
                    case LoadState.Loaded:
                        return true;
                    case LoadState.LoadAssetBundle:
                        {
                            if (bundle == null || bundle.error != null)
                                return true;

                            for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                            {
                                var item = bundle.dependencies[i];
                                if (item.error != null)
                                    return true;
                            }

                            if (!bundle.isDone)
                                return false;

                            for (int i = 0, max = bundle.dependencies.Count; i < max; i++)
                            {
                                var item = bundle.dependencies[i];
                                if (!item.isDone)
                                    return false;
                            }

                            _request = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                            loadState = LoadState.LoadAsset;
                            break;
                        }
                    case LoadState.Unload:
                        break;
                    case LoadState.LoadAsset:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (loadState != LoadState.LoadAsset)
                    return false;


                if (!_request.isDone)
                    return false;
                loadState = LoadState.Loaded;
                return true;
            }
        }

        internal override void Load()
        {
            if (!string.IsNullOrEmpty(assetBundleName))
            {
                bundle = Bundles.LoadAsync(assetBundleName);
                loadState = LoadState.LoadAssetBundle;
            }
            else
            {
                _request = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                loadState = LoadState.LoadAsset;
            }
        }

        internal override void Unload()
        {
            base.Unload();
            _request = null;
        }
    }

    public class WebAssetRequest : AssetRequest
    {
#if UNITY_2018_3_OR_NEWER
        private UnityWebRequest _www;
#else
        private WWW _www;
#endif

        public override bool isDone
        {
            get
            {
                if (loadState == LoadState.Init)
                    return false;
                if (loadState == LoadState.Loaded)
                    return true;

                if (loadState == LoadState.LoadAsset)
                {
                    if (_www == null || !string.IsNullOrEmpty(_www.error))
                        return true;

                    if (_www.isDone)
                    {
#if UNITY_2018_3_OR_NEWER
                        if (assetType != typeof(Texture2D))
                        {
                            if (assetType != typeof(TextAsset))
                            {
                                if (assetType != typeof(AudioClip))
                                    bytes = _www.downloadHandler.data;
                                else
                                    asset = DownloadHandlerAudioClip.GetContent(_www);
                            }
                            else
                            {
                                text = _www.downloadHandler.text;
                            }
                        }
                        else
                        {
                            asset = DownloadHandlerTexture.GetContent(_www);
                        }
#else
                        if (assetType != typeof(Texture2D))
                        {
                            if (assetType != typeof(TextAsset))
                            {
                                if (assetType != typeof(AudioClip))
                                    bytes = _www.bytes;
                                else
                                    asset = _www.GetAudioClip();
                            }
                            else
                            {
                                text = _www.text;
                            }
                        }
                        else
                        {
                            asset = _www.texture;
                        } 
#endif
                        loadState = LoadState.Loaded;
                        return true;
                    }
                    return false;
                }

                return true;
            }
        }

        public override string error
        {
            get {return _www.error; }
        }

        public override float progress
        {
#if UNITY_2018_3_OR_NEWER
            get { return _www.downloadProgress; }
#else
            get { return _www.progress;}
#endif
        }

        internal override void Load()
        {
#if UNITY_2018_3_OR_NEWER
            if (assetType == typeof(AudioClip))
            {
                _www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
            }
            else if (assetType == typeof(Texture2D))
            {
                _www = UnityWebRequestTexture.GetTexture(url);
            }
            else
            {
                _www = new UnityWebRequest(url);
                _www.downloadHandler = new DownloadHandlerBuffer();
            }
            _www.SendWebRequest();
#else
            _www = new WWW(name);
#endif
            loadState = LoadState.LoadAsset;
        }

        internal override void Unload()
        {
            if (asset != null)
            {
                Object.Destroy(asset);
                asset = null;
            }
            if (_www != null)
                _www.Dispose();

            bytes = null;
            text = null;
        }
    }


    public class BundleRequest : AssetRequest
    {
        public readonly List<BundleRequest> dependencies = new List<BundleRequest>();

        public AssetBundle assetBundle
        {
            get { return asset as AssetBundle; }
            internal set { asset = value; }
        }

        internal override void Load()
        {
            asset = AssetBundle.LoadFromFile(url);
            if (assetBundle == null)
                error = url + " LoadFromFile failed.";
        }

        internal override void Unload()
        {
            if (assetBundle == null)
                return;
            assetBundle.Unload(true);
            assetBundle = null;
        }
    }

    public class BundleAsyncRequest : BundleRequest
    {
        private AssetBundleCreateRequest _request;

        public override bool isDone
        {
            get
            {

                if (loadState == LoadState.Init)
                    return false;

                if (loadState == LoadState.Loaded)
                {

                    return true;
                }
                if (loadState == LoadState.LoadAssetBundle && _request.isDone)
                {
                    asset = _request.assetBundle;
                    if (_request.assetBundle == null)
                    {
                        error = string.Format("unable to load assetBundle:{0}", url);
                    }
                    loadState = LoadState.Loaded;
                }

                return _request == null || _request.isDone;
            }
        }

        public override float progress
        {
            get { return _request != null ? _request.progress : 0f; }
        }

        internal override void Load()
        {
            _request = AssetBundle.LoadFromFileAsync(url);
            if (_request == null)
            {
                error = url + " LoadFromFile failed.";
                return;
            }

            loadState = LoadState.LoadAssetBundle;
        }

        internal override void Unload()
        {
            if (_request != null)
            {
                _request = null;
            }
            loadState = LoadState.Unload;
            base.Unload();
        }
    }

    public class WebBundleRequest : BundleRequest
    {
#if UNITY_2018_3_OR_NEWER
        private UnityWebRequest _request;


#else
		private WWW _request;
#endif
        public bool cache;
        public Hash128 hash;

        public override string error
        {
            get { return _request != null ? _request.error : null; }
        }

        public override bool isDone
        {
            get
            {
                if (loadState == LoadState.Init)
                    return false;

                if (_request == null || loadState == LoadState.Loaded)
                    return true;
#if UNITY_2018_3_OR_NEWER
                if (_request.isDone)
                {
                    assetBundle = DownloadHandlerAssetBundle.GetContent(_request);
                    loadState = LoadState.Loaded;
                }
#else
                if (_request.isDone)
                {
                    assetBundle = _request.assetBundle;
                    loadState = LoadState.Loaded;
                }
#endif

                return _request.isDone;
            }
        }

        public override float progress
        {
#if UNITY_2018_3_OR_NEWER
            get { return _request != null ? _request.downloadProgress : 0f; }
#else
			get { return _request != null ? _request.progress : 0f; }
#endif
        }

        internal override void Load()
        {
#if UNITY_2018_3_OR_NEWER
            _request = cache ? UnityWebRequestAssetBundle.GetAssetBundle(url, hash) : UnityWebRequestAssetBundle.GetAssetBundle(url);
            _request.SendWebRequest();
#else
            _request = cache ? WWW.LoadFromCacheOrDownload(name, hash) : new WWW(name);
#endif
            loadState = LoadState.LoadAssetBundle;

        }

        internal override void Unload()
        {
            if (_request != null)
            {
                _request.Dispose();
                _request = null;
            }
            loadState = LoadState.Unload;
            base.Unload();
        }
    }
}