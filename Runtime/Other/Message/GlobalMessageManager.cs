using LWFramework;
using LWFramework.Core;
using LWFramework.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework.Message {
    /// <summary>
    /// 总消息控制 ，可以发消息给热更域
    /// </summary>
    [ManagerClass(ManagerType.Normal)]
    public class GlobalMessageManager : BaseMessageManager, IManager
    {
        public void Init()
        {

        }

        public void LateUpdate()
        {

        }

        public void Update()
        {
            UpdateMsg();
        }
    }

}
