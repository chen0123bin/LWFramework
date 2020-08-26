using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUILoad 
{
    GameObject LoadUIGameObject(string path);
    GameObject LoadUIGameObjectAsync(string path);
    Sprite GetSprite(string path);
}
