using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework {
    public interface IPoolGameObject
    {
        /// <summary>
        /// 是否为Active
        /// </summary>
        bool GetActive();
        void SetActive(bool active);
        void Create(GameObject gameObject);
        /// <summary>
        /// 回收进入池内
        /// </summary>
        void Unspawn();
        /// <summary>
        /// 释放掉，完全删除
        /// </summary>
        void Release();
    }
}

