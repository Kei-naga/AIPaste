using AIPaste.Models.Settings;
using ManagedCuda;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace AIPaste.Services.SettingsServices
{
    internal class SettingsService
    {
        private Windows.Storage.ApplicationDataContainer _container;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public SettingsService()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            _container = localSettings.CreateContainer("AIPasteContainer", ApplicationDataCreateDisposition.Always);
        }

        public AppSettings LoadSettings()
        {
            try
            {
                _logger.Info("Loading settings");
                var modelPath = (string)_container.Values["ModelPath"];
                var gpuLayerCount = (int)_container.Values["GpuLayerCount"];
                var contextSize = (uint)_container.Values["ContextSize"];
                var antiPrompts = new List<string>((string[])_container.Values["AntiPrompts"]);
                var maxTokens = (int)_container.Values["MaxTokens"];

                var llmModelSettings = new LLMModelSettings(
                        ModelPath: modelPath,
                        GpuLayerCount: gpuLayerCount,
                        ContextSize: contextSize,
                        AntiPrompts: antiPrompts,
                        MaxTokens: maxTokens
                        );
                var keySettings = new KeySettings(
                    KeyPattern: (string)_container.Values["KeyPattern"]
                    );
                var AutoStart = (bool)_container.Values["AutoStart"];
                var gpuAvailable = (bool)_container.Values["GpuEnabled"];
                var loadedSettings = new AppSettings(llmModelSettings, keySettings, AutoStart, gpuAvailable);
                _logger.Debug($"Loaded settimgs: {loadedSettings}");
                return loadedSettings;
            }
            catch(Exception e)
            {
                return ResetSettings();
            }
        }

        public void SaveSettings(AppSettings appSettings)
        {
            _logger.Info("Saving settings");
            _container.Values["GpuEnabled"] = appSettings.GpuEnabled;
            _container.Values["AutoStart"] = appSettings.AutoStart;
            _container.Values["KeyPattern"] = appSettings.KeySettings.KeyPattern;
            _container.Values["ModelPath"] = appSettings.LLMModelSettings.ModelPath;
            _container.Values["GpuLayerCount"] = appSettings.LLMModelSettings.GpuLayerCount;
            _container.Values["ContextSize"] = appSettings.LLMModelSettings.ContextSize;
            _container.Values["AntiPrompts"] = appSettings.LLMModelSettings.AntiPrompts.ToArray();
            _container.Values["MaxTokens"] = appSettings.LLMModelSettings.MaxTokens;
            _logger.Debug($"Saved settings: {appSettings}");
        }

        public AppSettings ResetSettings()
        {
            _logger.Info("Resetting settings");
            var defaultSettings = AppSettings.GetDefaultSettings();
            _container.Values.Clear();
            SaveSettings(defaultSettings);
            return defaultSettings;
        }

    }
}
