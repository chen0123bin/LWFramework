using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MonoEventListener : MonoBehaviour
{
    public Action<string, UnityEngine.Object> onMonoEventAction;
   

    /// <summary>
    /// 获取GameObject的MonoEventListener事件组件
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static MonoEventListener Get(GameObject gameObject)
    {
        return ComponentTool.AutoGet<MonoEventListener>(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (onMonoEventAction != null) {
            onMonoEventAction.Invoke("OnTriggerExit", other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (onMonoEventAction != null)
        {
            onMonoEventAction.Invoke("OnTriggerStay", other);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (onMonoEventAction != null)
        {
            onMonoEventAction.Invoke("OnTriggerEnter", other);
        }
    }


}
