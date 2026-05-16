using System;
using System.ComponentModel;
using System.Threading.Tasks;
using AIPaste.Models.ClipboardOperate;
using AIPaste.Models.LLMModels;
using AIPaste.Models.SettingsServices.SettingModels;
using AIPaste.common;

namespace AIPaste.ViewModels
{
    public partial class AiPastePageViewModel : INotifyPropertyChanged
    {
        private readonly IClipboardOperator _clipboardOperator;
        private readonly ILlmModelSettings _llmModelSettings;
        private ITextCorrectorFactory? _textCorrectorFactory;
        private ILlmTextCorrector? _llmTextCorrector;
        private readonly IMyLogger _logger;
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
            ILlmModelSettings llmModelSettings,
            ITextCorrectorFactory? textCorrectorFactory = null,
            IClipboardOperator? clipboardOperator = null,
            IResourceLoaderWrapper? resourceLoader = null,
            IMyLogger? logger = null)
        {
            _logger = logger ?? MyLogger.GetInstance();
            _logger.Trace("AiPastePageViewModel created");
            CrashDiagnostics.WriteTrace("AiPastePageViewModel constructor started");
            _resourceLoader = resourceLoader ?? new ResourceLoaderWrapper();
            _llmModelSettings = llmModelSettings;
            _textCorrectorFactory = textCorrectorFactory;
            _clipboardOperator = clipboardOperator ?? new ClipboardOperator();
            _clipboardOperator.RegisterContentChangedHandler(OnClipboardContentChanged);
            SetTargetTextFromClipboard();
            CrashDiagnostics.WriteTrace("AiPastePageViewModel constructor completed without initializing LLM");
        }

        public async Task GeneratingText(string userInput)
        {
            OutputText = "";
            try
            {
                var llmTextCorrector = EnsureLlmTextCorrector();
                await foreach (var chunk in llmTextCorrector.GeneratingText(TargetText, userInput))
                {
                    OutputText += chunk;
                }
                if (!CheckResponse(llmTextCorrector.PresentResponse))
                {
                    throw new InvalidOperationException("LLM generated an empty string");
                }
            }
            catch (Exception ex)
            {
                OutputText = _resourceLoader.GetString("AIPastePage_InappropriateOutput");
                _logger.Warn("FAILED_GENERATE_TEXT");
                _logger.Debug(ex);
                CrashDiagnostics.WriteException("AiPastePageViewModel.GeneratingText", ex);
            }
            OutputText = _llmTextCorrector?.PresentResponse ?? OutputText;
        }

        private ILlmTextCorrector EnsureLlmTextCorrector()
        {
            if (_llmTextCorrector != null)
            {
                return _llmTextCorrector;
            }

            CrashDiagnostics.WriteTrace($"EnsureLlmTextCorrector creating corrector for {_llmModelSettings.GetType().Name}");
            _textCorrectorFactory ??= new TextCorrectorFactory();
            _llmTextCorrector = _textCorrectorFactory.CreateLlmTextCorrector(_llmModelSettings, _resourceLoader, _logger);
            CrashDiagnostics.WriteTrace("EnsureLlmTextCorrector completed");
            return _llmTextCorrector;
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

        private async void SetTargetTextFromClipboard()
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

        private void OnClipboardContentChanged(object? sender, object? e) => SetTargetTextFromClipboard();

        ~AiPastePageViewModel()
        {
            _clipboardOperator.UnregisterContentChangedHandler(OnClipboardContentChanged);
        }
    }
}
