using AIPaste.common;
using AIPaste.DTO.Convertor;
using AIPaste.DTO.SettingsDTO;
using AIPaste.Models.SettingsServices.SettingModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPasteTests.DTO.Convertor
{
    [TestClass()]
    public class SettingsConvertorTests
    {
        private SettingsConvertor _convertor;

        [TestInitialize]
        public void Setup()
        {
            _convertor = new SettingsConvertor();
        }

        [TestMethod]
        public void ConvertToAppSettings_ValidDto_ReturnsCorrectAppSettings()
        {
            // Arrange
            var keyPatternDto = new KeyPatternDTO(HOT_KEY_MODIFIERS.MOD_CONTROL, Windows.System.VirtualKey.C);
            var keySettingsDto = new KeySettingsDTO(true, keyPatternDto);
            var localModelSettingsDto = new LocalModelSettingsDTO("modelPath", true, 4, 1024u, 512);
            var geminiSettingsDto = new GeminiSettingsDTO("apiKey", "modelName", "location", 2048u);
            var enabledModelDto = new EnabledModelDTO(true, false);
            var appSettingsDto = new AppSettingsDTO(true, keySettingsDto, enabledModelDto, localModelSettingsDto, geminiSettingsDto);

            // Act
            var result = _convertor.ConvertToAppSettings(appSettingsDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.AutoStart);
            Assert.IsTrue(result.KeySettings.IsHotkeyEnabled);
            Assert.AreEqual(HOT_KEY_MODIFIERS.MOD_CONTROL, result.KeySettings.KeyPattern.Modifiers);
            Assert.AreEqual(Windows.System.VirtualKey.C, result.KeySettings.KeyPattern.Key);

            Assert.AreEqual(2, result.ModelSettingsList.Length);
            var localModel = result.GetLlmModelSettings(ModelType.LocalLLM) as LlmLocalModelSettings;
            Assert.IsNotNull(localModel);
            Assert.AreEqual("modelPath", localModel.ModelPath);
            Assert.IsTrue(localModel.GpuEnabled);
            Assert.AreEqual(4, localModel.GpuLayerCount);
            Assert.AreEqual(1024u, localModel.MaxContextSize);
            Assert.AreEqual(512, localModel.MaxTokens);

            var geminiModel = result.GetLlmModelSettings(ModelType.Gemini) as GeminiModelSettings;
            Assert.IsNotNull(geminiModel);
            Assert.AreEqual("apiKey", geminiModel.ApiKey);
            Assert.AreEqual("modelName", geminiModel.ModelName);
            Assert.AreEqual("location", geminiModel.Location);
            Assert.AreEqual(2048u, geminiModel.MaxContextSize);

            Assert.IsTrue(result.ActiveLlmModels.IsLocalLlmActive);
            Assert.IsFalse(result.ActiveLlmModels.IsGeminiActive);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertToAppSettings_NullDto_ThrowsArgumentNullException()
        {
            _convertor.ConvertToAppSettings(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertToAppSettings_NullKeySettings_ThrowsArgumentNullException()
        {
            var enabledModelDto = new EnabledModelDTO(true, false);
            var appSettingsDto = new AppSettingsDTO(true, null, enabledModelDto, null, null);
            _convertor.ConvertToAppSettings(appSettingsDto);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConvertToAppSettings_NullActiveLlmModels_ThrowsArgumentNullException()
        {
            var keyPatternDto = new KeyPatternDTO(HOT_KEY_MODIFIERS.MOD_CONTROL, Windows.System.VirtualKey.C);
            var keySettingsDto = new KeySettingsDTO(true, keyPatternDto);
            var appSettingsDto = new AppSettingsDTO(true, keySettingsDto, null, null, null);
            _convertor.ConvertToAppSettings(appSettingsDto);
        }
    }
}