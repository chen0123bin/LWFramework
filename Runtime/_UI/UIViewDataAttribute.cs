using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LWFramework.UI {
    public enum UILayer
    {
        bottom, normal, top, //基础三层次
        world,    //世界坐标
        view,      //父级View节点
        local     //当前已经存在View对象
    }
    public enum FindType
    {
        Tag = 0,
        Name = 1
    }

    public class UIViewDataAttribute : Attribute
    {
        public string loadPath;
        public UILayer layer;
        public FindType findType;
        public string param;
        [Obsolete("使用新的特性，废弃掉之前的Layer")]
        public UIViewDataAttribute(string loadPath,UILayer uILayer)
        {
            this.loadPath = loadPath;
            this.layer = uILayer;
        }
        public UIViewDataAttribute(string loadPath, FindType findType,string param)
        {
            this.loadPath = loadPath;
            this.findType = findType;
            this.param = param;
        }
    }
}


