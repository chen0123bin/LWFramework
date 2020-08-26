using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework.UI {
    
    public class ViewData 
    {           
        private Hashtable _data = new Hashtable();
        private Action<string> _onDataChange;
        /// <summary>
        /// 数据发生变化的处理
        /// </summary>
        public Action<string> OnDataChange {
            set {
                _onDataChange = value;
            }
        }
        public T Get<T>(string name)
        {
            return (T)this[name];
        }
        public object this[string name]
        {
            get
            {
                return _data != null && _data.ContainsKey(name) ? _data[name] : null;
            }
            set
            {
                if (_data != null)
                {
                    _data[name] = value;
                    _onDataChange?.Invoke(name);                  
                }
            }
        }
        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear() {
            _data.Clear();
        }
       
    }

}
