using System;
using AIPaste.Services.BackgroudServices;

namespace AIPaste.Models.BackgroudServices
{
    public class HotkeyManagerFactory : IHotKeyManagerFactory
    {
        public IHotKeyManager CreateHotKeyManager(Action action)
        {
            return HotKeyManager.GetInstance(action);
        }
    }

    public interface IHotKeyManagerFactory
    {
        IHotKeyManager CreateHotKeyManager(Action action);
    }
}
