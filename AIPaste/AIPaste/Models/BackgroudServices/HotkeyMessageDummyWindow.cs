using System;
using System.Runtime.InteropServices;
using AIPaste.Models.DataModels;
using Microsoft.UI.Xaml;
using NLog;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace AIPaste.Models.BackgroudServices
{
    internal class HotkeyMessageDummyWindow : Window
    {
        private const uint WM_HOTKEY = 0x0312; // Hotkey message
        private IntPtr _origPrc;
        private readonly WNDPROC _hotKeyPrc;
        private readonly HWND _hwnd;
        private readonly Action _onHotKeyPressed;
        private int _hotkeyId;
        private readonly IHotkeyControler _hotkeyControler;
        private readonly ILogger _logger;

        public HotkeyMessageDummyWindow(Action action, IHotkeyControler hotkeyControler, ILogger? logger = null)
        {
            _hotkeyControler = hotkeyControler;
            _logger = logger ?? LogManager.GetCurrentClassLogger();
            _hotkeyId = GetHashCode();
            _onHotKeyPressed = action;
            _hotKeyPrc = HotKeyPrc;
            _hwnd = new HWND(WinRT.Interop.WindowNative.GetWindowHandle(this).ToInt32());
        }

        public bool RegisterHotKey(KeyPattern keyPattern)
        {
            int attempts = 0;
            while (attempts < 3)
            {
                var success = _hotkeyControler.RegisterHotKey(_hwnd, _hotkeyId, keyPattern.Modifiers, (uint)keyPattern.Key);
                if (success)
                {
                    _logger.Info($"Hotkey registered: {keyPattern}");
                    _origPrc = _hotkeyControler.SetHotKeyProc(_hwnd, _hotKeyPrc);
                    return true;
                }
                _logger.Trace("Failed to register hotkey, trying again");
                _hotkeyId = GetHashCode();
                attempts++;
            }
            _logger.Error("Failed to register hotkey");
            return false;
        }

        private LRESULT HotKeyPrc(HWND hwnd,
            uint uMsg,
            WPARAM wParam,
            LPARAM lParam)
        {
            if (uMsg == WM_HOTKEY)
            {
                _onHotKeyPressed.Invoke();

                return (LRESULT)nint.Zero;
            }
            var intPtrLRESULT = _hotkeyControler.CallWindowProc(
                _origPrc,
                hwnd, 
                uMsg,
                new IntPtr((int)wParam.Value), 
                lParam
            );
            return new LRESULT(intPtrLRESULT.ToInt32());
        }

        public void Dispose()
        {
            _logger.Info("Unregister hotkey");
            _hotkeyControler.UnregisterHotKey(_hwnd, _hotkeyId);
            Close();
        }
    }
}
