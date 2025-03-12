using System;
using System.Threading.Tasks;
using NLog;
using Windows.ApplicationModel;

namespace AIPaste.Models.StartupServices
{
    internal class StartupManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const string TASK_ID = "AIPasteAutoStart";

        public static async Task ToggleStartupAsync(bool enable)
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

        public static async Task<bool> IsAutoStartupMode()
        {
            StartupTask startupTask = await StartupTask.GetAsync(TASK_ID);
            return startupTask.State == StartupTaskState.Enabled;
        }
    }
}
