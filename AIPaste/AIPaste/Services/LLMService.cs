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

namespace AIPaste.Services
{
    internal class LLMService(string modelPath, int gpuLayerCount = 5, uint contextSize = 1024)
    {
        public string ModelPath { get; private set; } = modelPath;
        public int GpuLayerCount { get; private set; } = gpuLayerCount;
        public uint ContextSize { get; private set; } = contextSize;
        private InteractiveExecutor? Executor { get; set; } = null;
        public string SystemPrompt { get; private set; } = "";
        private ChatSession? ChatSession { get; set; } = null;
        private InferenceParams? InferenceParams { get; set; } = null;

        public void Initialize()
        {
            try
            {
                var parameters = new ModelParams(ModelPath)
                {
                    ContextSize = ContextSize,
                    GpuLayerCount = GpuLayerCount
                };

                InferenceParams = new InferenceParams()
                {
                    MaxTokens = 256,
                    AntiPrompts = new List<string> { "END" },

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

        public void StartChat(string SystemPrompt)
        {
            if (Executor == null)
            {
                throw new InvalidOperationException("Model has not been initialized. Please call Initialize() first.");
            }
            var chatHistory = new ChatHistory();
            chatHistory.AddMessage(AuthorRole.System, SystemPrompt);
            chatHistory.AddMessage(AuthorRole.User, "対象テキスト：この部分なんだけどさあ、もっと前後関係わかるようにしといて、" + Environment.NewLine + "ユーザ指示：敬語にして");
            chatHistory.AddMessage(AuthorRole.Assistant, "こちらの部分ですが、もう少し前後の関係が分かるようにしていただけますでしょうか。");
            ChatSession = new ChatSession(Executor, chatHistory);
        }

        public async IAsyncEnumerable<string> GeneratingText(string prompt)
        {
            if (ChatSession == null)
            {
                throw new InvalidOperationException("Chat session has not been started. Please call StartChat() first.");
            }

            var responseBuilder = new List<string>();
            await foreach (var text in ChatSession.ChatAsync(
                new ChatHistory.Message(AuthorRole.User, prompt),
                InferenceParams))
            {
                responseBuilder.Add(text);
                yield return text;
            }
        }

        public static bool IsGpuAvailable() // cheking for only nvidia gpu
        {
            try
            {
                using var cudaContext = new CudaContext();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
