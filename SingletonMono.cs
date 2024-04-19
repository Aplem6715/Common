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
        protected static ILogger _logger;

        private static T _instance;

        [SerializeField] private Color _logColor = Color.white;

        public static T Inst
        {
            get
            {
                //if (_instance is null)
                //{
                //    var t = typeof(T);

                //    _instance = (T)FindObjectOfType(t);
                //    if (_instance == null)
                //        _logger.ZLogError(t + " をアタッチしているGameObjectはありません");
                //}

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                var color = _logColor == Color.white ? null : $"#{ColorUtility.ToHtmlStringRGBA(_logColor)}";
                _logger = LogManager.GetLogger(typeof(T).Name, color);
                _instance = this as T;
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
