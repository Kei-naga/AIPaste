using System;
using System.Linq;
using System.Threading.Tasks;
using AIPaste.common;
using AIPaste.Models.DTO;
using AIPaste.ViewModels;
using AIPaste.Views;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AIPaste
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly MyLogger _logger = MyLogger.GetInstance();
        private readonly ResourceLoaderWrapper _resourceLoader = new();
        public MainWindowViewModel ViewModel;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel(OnHotKeyPressed);
            SetNavigationViewitems();
            Closed += OnWindowHideInsteadOfClose;
            AppWindow.Resize(new Windows.Graphics.SizeInt32(600,400));
        }

        private void SetNavigationViewitems()
        {
            mainTab.MenuItems.Clear();
            foreach (var item in GetNavigationViewItems())
            {
                mainTab.MenuItems.Add(item);
            }
        }

        private void MainTab_Loaded(object sender, RoutedEventArgs e)
        {
            contentFrame.Navigated += On_Navigated;
            SetFirstTab(TabName.AiPastePage);
        }

        private void OnWindowHideInsteadOfClose(object sender, WindowEventArgs args)
        {
            _logger.Info("CLOSE_WINDOW");
            args.Handled = true;
            this.Hide();
            this.SetFirstTab(TabName.AiPastePage);
        }

        private void OnHotKeyPressed()
        {
            this.ShowWindow(TabName.AiPastePage);
        }

        public void ShowWindow(TabName tabName)
        {
            if (contentFrame.SourcePageType.Name?.ToString() != tabName.ToString())
            {
                NavigateToPage(tabName);
            }

            if (this.Visible == true)
            {
                SetForegroundWindow();
            }
            Activate();
        }

        private void NavigateToPage(TabName tabName)
        {
            if (tabName == TabName.Settings)
            {
                _logger.Trace("navigating setting page");
                contentFrame.Navigate(typeof(SettingsPage));
            }
            else if (tabName == TabName.AiPastePage)
            {
                _logger.Trace("navigating AiPastePage");
                var appSettings = ViewModel.AppSettings;
                var llmModelSettings = appSettings.GetLlmModelSettings(appSettings.ActiveModelType);
                contentFrame.Navigate(typeof(AiPastePage), llmModelSettings);
            }
        }

        private void SetForegroundWindow()
        {
            var hwnd = new Windows.Win32.Foundation.HWND(WinRT.Interop.WindowNative.GetWindowHandle(this));
            Windows.Win32.PInvoke.SetForegroundWindow(hwnd);
        }

        public void RestoreDefaultClosingBehavior()
        {
            Closed -= OnWindowHideInsteadOfClose;
            Closed += (sender, args) =>
            {
                ViewModel.UnRegisterHotKey();
                args.Handled = false;
                _logger.Info("SHUTDOWN_APP");
            };
        }

        private void SetFirstTab(TabName tabName)
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
                        NavigateToPage(TabName.Settings);
                    }
                    switch (args.SelectedItemContainer.Tag.ToString())
                    {
                        case nameof(TabName.AiPastePage):
                            NavigateToPage(TabName.AiPastePage);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    FailedNavigation(ex, args.SelectedItemContainer.Tag.ToString() ?? "");
                }
            }
        }

        private void NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            FailedNavigation(e.Exception, e.SourcePageType.Name ?? "");
        }

        private void FailedNavigation(Exception ex, string pageName)
        {
            _logger.Error("FAILED_NAVIGATION", pageName);
            _logger.Debug(ex);
            contentFrame.Navigate(typeof(SettingsPage));
            SendDialog(
                _resourceLoader.GetString("Settings_DialogWarning"),
                _resourceLoader.GetString("Settings_DialogInvalidSettings")
            );
        }

        public async void SendDialog(string title, string content)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = _resourceLoader.GetString("Settings_DialogCloseButton")
            };
            if (RootGrid.XamlRoot == null)
            {
                var tcs = new TaskCompletionSource();
                RoutedEventHandler loadedHandler = null!;
                loadedHandler = (s, e) =>
                {
                    RootGrid.Loaded -= loadedHandler;
                    tcs.SetResult();
                };

                RootGrid.Loaded += loadedHandler;

                var timeoutTask = Task.Delay(10000);
                if (await Task.WhenAny(tcs.Task, timeoutTask) == timeoutTask)
                {
                    throw new TimeoutException("RootGrid.XamlRoot initialization timed out.");
                }
            }
            dialog.XamlRoot = RootGrid.XamlRoot;
            await dialog.ShowAsync();
        }

        private void On_Navigated(object? sender, NavigationEventArgs e)
        {
            if (e.Content is SettingsPage settingsPage)
            {
                mainTab.SelectedItem = (NavigationViewItem)mainTab.SettingsItem;
                settingsPage.SettingsUpdated += OnSettingsUpdated;
            }
            else if (e.Content is AiPastePage aiPastePage)
            {
                mainTab.SelectedItem = mainTab.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(i => i.Tag.Equals(contentFrame.SourcePageType.Name?.ToString()));
            }
        }

        private void OnSettingsUpdated(object? sender, EventArgs e)
        {
            SetNavigationViewitems();
        }

        private NavigationViewItem[] GetNavigationViewItems()
        {
            var llmModelType = ViewModel.AppSettings.ActiveModelType;
            var content = "";
            if (llmModelType == ModelType.LocalLLM)
            {
                content = _resourceLoader.GetString("MainPage_TabName_localLlm");
            }
            else if (llmModelType == ModelType.Gemini)
            {
                content = _resourceLoader.GetString("MainPage_TabName_gemini");
            }

            var navigationViewItems = new NavigationViewItem[]
                {
                new() {
                    Content = content,
                    Tag = TabName.AiPastePage.ToString(),
                    Name = llmModelType.ToString(),
                }
                };
            return navigationViewItems;
        }
    }
}
