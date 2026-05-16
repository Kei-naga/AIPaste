using System;
using NLog;

namespace AIPaste.common
{
    public class MyLogger: IMyLogger
    {
        private static MyLogger? _instance;
        private static readonly object _lock = new();
        private readonly Logger _nLogger;
        private readonly LogMessages _logMessages = new();
        public bool IsDevelopmentMode { get; private set; } = false;

        public void SetDevelopmentMode(bool isDev)
        {
            IsDevelopmentMode = isDev;
            if (LogManager.Configuration == null)
            {
                return;
            }

            if (isDev)
            {
                LogManager.Configuration.Variables["isDevelopment"] = "true";
                Info("START_DEBUG");
            }
            else
            {
                LogManager.Configuration.Variables["isDevelopment"] = "false";
            }

            LogManager.ReconfigExistingLoggers();
        }

        public static MyLogger GetInstance()
        {
            lock (_lock)
            {
                _instance ??= new MyLogger();
                return _instance;
            }
        }

        private MyLogger()
        {
            CrashDiagnostics.Initialize();
            if (LogManager.Configuration != null)
            {
                LogManager.Configuration.Variables["isDevelopment"] = "false";
                LogManager.Configuration.Variables["logRoot"] = CrashDiagnostics.LogDirectory;
                LogManager.ReconfigExistingLoggers();
            }

            _nLogger = LogManager.GetCurrentClassLogger();
        }

        public void Debug(string message)
        {
            _nLogger.Debug(message);
        }

        public void Debug(Exception ex, string message)
        {
            _nLogger.Debug(ex, message);
        }

        public void Debug(Exception ex)
        {
            _nLogger.Debug(ex);
        }
        public void Info(string messageKey)
        {
            _nLogger.Info(_logMessages._InfoMessages[messageKey]);
        }

        public void Info(string messageKey, string message)
        {
            _nLogger.Info(_logMessages._InfoMessages[messageKey] + ":" + message);
        }

        public void Warn(string messageKey)
        {
            _nLogger.Warn(_logMessages._warnMessages[messageKey]);
        }
        public void Warn(string messageKey, string message)
        {
            _nLogger.Warn(_logMessages._warnMessages[messageKey] + ":" + message);
        }

        public void Error(string messageKey)
        {
            _nLogger.Error(_logMessages._errorMessages[messageKey]);
        }
        public void Error(string messageKey, string message)
        {
            _nLogger.Error(_logMessages._errorMessages[messageKey] + ":" + message);
        }

        public void Trace(string message)
        {
            _nLogger.Trace(message);
        }
    }
}
