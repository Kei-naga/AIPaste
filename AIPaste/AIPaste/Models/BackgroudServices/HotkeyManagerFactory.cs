using System;
using NLog;

namespace AIPaste.Models.BackgroudServices
{
    public class HotkeyManagerFactory : IHotKeyManagerFactory
    {
        public IHotKeyManager CreateHotKeyManager(Action action)
        {
            var logger = LogManager.GetCurrentClassLogger();
            var hotkeyMessageManager = new HotkeyMessageManager(action, logger);
            return HotKeyManager.CreateInstance(hotkeyMessageManager, logger);
        }
    }

    public interface IHotKeyManagerFactory
    {
        IHotKeyManager CreateHotKeyManager(Action action);
    }
}
