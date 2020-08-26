using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 通过编辑器路径去查找对象
/// </summary>
public class UILoadScene : IUILoad
{
    public Sprite GetSprite(string path)
    {
        path = GetResPath(path);
        return Resources.Load<Sprite>(path);
    }

    public GameObject LoadUIGameObject(string path)
    {

        GameObject uiGameObject = GameObject.Find(path);
        return uiGameObject;
    }

    public GameObject LoadUIGameObjectAsync(string path)
    {
        throw new System.NotImplementedException();
    }

    private string GetResPath(string path) {
        path = path.Replace("Assets/Res/", "");
        path = path.Substring(0, path.LastIndexOf('.'));
        return path;
    }
}
