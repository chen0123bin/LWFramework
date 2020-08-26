using LWFramework.Core;
using System.Collections.Generic;
using UnityEngine;

namespace LWFramework.FMS
{
    /// <summary>
    /// 有限状态机管理者
    /// </summary>
    [ManagerClass(ManagerType.Normal)]
    public sealed class FSMManager : IManager
    {
        private Dictionary<string, FSMStateMachine> _fsms = new Dictionary<string, FSMStateMachine>();
        private Dictionary<string, List<AttributeTypeData>> _fsmStateList = new Dictionary<string, List<AttributeTypeData>>();
      
        /// <summary>
        /// 注册状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void RegisterFSM(FSMStateMachine fsm)
        {
            if (!_fsms.ContainsKey(fsm.Name))
            {
                _fsms.Add(fsm.Name, fsm);
            }
            else
            {
                LWDebug.Log("当前已经存在这个FMS " + fsm.Name);
            }
        }

        /// <summary>
        /// 移除已注册的状态机
        /// </summary>
        /// <param name="fsm">状态机</param>
        public void UnRegisterFSM(FSMStateMachine fsm)
        {
            if (_fsms.ContainsKey(fsm.Name))
            {
                _fsms[fsm.Name].ClearFMS();
                _fsms.Remove(fsm.Name);
            }
            else
            {
                LWDebug.Log("不存在这个FMS " + fsm.Name);
            }
        }

        /// <summary>
        /// 通过名称获取状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>状态机</returns>
        public FSMStateMachine GetFSMByName(string name)
        {
            if (_fsms.ContainsKey(name))
            {
                return _fsms[name];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 是否存在指定的状态机
        /// </summary>
        /// <param name="name">状态机名称</param>
        /// <returns>是否存在</returns>
        public bool IsExistFSM(string name)
        {
            return _fsms.ContainsKey(name);
        }
        /// <summary>
        /// 通过名称去获取分类的ClassData
        /// </summary>
        /// <param name="fsmName"></param>
        /// <returns></returns>
        public List<AttributeTypeData> GetFsmClassDataByName(string fsmName) {
            return _fsmStateList[fsmName];
        }
        public void InitFSMManager()
        {
            //找到所有的流程管理类
            List<AttributeTypeData> classDataList = MainManager.Instance.GetTypeListByAttr<FSMTypeAttribute>();
            for (int i = 0; i < classDataList.Count; i++)
            {
                string fsmName = (classDataList[i].attribute as FSMTypeAttribute).FSMName;
                if (!_fsmStateList.ContainsKey(fsmName))
                {
                    _fsmStateList.Add(fsmName, new List<AttributeTypeData>());
                }
                _fsmStateList[fsmName].Add(classDataList[i]);
            }
        }

        public void Update()
        {
            foreach (var item in _fsms)
            {
                item.Value.Update();
            }
        }
       
        public void LateUpdate()
        {
        }

        public void Init()
        {
        }
    }
}