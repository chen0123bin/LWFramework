using LWFramework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LWFramework.UI {
    /// <summary>
    /// 所有的UI管理器
    /// </summary>
    [ManagerClass(ManagerType.Normal)]
    public class UIManager : IManager
    {
        /// <summary>
        /// 所有的view字典
        /// </summary>
        private Dictionary<string, IUIView> _uiViewDic;
        /// <summary>
        /// 所有UI的父节点
        /// </summary>
        private Dictionary<string, Transform> _uiParentDic;
       
        #region 获取Canvas节点
       
        private Transform _editTransform;
        private Transform EditTransform
        {
            get
            {
                if (_editTransform == null)
                {
                    _editTransform = GameObject.Find("LWFramework/Canvas/Edit").transform;
                }
                return _editTransform;
            }
        }
       
        #endregion
        public void Init()
        {
            _uiViewDic = new Dictionary<string, IUIView>();
            _uiParentDic = new Dictionary<string, Transform>();
            //启动之后隐藏编辑层
            EditTransform.gameObject.SetActive(false);
        }

        public void LateUpdate()
        {
           
        }
        /// <summary>
        /// 更新所有的View
        /// </summary>
        public void Update()
        {
            foreach (var item in _uiViewDic.Values)
            {
                if (item.IsOpen)
                {
                    item.UpdateView();
                }                
            }         
        }
        /// <summary>
        /// 创建VIEW
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent">父节点，当创建子View的时候为必填</param>
        /// <returns></returns>
        public BaseUIView CreateView<T>(Transform parent ) {

            BaseUIView uiView = Activator.CreateInstance(typeof(T)) as BaseUIView;
            //获取UIViewDataAttribute特性
            var attr = (UIViewDataAttribute)typeof(T).GetCustomAttributes(typeof(UIViewDataAttribute), true).FirstOrDefault();
            if (attr != null)
            {
                GameObject uiGameObject = uiGameObject = parent.Find(attr.loadPath).gameObject;
                 if (uiGameObject == null) {
                    LWDebug.LogError("没有找到这个UI对象" + attr.loadPath);
                }
              
                //初始化UI
                uiView.CreateView(uiGameObject);

            }
            else {
                LWDebug.Log("没有找到UIViewDataAttribute这个特性");
            }
           
            LWDebug.Log("UIManager：" + typeof(T).ToString());
            return uiView;
        }
        /// <summary>
        /// 创建一个VIEW
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public BaseUIView CreateView<T>()
        {
            BaseUIView uiView = Activator.CreateInstance(typeof(T)) as BaseUIView;
            //获取UIViewDataAttribute特性
            var attr = (UIViewDataAttribute)typeof(T).GetCustomAttributes(typeof(UIViewDataAttribute), true).FirstOrDefault();
            if (attr != null)
            {
                GameObject uiGameObject = null;
                //创建UI对象
                uiGameObject = UIUtility.Instance.CreateViewEntity(attr.loadPath);
                Transform parent = GetParent(attr.findType, attr.param);
                if (uiGameObject == null) {
                    LWDebug.LogError("没有找到这个UI对象" + attr.loadPath);
                }
                if (parent == null)
                {
                    LWDebug.LogError("没有找到这个UI父节点" + attr.param);
                }
                if (parent != null) {
                    uiGameObject.transform.SetParent(parent,false);
                }            
                //初始化UI
                uiView.CreateView(uiGameObject);
            }
            else
            {
                LWDebug.Log("没有找到UIViewDataAttribute这个特性");
            }
            LWDebug.Log("UIManager：" + typeof(T).ToString());
            return uiView;
        }
        /// <summary>
        /// 打开View
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T OpenView<T>()
        {
            IUIView uiViewBase;
            if (!_uiViewDic.TryGetValue(typeof(T).ToString(), out uiViewBase)) {
                uiViewBase = CreateView<T>();
                _uiViewDic.Add(typeof(T).ToString(), uiViewBase);
            }
            if(!uiViewBase.IsOpen)
                uiViewBase.OpenView();
            return (T)uiViewBase;
        }
        /// <summary>
        /// 关闭View
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T CloseView<T>()
        {
            IUIView uiViewBase;
            if (_uiViewDic.TryGetValue(typeof(T).ToString(), out uiViewBase))
            {
                uiViewBase.CloseView();
            }
            return (T)uiViewBase;
        }
        /// <summary>
        /// 关闭其他所有的View
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void CloseOtherView<T>() {
            string viewName = typeof(T).ToString();
            foreach (var item in _uiViewDic.Keys)
            {
                if (item != viewName) {
                    _uiViewDic[item].CloseView();
                }
            }
        }
        /// <summary>
        /// 获取VIEW
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetView<T>() {
            return (T)_uiViewDic[typeof(T).ToString()];
        }
        /// <summary>
        /// 清理所有的view
        /// </summary>
        public void ClearAllView() {
            foreach (var item in _uiViewDic.Values)
            {
                item.ClearView();
            }
            _uiViewDic.Clear();
        }
        /// <summary>
        /// 根据特性 获取父级
        /// </summary>
        /// <param name="findType"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        private Transform GetParent(FindType findType,string param) {
            Transform ret = null;
            if (_uiParentDic.ContainsKey(param))
            {
                ret = _uiParentDic[param];
            }
            else if (findType == FindType.Name)
            {
                GameObject gameObject = GameObject.Find(param);
                if (gameObject == null)
                {
                    LWDebug.LogError(string.Format("当前没有找到{0}这个GameObject对象", param));
                }
                ret = gameObject.transform;
                _uiParentDic.Add(param, ret);
            }
            else if (findType == FindType.Tag) {
                GameObject gameObject = GameObject.FindGameObjectWithTag(param);
                if (gameObject == null)
                {
                    LWDebug.LogError(string.Format("当前没有找到{0}这个Tag GameObject对象", param));
                }
                ret = gameObject.transform;
                _uiParentDic.Add(param, ret);
            }
            return ret;
        }
    }
}

