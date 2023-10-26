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

        public virtual void Setup(Vector3 pos, int damage)
        {
            transform.position = pos;
            _text.SetText(damage);

            Play().Forget();
        }

        protected virtual async UniTask Play()
        {
            // TODO: 時間・アニメーション
            await UniTask.Delay(1000);
            ((IPoolable)this).Return();
        }


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
