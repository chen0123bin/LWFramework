using LWFramework;
using LWFramework.Asset;
using LWFramework.Message;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace LWFramework.Core {
    public enum HotfixCodeRunMode
    {
#if ILRUNTIME
        ByILRuntime = 0,
#endif
        ByReflection = 1,
        ByCode = 2,
    }
    /// <summary>
    /// 热更环境初始化处理
    /// </summary>
    [ManagerClass(ManagerType.Normal)]
    public class HotfixManager : IManager
    {
        public Assembly Assembly { get; private set; }
        public void Init()
        {

        }

        public void LateUpdate()
        {

        }

        public void Update()
        {

        }

        /// <summary>
        /// 加载Dll热更脚本
        /// </summary>
        /// <param name="root">路径</param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public IEnumerator IE_LoadScript(HotfixCodeRunMode mode)
        {
            string dllPath = "";
            if (Application.isEditor)
            {
                //这里情况比较复杂,Mobile上基本认为Persistent才支持File操作,
                dllPath = Application.streamingAssetsPath + "/Hotfix/" + LWUtility.HotfixFileName;
            }
            else
            {
                //这里情况比较复杂,Mobile上基本认为Persistent才支持File操作,
                //可寻址目录也只有 StreamingAsset
                var firstPath = FileTool.CombinePaths(LWUtility.updatePath, LWUtility.HotfixFileName); //LWUtility.updatePath + "/" + LWUtility.HotfixFileName;
                var secondPath = FileTool.CombinePaths(Application.streamingAssetsPath, LWUtility.AssetBundles, LWUtility.GetPlatform(), LWUtility.HotfixFileName);// Application.streamingAssetsPath + "/" + LWUtility.AssetBundles + "/" + LWUtility.GetPlatform() + "/" + LWUtility.HotfixFileName;
                if (!File.Exists(firstPath))
                {
                    var request = UnityWebRequest.Get(secondPath);
                    LWDebug.Log("firstPath:" + firstPath);
                    LWDebug.Log("secondPath:" + secondPath);
                    yield return request.SendWebRequest();

                    if (request.isDone && request.error == null)
                    {
                        LWDebug.Log("request.downloadHandler.data:" + request.downloadHandler.data.Length);
                        LWDebug.Log("拷贝dll成功:" + firstPath);
                        byte[] results = request.downloadHandler.data;
                        FileTool.WriteByteToFile(firstPath, results, "");
                    }
                  
                }

                dllPath = firstPath;
            }

            LWDebug.Log("Dll路径:" + dllPath);
            //反射执行
            if (mode == HotfixCodeRunMode.ByReflection)
            {
                var bytes = File.ReadAllBytes(dllPath);
                var mdb = dllPath + ".mdb";
                if (File.Exists(mdb))
                {
                    var bytes2 = File.ReadAllBytes(mdb);
                    Assembly = Assembly.Load(bytes, bytes2);
                }
                else
                {
                    Assembly = Assembly.Load(bytes);
                }

                //Debug.Log("反射模式,开始执行Start");
                //var type = Assembly.GetType("StartupBridge_Hotfix");
                //var method = type.GetMethod("Start", BindingFlags.Public | BindingFlags.Static);
                //method.Invoke(null, new object[] { false });
                StartupBridge_Hotfix.StartReflection(Assembly);
            }
#if ILRUNTIME
            //解释执行
            else if (mode == HotfixCodeRunMode.ByILRuntime)
            {
                //解释执行模式
            //    ILRuntimeHelper.LoadHotfix(dllPath);
           //     ILRuntimeHelper.AppDomain.Invoke("StartupBridge_Hotfix", "Start", null, new object[] { true });
            }
#endif
            else if (mode == HotfixCodeRunMode.ByCode)
            {
                LWDebug.Log("内置code模式!",LogColor.green);
                StartupBridge_Hotfix.StartCode();               
                //反射调用，防止编译报错
                //Assembly assembly = Assembly.GetExecutingAssembly();
                //var type = assembly.GetType("StartupBridge_Hotfix");
                //var method = type.GetMethod("Start", BindingFlags.Public | BindingFlags.Static);
                //method.Invoke(null, new object[] { false });
            }
        }



    }

}
