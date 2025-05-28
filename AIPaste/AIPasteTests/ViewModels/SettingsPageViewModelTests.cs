using AIPaste.Models.SettingsServices;
using Moq;
using AIPaste.Models.StartupServices;
using AIPaste.Models.LLMModels;
using AIPaste.common;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.SettingsServices.SettingModels;
using AIPaste.ViewModels;

namespace AIPasteTests.ViewModels
{
    [TestClass()]
    public class SettingsPageViewModelTests
    {
        private Mock<ISettingsService> _moqSettingsService;
        private Mock<IAutoStartupManager> _moqStartupManager;
        private Mock<ITextCorrectorFactory> _moqTextCorrectorFactory;
        private Mock<IHotKeyManager> _moqHotKeyManager;

        [TestInitialize]
        public void TestInitialize()
        {
            _moqSettingsService = new Mock<ISettingsService>();
            _moqStartupManager = new Mock<IAutoStartupManager>();
            _moqTextCorrectorFactory = new Mock<ITextCorrectorFactory>();
            _moqHotKeyManager = new Mock<IHotKeyManager>();
            _moqHotKeyManager.Setup(x => x.RegisterHotKey(It.IsAny<IKeyPattern>()));
            _moqHotKeyManager.Setup(x => x.UnRegisterHotKey());
        }

        private Mock<IAppSettings> GetAppSettingsMoq(ILlmModelSettings[] llmModelSettings, IKeySettings keySettings, IActiveLlmModels activeLlmModels)
        {
            var moqAppSettings = new Mock<IAppSettings>();
            moqAppSettings.Setup(x => x.KeySettings).Returns(keySettings);
            moqAppSettings.Setup(x => x.ModelSettingsList).Returns(llmModelSettings);
            moqAppSettings.Setup(x => x.ActiveLlmModels).Returns(activeLlmModels);
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

        [TestMethod()]
        public void SettingsPageViewModelTest()
        {
            var modelPath = "dummy_model_path";
            var gpuEnabled = true;
            var gpuLayerCount = 4;
            var maxContextSize = 1024u;
            var maxTokens = 512;
            var llmLocalModelSettingsdummy = new LlmLocalModelSettings(modelPath, gpuEnabled, gpuLayerCount, maxContextSize, maxTokens);
            var dummyGeminiApiKey = "dummy_gemini_api_key";
            var geminiModelSettingsdummy = new GeminiModelSettings(dummyGeminiApiKey);
            var moqKeyPattern = GetKeyPatternMoq();
            var moqKeySettings = GetKeySettingsMoq(moqKeyPattern, true);
            var activeLlmModels = new ActiveLlmModels(true, false);
            var moqAppSettings = GetAppSettingsMoq([llmLocalModelSettingsdummy, geminiModelSettingsdummy], moqKeySettings.Object, activeLlmModels);
            _moqSettingsService.Setup(x => x.LoadSettings()).Returns(moqAppSettings.Object);

            var viewModel = new SettingsPageViewModel(_moqSettingsService.Object, _moqStartupManager.Object, _moqTextCorrectorFactory.Object, _moqHotKeyManager.Object);

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
            Assert.IsTrue(viewModel.ActivatedLocalLlm);
            Assert.IsFalse(viewModel.ActivatedGemini);
            Assert.AreEqual(dummyGeminiApiKey, viewModel.ApiKey);
        }

        private Mock<IAppSettings> GetDummyAppSettingsMoq()
        {
            var moqKeyPattern = GetKeyPatternMoq();
            var moqKeySettings = GetKeySettingsMoq(moqKeyPattern, true);
            var llmLocalModelSettingsdummy = new LlmLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummyGeminiApiKey = "dummy_gemini_api_key";
            var geminiModelSettingsdummy = new GeminiModelSettings(dummyGeminiApiKey);
            var activeLlmModels = new ActiveLlmModels(true, false);
            return GetAppSettingsMoq([llmLocalModelSettingsdummy, geminiModelSettingsdummy], moqKeySettings.Object, activeLlmModels);
        }

        [TestMethod()]
        public void SaveSettingsTest_RetuenTrueWhenNoSettingsChanged()
        {
            var moqAppSettings = GetDummyAppSettingsMoq();
            _moqSettingsService.Setup(x => x.LoadSettings()).Returns(moqAppSettings.Object);

            var viewModel = new SettingsPageViewModel(_moqSettingsService.Object, _moqStartupManager.Object, _moqTextCorrectorFactory.Object, _moqHotKeyManager.Object);
            var result = viewModel.SaveSettings();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnFalseWhenInvalidLlmSettings()
        {
            var moqAppSettings = GetDummyAppSettingsMoq();
            _moqSettingsService.Setup(x => x.LoadSettings()).Returns(moqAppSettings.Object);
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", true);
            _moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<ILlmModelSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);

            var viewModel = new SettingsPageViewModel(_moqSettingsService.Object, _moqStartupManager.Object, _moqTextCorrectorFactory.Object, _moqHotKeyManager.Object);
            viewModel.LLMModelPath = "changing!";
            var result = viewModel.SaveSettings();

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnFalseWhenFailedToApplyHotkeySettings()
        {
            var dummyKeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL , Windows.System.VirtualKey.C);
            var dummyKeySettings = new KeySettings(true, dummyKeyPattern);
            var dummyLlmLocalModelSettings = new LlmLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummyGeminiApiKey = "dummy_gemini_api_key";
            var dummygeminiModelSettings = new GeminiModelSettings(dummyGeminiApiKey);
            var dummyactiveLlmModels = new ActiveLlmModels(true, false);
            var dummyAppSettings = new AppSettings(true, dummyKeySettings, [dummyLlmLocalModelSettings, dummygeminiModelSettings], dummyactiveLlmModels);
            _moqSettingsService.Setup(x => x.LoadSettings()).Returns(dummyAppSettings);
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            _moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<ILlmModelSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            _moqHotKeyManager.Setup(x => x.UpdateHotkeySettings(It.IsAny<IKeySettings>())).Throws(new Exception("Failed to register hotkey"));

            var viewModel = new SettingsPageViewModel(_moqSettingsService.Object, _moqStartupManager.Object, _moqTextCorrectorFactory.Object, _moqHotKeyManager.Object);
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
            var llmLocalModelSettingsdummy = new LlmLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummygeminiModelSettings = new GeminiModelSettings("dummy_gemini_api_key");
            var dummyActiveLlmModels = new ActiveLlmModels(true, false);
            var dummyAppSettings = new AppSettings(true, keySettingsMoq.Object, [llmLocalModelSettingsdummy, dummygeminiModelSettings], dummyActiveLlmModels);
            _moqSettingsService.Setup(x => x.LoadSettings()).Returns(dummyAppSettings);
            _moqStartupManager.Setup(x => x.IsAutoStartupMode()).ReturnsAsync(false);
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            _moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<ILlmModelSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);

            var viewModel = new SettingsPageViewModel(_moqSettingsService.Object, _moqStartupManager.Object, _moqTextCorrectorFactory.Object, _moqHotKeyManager.Object);
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
            var llmLocalModelSettingsdummy = new LlmLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummygeminiModelSettings = new GeminiModelSettings("dummy_gemini_api_key");
            var dummyActiveLlmModels = new ActiveLlmModels(true, false);
            var dummyAppSettings = new AppSettings(true, keySettingsMoq.Object, [llmLocalModelSettingsdummy, dummygeminiModelSettings], dummyActiveLlmModels);
            _moqSettingsService.Setup(x => x.LoadSettings()).Returns(dummyAppSettings);
            _moqStartupManager.Setup(x => x.IsAutoStartupMode()).ReturnsAsync(true);
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            _moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<ILlmModelSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);

            var viewModel = new SettingsPageViewModel(_moqSettingsService.Object, _moqStartupManager.Object, _moqTextCorrectorFactory.Object, _moqHotKeyManager.Object);
            viewModel.LLMModelPath = "changing!";
            var result = viewModel.SaveSettings();

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void SaveSettingsTest_ReturnFalseWhenSavingIsFailed()
        {
            var keyPatternMoq = GetKeyPatternMoq();
            var keySettingsMoq = GetKeySettingsMoq(keyPatternMoq, true);
            var llmLocalModelSettingsdummy = new LlmLocalModelSettings("dummy_model_path", true, 4, 1024u, 512);
            var dummygeminiModelSettings = new GeminiModelSettings("dummy_gemini_api_key");
            var dummyActiveLlmModels = new ActiveLlmModels(true, false);
            var dummyAppSettings = new AppSettings(true, keySettingsMoq.Object, [llmLocalModelSettingsdummy, dummygeminiModelSettings], dummyActiveLlmModels);
            _moqSettingsService.Setup(x => x.LoadSettings()).Returns(dummyAppSettings);
            var count = 0;
            _moqSettingsService.Setup(x => x.SaveSettings(It.IsAny<IAppSettings>())).Callback(() =>
            {
                count++;
                if (count == 1)
                {
                    throw new Exception("Failed to save settings");
                }
            });
            _moqStartupManager.Setup(x => x.IsAutoStartupMode()).ReturnsAsync(true);
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            _moqTextCorrectorFactory.Setup(x => x.CreateLlmTextCorrector(It.IsAny<ILlmModelSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);

            var viewModel = new SettingsPageViewModel(_moqSettingsService.Object, _moqStartupManager.Object, _moqTextCorrectorFactory.Object, _moqHotKeyManager.Object)
            {
                LLMModelPath = "changing!"
            };
            var result = viewModel.SaveSettings();

            Assert.IsFalse(result);
        }
    }
}