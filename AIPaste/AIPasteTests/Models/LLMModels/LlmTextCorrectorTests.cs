using Moq;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using AIPaste.Models.DataModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Services;
using HuggingfaceHub;
using LLamaSharp.SemanticKernel.ChatCompletion;
using LLamaSharp.SemanticKernel;
using AIPaste.Models.LLMModels;

namespace AIPasteTests.Models.LLMModels
{
    [TestClass()]
    public class LlmTextCorrectorTests
    {
        private Mock<ILlmStrategy> GetLlmStrategyMoq(string dummyAns, bool isError = false)
        {
            var moqLlmStrategy = new Mock<ILlmStrategy>();
            moqLlmStrategy.Setup(x => x.GetPromptExecutionSettings()).Returns(new PromptExecutionSettings());
            var streamingChatMessageContents = new[] { new StreamingChatMessageContent(AuthorRole.Assistant, dummyAns) };
            var dummyChatMessageContents = new[] { new ChatMessageContent(AuthorRole.Assistant, dummyAns) };
            var testChatCompletionService = new ChatCompletionServiceStub(streamingChatMessageContents, dummyChatMessageContents, isError);
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddKeyedSingleton<IChatCompletionService>("dummyService", testChatCompletionService);
            var testKernel = kernelBuilder.Build();
            moqLlmStrategy.Setup(x => x.GetKernel()).Returns(testKernel);
            return moqLlmStrategy;
        }

        [TestMethod()]
        public void LlmTextCorrectorTest()
        {
            var moqLlmStrategy = GetLlmStrategyMoq("dummy");
            var testSystemPrompt = "test system prompt";

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, testSystemPrompt);

            Assert.AreEqual(1, corrector.ChatHistory.Count());
            Assert.AreEqual("system", corrector.ChatHistory[0].Role.ToString());
            Assert.AreEqual(testSystemPrompt, corrector.ChatHistory[0].ToString());
        }


        [TestMethod()]
        public void ResetChatTest()
        {
            var moqLlmStrategy = GetLlmStrategyMoq("dummy");
            var testSystemPrompt = "test system prompt";
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage("test user message");
            chatHistory.AddAssistantMessage("test assistant message");

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, testSystemPrompt, chatHistory);
            corrector.ResetChat();

            Assert.AreEqual(1, corrector.ChatHistory.Count());
            Assert.AreEqual(testSystemPrompt, corrector.ChatHistory[0].ToString());
        }    

        [TestMethod()]
        public async Task GenerateText()
        {
            var dummyAns = "dummy answer";
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns);
            moqLlmStrategy.Setup(x => x.GetTokenCount(It.IsAny<ChatHistory>())).Returns(0);
            moqLlmStrategy.Setup(x => x.ModelSettings.MaxContextSize).Returns(1);
            var testSystemPrompt = "test system prompt";
            var moqRequest = new Mock<ILlmRequest>();
            moqRequest.Setup(x => x.ToOptimizedRequest()).Returns("dummy text");

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, testSystemPrompt);
            var results = corrector.GeneratingText(moqRequest.Object);

            await foreach (var result in results)
            {
                Assert.AreEqual(dummyAns, result);
            }
            Assert.AreEqual(dummyAns, corrector.PresentResponse);
        }

        [TestMethod()]
        public async Task TruncateChatHistory_WhenChatHistoryOverTokenLimit()
        {
            var dummyAns = "dummy answer";
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns);
            moqLlmStrategy.Setup(x => x.ModelSettings.MaxContextSize).Returns(1);
            moqLlmStrategy.SetupSequence(x => x.GetTokenCount(It.IsAny<ChatHistory>()))
                .Returns(2)
                .Returns(0);
            var chatHistory = new ChatHistory();
            chatHistory.AddUserMessage("dummy message1");
            chatHistory.AddAssistantMessage("dummy message2");
            var moqRequest = new Mock<ILlmRequest>();
            moqRequest.Setup(x => x.ToOptimizedRequest()).Returns("dummy text");

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, "dummy system prompt", chatHistory);
            var results = corrector.GeneratingText(moqRequest.Object);

            await foreach (var result in results)
            {
                Assert.AreEqual(dummyAns, result);
            }
            Assert.AreEqual(dummyAns, corrector.PresentResponse);
            Assert.IsTrue(corrector.ChatHistory.Count() < 5);
        }

        [TestMethod()]
        public void CheckIntegrityTest_returnTrue()
        {
            var dummyAns = "dummy answer";
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns);

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, "dummy");
            var result = corrector.CheckIntegrity();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CheckIntegrityTest_returnFalse()
        {
            var dummyAns = string.Empty;
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns);

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, "dummy");
            var result = corrector.CheckIntegrity();

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void CheckIntegrityTest_returnFalse_whenThrowException()
        {
            var dummyAns = "dummy answer";
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns, true);

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, "dummy");
            var result = corrector.CheckIntegrity();

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public async Task GenerateTextByLocalLlm()
        {
            var path = await HFDownloader.DownloadFileAsync("QuantFactory/Meta-Llama-3-8B-GGUF", "Meta-Llama-3-8B.Q2_K.gguf");
            var localLlmSettings = new LLMLocalModelSettings(path, true, 32, 1024, 256);
            var localLlmSingleton = LocalLlmSingleton.GetInstance(localLlmSettings);
            var llamaSharpPromptExecutionSettings = new LLamaSharpPromptExecutionSettings()
                {
                    MaxTokens = localLlmSettings.MaxTokens,
                    Temperature = 0.0,
                    TopP = 0.0,
                    StopSequences = new List<string>()
                };
            var localLlmStrategy = new LocalLlmStrategy(localLlmSingleton, new HistoryTransform(), llamaSharpPromptExecutionSettings);
            var moqRequest = new Mock<ILlmRequest>();
            moqRequest.Setup(x => x.ToOptimizedRequest()).Returns("hello");

            var corrector = new LlmTextCorrector(localLlmStrategy, "you are my assistant");
            var results = corrector.GeneratingText(moqRequest.Object);

            await foreach (var result in results)
            {
                Assert.IsNotEmpty(result);
            }
            Assert.IsNotEmpty(corrector.PresentResponse);
        }

        [TestMethod()]
        public async Task GenerateTextByGeminiLlm()
        {
            TestHelper.Load();
            var geminiSettings = new GeminiModelSettings(TestHelper.GEMINI_API_KEY);
            var geminiStrategy = new GeminiStrategy(geminiSettings);
            var moqRequest = new Mock<ILlmRequest>();
            moqRequest.Setup(x => x.ToOptimizedRequest()).Returns("hello");

            var corrector = new LlmTextCorrector(geminiStrategy, "you are my assistant");
            var results = corrector.GeneratingText(moqRequest.Object);

            await foreach (var result in results)
            {
                Assert.IsNotEmpty(result);
            }
            Assert.IsNotEmpty(corrector.PresentResponse);
        }
    }

    public class ChatCompletionServiceStub(IEnumerable<StreamingChatMessageContent> response, IReadOnlyList<ChatMessageContent> chatMessageContents, bool errorFlag) : IChatCompletionService
    {
        private readonly IEnumerable<StreamingChatMessageContent> _response = response;
        private readonly IReadOnlyList<ChatMessageContent> _chatMessageContents = chatMessageContents;
        private readonly bool _errorFlag = errorFlag;

        IReadOnlyDictionary<string, object?> IAIService.Attributes => throw new NotImplementedException();

        public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? settings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default)
        {
            if (_errorFlag)
            {
                throw new Exception("error");
            }
            return GetAsyncEnumerable(_response);
        }

        Task<IReadOnlyList<ChatMessageContent>> IChatCompletionService.GetChatMessageContentsAsync(ChatHistory chatHistory, PromptExecutionSettings? executionSettings, Kernel? kernel, CancellationToken cancellationToken)
        {
            if (_errorFlag)
            {
                throw new Exception("error");
            }
            return Task.FromResult(_chatMessageContents);
        }

        private async IAsyncEnumerable<StreamingChatMessageContent> GetAsyncEnumerable(IEnumerable<StreamingChatMessageContent> contents)
        {
            foreach (var content in contents)
            {
                yield return content;
                await Task.Delay(1);
            }
        }
    }
}