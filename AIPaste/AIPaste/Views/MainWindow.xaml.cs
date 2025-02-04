using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using AIPaste.Views;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
        private readonly IconClickedCommand IconClicked;
        private readonly ExitClickedCommand ExitClicked;
        private readonly SettingsClickedCommand SettingsClicked;
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            InitializeComponent();
            IconClicked = new IconClickedCommand();
            ExitClicked = new ExitClickedCommand();
            SettingsClicked = new SettingsClickedCommand();
            Closed += OnWindowHideInsteadOfClose;
            mainTab.SelectedItem = mainTab.MenuItems[0];
        }

        private void OnWindowHideInsteadOfClose(object sender, WindowEventArgs args)
        {
            _logger.Info("Close Window");
            args.Handled = true;
            this.Hide();
            this.SetFirstTab(0);
        }

        public void RestoreDefaultClosingBehavior()
        {
            Closed += (sender, args) => {
                _logger.Info("Shutdown application!");
                args.Handled = false;
            };
        }

        public void SetFirstTab(int tabIndex)
        {
            mainTab.SelectedItem = mainTab.MenuItems[tabIndex];
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

    public partial class IconClickedCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged = null;
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            App.MainWindow?.Activate();
        }
    }

    public partial class ExitClickedCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged = null;
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            App.MainWindow?.RestoreDefaultClosingBehavior();
            App.Current.Exit();
        }
    }

    public partial class SettingsClickedCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged = null;
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            App.MainWindow?.SetFirstTab(1);
            App.MainWindow?.Activate();
        }
    }
}
