
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework {
    public class GameObjectPool<T> : ObjectPool<T> where T : class, IPoolGameObject
    {
        private GameObject _template;
        public GameObjectPool(int poolMaxSize) : base(poolMaxSize)
        {

        }
        public GameObjectPool(int pooMaxSize, GameObject template) : base(pooMaxSize)
        {
            _template = template;
            _template.SetActive(false);
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public override T Spawn()
        {
            T ret;
            if (_poolList.Count > 0)
            {
                ret = _poolList[0];
                _poolList.RemoveAt(0);
               
            }
            else {
                ret = (T)Activator.CreateInstance(typeof(T));
                GameObject go = GameObject.Instantiate(_template, _template.transform.parent, false);
                ret.Create(go);               
                
            }
            ret.SetActive(true);
            return ret;
        }
        public override void Unspawn(T obj)
        {
            base.Unspawn(obj);
            obj.SetActive (false);
        }
    }

}
