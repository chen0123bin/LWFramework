using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T _instance = null;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = typeof(T).Name;
                    _instance = go.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
    protected virtual void OnDestroy()
    {
        _instance = null;
    }
}
