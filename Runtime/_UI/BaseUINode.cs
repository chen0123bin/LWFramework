using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LWFramework.UI
{
    public abstract class BaseUINode: IPoolGameObject
    {
        private GameObject _entity;
        
        
        /// <summary>
        /// 创建GameObject实体
        /// </summary>
        /// <param name="gameObject"></param>
        public virtual void Create(GameObject gameObject)
        {
            _entity = gameObject;
            //view上的组件
            UIUtility.Instance.SetViewElement(this, gameObject);
        }
        public virtual void Unspawn() {
            SetActive(false);
            _entity.transform.SetAsLastSibling();
            OnUnSpawn();
        }
        public abstract void OnUnSpawn();
        /// <summary>
        /// 释放引用，删除gameobject
        /// </summary>
        public virtual void Release()
        {
            GameObject.Destroy(_entity);
        }

        public bool GetActive()
        {
            return _entity.activeInHierarchy;
        }

        public void SetActive(bool active)
        {
            _entity.SetActive(active);
        }
    }
}