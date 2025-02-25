using Microsoft.UI.Xaml;
using AIPaste.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Input;

namespace AIPaste.Views
{
    public sealed partial class AiPastePage : Page
    {
        public AiPastePageViewModel ViewModel;
        private Window? _window;

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
            UserInputBox.Focus(FocusState.Programmatic);
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
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
            _window = App.MainWindow;
            if (_window != null)
            {
                _window.Activated += OnWindowActivated;
            }
            UserInputBox.Focus(FocusState.Programmatic);
        }

        // TODO : マウスでactivateされると一瞬文字にfocusがいくため、治す
        private void OnWindowActivated(object sender, WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == WindowActivationState.CodeActivated)
            {
                UserInputBox.Focus(FocusState.Programmatic);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_window != null)
            {
                _window.Activated -= OnWindowActivated;
                _window = null;
            }
        }
    }
}
