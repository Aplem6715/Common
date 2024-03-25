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
    [RequireComponent(typeof(Image))]
    public class BarPart : MonoBehaviour
    {
        private RectTransform _rect;
        private float _fullLength;
        private float _height;
        private float _animTime;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();

            var size = _rect.sizeDelta;
            size.x = 0;
            _rect.sizeDelta = size;

            _height = size.y;
        }

        public void Setup(float length, float animTime)
        {
            _fullLength = length;
            _animTime = animTime;
        }

        public Tweener DOAnim()
        {
            return _rect.DOSizeDelta(new Vector2(_fullLength, _height), _animTime);
        }

        public void ResetAnim()
        {
            _rect.sizeDelta = new Vector2(0, _height);
        }
    }
}
