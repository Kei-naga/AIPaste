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

namespace AIPaste.ViewModels
{
    public partial class AiPastePageViewModel : INotifyPropertyChanged
    {
        private readonly ILLMStrategy _llmStrategy;
        private readonly ClipboardOperator _clipboardOperator = new();
        private readonly ILLMProvider _llmProvider;

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
            var settingsService = new SettingsService();
            var appSettings = settingsService.LoadSettings();
            ClipboardOperator.RegisterContentChangedHandler(OnClipboardContentChanged);
            SetTargetTextFromClipboard();
            // _llmProvider = new LocalLLMProvider(appSettings.LocalLLMSettings);
            _llmProvider = new GeminiProvider(new GeminiModelSettings("AIzaSyCndL3XETlvzL3j0OJ6wQsDE458joENVnw"));
            _llmStrategy = new LocalLLMStrategy();
            _llmProvider.SetSystemPrompt(_llmStrategy.GetSystemPrompt());
            _llmProvider.StartNewChat();
            (string modelReq, string modelAns) = _llmStrategy.CreateModelPrompt();
            _llmProvider.AddChatHistory(modelReq,modelAns);
        }

        public async Task GeneratingText(string userInput)
        {
            OutputText = "";
            string optimizedUserInput = _llmStrategy.CreateOptimizedReq(TargetText, userInput);
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
