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

namespace AIPaste.ViewModels.Tests
{
    [TestClass()]
    public class MainWindowViewModelTests
    {
        private Mock<IHotKeyManagerFactory> GetHotKeyManagerFactoryMoq(bool isSuccessRegeisterHotkey)
        {
            var moqHotKeyManagerFactory = new Mock<IHotKeyManagerFactory>();
            var moqHotKeyManager = new Mock<IHotKeyManager>();
            moqHotKeyManager.Setup(x => x.RegisterHotKey(It.IsAny<IKeyPattern>())).Returns(isSuccessRegeisterHotkey);
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
            var hotKeyManagerFactoryMoq = GetHotKeyManagerFactoryMoq(true);
            var settingsServiceMoq = GetSettingsServiceMoq(true);

            var viewModel = new MainWindowViewModel(() => { }, hotKeyManagerFactoryMoq.Object, settingsServiceMoq.Object);

            Assert.IsNotNull(viewModel);
        }

        [TestMethod()]
        public void MainWindowViewModelTestWhenFailedUpdateHotkeySettings()
        {
            var hotKeyManagerFactoryMoq = GetHotKeyManagerFactoryMoq(false);
            var settingsServiceMoq = GetSettingsServiceMoq(true);

            var viewModel = new MainWindowViewModel(() => { }, hotKeyManagerFactoryMoq.Object, settingsServiceMoq.Object);
            
            Assert.IsNotNull(viewModel);
            settingsServiceMoq.Verify(x => x.SaveSettings(It.IsAny<IAppSettings>()), Times.Once);
        }

        [TestMethod()]
        public void UpdateHotkeySettingsTest_whenHotkeyIsEnbale()
        {
            var hotKeyManagerFactoryMoq = new Mock<IHotKeyManagerFactory>();
            var hotKeyManagerMoq = new Mock<IHotKeyManager>();
            hotKeyManagerMoq.Setup(x => x.RegisterHotKey(It.IsAny<IKeyPattern>())).Returns(true);
            hotKeyManagerFactoryMoq.Setup(x => x.CreateHotKeyManager(It.IsAny<Action>())).Returns(hotKeyManagerMoq.Object);
            var settingsServiceMoq = GetSettingsServiceMoq(true);
            var keySettingsMoq = new Mock<IKeySettings>();
            var keyPatternMoq = new Mock<IKeyPattern>();
            keySettingsMoq.Setup(x => x.IsHotkeyEnabled).Returns(true);
            keySettingsMoq.Setup(x => x.KeyPattern).Returns(keyPatternMoq.Object);

            var viewModel = new MainWindowViewModel(() => { }, hotKeyManagerFactoryMoq.Object, settingsServiceMoq.Object);
            var result = viewModel.UpdateHotkeySettings(keySettingsMoq.Object);

            Assert.IsTrue(result);
            hotKeyManagerMoq.Verify(x => x.RegisterHotKey(keyPatternMoq.Object), Times.Once);
        }

        [TestMethod()]
        public void UpdateHotkeySettingsTest_whenHotkeyIsDisable()
        {
            var hotKeyManagerFactoryMoq = new Mock<IHotKeyManagerFactory>();
            var hotKeyManagerMoq = new Mock<IHotKeyManager>();
            hotKeyManagerMoq.Setup(x => x.RegisterHotKey(It.IsAny<IKeyPattern>())).Returns(true);
            hotKeyManagerFactoryMoq.Setup(x => x.CreateHotKeyManager(It.IsAny<Action>())).Returns(hotKeyManagerMoq.Object);
            var settingsServiceMoq = GetSettingsServiceMoq(true);
            var keySettingsMoq = new Mock<IKeySettings>();
            var keyPatternMoq = new Mock<IKeyPattern>();
            keySettingsMoq.Setup(x => x.IsHotkeyEnabled).Returns(false);
            keySettingsMoq.Setup(x => x.KeyPattern).Returns(keyPatternMoq.Object);

            var viewModel = new MainWindowViewModel(() => { }, hotKeyManagerFactoryMoq.Object, settingsServiceMoq.Object);
            var result = viewModel.UpdateHotkeySettings(keySettingsMoq.Object);

            Assert.IsTrue(result);
            hotKeyManagerMoq.Verify(x => x.RegisterHotKey(keyPatternMoq.Object), Times.Never);
        }
    }
}