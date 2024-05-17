using System.Collections.Generic;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Aplem.Common
{
    public interface ISingletonBase
    {
        public void ResetInner();
    }
    
    public interface ISingleton
    {
        public void SingletonInitialize();
    }

    public static class SingletonCleaner
    {
        private static List<ISingletonBase> _singletons = new();
        
        public static void Register(ISingletonBase singleton)
        {
            _singletons.Add(singleton);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Reset()
        {
            // Resetにより新たなsingletonがRegisterされ，_singletonsに追加されるため
            // 先にコピー＆クリアしてからリセット
            var oldSingletons = _singletons.ToArray();
            _singletons.Clear();
            
            foreach (var singleton in oldSingletons)
            {
                singleton.ResetInner();
            }
        }
    }
    
    public abstract class Singleton<T> : ISingleton, ISingletonBase where T : ISingleton, new()
    {
        protected static readonly ILogger _logger = LogManager.GetLogger(typeof(T).Name);

        public static T Inst { get; private set; } = new();

        protected Singleton()
        {
            SingletonCleaner.Register(this);
        }
        
        public void ResetInner()
        {
            Inst = new T();
            Inst.SingletonInitialize();
        }

        public abstract void SingletonInitialize();
    }
}
