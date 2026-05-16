using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AIPaste.common
{
    public static class CrashDiagnostics
    {
        private static readonly object _lock = new();
        private static readonly string _logDirectory = GetLogDirectory();

        public static string LogDirectory => _logDirectory;

        public static void Initialize()
        {
            Directory.CreateDirectory(_logDirectory);
            WriteTrace($"Crash diagnostics initialized. PID={Environment.ProcessId}, LogDirectory={_logDirectory}");
        }

        public static void WriteTrace(string message)
        {
            Write("startup-trace.log", $"{DateTimeOffset.Now:O} | TRACE | {message}");
        }

        public static void WriteException(string source, Exception exception)
        {
            var builder = new StringBuilder()
                .AppendLine($"{DateTimeOffset.Now:O} | CRASH | Source={source}")
                .AppendLine($"{exception.GetType().FullName}: {exception.Message}")
                .AppendLine(exception.StackTrace ?? "<no stack trace>");

            var innerException = exception.InnerException;
            var depth = 0;
            while (innerException != null)
            {
                depth++;
                builder
                    .AppendLine()
                    .AppendLine($"InnerException[{depth}]")
                    .AppendLine($"{innerException.GetType().FullName}: {innerException.Message}")
                    .AppendLine(innerException.StackTrace ?? "<no stack trace>");
                innerException = innerException.InnerException;
            }

            Write($"crash-{DateTime.Now:yyyyMMdd}.log", builder.ToString().TrimEnd());
            WriteTrace($"Exception captured from {source}: {exception.GetType().FullName}: {exception.Message}");
        }

        private static void Write(string fileName, string content)
        {
            lock (_lock)
            {
                try
                {
                    Directory.CreateDirectory(_logDirectory);
                    File.AppendAllText(
                        Path.Combine(_logDirectory, fileName),
                        content + Environment.NewLine,
                        Encoding.UTF8);
                }
                catch (IOException ex)
                {
                    Debug.WriteLine($"CrashDiagnostics IO failure: {ex}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine($"CrashDiagnostics access failure: {ex}");
                }
            }
        }

        private static string GetLogDirectory()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrWhiteSpace(localAppData))
            {
                return Path.Combine(AppContext.BaseDirectory, "logs");
            }

            return Path.Combine(localAppData, "AIPaste", "logs");
        }
    }
}
