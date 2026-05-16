using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPasteTests.Models.SettingsServices.SettingModels
{
    [TestClass]
    public class GeminiModelSettingsTests
    {
        [TestMethod]
        public void Constructor_ShouldUseCurrentDefaultModel()
        {
            var settings = new GeminiModelSettings("test-api-key");

            Assert.AreEqual(GeminiModelSettings.DefaultModelName, settings.ModelName);
        }

        [TestMethod]
        public void Constructor_ShouldNormalizeDeprecatedGemini20FlashModel()
        {
            var settings = new GeminiModelSettings("test-api-key", "gemini-2.0-flash");

            Assert.AreEqual(GeminiModelSettings.DefaultModelName, settings.ModelName);
        }

        [TestMethod]
        public void Constructor_ShouldPreserveExplicitSupportedModel()
        {
            var settings = new GeminiModelSettings("test-api-key", "gemini-3.1-flash-lite");

            Assert.AreEqual("gemini-3.1-flash-lite", settings.ModelName);
        }
    }
}
