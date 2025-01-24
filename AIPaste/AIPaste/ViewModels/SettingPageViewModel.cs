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
            LLMModelPath = _appSettings.LLMModelSettings.ModelPath;
            GpuLayerCount = _appSettings.LLMModelSettings.GpuLayerCount;
            MaxTokens = _appSettings.LLMModelSettings.MaxTokens;
            GpuEnabled = _appSettings.GpuEnabled;
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
            var modelSettings = new LLMModelSettings(
                ModelPath: LLMModelPath,
                GpuLayerCount: GpuLayerCount,
                ContextSize: _appSettings.LLMModelSettings.ContextSize,
                AntiPrompts: _appSettings.LLMModelSettings.AntiPrompts,
                MaxTokens: MaxTokens
            );
            var keySettings = new KeySettings(KeyPattern);
            // AppSettingsに値を保存
            _settingsService.SaveSettings(new AppSettings(
                modelSettings, AutoStart, keySettings
            ));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}