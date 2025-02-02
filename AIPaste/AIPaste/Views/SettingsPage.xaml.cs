using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using AIPaste.ViewModels;

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
            ModelTypesCombo.SelectedIndex = (int)ViewModel.ModelType;
        }
    }
}