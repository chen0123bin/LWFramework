using LWFramework.FMS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace LWFramework.Core {
    /// <summary>
    /// 非热更环境主管理器
    /// </summary>
    [ManagerClass(ManagerType.Main)]
    public class MainManager : Singleton<MainManager>, IManager
    {  
        //热更DLL中所有的type
        private List<Type> typeHotfixArray;
        //管理热更中的所有的Type
        private Dictionary<string, List<AttributeTypeData>> attrTypeHotfixDic;
        private Dictionary<string, IManager> _managerDic;
        private List<IManager> _managerList;
        public MainManager()
        {
            _managerDic = new Dictionary<string, IManager>();
            _managerList = new List<IManager>();
            Type[] typeArray = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var t in typeArray)
            {
                if (t.IsClass)
                {
                    var attr = (ManagerClass)t.GetCustomAttribute(typeof(ManagerClass), false);
                    if (attr != null && attr.managerType == ManagerType.Normal)
                    {
                        IManager manager = Activator.CreateInstance(t) as IManager;
                        ////注册
                        AddManager(manager);

                    }
                }
            }
        }
    
        public void InitHotfixManager(List<Type> typeArray)
        {
            attrTypeHotfixDic = new Dictionary<string, List<AttributeTypeData>>();
            this.typeHotfixArray = typeArray;

            //将所有带有特性的类进行字典管理
            foreach (var item in typeArray)
            {
                if (item.IsClass)
                {
                    var attrs = item.GetCustomAttributes(false);
                    foreach (var attr in attrs)
                    {
                        if (!attrTypeHotfixDic.ContainsKey(attr.ToString()))
                        {
                            attrTypeHotfixDic[attr.ToString()] = new List<AttributeTypeData>();
                        }
                        AttributeTypeData classData = new AttributeTypeData { attribute = (Attribute)attr, type = item };
                        attrTypeHotfixDic[attr.ToString()].Add(classData);
                    }

                }
            }
            //获取所有带ManagerHotfixClass特性的类，这些都属于管理类
            List<AttributeTypeData> normalManamgerType = GetTypeListByAttr<ManagerHotfixClass>();
            for (int i = 0; normalManamgerType!=null && i < normalManamgerType.Count; i++)
            {
                Attribute attr = normalManamgerType[i].attribute;
                if (((ManagerHotfixClass)attr).managerType == ManagerHotfixType.NormalHotfix)
                {
                    // LWDebug.Log("ManagerHotfixClass: " + attr + "______ " + "   Type:" + normalManamgerType[i].type.ToString() + "____" );
                    IManager manager = Activator.CreateInstance(normalManamgerType[i].type) as IManager;
                    ////注册
                    AddManager(manager);
                }
            }
        }

        public void AddManager(IManager t)
        {
            t.Init();
            _managerDic.Add(t.GetType().ToString(), t);
            _managerList.Add(t);
        }
        public T GetManager<T>()
        {
            IManager manager;
            if (_managerDic.TryGetValue(typeof(T).ToString(), out manager))
            {
                return (T)_managerDic[typeof(T).ToString()];
            }
            else
            {
                LWDebug.LogError(typeof(T).ToString() + " 这个Manager 不存在，请先检查是否加入了ManagerClass特性");
                return (T)manager;
            }
        }
        public void Init()
        {
            //foreach (var item in managerDic.Values)
            //{
            //    item.Init();
            //}
        }

        public void Update()
        {
            for (int i = 0; i < _managerList.Count; i++)
            {
                _managerList[i].Update();
            }  
        }

        public void LateUpdate()
        {
            for (int i = 0; i < _managerList.Count; i++)
            {
                _managerList[i].LateUpdate();
            }
        }
        /// <summary>
        /// 启动流程管理
        /// </summary>
        public void StartProcedure()
        {
            GetManager<FSMManager>().InitFSMManager();
            //找到所有的流程管理类
            List<AttributeTypeData> procedureList = GetManager<FSMManager>().GetFsmClassDataByName(nameof(FSMName.Procedure));
            //创建一个流程管理状态机       
            FSMStateMachine stateMachine = new FSMStateMachine(nameof(FSMName.Procedure), procedureList);
            GetManager<FSMManager>().RegisterFSM(stateMachine);
            GetManager<FSMManager>().GetFSMByName(nameof(FSMName.Procedure)).StartFirst();
        }
        /// <summary>
        /// 根据特性去获取对应的所有type
        /// </summary>
        /// <typeparam name="T">特性</typeparam>
        /// <returns></returns>
        public List<AttributeTypeData> GetTypeListByAttr<T>()
        {
            if (!attrTypeHotfixDic.ContainsKey(typeof(T).FullName))
            {
                LWDebug.LogWarning("当前域下找不到这个包含这个特性的类" + typeof(T).FullName);
                return null;
            }
            else
            {
                return attrTypeHotfixDic[typeof(T).FullName];
            }

        }
    }
    public enum ManagerType
    {
        Main, Normal
    }
    public class ManagerClass : Attribute
    {
        public ManagerType managerType;
        public ManagerClass(ManagerType managerType)
        {
            this.managerType = managerType;
        }
    }
    public enum ManagerHotfixType
    {
        MainHotfix, NormalHotfix
    }
    public class ManagerHotfixClass : Attribute
    {
        public ManagerHotfixType managerType;
        public ManagerHotfixClass(ManagerHotfixType managerType)
        {
            this.managerType = managerType;
        }
    }
    public class AttributeTypeData
    {
        public Attribute attribute;
        public Type type;
    }
}
