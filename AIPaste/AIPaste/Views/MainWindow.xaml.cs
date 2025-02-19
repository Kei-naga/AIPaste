using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using AIPaste.Common;
using AIPaste.Models.KeyModels;
using AIPaste.Services.BackgroudServices;
using AIPaste.Views;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using NLog;
using Windows.System;
using Windows.Win32.UI.Input.KeyboardAndMouse;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AIPaste
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly HotKeyHandler _hotKeyHandler;

        public MainWindow()
        {
            InitializeComponent();
            Closed += OnWindowHideInsteadOfClose;
            SetFirstTab("AiPastePage");
            AppWindow.Resize(new Windows.Graphics.SizeInt32(600,400));

            _hotKeyHandler = new(() =>
            {
                this.ShowWindow();
            });
            _hotKeyHandler.RegisterHotKey(new KeyPattern(HOT_KEY_MODIFIERS.MOD_CONTROL | HOT_KEY_MODIFIERS.MOD_ALT, VirtualKey.C));
        }

        private void OnWindowHideInsteadOfClose(object sender, WindowEventArgs args)
        {
            _logger.Info("Close Window");
            args.Handled = true;
            this.Hide();
            this.SetFirstTab("AiPastePage");
        }

        public void ShowWindow()
        {
            if (contentFrame.Content is AiPastePage aiPastePage)
            {
                aiPastePage.FocusUserInputBox();
            }
            this.Activate();
        }

        public void RestoreDefaultClosingBehavior()
        {
            Closed += (sender, args) =>
            {
                _hotKeyHandler.Dispose();
                args.Handled = false;
                _logger.Info("Shutdown application!");
            };
        }

        public void SetFirstTab(string tabName)
        {
            if (tabName == "Settings")
            {
                mainTab.SelectedItem = mainTab.SettingsItem;
                return;
            }
            mainTab.SelectedItem = mainTab.MenuItems.FirstOrDefault(x => (x as NavigationViewItem)?.Tag.ToString() == tabName, mainTab.MenuItems.IndexOf(0));
        }

        private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                if (args.IsSettingsSelected)
                {
                    contentFrame.Navigate(typeof(SettingsPage));
                }
                switch (args.SelectedItemContainer.Tag.ToString())
                {
                    case "AiPastePage":
                        contentFrame.Navigate(typeof(AiPastePage));
                        break;
                }
            }
        }
    }
}
