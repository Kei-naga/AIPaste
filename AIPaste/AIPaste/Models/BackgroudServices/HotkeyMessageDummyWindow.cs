using System;
using System.Runtime.InteropServices;
using AIPaste.Models.DataModels;
using Microsoft.UI.Xaml;
using Windows.Win32;

namespace AIPaste.Models.BackgroudServices
{
    internal class HotkeyMessageDummyWindow : Window
    {
        private const uint WM_HOTKEY = 0x0312; // Hotkey message
        private Windows.Win32.UI.WindowsAndMessaging.WNDPROC? _origPrc;
        private readonly Windows.Win32.UI.WindowsAndMessaging.WNDPROC _hotKeyPrc;
        private readonly Windows.Win32.Foundation.HWND _hwnd;
        private readonly Action _onHotKeyPressed;
        private int _hotkeyId;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public HotkeyMessageDummyWindow(Action action)
        {
            _hotkeyId = GetHashCode();
            _onHotKeyPressed = action;
            _hotKeyPrc = HotKeyPrc;
            _hwnd = new Windows.Win32.Foundation.HWND(WinRT.Interop.WindowNative.GetWindowHandle(this).ToInt32());
        }

        public bool RegisterHotKey(KeyPattern keyPattern)
        {
            int attempts = 0;
            while (attempts < 3)
            {
                var success = PInvoke.RegisterHotKey(_hwnd, _hotkeyId, keyPattern.Modifiers, (uint)keyPattern.Key);
                if (success)
                {
                    _logger.Info($"Hotkey registered: {keyPattern}");
                    var hotKeyPrcPointer = Marshal.GetFunctionPointerForDelegate(_hotKeyPrc);
                    _origPrc = Marshal.GetDelegateForFunctionPointer<Windows.Win32.UI.WindowsAndMessaging.WNDPROC>(PInvoke.SetWindowLongPtr(_hwnd, Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, hotKeyPrcPointer));
                    return true;
                }
                _logger.Debug("Failed to register hotkey, trying again");
                _hotkeyId = GetHashCode();
                attempts++;
            }
            _logger.Error("Failed to register hotkey");
            return false;
        }

        private Windows.Win32.Foundation.LRESULT HotKeyPrc(Windows.Win32.Foundation.HWND hwnd,
            uint uMsg,
            Windows.Win32.Foundation.WPARAM wParam,
            Windows.Win32.Foundation.LPARAM lParam)
        {
            if (uMsg == WM_HOTKEY)
            {
                _onHotKeyPressed.Invoke();

                return (Windows.Win32.Foundation.LRESULT)nint.Zero;
            }

            return PInvoke.CallWindowProc(_origPrc, hwnd, uMsg, wParam, lParam);
        }

        public void Dispose()
        {
            _logger.Info("Unregister hotkey");
            PInvoke.UnregisterHotKey(_hwnd, _hotkeyId);
            Close();
        }
    }
}
