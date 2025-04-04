using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.DataModels;
using AIPaste.Models.LLMModels;
using HuggingfaceHub;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Testing.Platform.Logging;

namespace AIPasteTests
{
    public static class TestHelper
    {
        /// <summary>
        /// default .env file path. Create .env file in the root of the test project if it does not exist.
        /// </summary>
        private static readonly string DEFAULT_ENV_FILENAME = Path.GetFullPath
                (
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"..\..\..\.env")
                );

        /// <summary>
        /// load enviroment variables from .env file
        /// </summary>
        /// <param name="path"></param>
        public static void Load(string? path = null)
        {
            DotNetEnv.Env.Load(path ?? DEFAULT_ENV_FILENAME);
        }

        public static string GetString([CallerMemberName] string key = "") => DotNetEnv.Env.GetString(key);
        public static int GetInt([CallerMemberName] string key = "") => DotNetEnv.Env.GetInt(key);
        public static bool GetBool([CallerMemberName] string key = "") => DotNetEnv.Env.GetBool(key);
        public static double GetDouble([CallerMemberName] string key = "") => DotNetEnv.Env.GetDouble(key);

        public static string GEMINI_API_KEY => GetString();
    }

    public class LlmTextCorrectorStub(string response, bool errorFlag) : ILlmTextCorrector
    {
        public async IAsyncEnumerable<string> GeneratingText(ILlmRequest requestModel)
        {
            if (_errorFlag)
            {
                throw new Exception("error");
            }
            ChatHistory.AddUserMessage(requestModel.ToOptimizedRequest());
            foreach (var chunk in _dummyResponse.Select(x => x.ToString()).ToArray())
            {
                yield return chunk;
                await Task.Delay(10);
            }
            PresentResponse = _dummyResponse;
            ChatHistory.AddAssistantMessage(_dummyResponse);
        }
        public void ResetChat()
        {
            throw new NotImplementedException();
        }
        public bool CheckIntegrity()
        {
            return !_errorFlag;
        }
        public ChatHistory ChatHistory { get; } = [];
        public string PresentResponse { get; set; } = "";
        private string _dummyResponse = response;
        private bool _errorFlag = errorFlag;
    }
}
