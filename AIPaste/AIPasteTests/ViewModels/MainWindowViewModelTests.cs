using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIPaste.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.BackgroudServices;
using Moq;
using AIPaste.Models.DataModels;
using AIPaste.Models.SettingsServices;
using Windows.System;
using AIPaste.Models.DTO;

namespace AIPaste.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        private Mock<IHotKeyManagerFactory> GetHotKeyManagerFactoryMoq()
        {
            var moqHotKeyManagerFactory = new Mock<IHotKeyManagerFactory>();
            var moqHotKeyManager = new Mock<IHotKeyManager>();
            moqHotKeyManagerFactory.Setup(x => x.CreateHotKeyManager(It.IsAny<Action>())).Returns(moqHotKeyManager.Object);
            return moqHotKeyManagerFactory;
        }

        private Mock<ISettingsService> GetSettingsServiceMoq(bool isHotkeyEnabled)
        {
            var moqSettingsService = new Mock<ISettingsService>();
            var moqAppSettings = new Mock<IAppSettings>();
            var moqKeySettings = new Mock<IKeySettings>();
            moqKeySettings.Setup(x => x.IsHotkeyEnabled).Returns(isHotkeyEnabled);
            var moqKeyPattern = new Mock<IKeyPattern>();
            moqKeySettings.Setup(x => x.KeyPattern).Returns(moqKeyPattern.Object);
            moqAppSettings.Setup(x => x.KeySettings).Returns(moqKeySettings.Object);
            moqSettingsService.Setup(x => x.LoadSettings()).Returns(moqAppSettings.Object);
            return moqSettingsService;
        }

        [TestMethod()]
        public void MainWindowViewModelTest()
        {
            var hotKeyManagerFactoryMoq = GetHotKeyManagerFactoryMoq();
            var settingsServiceMoq = GetSettingsServiceMoq(true);

            var viewModel = new MainWindowViewModel(() => { }, hotKeyManagerFactoryMoq.Object, settingsServiceMoq.Object);

            Assert.IsNotNull(viewModel);
        }

        [TestMethod()]
        public void MainWindowViewModelTest_whenFailedUpdateHotkeySettings()
        {
            var hotKeyManagerFactoryMoq = new Mock<IHotKeyManagerFactory>();
            var hotKeyManagerMoq = new Mock<IHotKeyManager>();
            hotKeyManagerMoq.Setup(x => x.UpdateHotkeySettings(It.IsAny<IKeySettings>())).Throws(new Exception("Test exception"));
            hotKeyManagerFactoryMoq.Setup(x => x.CreateHotKeyManager(It.IsAny<Action>())).Returns(hotKeyManagerMoq.Object);
            var settingsServiceMoq = GetSettingsServiceMoq(true);

            var viewModel = new MainWindowViewModel(() => { }, hotKeyManagerFactoryMoq.Object, settingsServiceMoq.Object);
            
            Assert.IsNotNull(viewModel);
            settingsServiceMoq.Verify(x => x.SaveSettings(It.IsAny<IAppSettings>()), Times.Once);
        }
    }
}