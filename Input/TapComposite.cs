using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Aplem.Common
{
    public class VectorComparer : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            var lenX = x.sqrMagnitude;
            var lenY = y.sqrMagnitude;

            return Comparer<float>.Default.Compare(lenX, lenY);
        }
    }

    [InitializeOnLoad]
    [DisplayName("Tap Composite")]
    public class TapComposite : InputBindingComposite<Vector2>
    {
        [InputControl(layout = "Button")] [UsedImplicitly]
        public int Button;

        [InputControl(layout = "Vector2")] [UsedImplicitly]
        public int Position;


        static TapComposite()
        {
            InputSystem.RegisterBindingComposite<TapComposite>();
        }

        [RuntimeInitializeOnLoadMethod]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private static void Init()
        {
        }

        public override Vector2 ReadValue(ref InputBindingCompositeContext context)
        {
            return context.ReadValue<Vector2, VectorComparer>(Position);
        }

        public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
        {
            return ReadValue(ref context).magnitude;
        }
    }
}
