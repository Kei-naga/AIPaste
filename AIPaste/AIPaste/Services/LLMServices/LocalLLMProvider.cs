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
    internal class LocalLLMProvider(LLMModelSettings modelSettings) : ILLMProvider, IDisposable
    {
        static private InteractiveExecutor? _executor = null;
        private ChatSession? _chatSession = null;
        private InferenceParams? _inferenceParams;
        private LLMModelSettings _modelSettings = modelSettings;
        private string SystemPrompt { get; set; } = "";
        public string PresentResponse { get; private set; } = "";

        public void Initialize()
        {
            try
            {
                _inferenceParams = new InferenceParams()
                    {
                        MaxTokens = _modelSettings.MaxTokens,
                        AntiPrompts = _modelSettings.AntiPrompts,

                        SamplingPipeline = new DefaultSamplingPipeline(),
                    };

                if (_executor == null) { 
                    var parameters = new ModelParams(_modelSettings.ModelPath)
                    {
                        ContextSize = _modelSettings.ContextSize,
                        GpuLayerCount = _modelSettings.GpuLayerCount,
                    };

                    var model = LLamaWeights.LoadFromFile(parameters);
                    var context = model.CreateContext(parameters);
                    _executor = new InteractiveExecutor(context);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(" Model initialization failed. Please check the model path and parameters.", ex);
            }
        }

        public void Dispose()
        {
            _executor?.Context.Dispose();
            _executor = null;
        }

        public void StartNewChat()
        {
            if (_executor == null)
            {
                throw new InvalidOperationException("Model has not been initialized. Please call Initialize() first.");
            }
            var chatHistory = new ChatHistory();
            if (!string.IsNullOrEmpty(SystemPrompt))
            {
                chatHistory.AddMessage(AuthorRole.System, SystemPrompt);
            }
            _chatSession = new ChatSession(_executor, chatHistory);
        }
        public void SetSystemPrompt(string systemPrompt)
        {
            SystemPrompt = systemPrompt;
            if (_chatSession != null)
            {
                _chatSession.History.Messages.Clear();
                _chatSession.AddSystemMessage(SystemPrompt);
            }
        }

        public void AddChatHistory(string modelReq, string modelAns)
        {
            if (_chatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartChat() first.");
            }
            _chatSession.AddUserMessage(modelReq);
            _chatSession.AddAssistantMessage(modelAns);
        }

        public async IAsyncEnumerable<string> GeneratingText(string req)
        {
            if (_chatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartChat() first.");
            }

            var responseBuilder = new List<string>();
            await foreach (var text in _chatSession.ChatAsync(
                new ChatHistory.Message(AuthorRole.User, req),
                _inferenceParams))
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
