using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LWFramework.UI {
    /// <summary>
    /// 处理UI的逻辑
    /// </summary>
    /// <typeparam name="TUIView"></typeparam>
    public  class BaseUILogic <TUIView>: IUILogic where TUIView : class, IUIView
    {
        protected TUIView _view;
        public BaseUILogic(TUIView view) {
            _view = view;
        }
        public virtual void OnCreateView() { }
        public virtual void OnOpenView() { }
        public virtual void OnCloseView() { }
        public virtual void OnClearView() { }

    }
}

