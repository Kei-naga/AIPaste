using Moq;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.Services;
using HuggingfaceHub;
using LLamaSharp.SemanticKernel.ChatCompletion;
using AIPaste.Models.LLMModels;
using AIPaste.common;
using AIPaste.Models.SettingsServices.SettingModels;

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

        private Mock<IResourceLoaderWrapper> GetResourceLoaderMoq(string dummyAns)
        {
            var moqResourceLoader = new Mock<IResourceLoaderWrapper>();
            moqResourceLoader.Setup(x => x.GetString(It.IsAny<string>())).Returns(dummyAns);
            return moqResourceLoader;
        }

        [TestMethod()]
        public void LlmTextCorrectorTest()
        {
            var moqLlmStrategy = GetLlmStrategyMoq("dummy");
            var testSystemPrompt = "test system prompt";
            var dummyString = "dummy";
            var moqResourceLoader = GetResourceLoaderMoq(dummyString);

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, testSystemPrompt, moqResourceLoader.Object);

            Assert.IsNotNull(corrector);
            Assert.AreEqual(3, corrector.ChatHistory.Count());
            Assert.AreEqual("system", corrector.ChatHistory[0].Role.ToString());
            Assert.AreEqual(testSystemPrompt, corrector.ChatHistory[0].ToString());
            Assert.AreEqual("user", corrector.ChatHistory[1].Role.ToString());
            var expected = new LlmRequest(dummyString, dummyString, moqResourceLoader.Object).ToOptimizedRequest();
            Assert.AreEqual(expected, corrector.ChatHistory[1].ToString());
            Assert.AreEqual("assistant", corrector.ChatHistory[2].Role.ToString());
            Assert.AreEqual(dummyString, corrector.ChatHistory[2].ToString());
        }


        [TestMethod()]
        public void ResetChatTest()
        {
            var moqLlmStrategy = GetLlmStrategyMoq("dummy");
            var testSystemPrompt = "test system prompt";
            var dummyString = "dummy";
            var moqResourceLoader = GetResourceLoaderMoq(dummyString);

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, testSystemPrompt, moqResourceLoader.Object);
            corrector.ChatHistory.AddUserMessage("dummy message");
            corrector.ResetChat();

            Assert.AreEqual(3, corrector.ChatHistory.Count());
            Assert.AreEqual("system", corrector.ChatHistory[0].Role.ToString());
            Assert.AreEqual(testSystemPrompt, corrector.ChatHistory[0].ToString());
            Assert.AreEqual("user", corrector.ChatHistory[1].Role.ToString());
            var expected = new LlmRequest(dummyString, dummyString, moqResourceLoader.Object).ToOptimizedRequest();
            Assert.AreEqual(expected, corrector.ChatHistory[1].ToString());
            Assert.AreEqual("assistant", corrector.ChatHistory[2].Role.ToString());
            Assert.AreEqual(dummyString, corrector.ChatHistory[2].ToString());
        }

        [TestMethod()]
        public async Task GenerateText()
        {
            var dummyAns = "dummy answer";
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns);
            moqLlmStrategy.Setup(x => x.GetTokenCount(It.IsAny<ChatHistory>())).Returns(0);
            moqLlmStrategy.Setup(x => x.ModelSettings.MaxContextSize).Returns(1);
            var testSystemPrompt = "test system prompt";
            var dummyTargetText = "dummy target text";
            var dummyUserInput = "dummy user input";
            var moqResourceLoader = GetResourceLoaderMoq("dummy");

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, testSystemPrompt, moqResourceLoader.Object);
            var results = corrector.GeneratingText(dummyTargetText, dummyUserInput);

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
            var dummyTargetText = "dummy target text";
            var dummyUserInput = "dummy user input";
            var moqResourceLoader = GetResourceLoaderMoq("dummy");

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, "dummy system prompt", moqResourceLoader.Object);
            corrector.ChatHistory.AddUserMessage("dummy message1");
            corrector.ChatHistory.AddAssistantMessage("dummy message2");
            var results = corrector.GeneratingText(dummyTargetText, dummyUserInput);

            await foreach (var result in results)
            {
                Assert.AreEqual(dummyAns, result);
            }
            Assert.AreEqual(dummyAns, corrector.PresentResponse);
            Assert.IsTrue(corrector.ChatHistory.Count() == 6);
        }

        [TestMethod()]
        public void CheckIntegrityTest_returnTrue()
        {
            var dummyAns = "dummy answer";
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns);
            var moqResourceLoader = GetResourceLoaderMoq("dummy");

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, "dummy", moqResourceLoader.Object);
            var result = corrector.CheckIntegrity();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void CheckIntegrityTest_returnFalse()
        {
            var dummyAns = string.Empty;
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns);
            var moqResourceLoader = GetResourceLoaderMoq("dummy");

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, "dummy", moqResourceLoader.Object);
            var result = corrector.CheckIntegrity();

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void CheckIntegrityTest_returnFalse_whenThrowException()
        {
            var dummyAns = "dummy answer";
            var moqLlmStrategy = GetLlmStrategyMoq(dummyAns, true);
            var moqResourceLoader = GetResourceLoaderMoq("dummy");

            var corrector = new LlmTextCorrector(moqLlmStrategy.Object, "dummy", moqResourceLoader.Object);
            var result = corrector.CheckIntegrity();

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public async Task GenerateTextByLocalLlm()
        {
            var path = await HFDownloader.DownloadFileAsync("QuantFactory/Meta-Llama-3-8B-GGUF", "Meta-Llama-3-8B.Q2_K.gguf");
            var localLlmSettings = new LlmLocalModelSettings(path, true, 32, 1024, 256);
            var localLlmStrategy = new LocalLlmStrategy(localLlmSettings, new HistoryTransform());
            var dummyTargetText = "dummy target text";
            var dummyUserInput = "dummy user input";
            var moqResourceLoader = GetResourceLoaderMoq("dummy");

            var corrector = new LlmTextCorrector(localLlmStrategy, "you are my assistant", moqResourceLoader.Object);
            var results = corrector.GeneratingText(dummyTargetText, dummyUserInput);

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
            var dummyTargetText = "dummy target text";
            var dummyUserInput = "dummy user input";
            var moqResourceLoader = GetResourceLoaderMoq("dummy");

            var corrector = new LlmTextCorrector(geminiStrategy, "you are my assistant", moqResourceLoader.Object);
            var results = corrector.GeneratingText(dummyTargetText, dummyUserInput);

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