using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateScriptByTemplate
{
    #region 创建脚本
    /// <summary>
    /// 新建Manager类
    /// </summary>
    [@MenuItem("Assets/Create/LWFramework/C# Manager", false, 11)]
    private static void CreateManager()
    {
        string path = EditorUtility.SaveFilePanel("新建 Manager 类", Application.dataPath + "/Scripts", "NewManager", "cs");
        if (path != "")
        {
            string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
            if (!File.Exists(path))
            {
                
                TextAsset asset = Resources.Load<TextAsset>("Template/ManagerTemp"); 
                if (asset)
                {
                    string code = asset.text;
                    code = code.Replace("#SCRIPTNAME#", className);
                    File.AppendAllText(path, code);
                    asset = null;
                    AssetDatabase.Refresh();

                    string assetPath = path.Substring(path.LastIndexOf("Assets"));
                    TextAsset cs = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                    EditorGUIUtility.PingObject(cs);
                    Selection.activeObject = cs;
                    AssetDatabase.OpenAsset(cs);
                }
            }
            else
            {
                Debug.LogError("新建Manager失败，已存在类型 " + className);
            }
        }
    }
    /// <summary>
    /// 新建HotfixManager类
    /// </summary>
    [@MenuItem("Assets/Create/LWFramework/C# HotfixManager", false, 11)]
    private static void CreateHotfixManager()
    {
        string path = EditorUtility.SaveFilePanel("新建 HotfixManager 类", Application.dataPath + "/Scripts", "NewHotfixManager", "cs");
        if (path != "")
        {
            string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
            if (!File.Exists(path))
            {
                TextAsset asset = Resources.Load<TextAsset>("Template/HotfixManagerTemp"); //AssetDatabase.LoadAssetAtPath("Assets/LWFramework/Editor/Template/HotfixManagerTemp.txt", typeof(TextAsset)) as TextAsset;
                if (asset)
                {
                    string code = asset.text;
                    code = code.Replace("#SCRIPTNAME#", className);
                    File.AppendAllText(path, code);
                    asset = null;
                    AssetDatabase.Refresh();

                    string assetPath = path.Substring(path.LastIndexOf("Assets"));
                    TextAsset cs = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                    EditorGUIUtility.PingObject(cs);
                    Selection.activeObject = cs;
                    AssetDatabase.OpenAsset(cs);
                }
            }
            else
            {
                Debug.LogError("新建HotfixManager失败，已存在类型 " + className);
            }
        }
    }
    /// <summary>
    /// 新建HotfixManager类
    /// </summary>
    [@MenuItem("Assets/Create/LWFramework/C# Procedure", false, 11)]
    private static void CreateProcedure()
    {
        string path = EditorUtility.SaveFilePanel("新建 Procedure 类", Application.dataPath + "/Scripts", "NewProcedure", "cs");
        if (path != "")
        {
            string className = path.Substring(path.LastIndexOf("/") + 1).Replace(".cs", "");
            if (!File.Exists(path))
            {
                TextAsset asset = Resources.Load<TextAsset>("Template/ProcedureTemp");// AssetDatabase.LoadAssetAtPath("Assets/LWFramework/Editor/Template/ProcedureTemp.txt", typeof(TextAsset)) as TextAsset;
                if (asset)
                {
                    string code = asset.text;
                    code = code.Replace("#SCRIPTNAME#", className);
                    File.AppendAllText(path, code);
                    asset = null;
                    AssetDatabase.Refresh();

                    string assetPath = path.Substring(path.LastIndexOf("Assets"));
                    TextAsset cs = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset)) as TextAsset;
                    EditorGUIUtility.PingObject(cs);
                    Selection.activeObject = cs;
                    AssetDatabase.OpenAsset(cs);
                }
            }
            else
            {
                Debug.LogError("新建Procedure失败，已存在类型 " + className);
            }
        }
    }
    #endregion
}
