using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AIPaste.Models.Settings;
using AIPaste.Services.LLMServices;
using AIPaste.Services.ClipboardOperator;
using AIPaste.Services.SettingsServices;
using NLog;
using AIPaste.Models.LLMModels;
using Windows.ApplicationModel.Resources;

namespace AIPaste.ViewModels
{
    public partial class AiPastePageViewModel : INotifyPropertyChanged
    {
        private readonly ClipboardOperator _clipboardOperator = new();
        private readonly LlmTextCorrector _llmTextCorrector;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ResourceLoader _resourceLoader = new();

        private string _targetText = "";
        public string TargetText
        {
            get => _targetText;
            private set
            {
                if (_targetText != value)
                {
                    _targetText = value;
                    OnPropertyChanged(nameof(TargetText));
                }
            }
        }
        private string _outputText = "";
        public string OutputText
        {
            get => _outputText;
            private set
            {
                if (_outputText != value)
                {
                    _outputText = value;
                    OnPropertyChanged(nameof(OutputText));
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public AiPastePageViewModel()
        {
            _logger.Debug("AiPastePageViewModel created");
            var settingsService = SettingsService.Instance;
            var appSettings = settingsService.LoadSettings();
            ClipboardOperator.RegisterContentChangedHandler(OnClipboardContentChanged);
            SetTargetTextFromClipboard();
            _llmTextCorrector = new LlmTextCorrector(appSettings);
        }

        public async Task GeneratingText(string userInput)
        {
            var requestModel = new LlmRequestModel(TargetText, userInput);
            OutputText = "";
            try
            {
                await foreach (var chunk in _llmTextCorrector.GeneratingText(requestModel))
                {
                    OutputText += chunk;
                }
                if (!CheckResponse(_llmTextCorrector.PresentResponse))
                {
                    throw new InvalidOperationException("LLM generated an empty string");
                }
            }
            catch
            {
                OutputText = _resourceLoader.GetString("AIPastePage_InappropriateOutput");
            }
            OutputText = _llmTextCorrector.PresentResponse;
        }

        private static bool CheckResponse(string response)
        {
            return response != "";
        }

        public void ChangeTargetText()
        {
            ClipboardOperator.SetText(OutputText);
        }

        async private void SetTargetTextFromClipboard()
        {
            TargetText =  await _clipboardOperator.GetTextAsync();
        }

        void OnClipboardContentChanged(object? sender, object? e) => SetTargetTextFromClipboard();

    }


}
