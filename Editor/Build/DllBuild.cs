using LWFramework.Asset;
using LWFramework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
public class DllBuild
{
    /// <summary>
    /// 打包完成，返回一个bool表示成功还是失败
    /// </summary>
    public event Action<DllBuild, bool> onFinished;
    string _outputDir;
    string _outputAssemblyPath;
    /// <summary>
    /// 代码地址
    /// </summary>
    public string assemblyPath
    {
        get
        {
            return _outputAssemblyPath;
        }
    }
    public DllBuild(string outputDir)
    {
        _outputDir = outputDir;
        if (false == Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        _outputAssemblyPath = FileTool.CombinePaths(outputDir, LWUtility.HotfixFileName);
    }
    private static string define =
    @"ILRUNTIME;DEBUG;TRACE;UNITY_5_3_OR_NEWER;UNITY_5_4_OR_NEWER;UNITY_5_5_OR_NEWER;UNITY_5_6_OR_NEWER;UNITY_2017_1_OR_NEWER;UNITY_2017_2_OR_NEWER;UNITY_2017_3_OR_NEWER;UNITY_2017_4_OR_NEWER;UNITY_2018_1_OR_NEWER;UNITY_2018_2_OR_NEWER;UNITY_2018_3_OR_NEWER;UNITY_2018_3_8;UNITY_2018_3;UNITY_2018;PLATFORM_ARCH_64;UNITY_64;UNITY_INCLUDE_TESTS;ENABLE_AUDIO;ENABLE_CACHING;ENABLE_CLOTH;ENABLE_DUCK_TYPING;ENABLE_MICROPHONE;ENABLE_MULTIPLE_DISPLAYS;ENABLE_PHYSICS;ENABLE_SPRITES;ENABLE_GRID;ENABLE_TILEMAP;ENABLE_TERRAIN;ENABLE_TEXTURE_STREAMING;ENABLE_DIRECTOR;ENABLE_UNET;ENABLE_LZMA;ENABLE_UNITYEVENTS;ENABLE_WEBCAM;ENABLE_WWW;ENABLE_CLOUD_SERVICES_COLLAB;ENABLE_CLOUD_SERVICES_COLLAB_SOFTLOCKS;ENABLE_CLOUD_SERVICES_ADS;ENABLE_CLOUD_HUB;ENABLE_CLOUD_PROJECT_ID;ENABLE_CLOUD_SERVICES_USE_WEBREQUEST;ENABLE_CLOUD_SERVICES_UNET;ENABLE_CLOUD_SERVICES_BUILD;ENABLE_CLOUD_LICENSE;ENABLE_EDITOR_HUB;ENABLE_EDITOR_HUB_LICENSE;ENABLE_WEBSOCKET_CLIENT;ENABLE_DIRECTOR_AUDIO;ENABLE_DIRECTOR_TEXTURE;ENABLE_TIMELINE;ENABLE_EDITOR_METRICS;ENABLE_EDITOR_METRICS_CACHING;ENABLE_MANAGED_JOBS;ENABLE_MANAGED_TRANSFORM_JOBS;ENABLE_MANAGED_ANIMATION_JOBS;INCLUDE_DYNAMIC_GI;INCLUDE_GI;ENABLE_MONO_BDWGC;PLATFORM_SUPPORTS_MONO;RENDER_SOFTWARE_CURSOR;INCLUDE_PUBNUB;ENABLE_VIDEO;ENABLE_CUSTOM_RENDER_TEXTURE;ENABLE_LOCALIZATION;PLATFORM_STANDALONE_WIN;PLATFORM_STANDALONE;UNITY_STANDALONE_WIN;UNITY_STANDALONE;ENABLE_SUBSTANCE;ENABLE_RUNTIME_GI;ENABLE_MOVIES;ENABLE_NETWORK;ENABLE_CRUNCH_TEXTURE_COMPRESSION;ENABLE_UNITYWEBREQUEST;ENABLE_CLOUD_SERVICES;ENABLE_CLOUD_SERVICES_ANALYTICS;ENABLE_CLOUD_SERVICES_PURCHASING;ENABLE_CLOUD_SERVICES_CRASH_REPORTING;ENABLE_OUT_OF_PROCESS_CRASH_HANDLER;ENABLE_EVENT_QUEUE;ENABLE_CLUSTER_SYNC;ENABLE_CLUSTERINPUT;ENABLE_VR;ENABLE_AR;ENABLE_WEBSOCKET_HOST;ENABLE_MONO;NET_4_6;ENABLE_PROFILER;UNITY_ASSERTIONS;ENABLE_UNITY_COLLECTIONS_CHECKS;ENABLE_BURST_AOT;UNITY_TEAM_LICENSE;UNITY_PRO_LICENSE;ODIN_INSPECTOR;UNITY_POST_PROCESSING_STACK_V2;CSHARP_7_OR_LATER;CSHARP_7_3_OR_NEWER";
    public void Execute()
    {
        #region CS DLL引用搜集处理
        List<string> csFiles = new List<string>();
        csFiles = FindDLLByCSPROJ("Assembly-CSharp.csproj");
#if !ILRUNTIME
        var baseCs = csFiles.FindAll(f => !f.Contains("@hotfix") && !f.Contains("@h_Mono") && f.EndsWith(".cs"));
#else
        var baseCs = csFiles.FindAll(f => !f.Contains("@hotfix") && f.EndsWith(".cs"));
#endif
        var hotfixCs = csFiles.FindAll(f => f.Contains("@hotfix") && f.EndsWith(".cs"));
#if !ILRUNTIME
        var hotfixMonoCs = csFiles.FindAll(f => f.Contains("@h_Mono") && f.EndsWith(".cs"));
        hotfixCs.AddRange(hotfixMonoCs);
#endif
        #endregion
        //var scriptPaths = Directory.GetFiles(_sourcesDir, "*.cs", SearchOption.AllDirectories);
        var ab = new AssemblyBuilder(_outputAssemblyPath, hotfixCs.ToArray());
        ab.compilerOptions = new ScriptCompilerOptions();
        ab.flags = AssemblyBuilderFlags.DevelopmentBuild | AssemblyBuilderFlags.EditorAssembly;
        ab.additionalReferences = GetDepends();
        ab.additionalDefines = define.Split(';');
        ab.buildFinished += OnFinished;
        if (false == ab.Build())
        {
            onFinished?.Invoke(this, false);
            onFinished = null;
        }
    }
    public void GetAllScripts()
    {

    }
    string[] GetDepends()
    {
        //依赖Assets下的DLL
        var assetDir = Application.dataPath;
        var dllList0 = Directory.GetFiles(assetDir, "*.dll", SearchOption.AllDirectories);
        //依赖Library/ScriptAssemblies下的DLL
        var projectDir = Directory.GetParent(assetDir).FullName;
        var dllList1 = Directory.GetFiles(FileTool.CombineDirs(true, projectDir, "Library", "ScriptAssemblies"), "*.dll", SearchOption.AllDirectories);
        //依赖Unity安装目录下的DLL
        var dir = FileTool.CombineDirs(true, EditorApplication.applicationContentsPath, "Managed", "UnityEngine");
        var dllList2 = Directory.GetFiles(dir, "*.dll", SearchOption.AllDirectories);
        List<string> list0 = new List<string>(dllList0);
        for (int i = 0; i < list0.Count; i++)
        {
            if (list0[i].Contains("sqlite3.dll") || list0[i].Contains(LWUtility.HotfixFileName))
            {
                list0.RemoveAt(i);
                i--;
            }
        }
        List<string> list1 = new List<string>(dllList1);
        List<string> list2 = new List<string>(dllList2);
        list1.AddRange(list2);
        list1.AddRange(list0);
        return list1.ToArray();

    }
    /// <summary>
    /// 解析project中的dll
    /// </summary>
    /// <returns></returns>
    static List<string> FindDLLByCSPROJ(string projName)
    {
        //cs list
        List<string> csList = new List<string>();
        var projpath = LWUtility.ProjectRoot + "/" + projName;
        XmlDocument xml = new XmlDocument();
        xml.Load(projpath);
        XmlNode ProjectNode = null;
        foreach (XmlNode x in xml.ChildNodes)
        {
            if (x.Name == "Project")
            {
                ProjectNode = x;
                break;
            }
        }
        List<string> csprojList = new List<string>();
        foreach (XmlNode childNode in ProjectNode.ChildNodes)
        {
            if (childNode.Name == "ItemGroup")
            {
                foreach (XmlNode item in childNode.ChildNodes)
                {
                    if (item.Name == "Compile")  //cs 引用
                    {
                        var csproj = item.Attributes[0].Value;
                        csList.Add(csproj);
                    }
                    else if (item.Name == "Reference") //DLL 引用
                    {
                        var HintPath = item.FirstChild;
                        var dir = HintPath.InnerText.Replace("/", "\\");
                    }
                    else if (item.Name == "ProjectReference") //工程引用
                    {
                        var csproj = item.Attributes[0].Value;
                        csprojList.Add(csproj);
                    }
                }
            }
        }
        //csproj也加入
        foreach (var csproj in csprojList)
        {
            //有editor退出
            if (csproj.ToLower().Contains("editor")) continue;
            //添加扫描到的dll
            FindDLLByCSPROJ(csproj);
            //
            var gendll = LWUtility.Library + "/ScriptAssemblies/" + csproj.Replace(".csproj", ".dll");
            if (!File.Exists(gendll))
            {
                Debug.LogError("不存在:" + gendll);
            }
        }
        return csList;
    }
    private void OnFinished(string path, CompilerMessage[] msgs)
    {
        bool isFail = false;
        foreach (var msg in msgs)
        {
            if (msg.type == CompilerMessageType.Error)
            {
                Debug.LogError(msg.message);
                isFail = true;
            }
        }
        if (isFail)
        {
            onFinished?.Invoke(this, false);
        }
        else
        {
            onFinished?.Invoke(this, true);
        }
        onFinished = null;
    }
}