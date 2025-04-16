using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIPaste.Models.SettingsServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using AIPaste.Models.DTO;

namespace AIPaste.Models.SettingsServices.Tests
{
    [TestClass()]
    public class SettingsServiceTests
    {
        [TestMethod()]
        public void GetInstanceTest()
        {
            var moqSettingsStore = new Mock<ISettingsStore>();

            SettingsService.InitializeInstance();
            var firstSettingsService = SettingsService.GetInstance(moqSettingsStore.Object);
            var secondSettingsService = SettingsService.GetInstance(moqSettingsStore.Object);

            Assert.AreEqual(firstSettingsService, secondSettingsService);
        }

        [TestMethod()]
        public void LoadSettingsTest()
        {
            var moqSettingsStore = new Mock<ISettingsStore>();
            var appSettings = AppSettings.GetDefaultSettings();
            moqSettingsStore.Setup(x => x.LoadSettings()).Returns(appSettings);

            SettingsService.InitializeInstance();
            var settingsService = SettingsService.GetInstance(moqSettingsStore.Object);
            var loadedSettings = settingsService.LoadSettings();

            Assert.AreEqual(appSettings, loadedSettings);
        }

        [TestMethod()]
        public void SaveSettingsTest()
        {
            var moqSettingsStore = new Mock<ISettingsStore>();
            var appSettings = AppSettings.GetDefaultSettings();
            moqSettingsStore.Setup(x => x.LoadSettings()).Returns(appSettings);
            var savedSettings = AppSettings.GetDefaultSettings();
            savedSettings.AutoStart = false;
            moqSettingsStore.Setup(x => x.SaveSettings(savedSettings));

            SettingsService.InitializeInstance();
            var settingsService = SettingsService.GetInstance(moqSettingsStore.Object);
            settingsService.SaveSettings(savedSettings);
            var loadedSettings = settingsService.LoadSettings();

            Assert.AreNotEqual(appSettings, loadedSettings);
            Assert.AreEqual(savedSettings, loadedSettings);
        }

        [TestMethod()]
        public void ResetSettingsTest()
        {
            var moqSettingsStore = new Mock<ISettingsStore>();
            var defaultAppSettings = AppSettings.GetDefaultSettings();
            var appSettings = AppSettings.GetDefaultSettings();
            appSettings.AutoStart = false;
            moqSettingsStore.Setup(x => x.LoadSettings()).Returns(appSettings);
            moqSettingsStore.Setup(x => x.ResetSettings()).Returns(defaultAppSettings);

            SettingsService.InitializeInstance();
            var settingsService = SettingsService.GetInstance(moqSettingsStore.Object);
            settingsService.ResetSettings();
            var loadedSettings = settingsService.LoadSettings();

            Assert.AreEqual(defaultAppSettings, loadedSettings);
            Assert.AreNotEqual(appSettings, loadedSettings);
        }
    }
}