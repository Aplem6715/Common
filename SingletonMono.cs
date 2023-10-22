using System;
using Aplem.Common;
using UnityEngine;
using ZLogger;
using Sirenix.OdinInspector;

using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Aplem.Common
{
    public abstract class SingletonMono<T> : SerializedMonoBehaviour where T : MonoBehaviour
    {

        protected static readonly ILogger _logger = LogManager.GetLogger(typeof(T).Name);

        private static T _instance;

        public static T Inst
        {
            get
            {
                if (_instance == null)
                {
                    Type t = typeof(T);

                    _instance = (T)FindObjectOfType(t);
                    if (_instance == null)
                    {
                        _logger.ZLogError(t + " をアタッチしているGameObjectはありません");
                    }
                }

                return _instance;
            }
        }

        virtual protected void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}