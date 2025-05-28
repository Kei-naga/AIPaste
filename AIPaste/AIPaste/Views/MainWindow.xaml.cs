using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AIPaste.common;
using AIPaste.Models.SettingsServices.SettingModels;
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
        private readonly Win32ApiWrapper _win32ApiWrapper = new();

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
            SetFirstTab(GetFirstTab());
        }

        private void OnWindowHideInsteadOfClose(object sender, WindowEventArgs args)
        {
            _logger.Info("CLOSE_WINDOW");
            args.Handled = true;
            this.Hide();
            this.SetFirstTab(GetFirstTab());
        }

        private void OnHotKeyPressed()
        {
            this.ShowWindow(GetFirstTab());
        }

        public void ShowWindow()
        {
            var firstTab = GetFirstTab();
            ShowWindow(firstTab);
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
                mainTab.SelectedItem = mainTab.SettingsItem;
            }
            else if (tabName == TabName.LocalLlm)
            {
                _logger.Trace("navigating AiPastePage");
                var appSettings = ViewModel.AppSettings;
                var llmModelSettings = appSettings.GetLlmModelSettings(ModelType.LocalLLM);
                contentFrame.Navigate(typeof(AiPastePage), llmModelSettings);
                mainTab.SelectedItem = mainTab.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(i => i.Tag.Equals(TabName.LocalLlm.ToString()));
            }
            else if (tabName == TabName.Gemini)
            {
                _logger.Trace("navigating GeminiPage");
                var appSettings = ViewModel.AppSettings;
                var llmModelSettings = appSettings.GetLlmModelSettings(ModelType.Gemini);
                contentFrame.Navigate(typeof(AiPastePage), llmModelSettings);
                mainTab.SelectedItem = mainTab.MenuItems
                            .OfType<NavigationViewItem>()
                            .First(i => i.Tag.Equals(TabName.Gemini.ToString()));
            }
            else
            {
                throw new ArgumentException($"Unknown tab name: {tabName}");
            }

        }

        private void SetForegroundWindow()
        {
            var hwnd = new Windows.Win32.Foundation.HWND(WinRT.Interop.WindowNative.GetWindowHandle(this));
            _win32ApiWrapper.SetForegroundWindow(hwnd);
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

        private TabName GetFirstTab()
        {
            var firstTab = mainTab.MenuItems[0] as NavigationViewItem;
            return firstTab == null
                ? throw new InvalidOperationException("No items in the navigation view.")
                : (TabName)Enum.Parse(typeof(TabName), firstTab.Tag.ToString() ?? "");
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
                        case nameof(TabName.LocalLlm):
                            NavigateToPage(TabName.LocalLlm);
                            break;
                        case nameof(TabName.Gemini):
                            NavigateToPage(TabName.Gemini);
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
            NavigateToPage(TabName.Settings);
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
                settingsPage.SettingsUpdated += OnSettingsUpdated;
            }
        }

        private void OnSettingsUpdated(object? sender, EventArgs e)
        {
            SetNavigationViewitems();
        }

        private NavigationViewItem[] GetNavigationViewItems()
        {
            var activeLlmModels = ViewModel.AppSettings.ActiveLlmModels;
            var navigationViewItems = new List<NavigationViewItem>();

            if (activeLlmModels.IsLocalLlmActive)
            {
                var navigationViewItem = new NavigationViewItem
                {
                    Content = _resourceLoader.GetString("MainPage_TabName_localLlm"),
                    Tag = TabName.LocalLlm.ToString(),
                    Name = ModelType.LocalLLM.ToString(),
                };
                navigationViewItems.Add(navigationViewItem);
            }

            if (activeLlmModels.IsGeminiActive)
            {
                var navigationViewItem = new NavigationViewItem
                {
                    Content = _resourceLoader.GetString("MainPage_TabName_gemini"),
                    Tag = TabName.Gemini.ToString(),
                    Name = ModelType.Gemini.ToString(),
                };
                navigationViewItems.Add(navigationViewItem);
            }

            return [.. navigationViewItems];
        }
    }
}
