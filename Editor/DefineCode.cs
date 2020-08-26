using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefineCode", menuName = "LWFramework/DefineCode", order = 0)]
public class DefineCode : SerializedScriptableObject
{
    [TextArea(0,200)]
    public string Define;

    [TextArea(0, 200)]
    public string VS;
}
