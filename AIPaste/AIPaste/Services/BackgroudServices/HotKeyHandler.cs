using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.KeyModels;
using Microsoft.UI.Xaml;
using NLog;
using Windows.System;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace AIPaste.Services.BackgroudServices
{
    internal class HotKeyHandler : IDisposable
    {
        private HotkeyMessageDummyWindow? _DummuyWindow;
        private KeyPattern _keyPattern;
        private Action _onHotKeyPressed;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public HotKeyHandler(Action action)
        {
            _onHotKeyPressed = action;
            _keyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C); // Default hotkey
        }

        public void RegisterHotKey(KeyPattern keyPattern)
        {
            _DummuyWindow?.Dispose();
            _keyPattern = keyPattern;
            _DummuyWindow = new HotkeyMessageDummyWindow(_onHotKeyPressed, keyPattern);
        }

        public void Dispose()
        {
            _DummuyWindow?.Dispose();
            _DummuyWindow = null;
        }
    }
}
