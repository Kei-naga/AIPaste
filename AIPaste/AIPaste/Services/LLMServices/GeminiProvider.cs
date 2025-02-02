using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AIPaste.Models.Settings;
using GenerativeAI;
using GenerativeAI.Methods;
using GenerativeAI.Models;
using GenerativeAI.Types;
using NLog;
using static System.Net.Mime.MediaTypeNames;
using static LLama.Common.ChatHistory;

namespace AIPaste.Services.LLMServices
{
    class GeminiProvider : ILLMProvider
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly GeminiModelSettings _modelSettings;
        private string _presentResponse = "";
        private ChatSession? _chatSession = null;
        private readonly GenerativeModel _model;

        public string PresentResponse
        {
            get => _presentResponse;
            private set
            {
                if (_presentResponse != value)
                {
                    _presentResponse = value;
                    _logger.Debug($"PresentResponse changed to {value}");
                }
            }
        }

        public GeminiProvider(GeminiModelSettings modelSettings)
        {
            _modelSettings = modelSettings;
            _model = new GenerativeModel(_modelSettings.ApiKey);
            _logger.Debug($"Staeted Gemini");
        }

        public void SetSystemPrompt(string systemPrompt)
        {
            _model.SystemInstruction = systemPrompt;
            _logger.Debug($"SystemPrompt set to: {systemPrompt}");
        }

        public void StartNewChat()
        {
            _chatSession = _model.StartChat(new StartChatParams());
            _logger.Info("Started new chat session.");
        }

        public void AddChatHistory(string modelReq, string modelAns)
        {
            if (_chatSession == null)
            {
                _logger.Warn("Chat session is not started.");
                return;
            }
            _chatSession.History.Add(new Content(CreateParts(modelReq), "user"));
            _chatSession.History.Add(new Content(CreateParts(modelAns), "model"));
            _logger.Debug($"Added to chat history: {modelReq} -> {modelAns}");
        }

        private Part[] CreateParts(string text)
        {
            return [new Part() { Text = text }];
        }

        public async IAsyncEnumerable<string> GeneratingText(string req)
        {
            _logger.Debug($"GeneratingText called with {Environment.NewLine}{req}");
            if (_chatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartNewChat() first.");
            }

            var responseChunks = new List<string>();
            var tcs = new TaskCompletionSource();

            void Handler(string text)
            {
                responseChunks.Add(text);
                tcs.SetResult();
            }

            var responseBuilder = new List<string>();
            var timeout = TimeSpan.FromSeconds(1);

            var task = _model.StreamContentAsync(req, Handler);
            while (!task.IsCompleted)
            {
                var completedTask = await Task.WhenAny(tcs.Task, Task.Delay(timeout));
                if (completedTask == tcs.Task)
                {
                    tcs = new TaskCompletionSource();
                    yield return string.Join("", responseChunks);

                    responseBuilder.AddRange(responseChunks);
                    responseChunks.Clear();
                }
                else
                {
                    _logger.Debug("Streaming timed out.");
                }
            }

            PresentResponse = OptimizeResponse(responseBuilder);
        }

        private static string OptimizeResponse(List<string> responseBuilder)
        {
            string text = string.Join("", responseBuilder);
            return text;
        }
    }
}
