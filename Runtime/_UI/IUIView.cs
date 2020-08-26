using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LWFramework.UI
{
    public interface IUIView
    {
        /// <summary>
        /// View的数据
        /// </summary>
        ViewData ViewData { get; set; }
        /// <summary>
        /// ViewId
        /// </summary>
        int ViewId { get; set; }
        void CreateView(GameObject gameObject);
        void OnCreateView();
        /// <summary>
        /// 打开View
        /// </summary>
        void OpenView();
        /// <summary>
        /// 关闭View
        /// </summary>
        void CloseView();
        /// <summary>
        /// 判断当前是否打开
        /// </summary>
        /// <returns></returns>
        bool IsOpen { get;  set; }
        /// <summary>
        /// 更新View
        /// </summary>
        void UpdateView();
       /// <summary>
       /// 清空View
       /// </summary>
        void ClearView();
    }
}