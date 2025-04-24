using System;
using Microsoft.UI.Xaml;

namespace AIPaste.Models.BackgroudServices
{
    public class DummyWindowManager: IDummyWindowManager
    {
        private Window? _dummyWindow;
        public IntPtr GetHwndPtr()
        {
            ReleaseHwndPtr();
            _dummyWindow = new Window();
            return WinRT.Interop.WindowNative.GetWindowHandle(_dummyWindow);
        }

        public void ReleaseHwndPtr()
        {
            _dummyWindow?.Close();
            _dummyWindow = null;
        }

        ~DummyWindowManager()
        {
            ReleaseHwndPtr();
        }
    }

    public interface IDummyWindowManager
    {
        IntPtr GetHwndPtr();
        void ReleaseHwndPtr();
    }
}
