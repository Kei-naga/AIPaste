using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AIPaste.Models.LLMModels;
using AIPaste.Models.ClipboardOperate;
using AIPaste.Models.SettingsServices;
using AIPaste.Models.DTO;
using AIPaste.common;

namespace AIPaste.ViewModels
{
    public partial class AiPastePageViewModel : INotifyPropertyChanged
    {
        private readonly IClipboardOperator _clipboardOperator;
        private readonly ILlmTextCorrector _llmTextCorrector;
        private IMyLogger _logger;
        private readonly IResourceLoaderWrapper _resourceLoader;

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

        public AiPastePageViewModel(
            ISettingsService? settingsService = null,
            ITextCorrectorFactory? textCorrectorFactory = null,
            IClipboardOperator? clipboardOperator = null, 
            IResourceLoaderWrapper? resourceLoader = null, 
            IMyLogger? logger = null )
        {
            _logger = logger ?? MyLogger.GetInstance();
            _logger.Trace("AiPastePageViewModel created");
            _resourceLoader = resourceLoader ?? new ResourceLoaderWrapper();
            var appSettings = settingsService?.LoadSettings() ?? SettingsService.GetInstance().LoadSettings();
            _clipboardOperator = clipboardOperator ?? new ClipboardOperator();
            _clipboardOperator.RegisterContentChangedHandler(OnClipboardContentChanged);
            SetTargetTextFromClipboard();
            textCorrectorFactory ??= new TextCorrectorFactory();
            _llmTextCorrector = textCorrectorFactory.CreateLlmTextCorrector(appSettings);
        }

        public async Task GeneratingText(string userInput)
        {
            var requestModel = new LlmRequest(TargetText, userInput, _resourceLoader);
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
                _logger.Warn("FAILED_GENERATE_TEXT");
                _logger.Debug(ex);
            }
            OutputText = _llmTextCorrector.PresentResponse;
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
                _logger.Warn("FAILED_TO_SET_CLIPBOARD_TEXT");
                _logger.Debug(ex);
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
                _logger.Warn("FAILED_TO_GET_CLIPBOARD_TEXT");
                _logger.Debug(ex);
            }
        }

        void OnClipboardContentChanged(object? sender, object? e) => SetTargetTextFromClipboard();

        ~AiPastePageViewModel()
        {
            _clipboardOperator.UnregisterContentChangedHandler(OnClipboardContentChanged);
        }
    }


}
