using System;
using System.Linq;
using System.Reflection.Metadata;
using System.Resources;
using AIPaste.Models.Common;
using AIPaste.ViewModels;
using AIPaste.Views;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.ApplicationModel.Resources;
using NLog;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.Resources;


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
        public MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel(OnHotKeyPressed);
            AiPastePageItem.Tag = TabName.AiPastePage.ToString();

            Closed += OnWindowHideInsteadOfClose;
            AppWindow.Resize(new Windows.Graphics.SizeInt32(600,400));
        }

        private void MainTab_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigated += On_Navigated;
            SetFirstTab(TabName.AiPastePage);
        }

        private void OnWindowHideInsteadOfClose(object sender, WindowEventArgs args)
        {
            _logger.Info("Close Window");
            args.Handled = true;
            this.Hide();
            this.SetFirstTab(TabName.AiPastePage);
        }

        private void OnHotKeyPressed()
        {
            SetFirstTab(TabName.AiPastePage);
            this.ShowWindow();
        }

        public void ShowWindow()
        {
            if (contentFrame.Content is AiPastePage aiPastePage)
            {
                aiPastePage.FocusUserInputBox();
            }

            if (this.Visible == true)
            {
                var hwnd = new Windows.Win32.Foundation.HWND(WinRT.Interop.WindowNative.GetWindowHandle(this));
                Windows.Win32.PInvoke.SetForegroundWindow(hwnd);
            }
            else
            {
                this.Activate();
            }
        }

        public void RestoreDefaultClosingBehavior()
        {
            Closed += (sender, args) =>
            {
                ViewModel.UnRegisterHotKey();
                args.Handled = false;
                _logger.Info("Shutdown application!");
            };
        }

        public void SetFirstTab(TabName tabName)
        {
            if (tabName == TabName.Settings)
            {
                mainTab.SelectedItem = mainTab.SettingsItem;
                return;
            }
            mainTab.SelectedItem = mainTab.MenuItems.FirstOrDefault(
                x => (x as NavigationViewItem)?.Tag.ToString() == tabName.ToString(), mainTab.MenuItems.IndexOf(0)
            );
        }

        private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItemContainer != null)
            {
                try
                {
                    if (args.IsSettingsSelected)
                    {
                        contentFrame.Navigate(typeof(SettingsPage));
                    }
                    switch (args.SelectedItemContainer.Tag.ToString())
                    {
                        case nameof(TabName.AiPastePage):
                            contentFrame.Navigate(typeof(AiPastePage));
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to load Page " + args.SelectedItemContainer.Tag.ToString());
                    contentFrame.Navigate(typeof(SettingsPage));
                    var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                    SendDialog(
                        resourceLoader.GetString("Settings_DialogWarning"), 
                        resourceLoader.GetString("Settings_DialogInvalidSettings")
                    );
                }
            }
        }

        private void NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            _logger.Error("Failed to load Page " + e.SourcePageType.FullName);
            contentFrame.Navigate(typeof(SettingsPage));
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            SendDialog(
                resourceLoader.GetString("Settings_DialogWarning"), 
                resourceLoader.GetString("Settings_DialogInvalidSettings")
            );
        }

        public async void SendDialog(string title, string content)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = resourceLoader.GetString("Settings_DialogCloseButton")
            };
            while (RootGrid.XamlRoot == null)
            {
                await System.Threading.Tasks.Task.Delay(100);
            }
            dialog.XamlRoot = RootGrid.XamlRoot;
            ContentDialogResult result = await dialog.ShowAsync();
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            if (contentFrame.SourcePageType == typeof(SettingsPage))
            {
                mainTab.SelectedItem = (NavigationViewItem)mainTab.SettingsItem;
            }
            else if (contentFrame.SourcePageType != null)
            {
                mainTab.SelectedItem = mainTab.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(i => i.Tag.Equals(contentFrame.SourcePageType.Name?.ToString()));
            }
        }
    }
}
