using AIPaste.Models.SettingsServices;
using Moq;
using AIPaste.Models.LLMModels;
using AIPaste.Models.ClipboardOperate;
using AIPaste.common;
using AIPaste.Models.DataModels;
using AIPasteTests;

namespace AIPaste.ViewModels.Tests
{
    [TestClass()]
    public class AiPastePageViewModelTests
    {
        private Mock<IClipboardOperator> GetClipboardOperatorMoq()
        {
            var moqClipboardOperator = new Mock<IClipboardOperator>();
            moqClipboardOperator.Setup(x => x.RegisterContentChangedHandler(It.IsAny<EventHandler<object>>()));
            var stringTask = new Task<string>(() => "dummy");
            moqClipboardOperator.Setup(x => x.GetTextAsync()).Returns(stringTask);
            return moqClipboardOperator;
        }

        private Mock<ISettingsService> GetSettingsServiceMoq()
        {
            var moqSettingsService = new Mock<ISettingsService>();
            var moqAppSettings = new Mock<IAppSettings>();
            moqSettingsService.Setup(x => x.LoadSettings()).Returns(moqAppSettings.Object);
            return moqSettingsService;
        }

        [TestMethod()]
        public void AiPastePageViewModelTest()
        {
            var settingsServiceMoq = GetSettingsServiceMoq();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            var textCorrectorFactoryMoq = new Mock<ITextCorrectorFactory>();
            textCorrectorFactoryMoq.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            var clipboardOperatorMoq = GetClipboardOperatorMoq();
            var resourceLoaderMoq = new Mock<IResourceLoaderWrapper>();

            var viewModel = new AiPastePageViewModel(settingsServiceMoq.Object, textCorrectorFactoryMoq.Object, clipboardOperatorMoq.Object, resourceLoaderMoq.Object);

            Assert.IsNotNull(viewModel);
        }

        [TestMethod()]
        public async Task GeneratingTextTest()
        {
            var SettingsServiceMoq = GetSettingsServiceMoq();
            var clipboardOperatorMoq = GetClipboardOperatorMoq();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            var textCorrectorFactoryMoq = new Mock<ITextCorrectorFactory>();
            textCorrectorFactoryMoq.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            var resourceLoaderMoq = new Mock<IResourceLoaderWrapper>();
            resourceLoaderMoq.Setup(x => x.GetString(It.IsAny<string>())).Returns("");
            var didFire = false;
            var expected = "";

            var viewModel = new AiPastePageViewModel(SettingsServiceMoq.Object, textCorrectorFactoryMoq.Object, clipboardOperatorMoq.Object, resourceLoaderMoq.Object);
            viewModel.PropertyChanged += (sender, e) => {
                didFire = true;
                expected = viewModel.OutputText;
            };
            var result = viewModel.GeneratingText("dummy");
            await result;

            Assert.IsTrue(didFire);
            Assert.AreEqual("dummy", expected);
        }

        [TestMethod()]
        public void ChangeTargetTextTest()
        {
            var settingsServiceMoq = GetSettingsServiceMoq();
            var textCorrectorStub = new LlmTextCorrectorStub("dummy", false);
            var textCorrectorFactoryMoq = new Mock<ITextCorrectorFactory>();
            textCorrectorFactoryMoq.Setup(x => x.CreateLlmTextCorrector(It.IsAny<IAppSettings>(), It.IsAny<IResourceLoaderWrapper>(), It.IsAny<IMyLogger>())).Returns(textCorrectorStub);
            var clipboardOperatorMoq = GetClipboardOperatorMoq();
            clipboardOperatorMoq.Setup(x => x.SetText(It.IsAny<string>())).Verifiable();
            var resourceLoaderMoq = new Mock<IResourceLoaderWrapper>();

            var viewModel = new AiPastePageViewModel(settingsServiceMoq.Object, textCorrectorFactoryMoq.Object, clipboardOperatorMoq.Object, resourceLoaderMoq.Object);
            viewModel.ChangeTargetText();

            clipboardOperatorMoq.Verify(x => x.SetText(It.IsAny<string>()), Times.Once);
        }
    }
}