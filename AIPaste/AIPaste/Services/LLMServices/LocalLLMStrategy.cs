using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLama.Common;
using LLama;
using AIPaste.Models.Settings;

namespace AIPaste.Services.LLMServices
{
    internal class LocalLLMStrategy() : ILLMStrategy
    {
        public (string, string) CreateModelPrompt () {
            const string modelTargetText = "この部分なんだけどさあ、もっと前後関係わかるようにしといて、";
            const string modelInput = "敬語にして";
            string modelPrompt = CreateOptimizedReq(modelTargetText, modelInput);
            string modelAns = "こちらの部分ですが、もう少し前後の関係が分かるようにしていただけますでしょうか。";
            return (modelPrompt, modelAns);
        }

        public string CreateOptimizedReq(string targetText, string input)
        {
            return "--- 対象テキスト ---" + Environment.NewLine 
                + targetText + Environment.NewLine 
                + "--- ユーザ指示 ---" + Environment.NewLine + input;
        }
        public string GetSystemPrompt()
        {
            return "あなたは文章編集の専門家です。対象テキストとユーザ指示を与えるので、対象テキストをユーザ指示に厳密に従って、適切に修正してください。回答は理由等はなにも書かず、修正した文章のみを記載してください。また元の意味や意図が変わらないよう注意してください。";
        }
    }

}
