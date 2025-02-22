using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AIPaste.Models.LLMModels;
using AIPaste.Models.Settings;
using GenerativeAI;
using GenerativeAI.Types;
using NLog;
using Windows.Media.Protection.PlayReady;
using static System.Net.Mime.MediaTypeNames;

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
            _model = new GenerativeModel(_modelSettings.ApiKey, GoogleAIModels.Gemini2Flash);
            _logger.Debug($"Started Gemini");
        }

        public void SetSystemPrompt(string systemPrompt)
        {
            _model.SystemInstruction = systemPrompt;
            _logger.Debug($"SystemPrompt set to: {systemPrompt}");
        }

        public void StartNewChat()
        {
            var content = new Content();
            _chatSession = _model.StartChat();
            _logger.Info("Started new chat session.");
        }

        public void AddChatHistory(LlmRequestModel modelReq, string modelAns)
        {
            if (_chatSession == null)
            {
                _logger.Warn("Chat session is not started.");
                return;
            }
            _chatSession.History.Add(new Content(CreateParts(modelReq.GetRequest()), "user"));
            _chatSession.History.Add(new Content(CreateParts(modelAns), "model"));
            _logger.Debug($"Added to chat history: {modelReq} -> {modelAns}");
        }

        public static bool CheckSettingsIntegrity(ILLMModelSettings modelSettings)
        {
            // TODO: Implement
            return true;
        }

        private Part[] CreateParts(string text)
        {
            return [new Part() { Text = text }];
        }

        public async IAsyncEnumerable<string> GeneratingText(LlmRequestModel req)
        {
            _logger.Debug($"GeneratingText called with {Environment.NewLine}{req}");
            if (_chatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartNewChat() first.");
            }

            var responseBuilder = new List<string>();

            var request = new GenerateContentRequest();
            request.AddText(req.GetRequest());
            await foreach (var response in _model.StreamContentAsync(request))
            {
                // Remove trailing newlines because I don't know why but the response has them on the end
                string responseText = response.Text()?.TrimEnd('\r', '\n') ?? "";
                yield return responseText;
                responseBuilder.Add(responseText);
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
