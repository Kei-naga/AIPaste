using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.DataModels;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace AIPaste.Models.BackgroudServices
{
    public interface IHotkeyControler
    {
        bool RegisterHotKey(IntPtr hwnd, int hotkeyId, HOT_KEY_MODIFIERS modifier, uint key);
        IntPtr SetHotKeyProc(IntPtr hwnd, Delegate hotKeyPrc);
        void UnregisterHotKey(IntPtr hwnd, int hotkeyId);
        IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}
