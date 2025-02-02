using System.ComponentModel;
using AIPaste.Models.Settings;
using AIPaste.Services.SettingsServices;

namespace AIPaste.ViewModels
{
    public class SettingsPageViewModel : INotifyPropertyChanged
    {
        private AppSettings _appSettings;
        private SettingsService _settingsService;

        public SettingsPageViewModel()
        {
            // AppSettingsの読み込み
            _settingsService = new SettingsService();
            _appSettings = _settingsService.LoadSettings();

            // プロパティの初期化
            LLMModelPath = _appSettings.LocalLLMSettings.ModelPath;
            GpuLayerCount = _appSettings.LocalLLMSettings.GpuLayerCount;
            MaxTokens = _appSettings.LocalLLMSettings.MaxTokens;
            GpuEnabled = _appSettings.LocalLLMSettings.GpuEnabled;
            KeyPattern = _appSettings.KeySettings.KeyPattern;
            AutoStart = _appSettings.AutoStart;
        }

        private string _llmModelPath = "";
        public string LLMModelPath
        {
            get => _llmModelPath;
            set
            {
                if (_llmModelPath != value)
                {
                    _llmModelPath = value;
                    OnPropertyChanged(nameof(LLMModelPath));
                }
            }
        }

        private int _gpuLayerCount = 32;
        public int GpuLayerCount
        {
            get => _gpuLayerCount;
            set
            {
                if (_gpuLayerCount != value)
                {
                    _gpuLayerCount = value;
                    OnPropertyChanged(nameof(GpuLayerCount));
                }
            }
        }

        private int _maxTokens = 1024;
        public int MaxTokens
        {
            get => _maxTokens;
            set
            {
                if (_maxTokens != value)
                {
                    _maxTokens = value;
                    OnPropertyChanged(nameof(MaxTokens));
                }
            }
        }

        private bool _gpuEnabled = false;
        public bool GpuEnabled
        {
            get => _gpuEnabled;
            set
            {
                if (_gpuEnabled != value)
                {
                    _gpuEnabled = value;
                    OnPropertyChanged(nameof(GpuEnabled));
                }
            }
        }
        private string _keyPattern = "Ctrl+Shift+V";
        public string KeyPattern
        {
            get => _keyPattern;
            set
            {
                if (_keyPattern != value)
                {
                    _keyPattern = value;
                    OnPropertyChanged(nameof(KeyPattern));
                }
            }
        }

        bool _autoStart = true;
        public bool AutoStart
        {
            get => _autoStart;
            set
            {
                if (_autoStart != value)
                {
                    _autoStart = value;
                    OnPropertyChanged(nameof(AutoStart));
                }
            }
        }


        public void SaveSettings()
        {
            var modelSettings = new LLMLocalModelSettings(
                ModelPath: LLMModelPath,
                GpuEnable: GpuEnabled,
                GpuLayerCount: GpuLayerCount,
                ContextSize: _appSettings.LocalLLMSettings.ContextSize,
                AntiPrompts: _appSettings.LocalLLMSettings.AntiPrompts,
                MaxTokens: MaxTokens
            );
            var keySettings = new KeySettings(KeyPattern);
            var newSettings = new AppSettings(
                AutoStart,
                _appSettings.ModelType,
                keySettings,
                modelSettings,
                _appSettings.GeminiSettings
            );
            _settingsService.SaveSettings(newSettings);

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}