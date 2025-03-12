using System;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using AIPaste.Models.KeyModels;
using AIPaste.Models.Settings;
using NLog;
using Windows.Storage;
using Windows.System;

namespace AIPaste.Models.Settings.SettingsServices
{
    internal class SettingsStore
    {
        static private readonly ApplicationDataContainer _mainContainer;
        static private readonly ApplicationDataContainer _localLlmContainer;
        static private readonly ApplicationDataContainer _geminiContainer;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private const string AUTO_START_KEY = "AutoStart";
        private const string MODEL_TYPE_KEY = "ModelType";
        private const string MODEL_PATH_KEY = "ModelPath";
        private const string GPU_ENABLED_KEY = "GpuEnabled";
        private const string GPU_LAYER_COUNT_KEY = "GpuLayer";
        private const string CONTEXT_SIZE_KEY = "MaxContextSize";
        private const string MAX_TOKENS_KEY = "MaxTokens";
        private const string API_KEY_KEY = "ApiKey";
        private const string MODEL_NAME_KEY = "ModelName";
        private const string LOCATION_KEY = "Location";
        private const string IS_HOTKEY_ENABLED_KEY = "IsHotkeyEnabled";
        private const string MODIFERS_KEY = "Modifers";
        private const string HOTKEY_KEY = "Hotkey";

        static SettingsStore()
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
                var AutoStart = (bool)_mainContainer.Values[AUTO_START_KEY];
                var modelType = (ModelType)_mainContainer.Values[MODEL_TYPE_KEY];
                var appSettings = new AppSettings(AutoStart, modelType, keySettings, llmModelSettings, geminiModelSettings);
                _logger.Debug($"Loaded settimgs: {appSettings}");
                _logger.Debug($"Local LLM Settings: {llmModelSettings}");
                _logger.Debug($"Gemini Setting: {geminiModelSettings}");
                return appSettings;
            }
            catch (Exception e)
            {
                _logger.Info(e, "Failed to load settings, resetting settings");
                return ResetSettings();
            }
        }

        private LLMLocalModelSettings LoadLocalLLMModelSettings()
        {
            var modelPath = (string)_localLlmContainer.Values[MODEL_PATH_KEY];
            var gpuEnabled = (bool)_localLlmContainer.Values[GPU_ENABLED_KEY];
            var gpuLayerCount = (int)_localLlmContainer.Values[GPU_LAYER_COUNT_KEY];
            var contextSize = (uint)_localLlmContainer.Values[CONTEXT_SIZE_KEY];
            var maxTokens = (int)_localLlmContainer.Values[MAX_TOKENS_KEY];

            return new LLMLocalModelSettings(
                    ModelPath: modelPath,
                    GpuEnable: gpuEnabled,
                    GpuLayerCount: gpuLayerCount,
                    MaxContextSize: contextSize,
                    MaxTokens: maxTokens
                    );
        }

        private GeminiModelSettings LoadGeminiModelSettings()
        {
            var apiKey = (string)_geminiContainer.Values[API_KEY_KEY];
            var modelName = (string)_geminiContainer.Values[MODEL_NAME_KEY];
            var location = (string)_geminiContainer.Values[LOCATION_KEY];
            var maxContextsize = (uint)_localLlmContainer.Values[CONTEXT_SIZE_KEY];
            return new GeminiModelSettings(apiKey, modelName, location, maxContextsize);
        }

        private KeySettings LoadKeySettings()
        {
            var isHotkeyEnabled = (bool)_mainContainer.Values[IS_HOTKEY_ENABLED_KEY];
            var modifiers = (HOT_KEY_MODIFIERS)Enum.ToObject(typeof(HOT_KEY_MODIFIERS), (int)_mainContainer.Values[MODIFERS_KEY]);
            var hotkey = (VirtualKey)Enum.ToObject(typeof(VirtualKey), (int)_mainContainer.Values[HOTKEY_KEY]);
            var keyPattern = new KeyPattern(modifiers, hotkey);
            return new KeySettings(isHotkeyEnabled, keyPattern);
        }

        public void SaveSettings(AppSettings appSettings)
        {
            _logger.Info("Saving settings");
            _mainContainer.Values[AUTO_START_KEY] = appSettings.AutoStart;
            _mainContainer.Values[MODEL_TYPE_KEY] = (int)appSettings.ModelType;
            SaveKeySettings(appSettings.KeySettings);
            SaveLocalLLMModelSettings(appSettings.LocalLLMSettings);
            SaveGeminiModelSettings(appSettings.GeminiSettings);
            _logger.Debug($"Saved settings: {appSettings}");
        }

        private void SaveLocalLLMModelSettings(LLMLocalModelSettings llmModelSettings)
        {
            _logger.Debug($"Saving Local LLM Settings: {llmModelSettings}");
            _localLlmContainer.Values[MODEL_PATH_KEY] = llmModelSettings.ModelPath;
            _localLlmContainer.Values[GPU_ENABLED_KEY] = llmModelSettings.GpuEnabled;
            _localLlmContainer.Values[GPU_LAYER_COUNT_KEY] = llmModelSettings.GpuLayerCount;
            _localLlmContainer.Values[CONTEXT_SIZE_KEY] = llmModelSettings.MaxContextSize;
            _localLlmContainer.Values[MAX_TOKENS_KEY] = llmModelSettings.MaxTokens;
        }

        private void SaveGeminiModelSettings(GeminiModelSettings geminiModelSettings)
        {
            _logger.Debug($"Saving Gemini Settings: {geminiModelSettings}");
            _geminiContainer.Values[API_KEY_KEY] = geminiModelSettings.ApiKey;
            _geminiContainer.Values[MODEL_NAME_KEY] = geminiModelSettings.ModelName;
            _geminiContainer.Values[LOCATION_KEY] = geminiModelSettings.Location;
            _geminiContainer.Values[CONTEXT_SIZE_KEY] = geminiModelSettings.MaxContextSize;
        }

        private void SaveKeySettings(KeySettings keySettings)
        {
            _logger.Debug($"Saving Key Settings: {keySettings}");
            _mainContainer.Values[IS_HOTKEY_ENABLED_KEY] = keySettings.IsHotkeyEnabled;
            _mainContainer.Values[HOTKEY_KEY] = (int)keySettings.KeyPattern.Key;
            _mainContainer.Values[MODIFERS_KEY] = (int)keySettings.KeyPattern.Modifiers;
        }

        public AppSettings ResetSettings()
        {
            _logger.Info("Resetting settings");
            var defaultAppSettings = AppSettings.GetDefaultSettings();
            _mainContainer.Values.Clear();
            SaveSettings(defaultAppSettings);
            return defaultAppSettings;
        }
    }
}
