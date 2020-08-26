using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework.Message {
    /// <summary>
    /// 消息池数据管理
    /// </summary>
    public class MessagePool
    {
        public static List<Message> messageList = new List<Message>();
        public static Message GetMessage()
        {
            Message ret;
            if (messageList.Count <= 0)
            {
                ret = new Message();
                //  Debug.Log("创建一个消息对象");
            }
            else
            {
                ret = messageList[0];
                messageList.Remove(ret);
            }
            return ret;
        }
        public static Message GetMessage(string type)
        {
            Message ret = GetMessage();
            //ret.data = data;
            ret.type = type;
            return ret;
        }
        public static Message GetMessage(string type, GameObject sender)
        {
            Message ret = GetMessage(type);
            ret.sender = sender;
            return ret;
        }
        public static Message GetMessage(string type, GameObject sender, float delay)
        {
            Message ret = GetMessage(type, sender);
            ret.delay = delay;
            return ret;
        }
        public static void AddMessage(Message msg)
        {
            //Debug.Log("添加一个消息对象");
            msg.Clear();
            messageList.Add(msg);
        }
    }
}
