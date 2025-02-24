using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.Settings;
using AIPaste.Services.LLMServices;
using AIPaste.Services.ClipboardOperator;
using System.Diagnostics.CodeAnalysis;
using AIPaste.Services.SettingsServices;
using NLog;
using AIPaste.Models.LLMModels;
using Microsoft.UI.Xaml.Controls;
using AIPaste.Views;

namespace AIPaste.ViewModels
{
    public partial class AiPastePageViewModel : INotifyPropertyChanged
    {
        private readonly LLMStrategy _llmStrategy;
        private readonly ClipboardOperator _clipboardOperator = new();
        private readonly ILLMProvider _llmProvider;
        private Logger _logger = LogManager.GetCurrentClassLogger();

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
            var settingsService = new SettingsService();
            var appSettings = settingsService.LoadSettings();
            ClipboardOperator.RegisterContentChangedHandler(OnClipboardContentChanged);
            SetTargetTextFromClipboard();
            _llmProvider = GetLLMProvider(appSettings);
            _llmStrategy = new LLMStrategy();
            _llmProvider.SetSystemPrompt(_llmStrategy.GetSystemPrompt());
            _llmProvider.StartNewChat();
            (LlmRequestModel modelReq, string modelAns) = _llmStrategy.CreateModelPrompt();
            _llmProvider.AddChatHistory(modelReq,modelAns);
        }

        private static ILLMProvider GetLLMProvider(AppSettings appSettings)
        {
            switch (appSettings.ModelType)
            {
                case ModelType.LocalLLM:
                    return new LocalLLMProvider(appSettings.LocalLLMSettings);
                case ModelType.Gemini:
                    return new GeminiProvider(appSettings.GeminiSettings);
                default:
                    throw new InvalidOperationException("Invalid model type");
            }
        }

        public async Task GeneratingText(string userInput)
        {
            OutputText = "";
            var optimizedUserInput = new LlmRequestModel(TargetText, userInput);
            try
            {
                await foreach (var chunk in _llmProvider.GeneratingText(optimizedUserInput))
                {
                    OutputText += chunk;
                }
                if (!CheckResponse(_llmProvider.PresentResponse))
                {
                    throw new InvalidOperationException("LLM generated an empty string");
                }
            }
            catch
            {
                OutputText = "不適切な文章が生成されました。";
            }
            OutputText = _llmProvider.PresentResponse;
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
