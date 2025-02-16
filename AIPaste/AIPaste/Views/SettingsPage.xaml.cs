using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AIPaste.ViewModels;
using AIPaste.Models.Settings;

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
            ViewModel.SaveSettings();
        }

        private void ModelTypesLoaded(object sender, RoutedEventArgs e)
        {
            // Set the selected index of the combo box to the current model type
            ModelTypesCombo.SelectedIndex = (int)ViewModel.ModelType;
        }
        private void OnModelTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.ModelType = (ModelType)ModelTypesCombo.SelectedValue;
        }
    }
}