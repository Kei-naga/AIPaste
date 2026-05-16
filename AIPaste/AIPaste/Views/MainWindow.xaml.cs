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
        private bool _isUpdatingNavigationSelection;
        private TabName? _currentTab;

        public MainWindow()
        {
            CrashDiagnostics.WriteTrace("MainWindow constructor started");
            InitializeComponent();
            ViewModel = new MainWindowViewModel(OnHotKeyPressed);
            SetNavigationViewitems();
            Closed += OnWindowHideInsteadOfClose;
            AppWindow.Resize(new Windows.Graphics.SizeInt32(600, 400));
            CrashDiagnostics.WriteTrace("MainWindow constructor completed");
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
            CrashDiagnostics.WriteTrace("MainTab loaded");
            contentFrame.Navigated += On_Navigated;
            NavigateAndSelectTab(GetFirstTab());
        }

        private void OnWindowHideInsteadOfClose(object sender, WindowEventArgs args)
        {
            _logger.Info("CLOSE_WINDOW");
            args.Handled = true;
            this.Hide();
            NavigateAndSelectTab(GetFirstTab());
        }

        private void OnHotKeyPressed()
        {
            ShowWindow(GetFirstTab());
        }

        public void ShowWindow()
        {
            var firstTab = GetFirstTab();
            ShowWindow(firstTab);
        }

        public void ShowWindow(TabName tabName)
        {
            if (_currentTab != tabName)
            {
                NavigateAndSelectTab(tabName);
            }
            else
            {
                SelectNavigationItem(tabName);
            }

            if (Visible)
            {
                SetForegroundWindow();
            }

            Activate();
        }

        private void NavigateToPage(TabName tabName)
        {
            CrashDiagnostics.WriteTrace($"NavigateToPage start: {tabName}");
            bool isSucceeded;
            if (tabName == TabName.Settings)
            {
                _logger.Trace("navigating setting page");
                isSucceeded = contentFrame.Navigate(typeof(SettingsPage));
            }
            else if (tabName == TabName.LocalLlm)
            {
                _logger.Trace("navigating AiPastePage");
                var appSettings = ViewModel.AppSettings;
                var llmModelSettings = appSettings.GetLlmModelSettings(ModelType.LocalLLM);
                isSucceeded = contentFrame.Navigate(typeof(AiPastePage), llmModelSettings);
            }
            else if (tabName == TabName.Gemini)
            {
                _logger.Trace("navigating GeminiPage");
                var appSettings = ViewModel.AppSettings;
                var llmModelSettings = appSettings.GetLlmModelSettings(ModelType.Gemini);
                isSucceeded = contentFrame.Navigate(typeof(AiPastePage), llmModelSettings);
            }
            else
            {
                throw new ArgumentException($"Unknown tab name: {tabName}");
            }

            if (!isSucceeded)
            {
                throw new InvalidOperationException($"Failed to navigate to {tabName}.");
            }

            _currentTab = tabName;
            CrashDiagnostics.WriteTrace($"NavigateToPage completed: {tabName}");
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

        private void NavigateAndSelectTab(TabName tabName)
        {
            SelectNavigationItem(tabName);
            NavigateToPage(tabName);
        }

        private TabName GetFirstTab()
        {
            var firstTab = mainTab.MenuItems.OfType<NavigationViewItem>().FirstOrDefault();
            if (firstTab?.Tag is string tabNameText &&
                Enum.TryParse<TabName>(tabNameText, out var tabName))
            {
                return tabName;
            }

            return TabName.Settings;
        }

        private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (_isUpdatingNavigationSelection)
            {
                _isUpdatingNavigationSelection = false;
                return;
            }

            if (args.IsSettingsSelected || args.SelectedItemContainer != null)
            {
                try
                {
                    var tabName = GetSelectedTabName(args);
                    if (_currentTab == tabName)
                    {
                        return;
                    }

                    NavigateToPage(tabName);
                }
                catch (Exception ex)
                {
                    FailedNavigation(ex, args.SelectedItemContainer?.Tag?.ToString() ?? nameof(TabName.Settings));
                }
            }
        }

        private void NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            FailedNavigation(e.Exception, e.SourcePageType.Name ?? string.Empty);
        }

        private void FailedNavigation(Exception ex, string pageName)
        {
            CrashDiagnostics.WriteException($"MainWindow.FailedNavigation({pageName})", ex);
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
            CrashDiagnostics.WriteTrace($"Frame navigated to {e.SourcePageType?.Name}");
            if (e.Content is SettingsPage settingsPage)
            {
                settingsPage.SettingsUpdated += OnSettingsUpdated;
            }
        }

        private void OnSettingsUpdated(object? sender, EventArgs e)
        {
            SetNavigationViewitems();
            var nextTab = _currentTab.HasValue
                ? GetAvailableTab(_currentTab.Value)
                : GetFirstTab();
            NavigateAndSelectTab(nextTab);
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

        private void SelectNavigationItem(TabName tabName)
        {
            object? targetItem = GetNavigationItem(tabName);
            if (targetItem == null || ReferenceEquals(mainTab.SelectedItem, targetItem))
            {
                return;
            }

            _isUpdatingNavigationSelection = true;
            mainTab.SelectedItem = targetItem;
        }

        private object? GetNavigationItem(TabName tabName)
        {
            return tabName == TabName.Settings
                ? mainTab.SettingsItem
                : mainTab.MenuItems
                    .OfType<NavigationViewItem>()
                    .FirstOrDefault(i => i.Tag?.ToString() == tabName.ToString());
        }

        private TabName GetSelectedTabName(NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                return TabName.Settings;
            }

            if (args.SelectedItemContainer?.Tag is string tag &&
                Enum.TryParse<TabName>(tag, out var tabName))
            {
                return tabName;
            }

            throw new ArgumentException("Unknown tab name");
        }

        private TabName GetAvailableTab(TabName tabName)
        {
            return GetNavigationItem(tabName) != null
                ? tabName
                : GetFirstTab();
        }
    }
}
