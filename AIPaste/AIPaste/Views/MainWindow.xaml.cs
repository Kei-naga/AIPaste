using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using AIPaste.Common;
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
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AIPaste
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly ICommand IconClicked;
        private readonly ICommand ExitClicked;
        private readonly ICommand SettingsClicked;
        private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
            IconClicked = new RelayCommand(_ => App.MainWindow?.Activate());
            ExitClicked = new RelayCommand(_ =>
            {
                App.MainWindow?.RestoreDefaultClosingBehavior();
                App.Current.Exit();
            });
            SettingsClicked = new RelayCommand(_ =>
            {
                App.MainWindow?.SetFirstTab("SettingsPage");
                App.MainWindow?.Activate();
            });
            Closed += OnWindowHideInsteadOfClose;
            SetFirstTab("AiPastePage");
        }

        private void OnWindowHideInsteadOfClose(object sender, WindowEventArgs args)
        {
            _logger.Info("Close Window");
            args.Handled = true;
            this.Hide();
            this.SetFirstTab("AiPastePage");
        }

        public void RestoreDefaultClosingBehavior()
        {
            Closed += (sender, args) =>
            {
                _logger.Info("Shutdown application!");
                args.Handled = false;
            };
        }

        public void SetFirstTab(string tabName)
        {
            mainTab.SelectedItem = mainTab.MenuItems.FirstOrDefault(x => (x as NavigationViewItem)?.Tag.ToString() == tabName, mainTab.MenuItems.IndexOf(0));
        }

        private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                var selectedItemTag = args.SelectedItemContainer.Tag.ToString();
                switch (selectedItemTag)
                {
                    case "AiPastePage":
                        contentFrame.Navigate(typeof(AiPastePage));
                        break;
                    case "SettingsPage":
                        contentFrame.Navigate(typeof(SettingsPage));
                        break;
                }
            }
        }
    }
}
