using LWFramework.Asset;
using LWFramework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILoadAB : IUILoad
{
    public Sprite GetSprite(string path)
    {
        var asset = MainManager.Instance.GetManager<AssetsManager>().Load(path, typeof(UnityEngine.Sprite));
        return (Sprite)asset.asset;
    }

    public GameObject LoadUIGameObject(string path)
    {
        var asset = MainManager.Instance.GetManager<AssetsManager>().Load(path, typeof(UnityEngine.Object));
        GameObject uiGameObject = (GameObject)GameObject.Instantiate(asset.asset);
        asset.Require(uiGameObject);
        asset.Release();
        return uiGameObject;
    }

    public GameObject LoadUIGameObjectAsync(string path)
    {
        throw new System.NotImplementedException();
    }
}
