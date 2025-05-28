using Moq;
using AIPaste.common;
using AIPaste.Models.LLMModels;

namespace AIPasteTests.Models.LLMModels
{
    [TestClass()]
    public class LlmRequestTests
    {
        [TestMethod()]
        public void PropertiesTest()
        {
            var targetText = "targetText";
            var userInput = "userInput";

            var request = new LlmRequest(targetText, userInput);

            Assert.AreEqual(targetText, request.TargetText);
            Assert.AreEqual(userInput, request.UserInput);
        }

        [TestMethod()]
        public void ToOptimizedRequestTest()
        {
            var mockResourceLoader = new Mock<IResourceLoaderWrapper>();
            mockResourceLoader.Setup(x => x.GetString("/LLMResources/TargetTextFlagForOptimizingText")).Returns("TargetTextFlagForOptimizingText");
            mockResourceLoader.Setup(x => x.GetString("/LLMResources/UserInstructionFlagForOptimizingText")).Returns("UserInstructionFlagForOptimizingText");
            var targetText = "targetText";
            var userInput = "userInput";
            var request = new LlmRequest(targetText, userInput, mockResourceLoader.Object);

            var result = request.ToOptimizedRequest();

            var expected = "TargetTextFlagForOptimizingText" + Environment.NewLine
                + targetText + Environment.NewLine
                + "UserInstructionFlagForOptimizingText" + Environment.NewLine
                + userInput;
            Assert.AreEqual(expected, result);
        }
    }
}