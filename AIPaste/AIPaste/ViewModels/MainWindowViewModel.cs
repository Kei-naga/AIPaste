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

namespace AIPaste.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private LLMService _llmService;
        private ClipboardOperator _clipboardOperator = new ClipboardOperator();

        private string _targetText;
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
        private string _outputText;
        public string outputText
        {
            get => _outputText;
            private set
            {
                if (_outputText != value)
                {
                    _outputText = value;
                    OnPropertyChanged(nameof(outputText));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainWindowViewModel()
        {
            var llmModelSettings = new LLMModelSettings(
                ModelPath: @"C:\Users\keita\llama\Llama-3-ELYZA-JP-8B-GGUF\Llama-3-ELYZA-JP-8B-q4_k_m.gguf",
                GpuLayerCount: 32,
                ContextSize: 1024,
                antiPrompts: new string[] { "END" },
                MaxTokens: 256
            );
            _clipboardOperator.RegisterContentChangedHandler(OnClipboardContentChanged);
            _llmService = new LocalLLMService(llmModelSettings);
            _llmService.Initialize();
            SetTargetTextFromClipboard();
            const string modelTargetText = "この部分なんだけどさあ、もっと前後関係わかるようにしといて、";
            const string modelInput = "敬語にして";
            string modelPrompt = CreateReq(modelTargetText, modelInput);
            const string modelAns = "こちらの部分ですが、もう少し前後の関係が分かるようにしていただけますでしょうか。";
            _llmService.StartChat(GetSystemPrompt(), modelPrompt, modelAns);
        }

        public async Task GeneratingText(string userInput)
        {
            outputText = "";
            var req = CreateReq(TargetText, userInput);
            await foreach (var chunk in _llmService.GeneratingText(req))
            {
                outputText += chunk;
            }
            if (CheckResponse(_llmService.PresentResponse))
            {
                outputText = _llmService.PresentResponse;
            }
            else
            {
                outputText = "不適切な文章が生成されました。";
            }
        }

        private bool CheckResponse(string response)
        {
            return response != "";
        }
        public string GetPresentResponse()
        {
            return _llmService.PresentResponse;
        }

        public void ChangeTargetText()
        {
            _clipboardOperator.SetText(outputText);
        }

        private string CreateReq(string targetText, string input)
        {
            return "対象テキスト：" + targetText + Environment.NewLine + "ユーザ指示：" + input;
        }

        async private void SetTargetTextFromClipboard()
        {
            TargetText =  await _clipboardOperator.GetTextAsync();
        }

        private string GetSystemPrompt()
        {
            return "あなたは文章編集の専門家です。対象テキストとユーザ指示を与えるので、対象テキストをユーザ指示に厳密に従って、適切に修正してください。回答は理由等はなにも書かず、修正した文章のみを記載してください。また元の意味や意図が変わらないよう注意してください。";
        }

        [MemberNotNull(nameof(TargetText))]
        async void OnClipboardContentChanged(object? sender, object? e)
        {
            SetTargetTextFromClipboard();
        }

    }


}
