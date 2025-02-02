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
                var llmModelSettings = LoadLocalLLMModelSettings();
                var geminiModelSettings = LoadGeminiModelSettings();
                var keySettings = LoadKeySettings();
                var AutoStart = (bool)_container.Values["AutoStart"];
                var modelType = (ModelType)_container.Values["ModelType"];
                var appSettings = new AppSettings(AutoStart, modelType, keySettings, llmModelSettings, geminiModelSettings);
                _logger.Debug($"Loaded settimgs: {appSettings}");
                return appSettings;
            }
            catch(Exception e)
            {
                _logger.Debug(e, "Failed to load settings, resetting settings");
                return ResetSettings();
            }
        }

        private LLMLocalModelSettings LoadLocalLLMModelSettings()
        {
            var modelPath = (string)_container.Values["ModelPath"];
            var gpuEnabled = (bool)_container.Values["GpuEnabled"];
            var gpuLayerCount = (int)_container.Values["GpuLayerCount"];
            var contextSize = (uint)_container.Values["ContextSize"];
            var antiPrompts = new List<string>((string[])_container.Values["AntiPrompts"]);
            var maxTokens = (int)_container.Values["MaxTokens"];

            return new LLMLocalModelSettings(
                    ModelPath: modelPath,
                    GpuEnable: gpuEnabled,
                    GpuLayerCount: gpuLayerCount,
                    ContextSize: contextSize,
                    AntiPrompts: antiPrompts,
                    MaxTokens: maxTokens
                    );
        }

        private GeminiModelSettings LoadGeminiModelSettings()
        {
            // TODO: Implement loading GeminiModelSettings
            return (GeminiModelSettings)GeminiModelSettings.GetDefaultSettings();
        }

        private KeySettings LoadKeySettings()
        {
            return new KeySettings(
                    KeyPattern: (string)_container.Values["KeyPattern"]
                    );
        }

        public void SaveSettings(AppSettings appSettings)
        {
            _logger.Info("Saving settings");
            _container.Values["AutoStart"] = appSettings.AutoStart;
            _container.Values["ModelType"] = (int)appSettings.ModelType;
            SaveKeySettings(appSettings.KeySettings);
            SaveLocalLLMModelSettings(appSettings.LocalLLMSettings);
            SaveGeminiModelSettings(appSettings.GeminiSettings);
            _logger.Debug($"Saved settings: {appSettings}");
        }

        private void SaveLocalLLMModelSettings(LLMLocalModelSettings llmModelSettings)
        {
            _container.Values["ModelPath"] = llmModelSettings.ModelPath;
            _container.Values["GpuEnabled"] = llmModelSettings.GpuEnabled;
            _container.Values["GpuLayerCount"] = llmModelSettings.GpuLayerCount;
            _container.Values["ContextSize"] = llmModelSettings.ContextSize;
            _container.Values["AntiPrompts"] = llmModelSettings.AntiPrompts.ToArray();
            _container.Values["MaxTokens"] = llmModelSettings.MaxTokens;
        }

        private void SaveGeminiModelSettings(GeminiModelSettings geminiModelSettings)
        {
            // TODO: Implement saving GeminiModelSettings
        }

        private void SaveKeySettings(KeySettings keySettings)
        {
            _container.Values["KeyPattern"] = keySettings.KeyPattern;
        }

        public AppSettings ResetSettings()
        {
            _logger.Info("Resetting settings");
            var defaultAppSettings = AppSettings.GetDefaultSettings();
            _container.Values.Clear();
            SaveSettings(defaultAppSettings);
            return defaultAppSettings;
        }

    }
}
