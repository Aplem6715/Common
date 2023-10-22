using System.IO;
using Cysharp.Text;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ZLogger;

namespace Aplem.Common
{
    public static class LogManager
    {
        static Microsoft.Extensions.Logging.ILogger globalLogger;
        static ILoggerFactory loggerFactory;

#if (UNITY_STANDALONE && UNITY_DEBUG)
        static readonly string logFilePath = "log/debuglog.log";
#endif

        static LogManager()
        {
            Init();
        }

        [RuntimeInitializeOnLoadMethod]
        static void Reload()
        {
            Application.quitting -= Quit;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Application.quitting += Quit;

            // Standard LoggerFactory does not work on IL2CPP,
            // But you can use ZLogger's UnityLoggerFactory instead,
            // it works on IL2CPP, all platforms(includes mobile).
            loggerFactory = UnityLoggerFactory.Create(builder =>
            {

                builder.ClearProviders();

                // or more configuration, you can use builder.AddFilter
                builder.SetMinimumLevel(LogLevel.Trace);

#if (UNITY_STANDALONE && UNITY_DEBUG)
                File.Delete(logFilePath);
                builder.AddZLoggerFile(logFilePath, options => {
                    options.EnableStructuredLogging = false;
                    options.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "{0} 【{1}】 ", info.Timestamp.LocalDateTime, info.CategoryName);
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
                    options.PrefixFormatter = (writer, info) => ZString.Utf8Format(writer, "【{0}】 ", info.CategoryName);
                });

                // and other configuration(AddFileLog, etc...)
            });

            globalLogger = loggerFactory.CreateLogger("Global");
        }

        public static void Quit()
        {
            // when quit, flush unfinished log entries.
            loggerFactory.Dispose();
            loggerFactory = null;
        }

        public static Microsoft.Extensions.Logging.ILogger Logger => globalLogger;

        public static ILogger<T> GetLogger<T>() where T : class => loggerFactory.CreateLogger<T>();
        public static Microsoft.Extensions.Logging.ILogger GetLogger(string categoryName) => loggerFactory.CreateLogger(categoryName);
    }
}