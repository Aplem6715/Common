using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Aplem.Common
{
    [Serializable]
    public class Reactive<T> where T : IEquatable<T>
    {
        public event Action<T> OnValueChanged;

        public T Value
        {
            get => _value;
            set
            {
                if (value.Equals(_value))
                    return;
                _value = value;
                OnValueChanged?.Invoke(_value);
            }
        }

        [SerializeField, ReadOnly] private T _value;

        public Reactive(T defaultValue)
        {
            _value = defaultValue;
        }

        public WeakReference<Reactive<T>> ToWeak() => new(this);
    }
}
