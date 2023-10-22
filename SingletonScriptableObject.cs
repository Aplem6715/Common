
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Aplem.Common
{
    public abstract class SingletonScriptableObject<T> : ScriptableObject, ISerializationCallbackReceiver
    {
        protected static readonly ILogger _logger = LogManager.GetLogger(typeof(T).Name);

        protected static T _instance;

        public static T Inst
        {
            get
            {
                if (_instance == null)
                {
                    _logger.ZLogError("Not Initialized");
                }
                return _instance;
            }
        }

        public static async UniTask Init(string address, CancellationToken token)
        {
            if (_instance != null)
            {
                return;
            }
            _instance = await Addressables.LoadAssetAsync<T>(address).WithCancellation(token);
        }

        /// <summary>
        /// [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        /// をつけてリセットが必要
        /// </summary>
        protected abstract void Reset();

        public virtual void OnBeforeSerialize()
        {
        }

        public virtual void OnAfterDeserialize()
        {
        }
    }
}