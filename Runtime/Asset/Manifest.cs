
using System;
using UnityEngine;

namespace LWFramework.Asset
{
    [Serializable]
    public class AssetRef
    { 
        public int bundle;
        public int dir;
        public string name;
    }

    public class Manifest : ScriptableObject
    {
        public string downloadURL = "";
        public string[] activeVariants = new string[0];
        [HideInInspector]public string[] bundles = new string[0];
        [HideInInspector]public string[] dirs = new string[0];
        [HideInInspector]public AssetRef[] assets = new AssetRef[0];
    }
}
