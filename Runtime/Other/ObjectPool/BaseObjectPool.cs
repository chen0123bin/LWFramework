using System.Collections.Generic;

namespace LWFramework
{
    public abstract class BaseObjectPool<T>
    {
        public int poolMaxSize { get; protected set; }

        public string name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="poolMaxSize">线程池的大小，回收时如果池子大小已满，则多余的对象会被遗弃</param>
        public BaseObjectPool(int poolMaxSize)
        {
            this.poolMaxSize = poolMaxSize;
        }
        public abstract T Spawn();
        public abstract void Unspawn (T obj);
        public abstract void UnspawnAll();
        public abstract void Clear();
    };
    /// <summary>
    /// 对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> : BaseObjectPool<T> where T : class, IPoolGameObject
    {
        protected List<T> _poolList = new List<T>();

        public int CurrentSize
        {
            get
            {
                return _poolList.Count;
            }
        }

        public ObjectPool(int poolMaxSize) : base(poolMaxSize)
        {
        }

        /// <summary>
        /// 改变对象池大小
        /// </summary>
        /// <param name="poolMaxSize"></param>
        public void ChangeSize(int poolMaxSize)
        {
            this.poolMaxSize = poolMaxSize;
            if(_poolList.Count > poolMaxSize)
            {
                for(int i = _poolList.Count - 1; i >= poolMaxSize; i--)
                {
                    _poolList[i].Release();
                    _poolList.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public override T Spawn()
        {
            while (_poolList.Count > 0)
            {
                var ins = _poolList[0];
                _poolList.RemoveAt(0);
                return ins;
            }
            return default;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="obj"></param>
        public override void Unspawn(T obj)
        {
            if (_poolList.Count < poolMaxSize)
            {
                obj.Unspawn();
                _poolList.Add(obj);
            }
            else
            {
                obj.Release();
                _poolList.Remove(obj);
            }
        }

        /// <summary>
        /// 是否在对象池中
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsInPool(T obj)
        {
            return _poolList.IndexOf(obj) > -1 ? true : false;
        }
        /// <summary>
        /// 回收所有的对象
        /// </summary>
        public override void UnspawnAll()
        {
            foreach (var obj in _poolList)
            {
                Unspawn(obj);
            }
        }
        /// <summary>
        /// 清空对象池
        /// </summary>
        public override void Clear()
        {
            foreach (var obj in _poolList)
            {
                obj.Release();
            }
            _poolList.Clear();
        }
    }
}
