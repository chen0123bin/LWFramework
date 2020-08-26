using LWFramework;
using LWFramework.Asset;
using LWFramework.Core;
using LWFramework.FMS;
using LWFramework.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class Startup : MonoBehaviour
{
    public static Action OnStart { get; set; }
    public static Action OnUpdate { get; set; }
    public static Action OnLateUpdate { get; set; }
    void Awake()
    {
        DontDestroyOnLoad(gameObject);      
        MainManager.Instance.Init();
        MainManager.Instance.GetManager<UpdateManager>().onCompleted = OnCompleted;
        MainManager.Instance.GetManager<UpdateManager>().onProgress = OnProgress;
        MainManager.Instance.GetManager<UpdateManager>().onError = OnError;
        MainManager.Instance.GetManager<UpdateManager>().Check();
    }
 
    private void OnError(string obj)
    {
        LWDebug.Log(obj);
      //  MainManager.Instance.StartProcedure();
        StartCoroutine(MainManager.Instance.GetManager<HotfixManager>().IE_LoadScript(LWUtility.GlobalConfig.hotfixCodeRunMode));
    }

    private void OnProgress(string arg1, float arg2)
    {
        //Debug.Log(arg1 + "   " + arg2);
    }

    private void OnCompleted()
    {
        LWDebug.Log("下载完成");
     //   MainManager.Instance.StartProcedure();
        // Launch();
        //启动热更代码
         StartCoroutine(MainManager.Instance.GetManager<HotfixManager>().IE_LoadScript(LWUtility.GlobalConfig.hotfixCodeRunMode));     
    }


  
    // Update is called once per frame
    void Update()
    {
        MainManager.Instance.Update();
        if (OnUpdate != null)
        {
            OnUpdate();
        }
    }
    private void LateUpdate()
    {
        MainManager.Instance.LateUpdate();
        if (OnLateUpdate != null)
        {
            OnLateUpdate();
        }
    }

    void OnDestroy()
    {
    }

    private void OnApplicationQuit()
    {
        //MainManager.Instance.GetManager<GlobalMessageManager>().Dispatcher("OnApplicationQuit");
    }



}
