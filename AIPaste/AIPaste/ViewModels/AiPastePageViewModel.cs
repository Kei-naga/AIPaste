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
        private readonly ITextCorrectService _llmTextCorrectService;
        private ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ResourceLoader _resourceLoader;

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

        public AiPastePageViewModel(ISettingsService? settingsService = null, IClipboardOperator? clipboardOperator = null, ResourceLoader? resourceLoader = null)
        {
            _logger.Debug("AiPastePageViewModel created");
            _resourceLoader = resourceLoader ?? ResourceLoader.GetForViewIndependentUse();
            var appSettings = settingsService?.LoadSettings() ?? SettingsService.GetInstance().LoadSettings();
            _clipboardOperator = clipboardOperator ?? new ClipboardOperator();
            _clipboardOperator.RegisterContentChangedHandler(OnClipboardContentChanged);
            SetTargetTextFromClipboard();
            var systemPrompt = _resourceLoader.GetString("/LLMResources/SystemPrompt");
            _llmTextCorrectService = new TextCorrectService(appSettings, systemPrompt);
        }

        public async Task GeneratingText(string userInput)
        {
            var requestModel = new LlmRequest(TargetText, userInput);
            OutputText = "";
            try
            {
                await foreach (var chunk in _llmTextCorrectService.GeneratingText(requestModel))
                {
                    OutputText += chunk;
                }
                if (!CheckResponse(_llmTextCorrectService.PresentResponse))
                {
                    throw new InvalidOperationException("LLM generated an empty string");
                }
            }
            catch(Exception ex)
            {
                OutputText = _resourceLoader.GetString("AIPastePage_InappropriateOutput");
                _logger.Warn(ex, "Missing GeneratingText");
            }
            OutputText = _llmTextCorrectService.PresentResponse;
        }

        private static bool CheckResponse(string response)
        {
            return response != "";
        }

        public void ChangeTargetText()
        {
            try
            {
                _clipboardOperator.SetText(OutputText);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Failed to set text to clipboard");
            }   
        }

        async private void SetTargetTextFromClipboard()
        {
            try
            {
                TargetText = await _clipboardOperator.GetTextAsync();
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Failed to get text from clipboard");
            }
        }

        void OnClipboardContentChanged(object? sender, object? e) => SetTargetTextFromClipboard();

    }


}
