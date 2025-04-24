using System;
using AIPaste.common;
using AIPaste.Models.DTO;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace AIPaste.Models.BackgroudServices
{
    public partial class HotkeyMessageManager : IHotkeyMessageManager
    {
#pragma warning disable IDE1006 // 命名スタイル
        private const uint WM_HOTKEY = 0x0312; // Hotkey message
#pragma warning restore IDE1006 // 命名スタイル
        private readonly WNDPROC _hotKeyPrc;
        private readonly Action _onHotKeyPressed;
        private readonly IWin32ApiWrapper _win32ApiWrapper;
        private readonly IDummyWindowManager _dummyWindowManager;
        private readonly IMyLogger _logger;
        private bool _isRegistered = false;

        private int _hotkeyId;
        private HWND _hwnd;
        private IntPtr _origPrc;

        public HotkeyMessageManager(Action action, IWin32ApiWrapper? win32ApiWrapper = null, IDummyWindowManager? dummyWindowManager = null, IMyLogger? logger = null)
        {
            _logger = logger ?? MyLogger.GetInstance();
            _win32ApiWrapper = win32ApiWrapper ?? new Win32ApiWrapper();
            _dummyWindowManager = dummyWindowManager ?? new DummyWindowManager();
            _onHotKeyPressed = action;
            _hotKeyPrc = HotKeyPrc;
        }

        public void RegisterHotKey(IKeyPattern keyPattern)
        {
            IntializeSettings();
            int attempts = 0;
            while (attempts < 3)
            {
                var success = _win32ApiWrapper.RegisterHotKey(_hwnd, _hotkeyId, (uint)keyPattern.Modifiers, (uint)keyPattern.Key);
                if (success)
                {
                    _logger.Trace($"Hotkey registered: {keyPattern}");
                    var hotKeyPrcPointer = _win32ApiWrapper.GetFunctionPointerForDelegate(_hotKeyPrc);
                    _origPrc = _win32ApiWrapper.SetWindowLongPtr(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, hotKeyPrcPointer);
                    _isRegistered = true;
                    return;
                }
                _logger.Trace($"Failed to register hotkey (Attempt {attempts + 1}/3). Retrying...");
                _hotkeyId = GetHashCode();
                attempts++;
            }
            _isRegistered = false;
            throw new InvalidOperationException($"Failed to register hotkey: {keyPattern}. Error Code: {_win32ApiWrapper.GetLastWin32Error()}");
        }

        private void IntializeSettings()
        {
            UnregisterHotKey();
            _hotkeyId = GetHashCode();;
            _hwnd = new HWND(_dummyWindowManager.GetHwndPtr());
        }

        private LRESULT HotKeyPrc(HWND hwnd, uint uMsg, WPARAM wParam, LPARAM lParam)
        {
            try
            {
                if (uMsg == WM_HOTKEY)
                {
                    _onHotKeyPressed.Invoke();
                    return (LRESULT)nint.Zero;
                }
                var intPtrLRESULT = _win32ApiWrapper.CallWindowProc(_origPrc, hwnd, uMsg, wParam, lParam);
                return new LRESULT(intPtrLRESULT);
            }
            catch (Exception ex)
            {
                _logger.Error("Error in HotKeyPrc");
                _logger.Debug(ex);
                return (LRESULT)nint.Zero;
            }
        }

        public void UnregisterHotKey()
        {
            if (_isRegistered)
            {
                _win32ApiWrapper.UnregisterHotKey(_hwnd, _hotkeyId);
                _dummyWindowManager.ReleaseHwndPtr();
                _isRegistered = false;
            }
        }
    }

    public interface IHotkeyMessageManager
    {
        void RegisterHotKey(IKeyPattern keyPattern);
        void UnregisterHotKey();
    }
}
