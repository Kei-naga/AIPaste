using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Win32.UI.WindowsAndMessaging;
using Windows.Win32.Foundation;
using Windows.Win32;
using System.Runtime.InteropServices;
using AIPaste.Models.DataModels;

namespace AIPaste.Models.BackgroudServices
{
    internal class SystemHotkeyControler: IHotkeyControler
    {
        public bool RegisterHotKey(IntPtr hwnd, int hotkeyId, HOT_KEY_MODIFIERS modifier, uint key)
        {
            return PInvoke.RegisterHotKey(new HWND(hwnd), hotkeyId, (Windows.Win32.UI.Input.KeyboardAndMouse.HOT_KEY_MODIFIERS)modifier, key);
        }
        public IntPtr SetHotKeyProc(IntPtr hwnd, Delegate hotKeyPrc)
        {
            var hotKeyPrcPointer = Marshal.GetFunctionPointerForDelegate(hotKeyPrc);
            return PInvoke.SetWindowLongPtr(
                new HWND(hwnd), 
                WINDOW_LONG_PTR_INDEX.GWL_WNDPROC,
                hotKeyPrcPointer
            );
        }
        public void UnregisterHotKey(IntPtr hwnd, int hotkeyId)
        {
            PInvoke.UnregisterHotKey(new HWND(hwnd), hotkeyId);
        }

        public IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
        {
            var wndProc = Marshal.GetDelegateForFunctionPointer<Windows.Win32.UI.WindowsAndMessaging.WNDPROC>(lpPrevWndFunc);
            var hwnd = new HWND(hWnd);
            var wparam = new WPARAM((nuint)wParam);
            var lparam = new LPARAM(lParam);
            LRESULT result = PInvoke.CallWindowProc(wndProc, hwnd, Msg, wparam, lparam);
            return result.Value;
        }
    }
}
