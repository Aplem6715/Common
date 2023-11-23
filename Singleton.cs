using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Aplem.Common
{
    public interface ISingleton
    {
        public void Reset();
    }

    public abstract class Singleton<T> : ISingleton where T : ISingleton, new()
    {
        protected static readonly ILogger _logger = LogManager.GetLogger(typeof(T).Name);

        private static T _instance = new();

        public static T Inst => _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetInner()
        {
            Inst.Reset();
            _instance = default;
        }

        public abstract void Reset();
    }
}
