using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Aplem.Common
{

    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class AplemTapInteraction : IInputInteraction
    {
        // タップ開始から終了までの許容移動距離
        public float Distance = 20.0f;
        public float TapTime = 0.5f;
        private float _tapTime => TapTime > 0.0f ? TapTime : InputSystem.settings.defaultTapTime;
        private float _pressPoint = InputSystem.settings.defaultButtonPressPoint;

        private State _state = State.Idle;
        private Vector2 _startPos;
        private float _distSqr;

        enum State
        {
            Idle,
            Interacting,
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void Initialize()
        {
            InputSystem.RegisterInteraction<AplemTapInteraction>();
        }

        public void Process(ref InputInteractionContext context)
        {
            if (context.timerHasExpired)
            {
                _state = State.Idle;
                context.Canceled();
                return;
            }

            switch (_state)
            {
                case State.Idle:
                    if (((ButtonControl)context.action.controls[0]).isPressed)
                    {
                        _startPos = context.ReadValue<Vector2>();
                        context.Started();
                        context.SetTimeout(_tapTime);
                        _state = State.Interacting;
                    }
                    break;
                case State.Interacting:
                    if (!((ButtonControl)context.action.controls[0]).isPressed)
                    {
                        var currentPos = context.ReadValue<Vector2>();
                        var delta = _startPos - currentPos;
                        if (delta.sqrMagnitude < _distSqr)
                        {
                            context.Performed();
                            context.Waiting();
                            _state = State.Idle;
                        }
                    }
                    break;
                default:
                    Debug.LogError("Invalid Tap State");
                    break;
            }
        }

        public void Reset()
        {
            _distSqr = Distance * Distance;
        }
    }
}
