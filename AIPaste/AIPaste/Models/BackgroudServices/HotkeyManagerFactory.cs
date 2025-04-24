using System;
using AIPaste.common;

namespace AIPaste.Models.BackgroudServices
{
    public class HotkeyManagerFactory : IHotKeyManagerFactory
    {
        public IHotKeyManager CreateHotKeyManager(Action action)
        {

            var win32ApiWrapper = new Win32ApiWrapper();
            var dummyWindowManager = new DummyWindowManager();
            var logger = MyLogger.GetInstance();
            var hotkeyMessageManager = new HotkeyMessageManager(action, win32ApiWrapper, dummyWindowManager, logger);
            return HotKeyManager.CreateInstance(hotkeyMessageManager, logger);
        }
    }

    public interface IHotKeyManagerFactory
    {
        IHotKeyManager CreateHotKeyManager(Action action);
    }
}
