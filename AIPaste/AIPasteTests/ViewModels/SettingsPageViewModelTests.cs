using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIPaste.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.DataModels;
using AIPaste.Models.SettingsServices;
using Moq;
using AIPaste.Models.StartupServices;
using AIPaste.Models.LLMModels;
using AIPasteTests;
using AIPaste.common;
using NLog;

namespace AIPaste.ViewModels.Tests
{
    [TestClass()]
    public class SettingsPageViewModelTests
    {
        private Mock<IAppSettings> GetAppSettingsMoq(ILLMModelSettings[] llmModelSettings, Mock<IKeySettings> moqKeySettings)
        {
            var moqAppSettings = new Mock<IAppSettings>();
            moqAppSettings.Setup(x => x.KeySettings).Returns(moqKeySettings.Object);
            moqAppSettings.Setup(x => x.ModelSettingsList).Returns(llmModelSettings);
            moqAppSettings.Setup(x => x.ModelType).Returns(ModelType.LocalLLM);
            moqAppSettings.Setup(x => x.AutoStart).Returns(true);
            return moqAppSettings;
        }

        private Mock<IKeyPattern> GetKeyPatternMoq()
        {
            var moqKeyPattern = new Mock<IKeyPattern>();
            moqKeyPattern.Setup(x => x.Modifiers).Returns(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT);
            moqKeyPattern.Setup(x => x.Key).Returns(Windows.System.VirtualKey.C);
            return moqKeyPattern;
        }

        private Mock<IKeySettings> GetKeySettingsMoq(Mock<IKeyPattern> moqKeyPattern, bool isHotkeyEnable)
        {
            var moqKeySettings = new Mock<IKeySettings>();
            moqKeySettings.Setup(x => x.IsHotkeyEnabled).Returns(isHotkeyEnable);
            moqKeySettings.Setup(x => x.KeyPattern).Returns(moqKeyPattern.Object);
            return moqKeySettings;
        }

        private Mock<ISettingsService> GetSettingsServiceMoq(Mock<IAppSettings> moqAppSettings)
        {
            var moqSettingsService = new Mock<ISettingsService>();
            moqSettingsService.Setup(x => x.LoadSettings()).Returns(moqAppSettings.Object);
            return moqSettingsService;
        }

        [TestMethod()]
        public void SettingsPageViewModelTest()
        {
            var modelPath = "dummy_model_path";
            var gpuEnabled = true;
            var gpuLayerCount = 4;
            var maxContextSize = 1024u;
            var maxTokens = 512;
            var llmLocalModelSettingsdummy = new LLMLocalModelSettings(modelPath, gpuEnabled, gpuLayerCount, maxContextSize, maxTokens);
            var dummyGeminiApiKey = "dummy_gemini_api_key";
            var geminiModelSettingsdummy = new GeminiModelSettings(dummyGeminiApiKey);
            var moqKeyPattern = GetKeyPatternMoq();
            var moqKeySettings = GetKeySettingsMoq(moqKeyPattern, true);
            var moqAppSettings = GetAppSettingsMoq([llmLocalModelSettingsdummy, geminiModelSettingsdummy], moqKeySettings);
            var moqSettingsService = GetSettingsServiceMoq(moqAppSettings);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object);

            Assert.IsNotNull(viewModel);
            Assert.AreEqual(modelPath, viewModel.LLMModelPath);
            Assert.AreEqual(gpuLayerCount, viewModel.GpuLayerCount);
            Assert.AreEqual(maxTokens, viewModel.MaxTokens);
            Assert.AreEqual(gpuEnabled, viewModel.GpuEnabled);
            Assert.IsTrue(viewModel.IsHotkeyEnabled);
            Assert.AreEqual(moqKeyPattern.Object.Key, viewModel.Key);
            Assert.IsTrue(viewModel.CtrlModifier);
            Assert.IsTrue(viewModel.AltModifier);
            Assert.IsFalse(viewModel.ShiftModifier);
            Assert.IsFalse(viewModel.WinModifier);
            Assert.IsTrue(viewModel.AutoStart);
            Assert.AreEqual(ModelType.LocalLLM, viewModel.ModelTypeName);
            Assert.IsTrue(viewModel.IsLocalLLMSelected);
            Assert.IsFalse(viewModel.IsGeminiSelected);
            Assert.AreEqual(dummyGeminiApiKey, viewModel.ApiKey);
        }

        private Mock<IAppSettings> GetDummyAppSettingsMoq()
        {
            var moqKeyPattern = GetKeyPatternMoq();
            var moqKeySettings = GetKeySettingsMoq(moqKeyPattern, true);
            var llmLocalModelSettingsdummy = new LLMLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummyGeminiApiKey = "dummy_gemini_api_key";
            var geminiModelSettingsdummy = new GeminiModelSettings(dummyGeminiApiKey);
            return GetAppSettingsMoq([llmLocalModelSettingsdummy, geminiModelSettingsdummy], moqKeySettings);
        }

        [TestMethod()]
        public void SaveSettingsTest_RetuenTrueWhenNoSettingsChanged()
        {
            var moqAppSettings = GetDummyAppSettingsMoq();
            var moqSettingsService = GetSettingsServiceMoq(moqAppSettings);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object);
            var result = viewModel.SaveSettings();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnFalseWhenInvalidLlmSettings()
        {
            var moqAppSettings = GetDummyAppSettingsMoq();
            var moqSettingsService = GetSettingsServiceMoq(moqAppSettings);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", true);
            moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<ILogger>())).Returns(textCorrectorStub);

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object);
            viewModel.LLMModelPath = "changing!";
            var result = viewModel.SaveSettings();

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnFalseWhenFailedToApplyHotkeySettings()
        {
            var moqAppSettings = GetDummyAppSettingsMoq();
            var moqSettingsService = GetSettingsServiceMoq(moqAppSettings);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<ILogger>())).Returns(textCorrectorStub);

        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnTrueWhenFailedToApplyAutoStartSettings()
        {
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnTrueWhenSavingIsSuccess()
        {
        }
    }
}