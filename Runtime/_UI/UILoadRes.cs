using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 通过Resources加载UI资源，路径与AB保持一致的情况下，需要将Res名称更换成Resources
/// </summary>
public class UILoadRes : IUILoad
{
    public Sprite GetSprite(string path)
    {
        path = GetResPath(path);
        return Resources.Load<Sprite>(path);
    }

    public GameObject LoadUIGameObject(string path)
    {
        path = GetResPath(path);
        GameObject temp = Resources.Load<GameObject>(path);
        GameObject uiGameObject = (GameObject)GameObject.Instantiate(temp);
        return uiGameObject;
    }

    public GameObject LoadUIGameObjectAsync(string path)
    {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// 处理加载路径，统一跟UILoadAB一样的路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string GetResPath(string path) {
        path = path.Replace("Assets/Res/", "");
        path = path.Substring(0, path.LastIndexOf('.'));
        return path;
    }
}
