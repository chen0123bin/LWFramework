using LWFramework.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LWFramework.FMS {
    public class FSMStateMachine 
    {
        public string Name { get; set; }
        private Dictionary<Type, BaseFSMState> _stateDic = new Dictionary<Type, BaseFSMState>();
        private BaseFSMState _currState;
        private BaseFSMState _firstState;
        public  FSMStateMachine(string fmsName) {
            _stateDic = new Dictionary<Type, BaseFSMState>();
            Name = fmsName;
        }
        public FSMStateMachine(string fmsName,List<AttributeTypeData>classDataList)
        {
            _stateDic = new Dictionary<Type, BaseFSMState>();
            Name = fmsName;
            //根据Type 实例化状态
            for (int i = 0; i < classDataList.Count; i++)
            {
                //获取与当前名称一致的stateBase
                if (((FSMTypeAttribute)classDataList[i].attribute).FSMName == fmsName)
                {
                    BaseFSMState stateBase = Activator.CreateInstance(classDataList[i].type) as BaseFSMState;
                    stateBase.StateMachine = this;
                    stateBase.OnInit();
                    if (((FSMTypeAttribute)classDataList[i].attribute).isFirst)
                    {
                        _firstState = stateBase;
                       
                    }
                    _stateDic.Add(classDataList[i].type, stateBase);
                }
            }
        }
        /// <summary>
        /// 帧更新
        /// </summary>
        public void Update() {
            if (_currState != null) {
                _currState.OnUpdate();
            }
        }
        /// <summary>
        /// 当前状态
        /// </summary>
        public BaseFSMState CurrentState
        {
            get
            {
                return _currState;
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="type">状态类型</param>
        public void SwitchState(Type type)
        {
            if (_stateDic.ContainsKey(type))
            {
                if (_currState == _stateDic[type])
                {
                    return;
                }

                BaseFSMState lastState = _currState;
                BaseFSMState nextState = _stateDic[type];
                if (lastState != null)
                {
                    lastState.OnLeave(nextState);
                }
                nextState.OnEnter(lastState);
                _currState = nextState;

            }
            else
            {
               
            }
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="type">状态类型</param>
        /// <returns>状态实例</returns>
        public BaseFSMState GetState(Type type)
        {
            if (_stateDic.ContainsKey(type))
            {
                return _stateDic[type];
            }
            else
            {
                LWDebug.LogError("");
                return null;
               // throw new HTFrameworkException(HTFrameworkModule.FSM, "获取状态失败：有限状态机 " + Name + " 不存在状态 " + type.Name + "！");
            }
        }
        /// <summary>
        /// 是否存在状态
        /// </summary>
        /// <param name="type">状态类型</param>
        /// <returns>是否存在</returns>
        public bool IsExistState(Type type)
        {
            return _stateDic.ContainsKey(type);
        }
        //清空所有的状态
        public void ClearFMS()
        {
            foreach (var state in _stateDic)
            {
                state.Value.OnTermination();
            }
            _stateDic.Clear();

            
        }
        /// <summary>
        /// 启动默认状态
        /// </summary>
        public void StartFirst() {
            _currState = _firstState;
            _currState.OnEnter(null);
        }
    }
}


