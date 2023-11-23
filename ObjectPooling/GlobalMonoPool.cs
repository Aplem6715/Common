using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aplem.Common
{
#if UNITY_EDITOR
    public class GlobalMonoPoolReset
    {
        public static event Action OnInitialize;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Clear()
        {
            OnInitialize?.Invoke();
        }
    }
#endif

    public class GlobalMonoPool<T> where T : IPoolableMono
    {
        private static MonoPool<T> _instance;

        public static MonoPool<T> Inst => _instance;

        public static void ResetInstance()
        {
            _instance = null;
        }

        public static void Init(GameObject motherPref)
        {
            Init(motherPref, null, 0);
        }

        public static void Init(GameObject motherPref, int capacity)
        {
            Init(motherPref, null, capacity);
        }

        public static void Init(GameObject motherPref, Transform parent)
        {
            Init(motherPref, parent, 0);
        }

        public static void Init(GameObject motherPref, Transform parent, int capacity)
        {
            _instance = new MonoPool<T>(motherPref, parent, capacity);
#if UNITY_EDITOR
            GlobalMonoPoolReset.OnInitialize -= ResetInstance;
            GlobalMonoPoolReset.OnInitialize += ResetInstance;
#endif
        }
    }
}
