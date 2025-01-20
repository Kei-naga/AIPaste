using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models;
using AIPaste.Services;

namespace AIPaste.ViewModels
{
    internal class MainWindowViewModel
    {
        private LLMService _llmService;
        public string TargetText { get; private set; }
        public string outputText { get; private set; }

        public MainWindowViewModel()
        {
            var llmModelSettings = new LLMModelSettings(
                ModelPath: @"C:\Users\keita\llama\Llama-3-ELYZA-JP-8B-GGUF\Llama-3-ELYZA-JP-8B-q4_k_m.gguf",
                GpuLayerCount: 32,
                ContextSize: 1024,
                antiPrompts: new string[] { "END" },
                MaxTokens: 256
            );
            _llmService = new LLMService(llmModelSettings);
            _llmService.Initialize();
            TargetText = GetTargetText();
            const string modelTargetText = "この部分なんだけどさあ、もっと前後関係わかるようにしといて、";
            const string modelInput = "敬語にして";
            string modelPrompt = CreateReq(modelTargetText, modelInput);
            const string modelAns = "こちらの部分ですが、もう少し前後の関係が分かるようにしていただけますでしょうか。";
            _llmService.StartChat(GetSystemPrompt(), modelPrompt, modelAns);
        }

        public async IAsyncEnumerable<string> GeneratingText(string userInput)
        {
            var req = CreateReq(TargetText, userInput);
            await foreach (var response in _llmService.GeneratingText(req))
            {
                yield return response;
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
            TargetText = _llmService.PresentResponse;
        }

        private string CreateReq(string targetText, string input)
        {
            return "対象テキスト：" + targetText + Environment.NewLine + "ユーザ指示：" + input;
        }

        private string GetTargetText()
        {
            return "モデルが不適切なやつ表示したらまずいから、特定のフレーズ避けるようにできる";
        }

        private string GetSystemPrompt()
        {
            return "あなたは文章編集の専門家です。対象テキストとユーザ指示を与えるので、対象テキストをユーザ指示に厳密に従って、適切に修正してください。回答は理由等はなにも書かず、修正した文章のみを記載してください。また元の意味や意図が変わらないよう注意してください。";
        }
    }


}
