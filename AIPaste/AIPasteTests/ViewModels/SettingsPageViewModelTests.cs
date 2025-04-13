using AIPaste.Models.DataModels;
using AIPaste.Models.SettingsServices;
using Moq;
using AIPaste.Models.StartupServices;
using AIPaste.Models.LLMModels;
using AIPasteTests;
using AIPaste.common;
using AIPaste.Models.BackgroudServices;

namespace AIPaste.ViewModels.Tests
{
    [TestClass()]
    public class SettingsPageViewModelTests
    {
        private Mock<IAppSettings> GetAppSettingsMoq(ILLMModelSettings[] llmModelSettings, IKeySettings keySettings)
        {
            var moqAppSettings = new Mock<IAppSettings>();
            moqAppSettings.Setup(x => x.KeySettings).Returns(keySettings);
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

        private Mock<ISettingsService> GetSettingsServiceMoq(IAppSettings moqAppSettings)
        {
            var moqSettingsService = new Mock<ISettingsService>();
            moqSettingsService.Setup(x => x.LoadSettings()).Returns(moqAppSettings);
            return moqSettingsService;
        }

        private Mock<IHotKeyManager> GetHotKeyManagerMoq()
        {
            var moqHotKeyManager = new Mock<IHotKeyManager>();
            moqHotKeyManager.Setup(x => x.RegisterHotKey(It.IsAny<IKeyPattern>()));
            moqHotKeyManager.Setup(x => x.UnRegisterHotKey());
            return moqHotKeyManager;
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
            var moqAppSettings = GetAppSettingsMoq([llmLocalModelSettingsdummy, geminiModelSettingsdummy], moqKeySettings.Object);
            var moqSettingsService = GetSettingsServiceMoq(moqAppSettings.Object);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var moqHotKeyManager = GetHotKeyManagerMoq();

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object, moqHotKeyManager.Object);

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
            return GetAppSettingsMoq([llmLocalModelSettingsdummy, geminiModelSettingsdummy], moqKeySettings.Object);
        }

        [TestMethod()]
        public void SaveSettingsTest_RetuenTrueWhenNoSettingsChanged()
        {
            var moqAppSettings = GetDummyAppSettingsMoq();
            var moqSettingsService = GetSettingsServiceMoq(moqAppSettings.Object);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var moqHotKeyManager = GetHotKeyManagerMoq();

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object, moqHotKeyManager.Object);
            var result = viewModel.SaveSettings();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnFalseWhenInvalidLlmSettings()
        {
            var moqAppSettings = GetDummyAppSettingsMoq();
            var moqSettingsService = GetSettingsServiceMoq(moqAppSettings.Object);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", true);
            moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            var moqHotKeyManager = GetHotKeyManagerMoq();

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object, moqHotKeyManager.Object);
            viewModel.LLMModelPath = "changing!";
            var result = viewModel.SaveSettings();

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnFalseWhenFailedToApplyHotkeySettings()
        {
            var dummyKeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL , Windows.System.VirtualKey.C);
            var dummyKeySettings = new KeySettings(true, dummyKeyPattern);
            var dummyLlmLocalModelSettings = new LLMLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummyGeminiApiKey = "dummy_gemini_api_key";
            var dummygeminiModelSettings = new GeminiModelSettings(dummyGeminiApiKey);
            var dummyAppSettings = new AppSettings(true, ModelType.LocalLLM, dummyKeySettings, [dummyLlmLocalModelSettings, dummygeminiModelSettings]);
            var moqSettingsService = GetSettingsServiceMoq(dummyAppSettings);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            var moqHotKeyManager = GetHotKeyManagerMoq();
            moqHotKeyManager.Setup(x => x.UpdateHotkeySettings(It.IsAny<IKeySettings>())).Throws(new Exception("Failed to register hotkey"));

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object, moqHotKeyManager.Object);
            viewModel.LLMModelPath = "changing!";
            var result = viewModel.SaveSettings();

            Assert.IsFalse(result);
            Assert.IsFalse(viewModel.IsHotkeyEnabled);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnFalseWhenFailedToApplyAutoStartSettings()
        {
            var keyPatternMoq = GetKeyPatternMoq();
            var keySettingsMoq = GetKeySettingsMoq(keyPatternMoq, true);
            var llmLocalModelSettingsdummy = new LLMLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummygeminiModelSettings = new GeminiModelSettings("dummy_gemini_api_key");
            var dummyAppSettings = new AppSettings(true, ModelType.LocalLLM, keySettingsMoq.Object, [llmLocalModelSettingsdummy, dummygeminiModelSettings]);
            var moqSettingsService = GetSettingsServiceMoq(dummyAppSettings);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            moqStartupManager.Setup(x => x.IsAutoStartupMode()).ReturnsAsync(false);
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            var moqHotKeyManager = GetHotKeyManagerMoq();

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object, moqHotKeyManager.Object);
            viewModel.LLMModelPath = "changing!";
            var result = viewModel.SaveSettings();

            Assert.IsFalse(result);
            Assert.IsFalse(viewModel.AutoStart);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnTrueWhenSavingIsSuccess()
        {
            var keyPatternMoq = GetKeyPatternMoq();
            var keySettingsMoq = GetKeySettingsMoq(keyPatternMoq, true);
            var llmLocalModelSettingsdummy = new LLMLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummygeminiModelSettings = new GeminiModelSettings("dummy_gemini_api_key");
            var dummyAppSettings = new AppSettings(true, ModelType.LocalLLM, keySettingsMoq.Object, [llmLocalModelSettingsdummy, dummygeminiModelSettings]);
            var moqSettingsService = GetSettingsServiceMoq(dummyAppSettings);
            var moqStartupManager = new Mock<IAutoStartupManager>();
            moqStartupManager.Setup(x => x.IsAutoStartupMode()).ReturnsAsync(true);
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            var moqHotKeyManager = GetHotKeyManagerMoq();

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object, moqHotKeyManager.Object);
            viewModel.LLMModelPath = "changing!";
            var result = viewModel.SaveSettings();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void SaceSettingsTest_ReturnFalseWhenSavingIsFailed()
        {
            var keyPatternMoq = GetKeyPatternMoq();
            var keySettingsMoq = GetKeySettingsMoq(keyPatternMoq, true);
            var llmLocalModelSettingsdummy = new LLMLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummygeminiModelSettings = new GeminiModelSettings("dummy_gemini_api_key");
            var dummyAppSettings = new AppSettings(true, ModelType.LocalLLM, keySettingsMoq.Object, [llmLocalModelSettingsdummy, dummygeminiModelSettings]);
            var moqSettingsService = GetSettingsServiceMoq(dummyAppSettings);
            var count = 0;
            moqSettingsService.Setup(x => x.SaveSettings(It.IsAny<IAppSettings>())).Callback(() =>
            {
                count++;
                if (count == 1)
                {
                    throw new Exception("Failed to save settings");
                }
            });
            var moqStartupManager = new Mock<IAutoStartupManager>();
            moqStartupManager.Setup(x => x.IsAutoStartupMode()).ReturnsAsync(true);
            var moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            var moqHotKeyManager = GetHotKeyManagerMoq();

            var viewModel = new SettingsPageViewModel(moqSettingsService.Object, moqStartupManager.Object, moqTextCorrectorFactory.Object, moqHotKeyManager.Object);
            viewModel.LLMModelPath = "changing!";
            var result = viewModel.SaveSettings();

            Assert.IsFalse(result);
        }
    }
}