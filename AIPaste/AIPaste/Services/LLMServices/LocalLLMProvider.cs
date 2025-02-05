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
using AIPaste.Models.LLMModels;

namespace AIPaste.Services.LLMServices
{
    internal class LocalLLMProvider : ILLMProvider
    {
        static private Logger _logger = LogManager.GetCurrentClassLogger();
        static private readonly object _lock = new();
        static private LLamaWeights? _model = null;
        static private LLamaContext? _context = null;
        static private LLMLocalModelSettings? _modelSettings = null;
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

        public LocalLLMProvider(LLMLocalModelSettings modelSettings)
        {
            if (_modelSettings == null || !modelSettings.Equals(_modelSettings))
            {
                try
                {
                    Dispose();
                    _modelSettings = modelSettings;
                    _logger.Debug($"Model settings changed to {modelSettings}");
                    Initialize();
                }
                catch (Exception ex)
                {
                    _modelSettings = null;
                    _logger.Error(ex, "Failed to initialize model");
                    throw;
                }
            }
            _logger.Debug($"Sterted Local LLM");
        }

        private static void Initialize()
        {
            lock (_lock)
            {
                try
                {
                    if (_modelSettings == null)
                    {
                        throw new InvalidOperationException("Model settings have not been provided.");
                    }

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

        public static bool CheckSettingsIntegrity(ILLMModelSettings modelSettings)
        {
            if (modelSettings is not LLMLocalModelSettings)
            {
                return false;
            }
            if (_modelSettings != null && _modelSettings.Equals(modelSettings))
            {
                return true;
            }
            _logger.Info("Despose Local LLM");
            Dispose();
            _modelSettings = (LLMLocalModelSettings)modelSettings;
            _logger.Debug($"Model settings changed to {modelSettings}");
            try { Initialize(); }
            catch
            {
                _modelSettings = null;
                return false;
            }
            return true;
        }

        public void StartNewChat()
        {
            if (_context == null || _modelSettings == null)
            {
                throw new InvalidOperationException("Model has not been initialized succesfully. Please start over.");
            }
            _inferenceParams = new InferenceParams()
            {
                MaxTokens = _modelSettings.MaxTokens,
                AntiPrompts = _modelSettings.AntiPrompts,
                SamplingPipeline = new DefaultSamplingPipeline(),
            };
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

        public void AddChatHistory(LlmRequestModel modelReq, string modelAns)
        {
            if (_chatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartNewChat() first.");
            }
            _chatSession.AddUserMessage(modelReq.GetRequest());
            _chatSession.AddAssistantMessage(modelAns);
        }

        public async IAsyncEnumerable<string> GeneratingText(LlmRequestModel req)
        {
            _logger.Debug($"GeneratingText called with {Environment.NewLine}{req}");
            if (_chatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartNewChat() first.");
            }

            var responseBuilder = new List<string>();
                await foreach (var text in _chatSession.ChatAsync(
                    new ChatHistory.Message(AuthorRole.User, req.GetRequest()),
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
