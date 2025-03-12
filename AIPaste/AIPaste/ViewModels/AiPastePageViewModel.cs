using System;
using System.ComponentModel;
using System.Threading.Tasks;
using NLog;
using AIPaste.Models.LLMModels;
using Windows.ApplicationModel.Resources;
using AIPaste.Models.ClipboardOperate;
using AIPaste.Models.SettingsServices;
using AIPaste.Models.DataModels;

namespace AIPaste.ViewModels
{
    public partial class AiPastePageViewModel : INotifyPropertyChanged
    {
        private readonly IClipboardOperator _clipboardOperator;
        private readonly ILlmTextCorrector _llmTextCorrector;
        private Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();

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
            var appSettings = SettingsService.GetInstance().LoadSettings();
            _clipboardOperator = new ClipboardOperator();
            _clipboardOperator.RegisterContentChangedHandler(OnClipboardContentChanged);
            SetTargetTextFromClipboard();
            var llmStrategyProvider = new LlmStrategyProvider(appSettings);
            var llmStrategy = llmStrategyProvider.GetLlmStrategy();
            _llmTextCorrector = new LlmTextCorrector(llmStrategy, ResourceLoader.GetForViewIndependentUse().GetString("/LLMResources/SystemPrompt"));
        }

        public async Task GeneratingText(string userInput)
        {
            var requestModel = new LlmRequest(TargetText, userInput);
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
            catch(Exception ex)
            {
                OutputText = _resourceLoader.GetString("AIPastePage_InappropriateOutput");
                _logger.Warn(ex, "Missing GeneratingText");
            }
            OutputText = _llmTextCorrector.PresentResponse;
        }

        private static bool CheckResponse(string response)
        {
            return response != "";
        }

        public void ChangeTargetText()
        {
            _clipboardOperator.SetText(OutputText);
        }

        async private void SetTargetTextFromClipboard()
        {
            TargetText =  await _clipboardOperator.GetTextAsync();
        }

        void OnClipboardContentChanged(object? sender, object? e) => SetTargetTextFromClipboard();

    }


}
