

using System;
using System.Collections.Generic;
using UnityEngine;
using ZLogger;

namespace Aplem.Common
{
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class MonoPool<T> : IPool where T : IPoolableMono
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(MonoPool<T>).Name);
        private Stack<T> _pool;
        private GameObject _motherPref;
        private Action<T> _instantiateProcessor;
        public Transform _parent { get; private set; }


        public MonoPool() : this(null, null, 0) { }
        public MonoPool(GameObject motherPref) : this(motherPref, null, 0) { }
        public MonoPool(GameObject motherPref, int capacity) : this(motherPref, null, capacity) { }
        public MonoPool(GameObject motherPref, Transform parent) : this(motherPref, parent, 0) { }

        public MonoPool(GameObject motherPref, Transform parent, int capacity, Action<T> onInstantiateProcessor = null)
        {
            bool hasComponent = motherPref.TryGetComponent<T>(out _);
            if (motherPref == null || !hasComponent)
            {
                _logger.ZLogWarning($"MotherPref Don't have Component:{typeof(T)}");
                return;
            }

            _instantiateProcessor = onInstantiateProcessor;
            _motherPref = motherPref;
            _parent = parent;
            _pool = new Stack<T>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                T comp = Create();
                _pool.Push(comp);
            }
        }

        private T Create()
        {
            T comp = GameObject.Instantiate(_motherPref).GetComponent<T>();
            comp.SetPool(this);
            comp.gameObject.SetActive(false);
            if (_parent)
            {
                comp.transform.SetParent(_parent);
            }
            _instantiateProcessor?.Invoke(comp);
            return comp;
        }

        public T Rent()
        {
            T obj;
            if (_pool.Count == 0)
            {
                obj = Create();
            }
            else
            {
                obj = _pool.Pop();
            }
            obj.IsPooling = false;
            obj.OnRent();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(IPoolable obj)
        {
            T retObj = (T)obj;
            if (retObj is null)
            {
                _logger.ZLogError("returned object is not type of {0}", typeof(T));
            }

            if (_parent)
            {
                retObj.transform.SetParent(_parent);
            }
            retObj.gameObject.SetActive(false);
            obj.IsPooling = true;
            retObj.OnReturned();
            _pool.Push(retObj);
        }

        public void Clear()
        {
            while (_pool.Count > 0)
            {
                T obj = _pool.Pop();
                GameObject.Destroy(obj.gameObject);
            }
        }
    }
}