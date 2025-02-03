using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;

namespace AIPaste.Models.LLMModels
{
    internal class LlmRequestModel(string targetText, string userInput)
    {
        public string TargetText { get; set; } = targetText;
        public string UserInput { get; set; } = userInput;
        public string GetRequest()
        {
            return "--- 対象テキスト ---" + Environment.NewLine
                + TargetText + Environment.NewLine
                + "--- ユーザ指示 ---" + Environment.NewLine
                + UserInput;
        }
        public override string ToString()
        {
            return GetRequest();
        }
    }
}
