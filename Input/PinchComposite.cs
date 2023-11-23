using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;

namespace Aplem.Common
{
    public class TouchDeltaMagnitudeComparer : IComparer<TouchState>
    {
        public int Compare(TouchState x, TouchState y)
        {
            var lenX = x.delta.sqrMagnitude;
            var lenY = y.delta.sqrMagnitude;

            return Comparer<float>.Default.Compare(lenX, lenY);
        }
    }

    [DisplayName("Pinch Composite")]
    public class PinchComposite : InputBindingComposite<float>
    {
        // タッチ入力
        [InputControl(layout = "Touch")] public int touch0 = 0;
        [InputControl(layout = "Touch")] public int touch1 = 0;

        public float speed = 1.0f;

        /// <summary>
        /// 初期化
        /// </summary>
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
        private static void Initialize()
        {
            // 初回にCompositeBindingを登録する必要がある
            InputSystem.RegisterBindingComposite(typeof(PinchComposite), "PinchComposite");
        }

        /// <summary>
        /// 値の大きさを返す
        /// </summary>
        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            return ReadValue(ref context) * speed;
        }

        /// <summary>
        /// ピンチ操作量の取得
        /// </summary>
        public override float ReadValue(ref InputBindingCompositeContext context)
        {
            var touchState0 = context.ReadValue<TouchState, TouchDeltaMagnitudeComparer>(touch0);
            var touchState1 = context.ReadValue<TouchState, TouchDeltaMagnitudeComparer>(touch1);

            // ２本指が移動していなかれば操作なしと判断
            if (!touchState0.isInProgress || !touchState1.isInProgress)
                return 0;

            // タッチ位置（スクリーン座標）
            var pos0 = touchState0.position;
            var pos1 = touchState1.position;

            // 移動量（スクリーン座標）
            var delta0 = touchState0.delta;
            var delta1 = touchState1.delta;

            // 移動前の位置（スクリーン座標）
            var prevPos0 = pos0 - delta0;
            var prevPos1 = pos1 - delta1;

            return Vector2.Distance(pos0, pos1) - Vector2.Distance(prevPos0, prevPos1);
        }
    }
}
