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

    public class AplemDragInteraction : IInputInteraction
    {
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        public static void Initialize()
        {
            InputSystem.RegisterInteraction<AplemDragInteraction>();
        }

        public void Process(ref InputInteractionContext context)
        {
            // if (context.timerHasExpired)
            // {
            //     _state = State.Idle;
            //     context.Canceled();
            //     return;
            // }

            switch (context.phase)
            {
                case InputActionPhase.Disabled:
                    break;
                case InputActionPhase.Waiting:
                    if (((ButtonControl)context.action.controls[0]).isPressed)
                        context.Started();
                    break;
                case InputActionPhase.Started:
                    context.PerformedAndStayPerformed();
                    break;
                case InputActionPhase.Performed:
                    if (((ButtonControl)context.action.controls[0]).isPressed)
                        context.PerformedAndStayPerformed();
                    else
                        context.Canceled();
                    break;
                case InputActionPhase.Canceled:
                    break;
                default:
                    Debug.LogError("Invalid Tap State");
                    break;
            }
        }

        public void Reset()
        {
        }
    }
}
