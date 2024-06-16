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
    /// (min, max)の範囲を表すゲージ
    /// </summary>
    public class RangeGauge : MonoBehaviour
    {
        [SerializeField] private LayoutElement _leftSpacer;
        [SerializeField] private LayoutElement _rightSpacer;
        [SerializeField] private LayoutElement _gaugeLayout;

        /// <summary>
        /// ゲージの設定
        /// </summary>
        /// <param name="min">左端(0~1)</param>
        /// <param name="max">右端(0~1)</param>
        public void Setup(float min, float max)
        {
            Debug.Assert(min <= max);

            min = Mathf.Max(0, min);
            max = Mathf.Min(1, max);
            _leftSpacer.flexibleWidth = min;
            _rightSpacer.flexibleWidth = 1 - max;
            _gaugeLayout.flexibleWidth = max - min;
        }
    }
}
