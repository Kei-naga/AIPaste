using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using AIPaste.Services;
using AIPaste.ViewModels;
using System.ComponentModel;
using WinRT;
using Microsoft.UI.Xaml.Controls;
using Windows.Devices.Enumeration;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Navigation;

namespace AIPaste.Views
{
    public sealed partial class AiPastePage : Page
    {
        public AiPastePageViewModel ViewModel;

        public AiPastePage()
        {
            this.InitializeComponent();
            ViewModel = new AiPastePageViewModel();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            string userInput = UserInputBox.Text;
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return;
            }
            UserInputBox.IsEnabled = false;
            UserInputBox.Text = "";
            await ViewModel.GeneratingText(userInput);
            UserInputBox.IsEnabled = true;
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeTargetText();
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
            UserInputBox.Focus(FocusState.Programmatic);
        }
        public void FocusUserInputBox()
        {
            UserInputBox.Focus(FocusState.Programmatic);
        }
    }
}
