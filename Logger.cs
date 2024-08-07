using System.IO;
using Cysharp.Text;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;
using ZLogger.Unity;

namespace Aplem.Common
{
    public static class LogManager
    {
        private static Microsoft.Extensions.Logging.ILogger _globalLogger;
        private static ILoggerFactory _loggerFactoryInner;

        private static ILoggerFactory _loggerFactory
        {
            get
            {
                if (_loggerFactoryInner == null)
                    Init();
                return _loggerFactoryInner;
            }
            set => _loggerFactoryInner = value;
        }

#if (UNITY_STANDALONE && UNITY_DEBUG)
        static readonly string logFilePath = "log/debuglog.log";
#endif

        static LogManager()
        {
            Init();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Reload()
        {
            Application.quitting -= Quit;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Application.quitting -= Quit;
            Application.quitting += Quit;

            // Standard LoggerFactory does not work on IL2CPP,
            // But you can use ZLogger's UnityLoggerFactory instead,
            // it works on IL2CPP, all platforms(includes mobile).
            _loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();

                // or more configuration, you can use builder.AddFilter
                builder.SetMinimumLevel(LogLevel.Trace);

#if (UNITY_STANDALONE && UNITY_DEBUG)
                File.Delete(logFilePath);
                builder.AddZLoggerFile(logFilePath, options => {
                    options.EnableStructuredLogging = false;
                    options.PrefixFormatter =
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                (writer, info) => ZString.Utf8Format(writer, "{0} 【{1}】 ", info.Timestamp.LocalDateTime, info.CategoryName);
                });
#endif

                // AddZLoggerUnityDebug is only available for Unity, it send log to UnityEngine.Debug.Log.
                // LogLevels are translate to
                // * Trace/Debug/Information -> LogType.Log
                // * Warning/Critical -> LogType.Warning
                // * Error without Exception -> LogType.Error
                // * Error with Exception -> LogException
                builder.AddZLoggerUnityDebug(options =>
                {
                    options.UsePlainTextFormatter(formatter =>
                    {
                        formatter.SetPrefixFormatter($"【{0}】 ",
                            (in MessageTemplate template, in LogInfo info) => template.Format(info.Category));
                    });
                });

                // and other configuration(AddFileLog, etc...)
            });

            _globalLogger = _loggerFactory.CreateLogger("Global");
        }

        public static void Quit()
        {
            // when quit, flush unfinished log entries.
            _loggerFactory.Dispose();
            _loggerFactory = null;
        }

        public static Microsoft.Extensions.Logging.ILogger Logger => _globalLogger;

        public static ILogger<T> GetLogger<T>() where T : class
        {
            return _loggerFactory.CreateLogger<T>();
        }

        /// <summary>
        /// カテゴリー名を指定してロガーを取得する
        /// </summary>
        /// <param name="categoryName">カテゴリー名</param>
        /// <param name="color">色（https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/StyledText.html）</param>
        /// <returns>ロガー</returns>
        public static Microsoft.Extensions.Logging.ILogger GetLogger(string categoryName, string color = null)
        {
            if (color == null)
            {
                return _loggerFactory.CreateLogger(categoryName);
            }
            else
            {
                // categoryNameをUnityの色タグで囲む
                var coloredCategoryName = ZString.Format("<b><color={0}>{1}</color></b>", color, categoryName);
                return _loggerFactory.CreateLogger(coloredCategoryName);
            }
        }
    }
}
