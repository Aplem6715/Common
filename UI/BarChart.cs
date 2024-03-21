using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

namespace Aplem.Common.UI
{
    public class BarChart : MonoBehaviour
    {
        [SerializeField] private List<BarPart> _parts;

        private float _length;
        private float _animTime = 0.5f;
        private Sequence _currentSeq;

        private void Awake()
        {
            var rectTrans = GetComponent<RectTransform>();
            if (rectTrans == null)
            {
                Debug.LogError("RectTransform not found", gameObject);
                return;
            }
            var rect = rectTrans.rect;

            _length = rect.width;
        }

        /// <summary>
        /// 値を設定
        /// </summary>
        /// <param name="values">0~1で各項目の割合を指定</param>
        public void Setup(IReadOnlyList<float> values)
        {
            int last = Mathf.Min(values.Count, _parts.Count);
            for (int i = 0; i < last; i++)
            {
                var childWidth = _length * values[i];
                var animTime = _animTime * values[i];
                _parts[i].Setup(childWidth, animTime);
            }
        }

        public void PlayAnim()
        {
            var seq = DOTween.Sequence();
            foreach(var child in _parts)
            {
                seq.Append(child.DOAnim());
            }
            _currentSeq = seq.Play();
        }

        public void SkipAnim()
        {
            _currentSeq?.Complete();
        }
    }
}
