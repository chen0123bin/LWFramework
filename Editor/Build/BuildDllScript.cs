using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Reflection;
using LWFramework;
#if ILRUNTIME
using ILRuntime.Runtime.CLRBinding;
#endif
using Tool;
using Debug = UnityEngine.Debug;


public class BuildDllScript 
{


    private static string DLLPATH = "/Hotfix/"+ LWFramework.Asset.LWUtility.HotfixFileName;


    /// <summary>
    /// 编译模式
    /// </summary>
    /// <param name="mode"></param>
    public static void RoslynBuild(BuildDllTools.BuildMode mode)
    {
        //var cmd = new DllBuild(Application.streamingAssetsPath + "/Hotfix");
        //cmd.onFinished += ((DllBuild self, bool isSuccess) => {
        //    var tip = isSuccess ? "Dll生成成功!" : "Dll生成失败!";
        //    Debug.Log(tip);
        //});
        //cmd.Execute();

        //1.build dll
        var outpath_win = Application.streamingAssetsPath + "/";
        BuildDllTools.BuildDll(Application.dataPath, outpath_win, mode);
        //3.生成CLRBinding
        GenCLRBindingByAnalysis();
        AssetDatabase.Refresh();
        Debug.Log("脚本打包完毕");
    }

    /// <summary>
    /// 生成类适配器
    /// </summary>
    public static void GenCrossBindAdapter()
    {
#if ILRUNTIME
        var types = new List<Type>();
        types.Add((typeof(UnityEngine.ScriptableObject)));
        types.Add((typeof(System.Exception)));
        types.Add(typeof(System.Collections.IEnumerable));
        types.Add(typeof(System.Runtime.CompilerServices.IAsyncStateMachine));
        types.Add(typeof(IManager));
        //types.Add(typeof(ADataListener));
        //types.Add(typeof(Attribute));
        //types.Add(typeof(SerializedMonoBehaviour));
        GenAdapter.CreateAdapter(types, "Assets/LWFramework/ILRuntime/Adapter");
#else
        Debug.Log("当前不是IL模式");
#endif
    }

    //生成clr绑定
    public  static  void GenCLRBindingByAnalysis(RuntimePlatform platform = RuntimePlatform.Lumin,string dllpath ="")
    {
#if ILRUNTIME
        if (platform == RuntimePlatform.Lumin)
        {
            platform = Application.platform;
        }
        //默认读StreammingAssets下面path
        if (dllpath == "")
        {
             dllpath = Application.streamingAssetsPath + "/"+ DLLPATH;
        }
        
        //不参与自动绑定的
        List<Type> notGenerateTypes  =new List<Type>()
        {
            typeof(MethodBase),typeof(MemberInfo),typeof(FieldInfo),typeof(MethodInfo),typeof(PropertyInfo)
            ,typeof(Component),typeof(Type)
        };


        //用新的分析热更dll调用引用来生成绑定代码


        ILRuntimeHelper.LoadHotfix(dllpath, false);       
        BindingCodeGenerator.GenerateBindingCode(ILRuntimeHelper.AppDomain, "Assets/LWFramework/ILRuntime/Binding/Analysis", notGenTypes:notGenerateTypes);       
        ILRuntimeHelper.Close();
        AssetDatabase.Refresh();
#else
        Debug.Log("当前不是IL模式");
#endif


        //暂时先不处理
    }

    static public void GenCLRBindingBySelf()
    {
#if ILRUNTIME
        var types = new List<Type>();
        //反射类优先生成
        types.Add(typeof(Type));
        //PreBinding 

        BindingCodeGenerator.GenerateBindingCode(types, "Assets/LWFramework/ILRuntime/Binding/PreBinding");
        AssetDatabase.Refresh();
#else
        Debug.Log("当前不是IL模式");
#endif
    }
}