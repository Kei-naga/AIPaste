using AIPaste.common;
using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPasteTests.Models.SettingsServices.SettingModels
{
    [TestClass()]
    public class AppSettingsTests
    {
        private AppSettings CreateTestAppSettings()
        {
            var keyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL, Windows.System.VirtualKey.C);
            var keySettings = new KeySettings(true, keyPattern);

            var localLlmSettings = new LlmLocalModelSettings(
                ModelPath: "test/path/to/model",
                GpuEnable: true,
                GpuLayerCount: 16,
                MaxContextSize: 2048,
                MaxTokens: 512
            );

            var geminiSettings = new GeminiModelSettings(
                apiKey: "test-api-key",
                modelName: "test-model",
                location: "test-location",
                maxContextSize: 1024
            );

            var activeLlmModels = new ActiveLlmModels(
                isLocalLlmActive: true,
                isGeminiActive: false
            );

            return new AppSettings(
                autoStartSetting: true,
                keySettings: keySettings,
                modelSettingsList: [localLlmSettings, geminiSettings],
                activeLlmModels: activeLlmModels
            );
        }

        [TestMethod()]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var appSettings = CreateTestAppSettings();

            // Act
            var result = appSettings.ToString();

            // Assert
            Assert.AreEqual("KeySettings: [IsHotkeyEnabled:True, HotKey:Ctrl+C],  AutoStart: True, IsLocalLlmActive: [IsLocalLlmActive: True, IsGeminiActive: False]", result);
        }

        [TestMethod]
        public void Equals_ShouldReturnTrueForEqualSettings()
        {
            // Arrange
            var settings1 = CreateTestAppSettings();
            var settings2 = CreateTestAppSettings();

            // Act
            var areEqual = settings1.Equals(settings2);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [TestMethod]
        public void Equals_ShouldReturnFalseForDifferentSettings()
        {
            // Arrange
            var settings1 = CreateTestAppSettings();
            var settings2 = CreateTestAppSettings();
            settings2.AutoStart = false;

            // Act
            var areEqual = settings1.Equals(settings2);

            // Assert
            Assert.IsFalse(areEqual);
        }

        [TestMethod]
        public void GetLlmModelSettings_ShouldReturnCorrectSettingsForLocalLLM()
        {
            // Arrange
            var appSettings = CreateTestAppSettings();

            // Act
            var localLlmSettings = appSettings.GetLlmModelSettings(ModelType.LocalLLM);

            // Assert
            Assert.IsNotNull(localLlmSettings);
            Assert.IsInstanceOfType(localLlmSettings, typeof(LlmLocalModelSettings));
        }

        [TestMethod]
        public void GetLlmModelSettings_ShouldReturnCorrectSettingsForGemini()
        {
            // Arrange
            var appSettings = CreateTestAppSettings();

            // Act
            var geminiSettings = appSettings.GetLlmModelSettings(ModelType.Gemini);

            // Assert
            Assert.IsNotNull(geminiSettings);
            Assert.IsInstanceOfType(geminiSettings, typeof(GeminiModelSettings));
        }

        [TestMethod]
        public void GetLlmModelSettings_ShouldReturnNullForInvalidModelType()
        {
            // Arrange
            var appSettings = CreateTestAppSettings();

            // Act
            var invalidSettings = appSettings.GetLlmModelSettings((ModelType)999);

            // Assert
            Assert.IsNull(invalidSettings);
        }
    }
}