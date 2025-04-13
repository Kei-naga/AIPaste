using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AIPaste.ViewModels;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.ApplicationModel.Resources;
using AIPaste.common;

namespace AIPaste.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPageViewModel ViewModel;
        private readonly ResourceLoader _resourceLoader = ResourceLoader.GetForViewIndependentUse();
        private readonly MyLogger _logger = MyLogger.GetInstance();

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = new SettingsPageViewModel();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ViewModel.SaveSettings())
                {
                    App.MainWindow?.SendDialog(
                        _resourceLoader.GetString("Settings_DialogWarning"),
                        _resourceLoader.GetString("Settings_DialogFailedSave"));

                    // Manually updates the display of the model types combo box because, for some reason, it does not update automatically.
                    ModelTypesCombo.SelectedIndex = (int)ViewModel.ModelTypeName;
                }
            }
            catch (System.Exception ex)
            {
                _logger.Error("FAILED_SAVING");
                _logger.Debug(ex);
            }
        }

        private void ModelTypesLoaded(object sender, RoutedEventArgs e)
        {
            ModelTypesCombo.SelectedIndex = (int)ViewModel.ModelTypeName;
        }

        private void HotkeyModifiersToggle_UnChecked(object sender, RoutedEventArgs e)
        {
            if (!(HotkeyCtrl.IsChecked == true || HotkeyAlt.IsChecked == true || HotkeyShift.IsChecked == true || HotkeyWin.IsChecked == true))
            {
                ToggleButton toggleButton = (ToggleButton)sender;
                toggleButton.IsChecked = true;
            }
        }
    }
}