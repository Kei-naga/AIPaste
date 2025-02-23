using System;
using System.IO;
using System.Threading.Tasks;
using NLog;
using Windows.ApplicationModel;

namespace AIPaste.Services.StartupServices
{
    internal class StartupManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const string TASK_ID = "AIPasteAutoStart";

        public static async Task ToggleStartupAsync(bool enable)
        {
            try
            {
                var startupTask = await StartupTask.GetAsync(TASK_ID);
                if (enable)
                {
                    if (startupTask.State == StartupTaskState.Disabled)
                    {
                        _logger.Info("auto start activating...");
                        await startupTask.RequestEnableAsync();
                    }
                }
                else
                {
                    if (startupTask.State == StartupTaskState.Enabled)
                    {
                        _logger.Info("auto start deactivating...");
                        startupTask.Disable();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to set auto start mode");
            }
        }

        public static async Task<bool> IsAutoStartupMode()
        {
            StartupTask startupTask = await StartupTask.GetAsync(TASK_ID);
            return startupTask.State == StartupTaskState.Enabled;
        }
    }
}
