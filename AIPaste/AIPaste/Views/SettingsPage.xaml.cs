using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AIPaste.ViewModels;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.Resources;
using AIPaste.common;
using System;
using Microsoft.UI.Xaml.Navigation;

namespace AIPaste.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPageViewModel ViewModel;
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();
        private readonly MyLogger _logger = MyLogger.GetInstance();

        public event EventHandler? SettingsUpdated;

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = new SettingsPageViewModel();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ViewModel.SaveSettings())
                {
                    SettingsUpdated?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    App.MainWindow?.SendDialog(
                        _resourceLoader.GetString("Settings_DialogWarning"),
                        _resourceLoader.GetString("Settings_DialogFailedSave"));
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error("FAILED_SAVING");
                _logger.Debug(ex);
            }
        }

        private void HotkeyModifiersToggle_UnChecked(object sender, RoutedEventArgs e)
        {
            if (!(HotkeyCtrl.IsChecked == true || HotkeyAlt.IsChecked == true || HotkeyShift.IsChecked == true || HotkeyWin.IsChecked == true))
            {
                ToggleButton toggleButton = (ToggleButton)sender;
                toggleButton.IsChecked = true;
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SettingsUpdated = null;
        }
    }
}