using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AIPaste.ViewModels;
using AIPaste.Models.Settings;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;

namespace AIPaste.Views
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPageViewModel ViewModel;

        public SettingsPage()
        {
            InitializeComponent();
            ViewModel = new SettingsPageViewModel();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.SaveSettings())
            {
                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse();
                App.MainWindow?.SendDialog(
                    resourceLoader.GetString("Settings_DialogWarning"),
                    resourceLoader.GetString("Settings_DialogFailedSave"));

                // Manually updates the display of the model types combo box because, for some reason, it does not update automatically.
                ModelTypesCombo.SelectedIndex = (int)ViewModel.ModelTypeName;
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