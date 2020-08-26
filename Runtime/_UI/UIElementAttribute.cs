using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework.UI
{
    public class UIElementAttribute : Attribute
    {
        public readonly string rootPath;
        public readonly string resPath;
        /// <summary>
        /// 界面对象
        /// </summary>
        /// <param name="rootPath">查找的路径</param>
        /// <param name="resPath">默认的资源主要是图片</param>
        public UIElementAttribute(string rootPath, string resPath = "")
        {
            this.rootPath = rootPath;
            this.resPath = resPath;
        }
    }

}
