using LWFramework.Asset;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
namespace LWFramework.Core {
    [ManagerClass(ManagerType.Normal)]
    public class UpdateManager : IManager
    {
        public enum State
        {
            Wait,
            Checking,
            WaitDownload,
            Downloading,
            Completed,
            Error,
        }

        public State state;

        public Action onCompleted;

        public Action<string, float> onProgress;

        public Action<string> onError;

        private Dictionary<string, string> _versions = new Dictionary<string, string>();
        private Dictionary<string, string> _serverVersions = new Dictionary<string, string>();
        private readonly List<Download> _downloads = new List<Download>();
        private int _downloadIndex;

        [SerializeField] string versionsTxt = "versions.txt";

        private void OnError(string e)
        {
            if (onError != null)
            {
                onError(e);
            }
            LWDebug.LogError("Android 出现 未知错误 需要将target api 修改为 非最高" + e);
            state = State.Error;
        }
        public async void Check()
        {
            if (state != State.Wait) { return; }
            //不连服务器
            if (!LWUtility.GlobalConfig.connServer) {
              
                AssetsManagerRequest _Request = await MainManager.Instance.GetManager<AssetsManager>().InitializeAsync();
                if (_Request.isSuccess)
                {
                    onCompleted();
                }
                else {
                    onError(_Request.error);
                }
                return;
            }
            state = State.Checking;
            AssetsManagerRequest request = await MainManager.Instance.GetManager<AssetsManager>().InitializeAsync();
            if (request.isSuccess)
            {
                //获取本地的版本文件路径
                var path = LWUtility.GetRelativePath4Update(versionsTxt);
                //当前不存在该文件，即第一次打开应用
                if (!File.Exists(path))
                {
                    //加载StreamingAssets下的的版本文件
                    var asset = MainManager.Instance.GetManager<AssetsManager>().LoadAsync(LWUtility.GetWebUrlFromDataPath(versionsTxt), typeof(TextAsset));
                    asset.completed += delegate
                    {
                        if (asset.error != null)
                        {
                            LoadVersions(string.Empty);
                            return;
                        }

                        var dir = Path.GetDirectoryName(path);
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        //将StreamingAssets下的版本文件写入到persistentDataPath
                        File.WriteAllText(path, asset.text);
                        LoadVersions(asset.text);
                        asset.Release();
                    };
                }
                else
                {
                    LoadVersions(File.ReadAllText(path));
                }
            }
            else {
                // 本地没有文件，直接更新
                LoadVersions(string.Empty);
            }
        }
        private void Download()
        {
            _downloadIndex = 0;
            _downloads[_downloadIndex].Start();
            state = State.Downloading;
        }

        

        //下载完成
        private async void DownloadComplete()
        {
            DownloadTxtFile.Save();

            if (_downloads.Count > 0)
            {
                for (int i = 0; i < _downloads.Count; i++)
                {
                    var item = _downloads[i];
                    if (!item.isDone)
                    {
                        break;
                    }
                    else
                    {
                        if (_serverVersions.ContainsKey(item.path))
                        {
                            _versions[item.path] = _serverVersions[item.path];
                        }
                    }
                }

                StringBuilder sb = new StringBuilder();
                foreach (var item in _versions)
                {
                    sb.AppendLine(string.Format("{0}:{1}", item.Key, item.Value));
                }

                var path = LWUtility.GetRelativePath4Update(versionsTxt);
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.WriteAllText(path, sb.ToString());
                AssetsManagerRequest assetsManagerRequest = await MainManager.Instance.GetManager<AssetsManager>().InitializeAsync();
                if (assetsManagerRequest.isSuccess)
                {
                    onCompleted();
                }
                else
                {
                    onError(assetsManagerRequest.error);
                }
                state = State.Completed;

                return;
            }

            if (onCompleted != null)
            {
                onCompleted();
            }

            state = State.Completed;
        }



        private void LoadVersions(string text)
        {
            //解析本地版本文件
            LoadText2Map(text, ref _versions);
            //加载服务器版本文件
            var asset = MainManager.Instance.GetManager<AssetsManager>().LoadAsync(LWUtility.GetDownloadURL(versionsTxt), typeof(TextAsset));

            asset.completed += delegate {
                if (asset.error != null)
                {
                    OnError(asset.error);
                    return;
                }
                //解析服务器版本文件
                LoadText2Map(asset.text, ref _serverVersions);
                foreach (var item in _serverVersions)
                {
                    string ver;
                    //对比本地版本与服务器版本
                    if (!_versions.TryGetValue(item.Key, out ver) || !ver.Equals(item.Value))
                    {
                        var downloader = new Download();
                        downloader.url = LWUtility.GetDownloadURL(item.Key);
                        downloader.path = item.Key;
                        downloader.versionHash = item.Value;
                        downloader.savePath = LWUtility.GetRelativePath4Update(item.Key);
                        _downloads.Add(downloader);
                    }
                }
                //不需要下载
                if (_downloads.Count == 0)
                {
                    DownloadComplete();
                }
                else
                {
                    //下载对应的依赖关系文件
                    var downloader = new Download();
                    downloader.url = LWUtility.GetDownloadURL(LWUtility.GetPlatform());
                    downloader.path = LWUtility.GetPlatform();
                    downloader.savePath = LWUtility.GetRelativePath4Update(LWUtility.GetPlatform());
                    _downloads.Add(downloader);
                    state = State.WaitDownload;

                    Debug.Log("开始下载资源");
                    Download();
                }
            };
        }
        /// <summary>
        /// 解析versions.txt文件
        /// </summary>
        /// <param name="text"></param>
        /// <param name="map"></param>
        private static void LoadText2Map(string text, ref Dictionary<string, string> map)
        {
            map.Clear();
            using (var reader = new StringReader(text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = line.Split(':');
                    if (fields.Length > 1)
                    {
                        map.Add(fields[0], fields[1]);
                    }
                }
            }
        }

        public void Init()
        {

            state = State.Wait;
            DownloadTxtFile.Load();
            
        }
        //处理下载
        public void Update()
        {
            if (state == State.Downloading)
            {
                if (_downloadIndex < _downloads.Count)
                {
                    var download = _downloads[_downloadIndex];
                    download.Update();
                    if (download.isDone)
                    {
                        _downloadIndex = _downloadIndex + 1;
                        if (_downloadIndex == _downloads.Count)
                        {
                            DownloadComplete();
                        }
                        else
                        {
                            _downloads[_downloadIndex].Start();
                        }
                    }
                    else
                    {
                        if (onProgress != null)
                        {
                            onProgress.Invoke(download.url, download.progress);
                        }
                    }
                }
            }
        }

        public void LateUpdate()
        {

        }
    }

}
