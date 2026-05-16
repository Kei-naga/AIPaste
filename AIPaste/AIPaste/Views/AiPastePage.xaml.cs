using System;
using AIPaste.common;
using AIPaste.Models.SettingsServices.SettingModels;
using AIPaste.ViewModels;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;

namespace AIPaste.Views
{
    public sealed partial class AiPastePage : Page
    {
        public AiPastePageViewModel? ViewModel;
        private readonly MyLogger _logger = MyLogger.GetInstance();

        public AiPastePage()
        {
            InitializeComponent();
            CrashDiagnostics.WriteTrace("AiPastePage constructed");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CrashDiagnostics.WriteTrace($"AiPastePage.OnNavigatedTo parameter type: {e.Parameter?.GetType().FullName ?? "<null>"}");

            if (e.Parameter is ILlmModelSettings llmModelSettings)
            {
                ViewModel = new AiPastePageViewModel(llmModelSettings);
                CrashDiagnostics.WriteTrace("AiPastePageViewModel created successfully");
            }
            else
            {
                throw new ArgumentException("Parameter is not of type ILlmModelSettings");
            }
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                _logger.Debug("ViewModel is null when clicking the generate text button");
                return;
            }

            string userInput = UserInputBox.Text;
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return;
            }
            UserInputBox.IsEnabled = false;
            UserInputBox.Text = "";
            await ViewModel.GeneratingText(userInput);
            UserInputBox.IsEnabled = true;
            UserInputBox.Focus(FocusState.Programmatic);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel == null)
            {
                _logger.Debug("ViewModel is null when clicking the confirm button");
                return;
            }

            ViewModel.ChangeTargetText();
            UserInputBox.Focus(FocusState.Programmatic);
        }

        private void UserInputBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter &&
                !InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Shift).HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down))
            {
                e.Handled = true;
                GenerateButton_Click(sender, e);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CrashDiagnostics.WriteTrace("AiPastePage loaded");
            UserInputBox.Focus(FocusState.Programmatic);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
