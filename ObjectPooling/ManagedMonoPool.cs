using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;
using System;


namespace Aplem.Common
{

    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public interface IManagedPool : IPool
    {
        public void DestroyAll();
    }

    public class ManagedMonoPool<T> : MonoPool<T>, IManagedPool where T : class, IPoolableMono
    {
        private List<WeakReference<T>> _all;

        public ManagedMonoPool() : this(null, null, 0) { }
        public ManagedMonoPool(GameObject motherPref) : this(motherPref, null, 0) { }
        public ManagedMonoPool(GameObject motherPref, int capacity) : this(motherPref, null, capacity) { }
        public ManagedMonoPool(GameObject motherPref, Transform parent) : this(motherPref, parent, 0) { }

        public ManagedMonoPool(GameObject motherPref, Transform parent, int capacity, Action<T> onInstantiateProcessor = null)
        : base(motherPref, parent, capacity, onInstantiateProcessor)
        {
        }

        public void DestroyAll()
        {
            foreach (var weak in _all)
            {
                if (weak.TryGetTarget(out T comp) && comp.gameObject != null)
                {
                    GameObject.Destroy(comp.gameObject);
                }
            }
            _all.Clear();
            DestroyAllPooled();
            Capacity = 0;
        }

        protected override T Create()
        {
            var created = base.Create();
            if (_all == null)
            {
                _all = new List<WeakReference<T>>(Capacity);
            }

            _all.Add(new WeakReference<T>(created));
            return created;
        }
    }
}
