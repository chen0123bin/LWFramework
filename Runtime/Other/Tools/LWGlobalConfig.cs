using LWFramework.Core;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LWGlobalConfig", menuName = "LWFramework/LWGlobalConfig", order = 0)]
public class LWGlobalConfig : ScriptableObject
{
    //是否连接服务器
    public bool connServer;
    //是否开启ab模式
    public bool assetBundleMode;
    //热更模式
    public HotfixCodeRunMode hotfixCodeRunMode;
    //服务器地址
    //public string serverURL;
}
