using System;
using System.Threading.Tasks;
using AIPaste.common;
using Windows.ApplicationModel;

namespace AIPaste.Models.StartupServices
{
    public class AutoStartupManager(IMyLogger? logger = null) : IAutoStartupManager
    {
        private readonly IMyLogger _logger = logger ?? MyLogger.GetInstance();
        #pragma warning disable IDE1006 // 命名スタイル
        private const string TASK_ID = "AIPasteAutoStart";
        #pragma warning restore IDE1006 // 命名スタイル

        public async Task ToggleStartupAsync(bool enable)
        {
            var startupTask = await StartupTask.GetAsync(TASK_ID);
            if (enable)
            {
                if (startupTask.State == StartupTaskState.Disabled)
                {
                    _logger.Info("ACTIVATE_AUTOSTART");
                    await startupTask.RequestEnableAsync();
                }
            }
            else
            {
                if (startupTask.State == StartupTaskState.Enabled)
                {
                    _logger.Info("DEACTIVATE_AUTOSTART");
                    startupTask.Disable();
                }
            }
        }

        public async Task<bool> IsAutoStartupMode()
        {
            StartupTask startupTask = await StartupTask.GetAsync(TASK_ID);
            return startupTask.State == StartupTaskState.Enabled;
        }
    }

    public interface IAutoStartupManager
    {
        Task ToggleStartupAsync(bool enable);
        Task<bool> IsAutoStartupMode();
    }
}
