using LWFramework.Asset;
using LWFramework.Asset.Editor;
using LWFramework.Core;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tool;
using UnityEditor;
using UnityEngine;

public class LWEditorWindow : OdinMenuEditorWindow
{
    public static LWEditorWindow LWEditor;
    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree();
        tree.DefaultMenuStyle = OdinMenuStyle.TreeViewStyle;
        // tree.Add("Menu Style", tree.DefaultMenuStyle);
        tree.Selection.SupportsMultiSelect = false;

        tree.Add("AB管理", new ABManager());
        tree.Add("DLL管理", new DLLManager());
        tree.Add("Manifest", AssetDatabase.LoadAssetAtPath<UnityEngine.Object>("Assets/Manifest.asset"));
        tree.Add("配置", Resources.Load<LWGlobalConfig>("LWGlobalConfig"));
        tree.Add("其他", new OtherToolManger());
        tree.AddAllAssetsAtPath("序列化文件", "Assets/LWFramework/Editor", typeof(ScriptableObject), true, true);


        //var allAssets = AssetDatabase.GetAllAssetPaths()
        //   .Where(x => x.StartsWith("Assets/"))
        //   .OrderBy(x => x);
        //foreach (var path in allAssets)
        //{
        //    tree.AddAssetAtPath(path.Substring("Assets/".Length), path);
        //}
        tree.EnumerateTree().AddThumbnailIcons();
        return tree;
    }
    [MenuItem("LWFramework/打开工具")]
    private static void OpenWindow()
    {
        LWEditor = GetWindow<LWEditorWindow>();
        LWEditor.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        //GetWindow<LWEditorWindow>().ShowPopup();
    }
    [MenuItem("LWFramework/Test")]
    private static void OpenWindow222()
    {
        EditorWindow editorWindow = EditorWindow.GetWindow(typeof(SceneView));
        editorWindow.ShowNotification(new GUIContent("aaaaaaaaa"));
        //   GetWindow<TestWindow>()..ShowUtility();
    }



    [MenuItem("Assets/复制路径(Shift+D) #d")]
    static void CopyAssetPath()
    {
        if (EditorApplication.isCompiling)
        {
            return;
        }
        string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        GUIUtility.systemCopyBuffer = path;
        Debug.Log(string.Format("systemCopyBuffer: {0}", path));
    }
    [MenuItem("GameObject/UIFramework/复制路径(Shift+C) #c", false, -101)]
    static void CopyParents()
    {
        if (EditorApplication.isCompiling)
        {
            return;
        }
        string path = GetParentPath(Selection.activeObject as GameObject, "");
        GUIUtility.systemCopyBuffer = path;
        Debug.Log(string.Format("systemCopyBuffer: {0}", path));
    }
    [MenuItem("GameObject/UIFramework/改变界面状态(Shift+T)  #t", false, -101)]
    static void ChangeViewState()
    {
        GameObject view = Selection.activeObject as GameObject;
        CanvasGroup canvasGroup = view.GetComponent<CanvasGroup>();
        canvasGroup.SetActive(canvasGroup.alpha == 0);
    }
    static string GetParentPath(GameObject child, string str)
    {
        if (child.transform.parent == null)
        {
            str = child.name + str;
            return str;
        }
        else
        {
            str = "/" + child.name + str;
            return GetParentPath( child.transform.parent.gameObject, str);
        }

    }
}


public class ABManager
{
    [FolderPath]
    public string ResPath = "Assets/Res/Runtime";
    [EnumToggleButtons]
    [EnumPaging]
    public ABBuildTarget abBuildTarget = BuildABScript.GetABBuildTarget();

    [Button("标记文件")]
    public void MarkAssetsBtn()
    {
        BuildABScript.MarkAssets(ResPath);
        GUIUtility.ExitGUI();
    }
    [Button("清空标记文件")]
    public void ClearMarkAssetsBtn()
    {
        BuildABScript.MarkAssets(ResPath, true);
        AssetDatabase.RemoveUnusedAssetBundleNames();
    }

    [Button("生成配置")]
    public void CreateManifestBtn()
    {
        BuildABScript.BuildManifest();
    }
    [Button("打包AB资源", ButtonSizes.Medium)]
    public void CreateABBtn()
    {
        BuildABScript.BuildManifest();
        //避免报错
        EditorApplication.delayCall += () => { BuildABScript.BuildAssetBundles(BuildABScript.GetBuildTarget(abBuildTarget)); };
    }
    [Button("拷贝资源到StreamingAssets", ButtonSizes.Medium)]
    public void CopyAB2SABtn()
    {
        BuildABScript.CopyAssetBundlesTo(FileTool.CombinePaths(Application.streamingAssetsPath, LWUtility.AssetBundles));
        AssetDatabase.Refresh();
    }

    [Button("打开Persistent文件夹")]
    public void OpenPersistentPath()
    {
        string path = Application.persistentDataPath.Replace("/", "\\");
        System.Diagnostics.Process.Start("explorer.exe", path);
    }
    [Button("打开AB文件夹")]
    public void OpenABPath()
    {
        string path = System.Environment.CurrentDirectory.Replace("/", "\\") + "\\AssetBundles";
        System.Diagnostics.Process.Start("explorer.exe", path);
    }

}

public class DLLManager
{
    //   [HorizontalGroup("Split",0.5f)]
    [Button("编译dll(Release)", ButtonSizes.Medium)]
    public void Button()
    {
        BuildDllScript.RoslynBuild(BuildDllTools.BuildMode.Release);
    }
    //   [HorizontalGroup("Split", 0.5f)]
    [Button("编译dll(Debug)", ButtonSizes.Medium)]
    public void Button2()
    {
        BuildDllScript.RoslynBuild(BuildDllTools.BuildMode.Debug);
    }
#if ILRUNTIME
    [Button("分析DLL生成绑定", ButtonSizes.Medium)]
    public void Button3()
    {
        BuildDllScript.GenCLRBindingByAnalysis(); ;
    }
    [Button("手动绑定生成", ButtonSizes.Medium)]
    public void Button4()
    {
        BuildDllScript.GenCLRBindingBySelf();
    }
    [Button("生成跨域Adapter")]
    public void Button5()
    {
        BuildDllScript.GenCrossBindAdapter();
    }
    [Button("生成Link.xml")]
    public void Button6()
    {
        StripCode.GenLinkXml();

    }
#endif
    [Button("添加IL包")]
    public void Button7()
    {
        bool exitsIL = FileTool.ExistsFile("Assets/LWFramework/ILRuntime/Ilr/CLRBinding.zip", LWUtility.ProjectRoot);
        if (!exitsIL)
        {
            DefineSymbolsTool.AddDefine("ILRUNTIME");
            FileTool.CopyDir(LWUtility.ProjectRoot + "/ILFile/ILRuntime", LWUtility.ProjectRoot + "/Assets/LWFramework");

        }
        AssetDatabase.Refresh();
    }
    [Button("删除IL包")]
    public void Button8()
    {
        bool exitsIL = FileTool.ExistsFile("Assets/LWFramework/ILRuntime/Ilr/CLRBinding.zip", LWUtility.ProjectRoot);
        if (exitsIL)
        {
            DefineSymbolsTool.DeleteDefine("ILRUNTIME");
            FileTool.DeleteDir(LWUtility.ProjectRoot + "/Assets/LWFramework/ILRuntime");
        }
        AssetDatabase.Refresh();
    }
}
public class OtherToolManger 
{
    [Title("查找脚本")]
    [LabelText("脚本")]
    public UnityEngine.Object scriptsName;
    [Button("查找脚本")]
    public void FildScripts()
    {
        var obj = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //关键代码，获取所有gameobject元素给数组obj
        foreach (GameObject child in obj)    //遍历所有gameobject
        {
            if (child.GetComponent(scriptsName.name))
            {
                Debug.Log(child.gameObject.name);
            }

        }
    }
  
    
}