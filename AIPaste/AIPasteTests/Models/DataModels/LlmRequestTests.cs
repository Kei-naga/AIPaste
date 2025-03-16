using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIPaste.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Moq;
using AIPaste.common;

namespace AIPaste.Models.DataModels.Tests
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