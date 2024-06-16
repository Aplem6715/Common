using System;
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
    /// <summary>
    /// anchorを左端に固定したゲージ
    /// </summary>
    public class UIGauge : MonoBehaviour
    {
        [SerializeField] private RectTransform _gaugeMainRect;
        [SerializeField] private RectTransform _backRect;

        private void Awake()
        {
            _gaugeMainRect.pivot = new Vector2(0, 0.5f);
        }

        /// <summary>
        /// ゲージの設定
        /// </summary>
        /// <param name="value">0~1</param>
        public void SetGauge(float value)
        {
            value  = Mathf.Clamp01(value);
            var backSize = _backRect.sizeDelta;
            _gaugeMainRect.sizeDelta = backSize * new Vector2(value, 0);
        }
    }
}
