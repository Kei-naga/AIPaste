using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLama.Common;
using LLama;
using Microsoft.UI.Content;
using ManagedCuda;
using LLama.Abstractions;
using static System.Collections.Specialized.BitVector32;
using LLama.Sampling;
using System.Text.RegularExpressions;
using AIPaste.Models.Settings;

namespace AIPaste.Services.LLMServices
{
    internal class LocalLLMService(LLMModelSettings modelSettings) : LLMService
    {
        private InteractiveExecutor? Executor { get; set; } = null;
        private ChatSession? ChatSession { get; set; } = null;
        private InferenceParams? InferenceParams { get; set; } = null;
        LLMModelSettings ModelSettings = modelSettings;
        public string PresentResponse { get; private set; } = "";

        public void Initialize()
        {
            try
            {
                var parameters = new ModelParams(ModelSettings.ModelPath)
                {
                    ContextSize = ModelSettings.ContextSize,
                    GpuLayerCount = ModelSettings.GpuLayerCount,
                };

                InferenceParams = new InferenceParams()
                {
                    MaxTokens = ModelSettings.MaxTokens,
                    AntiPrompts = ModelSettings.antiPrompts,

                    SamplingPipeline = new DefaultSamplingPipeline(),
                };

                var model = LLamaWeights.LoadFromFile(parameters);
                var context = model.CreateContext(parameters);

                Executor = new InteractiveExecutor(context);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(" Model initialization failed. Please check the model path and parameters.", ex);
            }
        }

        public void StartChat(string SystemPrompt, string modelReq, string modelAns)
        {
            if (Executor == null)
            {
                throw new InvalidOperationException("Model has not been initialized. Please call Initialize() first.");
            }
            var chatHistory = new ChatHistory();
            chatHistory.AddMessage(AuthorRole.System, SystemPrompt);
            chatHistory.AddMessage(AuthorRole.User, modelReq);
            chatHistory.AddMessage(AuthorRole.Assistant, modelAns);
            ChatSession = new ChatSession(Executor, chatHistory);
        }

        public async IAsyncEnumerable<string> GeneratingText(string req)
        {
            if (ChatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartChat() first.");
            }

            var responseBuilder = new List<string>();
            await foreach (var text in ChatSession.ChatAsync(
                new ChatHistory.Message(AuthorRole.User, req),
                InferenceParams))
            {
                responseBuilder.Add(text);
                yield return text;
            }
            PresentResponse = GetTrimmedResponse(responseBuilder);
        }

        private string GetTrimmedResponse(List<string> responseBuilder)
        {
            string text = string.Join("", responseBuilder);
            text = Regex.Replace(text, $"{Regex.Escape("assistant:")}\\s*", "");
            text = text.Replace("�END", "");
            return text;
        }
    }
}
