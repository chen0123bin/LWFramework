using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LWFramework.Message {
    /// <summary>
    /// 消息数据
    /// </summary>
    public class Message
    {
        // public MessageData data;
        public GameObject sender;
        public float delay = 0;
        public string type;
        private Dictionary<string, object> _param;
        public Message()
        {
            _param = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
        }
        public T Get<T>(string name)
        {
            return (T)this[name];
        }

        /// <summary>
        /// Find param
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                return _param != null && _param.ContainsKey(name) ? _param[name] : null;
            }
            set
            {
                if (_param != null)
                {
                    _param[name] = value;
                }
            }
        }

        public void Clear()
        {
            sender = null;
            delay = 0;
            type = "";
            _param.Clear();
        }

    }
}

