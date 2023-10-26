using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;
using TMPro;
using Cysharp.Text;
using Cysharp.Threading.Tasks;


namespace Aplem.Common
{

    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class DamagePop : MonoBehaviour, IPoolableMono
    {
        [SerializeField]
        protected TextMeshPro _text;

        public virtual async UniTask Open(Vector3 pos, int damage)
        {
            transform.position = pos;
            _text.SetText(damage);

            await Play();

            ((IPoolable)this).Return();
        }

        protected virtual UniTask Play() { return UniTask.CompletedTask; }


        /* implementation of IPoolableMono */

        public bool IsPooling { get; set; }
        public IPool _pool { get; set; }

        public void OnRent()
        {
        }

        public void OnReturned()
        {
        }
    }
}
