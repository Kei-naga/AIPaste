using System;
using System.Runtime.InteropServices;
using AIPaste.Models.DataModels;
using Microsoft.UI.Xaml;
using NLog;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace AIPaste.Models.BackgroudServices
{
    internal partial class HotkeyMessageManager : IDisposable
    {
        #pragma warning disable IDE1006 // 命名スタイル
        private const uint WM_HOTKEY = 0x0312; // Hotkey message
        #pragma warning restore IDE1006 // 命名スタイル
        private IntPtr _origPrc;
        private readonly WNDPROC _hotKeyPrc;
        private readonly HWND _hwnd;
        private readonly Action _onHotKeyPressed;
        private int _hotkeyId;
        private readonly Window _dummyWindow;
        private readonly ILogger _logger;

        public HotkeyMessageManager(Action action, ILogger? logger = null)
        {
            _logger = logger ?? LogManager.GetCurrentClassLogger();
            _hotkeyId = GetHashCode();
            _onHotKeyPressed = action;
            _hotKeyPrc = HotKeyPrc;
            _dummyWindow = new Window();
            _hwnd = new HWND(WinRT.Interop.WindowNative.GetWindowHandle(_dummyWindow).ToInt32());
        }

        public bool RegisterHotKey(IKeyPattern keyPattern)
        {
            int attempts = 0;
            while (attempts < 3)
            {
                var success = PInvoke.RegisterHotKey(_hwnd, _hotkeyId, (Windows.Win32.UI.Input.KeyboardAndMouse.HOT_KEY_MODIFIERS)keyPattern.Modifiers, (uint)keyPattern.Key);
                if (success)
                {
                    _logger.Info($"Hotkey registered: {keyPattern}");
                    var hotKeyPrcPointer = Marshal.GetFunctionPointerForDelegate(_hotKeyPrc);
                    _origPrc = PInvoke.SetWindowLongPtr(_hwnd, WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, hotKeyPrcPointer);
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
            var wndProc = Marshal.GetDelegateForFunctionPointer<Windows.Win32.UI.WindowsAndMessaging.WNDPROC>(_origPrc);
            var intPtrLRESULT = PInvoke.CallWindowProc(wndProc, hwnd, uMsg, wParam, lParam);
            return new LRESULT(intPtrLRESULT);
        }

        public void Dispose()
        {
            _logger.Info("Unregister hotkey");
            PInvoke.UnregisterHotKey(_hwnd, _hotkeyId);
            _dummyWindow.Close();
        }
    }
}
