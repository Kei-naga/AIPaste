using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIPaste.Models.BackgroudServices;
using AIPaste.Models.DataModels;
using Windows.System;

namespace AIPaste.Services.BackgroudServices.Tests
{

    // TODO
    [TestClass()]
    public class HotKeyManagerTests
    {
        
        //[TestMethod]
        //public void RegisterHotKeyTest()
        //{
        //        var hotkeyControler = new HotkeyControlerStub();
        //        var count = 0;
        //        var action = new Action(() => { count++; });
        //        var hotKeyManager = new HotKeyManager(action);
        //        var dummyKeyPattern = new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C);

        //        hotKeyManager.RegisterHotKey(dummyKeyPattern, hotkeyControler);
        //        hotkeyControler.PressHotKey();
        //        hotkeyControler.PressHotKey();


        //        Assert.AreEqual(2, count);
        //}

        //[TestMethod()]
        //public void DisposeTest()
        //{
        //    Assert.Fail();
        //}
    }

    internal class HotkeyControlerStub(bool isSuccessRegister = true) : IHotkeyControler
    {
        private bool _isSuccessRegister = isSuccessRegister;
        private Delegate? _hotKeyPrc;

        public bool RegisterHotKey(IntPtr hwnd, int hotkeyId, HOT_KEY_MODIFIERS modifier, uint key)
        {
            return _isSuccessRegister;
        }

        public IntPtr SetHotKeyProc(IntPtr hwnd, Delegate hotKeyPrc)
        {
            _hotKeyPrc = hotKeyPrc;
            return IntPtr.Zero;
        }

        public void UnregisterHotKey(IntPtr hwnd, int hotkeyId) { }

        public IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam)
        {
            return IntPtr.Zero;
        }

        public void PressHotKey()
        {
            _hotKeyPrc?.DynamicInvoke();
        }
    }
}