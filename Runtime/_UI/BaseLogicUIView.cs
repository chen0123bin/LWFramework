using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework.UI {
    /// <summary>
    /// 用于拆分View Logic的View基类
    /// </summary>
    /// <typeparam name="TUILogic"></typeparam>
    public class BaseLogicUIView <TUILogic>: BaseUIView where TUILogic : class,IUILogic
    {
        protected TUILogic _logic;
        public override void CreateView(GameObject gameObject)
        {
            _logic = Activator.CreateInstance(typeof(TUILogic),this) as TUILogic;
            base.CreateView(gameObject);
            _logic.OnCreateView();
        }
        public override void OpenView()
        {
            base.OpenView();
            _logic.OnOpenView();
        }
        public override void CloseView()
        {
            base.CloseView();
            _logic.OnCloseView();
        }
        public override void ClearView()
        {
            base.ClearView();
            _logic.OnClearView();
        }
    }

}
