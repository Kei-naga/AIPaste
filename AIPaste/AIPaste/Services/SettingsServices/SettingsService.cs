using AIPaste.Models.KeyModels;
using AIPaste.Models.Settings;
using ManagedCuda;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.System;
using AIPaste.Services.LLMServices;
using AIPaste.Services.StartupServices;

namespace AIPaste.Services.SettingsServices
{
    internal class SettingsService
    {
        private readonly ApplicationDataContainer _mainContainer;
        private readonly ApplicationDataContainer _localLlmContainer;
        private readonly ApplicationDataContainer _geminiContainer;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SettingsService()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            _mainContainer = localSettings.CreateContainer("MainContainer", ApplicationDataCreateDisposition.Always);
            _localLlmContainer = _mainContainer.CreateContainer("LocalLLMContainer", ApplicationDataCreateDisposition.Always);
            _geminiContainer = _mainContainer.CreateContainer("GeminiContainer", ApplicationDataCreateDisposition.Always);
        }

        public AppSettings LoadSettings()
        {
            try
            {
                _logger.Info("Loading settings");
                var llmModelSettings = LoadLocalLLMModelSettings();
                var geminiModelSettings = LoadGeminiModelSettings();
                var keySettings = LoadKeySettings();
                var AutoStart = (bool)_mainContainer.Values["AutoStart"];
                var modelType = (ModelType)_mainContainer.Values["ModelType"];
                var appSettings = new AppSettings(AutoStart, modelType, keySettings, llmModelSettings, geminiModelSettings);
                _logger.Debug($"Loaded settimgs: {appSettings}");
                _logger.Debug($"Local LLM Settings: {llmModelSettings}");
                _logger.Debug($"Gemini Setting: {geminiModelSettings}");
                return appSettings;
            }
            catch(Exception e)
            {
                _logger.Info(e, "Failed to load settings, resetting settings");
                return ResetSettings();
            }
        }

        private LLMLocalModelSettings LoadLocalLLMModelSettings()
        {
            var modelPath = (string)_localLlmContainer.Values["ModelPath"];
            var gpuEnabled = (bool)_localLlmContainer.Values["GpuEnabled"];
            var gpuLayerCount = (int)_localLlmContainer.Values["GpuLayerCount"];
            var contextSize = (uint)_localLlmContainer.Values["ContextSize"];
            var maxTokens = (int)_localLlmContainer.Values["MaxTokens"];

            return new LLMLocalModelSettings(
                    ModelPath: modelPath,
                    GpuEnable: gpuEnabled,
                    GpuLayerCount: gpuLayerCount,
                    ContextSize: contextSize,
                    MaxTokens: maxTokens
                    );
        }

        private GeminiModelSettings LoadGeminiModelSettings()
        {
            var apiKey = (string)_geminiContainer.Values["ApiKey"];
            var modelName = (string)_geminiContainer.Values["ModelName"];
            var location = (string)_geminiContainer.Values["Location"];
            return new GeminiModelSettings(apiKey, modelName, location);
        }

        private KeySettings LoadKeySettings()
        {
            var isHotkeyEnabled = (bool)_mainContainer.Values["IsHotkeyEnabled"];
            var modifiers = (HOT_KEY_MODIFIERS)Enum.ToObject(typeof(HOT_KEY_MODIFIERS), (int)_mainContainer.Values["Modifers"]);
            var hotkey = (VirtualKey)Enum.ToObject(typeof(VirtualKey), (int)_mainContainer.Values["Hotkey"]);
            var keyPattern = new KeyPattern(modifiers, hotkey);
            return new KeySettings(isHotkeyEnabled, keyPattern);
        }

        public void SaveSettings(AppSettings appSettings)
        {
            _logger.Info("Saving settings");
            _mainContainer.Values["AutoStart"] = appSettings.AutoStart;
            _mainContainer.Values["ModelType"] = (int)appSettings.ModelType;
            SaveKeySettings(appSettings.KeySettings);
            SaveLocalLLMModelSettings(appSettings.LocalLLMSettings);
            SaveGeminiModelSettings(appSettings.GeminiSettings);
            _logger.Debug($"Saved settings: {appSettings}");
        }

        private void SaveLocalLLMModelSettings(LLMLocalModelSettings llmModelSettings)
        {
            _logger.Debug($"Saving Local LLM Settings: {llmModelSettings}");
            _localLlmContainer.Values["ModelPath"] = llmModelSettings.ModelPath;
            _localLlmContainer.Values["GpuEnabled"] = llmModelSettings.GpuEnabled;
            _localLlmContainer.Values["GpuLayerCount"] = llmModelSettings.GpuLayerCount;
            _localLlmContainer.Values["ContextSize"] = llmModelSettings.ContextSize;
            _localLlmContainer.Values["MaxTokens"] = llmModelSettings.MaxTokens;
        }

        private void SaveGeminiModelSettings(GeminiModelSettings geminiModelSettings)
        {
            _logger.Debug($"Saving Gemini Settings: {geminiModelSettings}");
            _geminiContainer.Values["ApiKey"] = geminiModelSettings.ApiKey;
            _geminiContainer.Values["ModelName"] = geminiModelSettings.ModelName;
            _geminiContainer.Values["Location"] = geminiModelSettings.Location;
        }

        private void SaveKeySettings(KeySettings keySettings)
        {
            _logger.Debug($"Saving Key Settings: {keySettings}");
            _mainContainer.Values["IsHotkeyEnabled"] = keySettings.IsHotkeyEnabled;
            _mainContainer.Values["Hotkey"] = (int)keySettings.KeyPattern.Key;
            _mainContainer.Values["Modifers"] = (int)keySettings.KeyPattern.Modifiers;
        }

        public AppSettings ResetSettings()
        {
            _logger.Info("Resetting settings");
            var defaultAppSettings = AppSettings.GetDefaultSettings();
            _mainContainer.Values.Clear();
            SaveSettings(defaultAppSettings);
            return defaultAppSettings;
        }

        // TODO: ストレージに保存するとかの処理とここらへんの更新処理は分けたい
        public AppSettings SettingsUpdate(MainWindow? mainWindow, AppSettings newSettings)
        {
            if (newSettings.ModelType != ModelType.LocalLLM)
            {
                _logger.Info("Despose Local LLM");
                LocalLLMProvider.Dispose();
            }

            // TODO: ここらへんの更新処理はもう少しスマートに書けるかも
            if (!mainWindow?.ViewModel.UpdateSettings(newSettings) ?? false)
            {
                newSettings.KeySettings = new KeySettings(false, newSettings.KeySettings.KeyPattern);
            }

            try { AutoStartToggleChanged(newSettings.AutoStart); }
            catch (Exception e)
            {
                _logger.Warn(e);
                newSettings.AutoStart = false;
            }
            return newSettings;
        }

        private static async void AutoStartToggleChanged(bool changedStatus)
        {
            await StartupManager.ToggleStartupAsync(changedStatus);
            var actualState = await StartupManager.IsAutoStartupMode();
            if (changedStatus != actualState)
            {
                throw new Exception("Failed to set AutoStart");
            }
        }
    }
}
