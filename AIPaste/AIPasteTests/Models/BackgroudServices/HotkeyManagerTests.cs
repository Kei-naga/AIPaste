using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIPaste.Models.BackgroudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.common;
using Moq;
using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPasteTests.Models.BackgroudServices
{
    [TestClass]
    public class HotKeyManagerTests
    {
        private Mock<IHotkeyMessageManager> _mockHotkeyMessageManager = new();
        private Mock<IMyLogger> _mockLogger = new();

        [TestInitialize]
        public void Setup()
        {
            _mockHotkeyMessageManager = new Mock<IHotkeyMessageManager>();
            _mockLogger = new Mock<IMyLogger>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            HotKeyManager.DisposeInstance();
        }

        [TestMethod]
        public void CreateInstance_Always_ReturnsNewSingleton()
        {
            var instance = HotKeyManager.CreateInstance(_mockHotkeyMessageManager.Object, _mockLogger.Object);
            Assert.IsNotNull(instance);
            Assert.AreSame(instance, HotKeyManager.GetInstance());
        }

        [TestMethod]
        public void GetInstance_BeforeCreateInstance_ReturnsNull()
        {
            // まだ CreateInstance が呼ばれていない
            var instance = HotKeyManager.GetInstance();
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void RegisterHotKey_ValidPattern_CallsHotkeyMessageManagerRegister()
        {
            var instance = HotKeyManager.CreateInstance(_mockHotkeyMessageManager.Object, _mockLogger.Object);
            var mockKeyPattern = new Mock<IKeyPattern>();

            instance.RegisterHotKey(mockKeyPattern.Object);

            _mockHotkeyMessageManager.Verify(
                x => x.RegisterHotKey(mockKeyPattern.Object),
                Times.Once
            );
        }

        [TestMethod]
        public void UnRegisterHotKey_Always_CallsHotkeyMessageManagerUnregister()
        {
            var instance = HotKeyManager.CreateInstance(_mockHotkeyMessageManager.Object, _mockLogger.Object);

            instance.UnRegisterHotKey();

            _mockHotkeyMessageManager.Verify(
                x => x.UnregisterHotKey(),
                Times.Once
            );
        }

        [TestMethod]
        public void UpdateHotkeySettings_WhenEnabled_CallsRegisterHotKey()
        {
            var instance = HotKeyManager.CreateInstance(_mockHotkeyMessageManager.Object, _mockLogger.Object);
            var mockSettings = new Mock<IKeySettings>();
            var mockKeyPattern = new Mock<IKeyPattern>();

            mockSettings.Setup(x => x.IsHotkeyEnabled).Returns(true);
            mockSettings.Setup(x => x.KeyPattern).Returns(mockKeyPattern.Object);

            instance.UpdateHotkeySettings(mockSettings.Object);

            _mockHotkeyMessageManager.Verify(
                x => x.RegisterHotKey(mockKeyPattern.Object),
                Times.Once
            );
            _mockHotkeyMessageManager.Verify(
                x => x.UnregisterHotKey(),
                Times.Never
            );
        }

        [TestMethod]
        public void UpdateHotkeySettings_WhenDisabled_CallsUnregisterHotKey()
        {
            var instance = HotKeyManager.CreateInstance(_mockHotkeyMessageManager.Object, _mockLogger.Object);
            var mockSettings = new Mock<IKeySettings>();

            mockSettings.Setup(x => x.IsHotkeyEnabled).Returns(false);

            instance.UpdateHotkeySettings(mockSettings.Object);

            _mockHotkeyMessageManager.Verify(
                x => x.UnregisterHotKey(),
                Times.Once
            );
            _mockHotkeyMessageManager.Verify(
                x => x.RegisterHotKey(It.IsAny<IKeyPattern>()),
                Times.Never
            );
        }
    }
}