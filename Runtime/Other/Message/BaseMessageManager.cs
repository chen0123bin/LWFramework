using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LWFramework.Message
{
    public class MessageDelegate
    {
        public delegate void MessageHandler(Message msg);
    }    
    /// <summary>
    /// 消息数据管理
    /// </summary>
    public class BaseMessageManager
    {
        private Dictionary<string, List<MessageDelegate.MessageHandler>> dict = new Dictionary<string, List<MessageDelegate.MessageHandler>>();
       
        private List<Message> deleyMessage = new List<Message>();


       
        /// <summary>
        /// 添加一个消息监听
        /// </summary>
        /// <param name="type">消息的标识符</param>
        /// <param name="handler"></param>
        public void AddListener(string type, MessageDelegate.MessageHandler handler)
        {

            if (!dict.ContainsKey(type))
            {
                List<MessageDelegate.MessageHandler> list = new List<MessageDelegate.MessageHandler>();
                list.Add(handler);
                dict.Add(type, list); 
            }
            else
            {
                dict[type].Add(handler);
               // Debug.LogError("已经存在" + type + "这个事件");
            }
        }
        /// <summary>
        /// 检测是否包含这种类型的监听
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public bool CheckListener(string type) {
            return dict.ContainsKey(type);
        }
        /// <summary>
        /// 添加唯一一个消息监听
        /// </summary>
        /// <param name="type">消息的标识符</param>
        /// <param name="handler"></param>
        public void AddListenerSingle(string type, MessageDelegate.MessageHandler handler)
        {

            if (!dict.ContainsKey(type))
            {
                List<MessageDelegate.MessageHandler> list = new List<MessageDelegate.MessageHandler>();
                list.Add(handler);
                dict.Add(type, list);
            }
        }
        /// <summary>
        /// 移除一个消息监听
        /// </summary>
        /// <param name="type"></param>
        public void RemoveListener(string type)
        {

            if (dict.ContainsKey(type))
            {
                dict[type].Clear();
               dict.Remove(type);
            }
            else
            {
                Debug.LogWarning("找不到" + type + "这个事件");
            }
        }
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="msg"></param>
        public void Dispatcher(Message msg)
        {
            if (!dict.ContainsKey(msg.type))
            {
                Debug.LogWarning("找不到" + msg.type + "这个事件");
                return;
            }

            if (msg.delay == 0)
            {
                for (int i = 0; dict.ContainsKey(msg.type) && i < dict[msg.type].Count; i++)
                {
                    dict[msg.type][i](msg);
                }
               
                MessagePool.AddMessage(msg);
            }
            else
            {
                for (int i = 0; dict.ContainsKey(msg.type) && i < dict[msg.type].Count; i++)
                {
                    deleyMessage.Add(msg);
                }
               
            }

        }
        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="type">不传参数的情况下使用</param>
        public void Dispatcher(string type)
        {
            if (!dict.ContainsKey(type))
            {
                LWDebug.LogWarning("找不到" + type + "这个事件");
                return;
            }
            Message msg = MessagePool.GetMessage();
            for (int i = 0; dict.ContainsKey(type) && i < dict[type].Count; i++)
            {
                dict[type][i](msg);
            }
            
             MessagePool.AddMessage(msg);

        }
        public void UpdateMsg()
        {

            for (int i = 0; i < deleyMessage.Count; i++)
            {
                if (deleyMessage[i].delay > 0)
                {
                    deleyMessage[i].delay -= Time.deltaTime;

                }
                else
                {
                    deleyMessage[i].delay = 0;
                    Dispatcher(deleyMessage[i]);
                    deleyMessage.Remove(deleyMessage[i]);
                }

            }
        }
        public void Destory()
        {
            foreach (var item in dict.Values)
            {
                item.Clear();
            }
            dict.Clear();
            deleyMessage.Clear();
        }
    }
}