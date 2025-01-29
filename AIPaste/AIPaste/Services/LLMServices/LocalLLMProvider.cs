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
using NLog;

namespace AIPaste.Services.LLMServices
{
    internal class LocalLLMProvider : ILLMProvider
    {
        static private Logger _logger = LogManager.GetCurrentClassLogger();
        static private readonly object _lock = new();
        static private LLamaWeights? _model = null;
        static private LLamaContext? _context = null;
        static private LLMModelSettings _modelSettings;
        private ChatSession? _chatSession = null;
        private InferenceParams? _inferenceParams;
        public string SystemPrompt { get; private set; } = "";
        private string _presentResponse = "";
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

        public LocalLLMProvider(LLMModelSettings modelSettings)
        {
            if (!modelSettings.Equals(_modelSettings))
            {
                Dispose();
                _modelSettings = modelSettings;
                _logger.Debug($"Model settings changed to {modelSettings}");
            }
        }

        public void Initialize()
        {
            lock (_lock)
            {
                try
                {
                    _inferenceParams = new InferenceParams()
                    {
                        MaxTokens = _modelSettings.MaxTokens,
                        AntiPrompts = _modelSettings.AntiPrompts,
                        SamplingPipeline = new DefaultSamplingPipeline(),
                    };
                    if (_context == null || _model == null)
                    {
                        _logger.Info("creating Model");
                        var parameters = new ModelParams(_modelSettings.ModelPath)
                        {
                            ContextSize = _modelSettings.ContextSize,
                            GpuLayerCount = _modelSettings.GpuLayerCount,
                        };

                        _model = LLamaWeights.LoadFromFile(parameters);
                        _context = _model.CreateContext(parameters);
                        _logger.Info("successfully created model");
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Model initialization failed. Please check the model path and parameters.", ex);
                }
            }
        }

        public static void Dispose()
        {
            lock (_lock)
            {
                _model?.Dispose();
                _context?.Dispose();
                _model = null;
                _context = null;
                _modelSettings = default;

                _logger.Info("disposed Model");
            }
        }

        public void StartNewChat()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Model has not been initialized. Please call Initialize() first.");
            }
            var executor = new InteractiveExecutor(_context);
            var chatHistory = new ChatHistory();
            if (!string.IsNullOrEmpty(SystemPrompt))
            {
                chatHistory.AddMessage(AuthorRole.System, SystemPrompt);
            }
            _chatSession = new ChatSession(executor, chatHistory);
        }

        public void SetSystemPrompt(string systemPrompt)
        {
            _logger.Debug($"Setting SystemPrompt: {systemPrompt}");
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
                throw new InvalidOperationException("Chat session has not been started. Please call StartNewChat() first.");
            }
            _chatSession.AddUserMessage(modelReq);
            _chatSession.AddAssistantMessage(modelAns);
        }

        public async IAsyncEnumerable<string> GeneratingText(string req)
        {
            _logger.Debug($"GeneratingText called with {Environment.NewLine}{req}");
            if (_chatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartNewChat() first.");
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

        private static string GetTrimmedResponse(List<string> responseBuilder)
        {
            string text = string.Join("", responseBuilder);
            text = Regex.Replace(text, $"{Regex.Escape("assistant:")}\\s*", "");
            text = text.Replace("�END", "");
            return text;
        }
    }
}
