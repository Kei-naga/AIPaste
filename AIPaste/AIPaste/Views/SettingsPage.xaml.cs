using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AIPaste.ViewModels;
using AIPaste.Models.Settings;
using Microsoft.UI.Xaml.Controls.Primitives;

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
                App.MainWindow?.SendDialog("Warning", "保存に失敗しました。設定を修正前に戻します。設定を見直してください。");
            }
        }

        private void ModelTypesLoaded(object sender, RoutedEventArgs e)
        {
            // Set the selected index of the combo box to the current model type
            ModelTypesCombo.SelectedIndex = (int)ViewModel.ModelType;
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