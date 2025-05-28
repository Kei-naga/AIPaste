using Microsoft.VisualStudio.TestTools.UnitTesting;
using AIPaste.Models.BackgroudServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.common;
using Moq;
using Windows.System;
using AIPaste.Models.SettingsServices.SettingModels;

namespace AIPasteTests.Models.BackgroudServices
{
    [TestClass()]
    public class HotkeyMessageManagerTests
    {
        Mock<IMyLogger> _mockLogger = new();
        Mock<IWin32ApiWrapper> _mockWin32 = new();
        Mock<IDummyWindowManager> _mockDummyWndManager = new();

        [TestInitialize]
        public void TestInitialize()
        {
            _mockWin32 = new Mock<IWin32ApiWrapper>();
            _mockDummyWndManager = new Mock<IDummyWindowManager>();
            _mockLogger = new Mock<IMyLogger>();
        }

        [TestMethod]
        public void HotkeyMessageManagerTest()
        {
            // Act
            var manager = new HotkeyMessageManager(
                () => { /* コールバック検証用 */ },
                _mockWin32.Object,
                _mockDummyWndManager.Object,
                _mockLogger.Object);

            // Assert
            Assert.IsNotNull(manager);
        }

        [TestMethod]
        public void RegisterHotKeyTest_Success()
        {
            _mockWin32
                .SetupSequence(m => m.RegisterHotKey(It.IsAny<nint>(), It.IsAny<int>(), It.IsAny<uint>(), It.IsAny<uint>()))
                .Returns(false)  // 1回目失敗 -> 再試行させるため
                .Returns(true);  // 2回目成功

            var keyPattern = new Mock<IKeyPattern>();
            keyPattern.Setup(k => k.Modifiers).Returns(HOT_KEY_MODIFIERS.MOD_CONTROL);
            keyPattern.Setup(k => k.Key).Returns(VirtualKey.C);

            var manager = new HotkeyMessageManager(
                () => { },
                _mockWin32.Object,
                _mockDummyWndManager.Object,
                _mockLogger.Object);

            // Act
            manager.RegisterHotKey(keyPattern.Object);

            // Assert
            _mockWin32.Verify(m => m.RegisterHotKey(It.IsAny<nint>(), It.IsAny<int>(), It.IsAny<uint>(), It.IsAny<uint>()), Times.Exactly(2));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterHotKeyTest_FailAfterRetries()
        {
            _mockWin32
                .Setup(m => m.RegisterHotKey(It.IsAny<nint>(), It.IsAny<int>(), It.IsAny<uint>(), It.IsAny<uint>()))
                .Returns(false);

            var keyPattern = new Mock<IKeyPattern>();
            var manager = new HotkeyMessageManager(
                () => { },
                _mockWin32.Object,
                _mockDummyWndManager.Object,
                _mockLogger.Object);

            manager.RegisterHotKey(keyPattern.Object);
        }

        [TestMethod]
        public void UnregisterHotKeyTest()
        {
            _mockWin32
                .Setup(m => m.RegisterHotKey(It.IsAny<nint>(), It.IsAny<int>(), It.IsAny<uint>(), It.IsAny<uint>()))
                .Returns(true);

            var keyPattern = new Mock<IKeyPattern>();
            var manager = new HotkeyMessageManager(
                () => { },
                _mockWin32.Object,
                _mockDummyWndManager.Object,
                _mockLogger.Object);

            manager.RegisterHotKey(keyPattern.Object);

            // Act
            manager.UnregisterHotKey();

            // Assert
            _mockWin32.Verify(m => m.UnregisterHotKey(It.IsAny<nint>(), It.IsAny<int>()), Times.Once);
            _mockDummyWndManager.Verify(dm => dm.ReleaseHwndPtr(), Times.Once);
        }
    }
}