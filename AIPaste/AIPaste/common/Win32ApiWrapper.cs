using System;
using System.Runtime.InteropServices;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.WindowsAndMessaging;

namespace AIPaste.common;

public class Win32ApiWrapper : IWin32ApiWrapper
{
    public bool RegisterHotKey(IntPtr hwnd, int id, uint modifiers, uint key)
    {
        return PInvoke.RegisterHotKey(new HWND(hwnd.ToInt32()), id, (HOT_KEY_MODIFIERS)modifiers, key);
    }

    public bool UnregisterHotKey(IntPtr hwnd, int id)
    {
        return PInvoke.UnregisterHotKey(new HWND(hwnd.ToInt32()), id);
    }

    public IntPtr SetWindowLongPtr(IntPtr hwnd, int nIndex, IntPtr dwNewLong)
    {
        return PInvoke.SetWindowLongPtr(new HWND(hwnd.ToInt32()), (WINDOW_LONG_PTR_INDEX)nIndex, dwNewLong);
    }

    public IntPtr GetFunctionPointerForDelegate(Delegate d)
    {
        return Marshal.GetFunctionPointerForDelegate(d);
    }

    public TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr ptr) where TDelegate : Delegate
    {
        return Marshal.GetDelegateForFunctionPointer<TDelegate>(ptr);
    }

    public nint CallWindowProc(IntPtr origPrc, IntPtr hwndPtr, uint uMsg, ulong wParam, long lParam)
    {
        HWND hwnd = new(hwndPtr.ToInt32());
        var wndProc = GetDelegateForFunctionPointer<Windows.Win32.UI.WindowsAndMessaging.WNDPROC>(origPrc);
        return PInvoke.CallWindowProc(wndProc, hwnd, uMsg, (WPARAM)wParam, (LPARAM)lParam);
    }

    public int GetLastWin32Error()
    {
        return Marshal.GetLastWin32Error();
    }

    public bool SetForegroundWindow(IntPtr hwnd)
    {
        return PInvoke.SetForegroundWindow(new HWND(hwnd.ToInt32()));
    }
}

public interface IWin32ApiWrapper
{
    bool RegisterHotKey(IntPtr hwnd, int id, uint modifiers, uint key);
    bool UnregisterHotKey(IntPtr hwnd, int id);
    IntPtr SetWindowLongPtr(IntPtr hwnd, int nIndex, IntPtr dwNewLong);
    IntPtr GetFunctionPointerForDelegate(Delegate d);
    TDelegate GetDelegateForFunctionPointer<TDelegate>(IntPtr ptr) where TDelegate : Delegate;
    nint CallWindowProc(IntPtr wndProc, IntPtr hwnd, uint uMsg, ulong wParam, long lParam);
    int GetLastWin32Error();
    bool SetForegroundWindow(IntPtr hwnd);
}
