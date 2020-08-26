using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class DefineSymbolsTool 
{
    public static List<string> GetDefine(BuildTargetGroup buildTargetGroup) {

        string defineStr = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        List<string>  defineList =new List<string>( defineStr.Split(';'));
        return defineList;
    }
    public static void AddDefine(string newDefine) {
        List<string> defineList =  GetDefine(BuildTargetGroup.Standalone);
        defineList.Add(newDefine);
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < defineList.Count; i++)
        {        
            if (defineList[i] != "") {
                if (i == defineList.Count - 1)
                {
                    stringBuilder.Append(defineList[i]);
                }
                else {
                    stringBuilder.Append(defineList[i] + ";");
                }
                
            }
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, stringBuilder.ToString());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, stringBuilder.ToString());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, stringBuilder.ToString());
    }
    public static void DeleteDefine(string define)
    {
        List<string> defineList = GetDefine(BuildTargetGroup.Standalone);
        defineList.Remove(define);
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < defineList.Count; i++)
        {
            if (defineList[i] != "")
            {
                if (i == defineList.Count - 1)
                {
                    stringBuilder.Append(defineList[i]);
                }
                else
                {
                    stringBuilder.Append(defineList[i] + ";");
                }

            }
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, stringBuilder.ToString());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, stringBuilder.ToString());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, stringBuilder.ToString());
    }
}
