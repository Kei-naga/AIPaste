using System;

namespace AIPaste.common
{
    public interface IMyLogger
    {
        bool IsDevelopmentMode { get; }
        void SetDevelopmentMode(bool isDev);
        void Info(string messageKey);
        void Info(string messageKey, string message);
        void Warn(string messageKey);
        void Warn(string messageKey, string message);
        void Error(string messageKey);
        void Error(string messageKey, string message);
        void Debug(string message);
        void Debug(Exception ex, string message);
        void Debug(Exception ex);
        void Trace(string message);

    }
}
