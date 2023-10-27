

using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using Sirenix.OdinInspector;
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

        // Implementation of IPool
        [ShowInInspector]
        public int PoolingCount => _pool.Count;
        [ShowInInspector]
        public int ActiveCount => Capacity - PoolingCount;

        public bool IsPendingDestroy { get; private set; }

        private const int AsyncDestroyPerFrame = 10;

        [ShowInInspector]
        private int _capacity;
        public int Capacity
        {
            get { return _capacity; }
            protected set
            {
                _capacity = value;
                Debug.Assert(ActiveCount >= 0);
            }
        }

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

        protected virtual T Create()
        {
            T comp = GameObject.Instantiate(_motherPref).GetComponent<T>();
            comp.SetPool(this);
            comp.gameObject.SetActive(false);
            if (_parent)
            {
                comp.transform.SetParent(_parent);
            }
            Capacity++;
            comp.gameObject.name = $"{_motherPref.name}_{Capacity}";
            _instantiateProcessor?.Invoke(comp);
            return comp;
        }

        public virtual T Rent()
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

        public virtual void Return(IPoolable obj)
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
            obj.IsPooling = true && !IsPendingDestroy;
            retObj.OnReturned();

            if (IsPendingDestroy)
            {
                GameObject.Destroy(retObj.gameObject);
                Capacity--;
            }
            else
            {
#if DEBUG
                if (_pool.Contains(retObj))
                {
                    Debug.LogError("重複Return", retObj.gameObject);
                }
#endif
                _pool.Push(retObj);
            }
        }

        public async UniTask DestroyAsync(CancellationToken token)
        {
            IsPendingDestroy = true;
            while (_pool.Count != 0)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                for (int i = 0; i < AsyncDestroyPerFrame; i++)
                {
                    if (!_pool.TryPop(out T item))
                    {
                        break;
                    }
                    GameObject.Destroy(item.gameObject);
                    Capacity--;
                }
                await UniTask.DelayFrame(1);
            }

            await ((IPool)this).WaitUntilReturnAll(token);
        }

        public void DestroyAllPooled()
        {
            while (_pool.Count > 0)
            {
                var item = _pool.Pop();
                if (item?.gameObject != null)
                {
                    GameObject.Destroy(item.gameObject);
                }
            }
        }
    }
}
