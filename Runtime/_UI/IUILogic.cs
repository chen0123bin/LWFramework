using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework.UI {
    public interface IUILogic
    {
        /// <summary>
        /// 打开View
        /// </summary>
        void OnOpenView();
        /// <summary>
        /// 关闭View
        /// </summary>
        void OnCloseView();
        /// <summary>
        /// 清空View
        /// </summary>
        void OnClearView();
        void OnCreateView();
    }
}

