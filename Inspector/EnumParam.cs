// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY AccessorBuilder. DO NOT CHANGE IT.
// </auto-generated>

using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using UnityEngine;

namespace Aplem.Data
{
    [Serializable]
    public struct EnumParam<T>
#if UNITY_EDITOR
        : ISerializationCallbackReceiver
#endif
        where T : struct, Enum
    {
        [SerializeField, OnValueChanged("OnEnumChanged", InvokeOnInitialize = false)]
        private T _value;
        public T Value => _value;
#if UNITY_EDITOR
        [SerializeField, ReadOnly] private string _enumStr;
#endif


        public static implicit operator T(EnumParam<T> self)
        {
            return self._value;
        }


        // 以下 Editor Only
#if UNITY_EDITOR
        public void SetLabel(T value)
        {
            _value = value;
            _enumStr = _value.ToString();
        }

        public void SetStr(string enumStr)
        {
            _enumStr = enumStr;
        }

        public void OnAfterDeserialize()
        {
            Fix();
        }

        public void OnBeforeSerialize()
        {
            Fix();
        }

        private void Fix()
        {
            // ラベル名が空の場合は無視
            if(_enumStr.IsNullOrWhitespace())
            {
                SetLabel(default(T));
                return;
            }

            if (_value.ToString() != _enumStr)
            {
                try
                {
                    var prev = _value;
                    _value = (T)Enum.Parse<T>(_enumStr);
                    Debug.LogFormat("EnumParamの値が保存された文字列と異なるため文字列を元に復元しました。\nstr:{0:s} beforeNo:{1:d} afterNo:{2:d}", _enumStr, prev, _value);
                }
                catch (ArgumentException ex)
                {
                    Debug.LogError($"シリアライズされたEnumから以前までのEnum名:[{_enumStr}] (現在のEnum名:{_value.ToString()})のデータが削除されました。\n応急処置としてdefault={default(T)}を設定します。\n{ex}");
                    SetLabel(default(T));
                }
            }
        }

        private void OnEnumChanged()
        {
            _enumStr = _value.ToString();
        }
#endif
    }
}