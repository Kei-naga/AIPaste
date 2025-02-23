using System;
using System.IO;
using Microsoft.Win32;
using NLog;

namespace AIPaste.Services.StartupServices
{
    internal class StartupManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static RegistryKey? _rkApp;
        private const string STARTUP_PATH = @"Software\Microsoft\Windows\CurrentVersion\Run";
        public static bool SetAutoStartupMode()
        {
            _rkApp = Registry.CurrentUser.OpenSubKey(STARTUP_PATH, true);
            if (_rkApp == null)
            {
                _logger.Warn("Failed to auto start registry key");
                return false;
            }
            _rkApp.SetValue("AIPaste", System.Reflection.Assembly.GetExecutingAssembly().Location);
            _logger.Info("Set auto start mode");
            return true;
        }

        public static void UnsetAutoStartupMode()
        {
            _rkApp?.DeleteValue("AIPaste", false);
            _logger.Info("Unset auto start mode");
        }

        public static bool IsAutoStartupMode()
        {
            return _rkApp?.GetValue("AIPaste") != null;
        }
    }
}
