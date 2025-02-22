using System.Linq;
using AIPaste.Models.Common;
using AIPaste.ViewModels;
using AIPaste.Views;
using H.NotifyIcon;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using NLog;


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
            SetFirstTab(TabName.AiPastePage);
            AppWindow.Resize(new Windows.Graphics.SizeInt32(600,400));
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
        }
    }
}
