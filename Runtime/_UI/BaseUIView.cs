using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LWFramework.UI
{
    public  class BaseUIView: IUIView
    {
        /// <summary>
        /// UIGameObject
        /// </summary>
        protected GameObject _entity;
        protected CanvasGroup _canvasGroup;
        /// <summary>
        /// View的数据
        /// </summary>
        protected ViewData _viewData;
        public ViewData ViewData { get => _viewData; set => _viewData = value; }
        /// <summary>
        /// ViewId 动态生成
        /// </summary>
        private int _viewId;
        public int ViewId { get => _viewId; set => _viewId = value; }
        private bool _isOpen = false;
        public bool IsOpen {
            get => _isOpen;
            set => _isOpen = value;
        }
        public virtual void CreateView(GameObject gameObject) {
            _entity = gameObject;
            //view上的组件
            UIUtility.Instance.SetViewElement(this, gameObject);
            _canvasGroup = _entity.GetComponent<CanvasGroup>();
            if (_canvasGroup == null) {
                LWDebug.LogError(string.Format("{0}上没有CanvasGroup这个组件", _entity.name));
            }
            ViewId = UIUtility.Instance.ViewId;        
            _viewData = new ViewData();
            _viewData.OnDataChange = OnDataChange;
            OnCreateView();
        }
        public virtual void OnCreateView() { 
        
        }
        public virtual void OnDataChange(string dataName) { }
        /// <summary>
        /// 打开view
        /// </summary>
        public virtual void OpenView() {
            //_entity.SetActive(true);
            _canvasGroup.SetActive(true);
            _isOpen = true;
        }
        /// <summary>
        ///关闭view 
        /// </summary>
        public virtual void CloseView() {
            //_entity.SetActive(false);
            _canvasGroup.SetActive(false);
            _isOpen = false;
        }
       
        //更新VIEW
        public virtual void UpdateView()
        {
            
        }             
        //删除VIEW
        public virtual void ClearView()
        {
            _viewData.Clear();
            GameObject.Destroy(_entity);
        }
    }    
}

