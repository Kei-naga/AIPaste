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
            // ユーザー入力取得
            string userInput = UserInputBox.Text;

            if (string.IsNullOrWhiteSpace(userInput))
            {
                OutputTextBlock.Text = "Please enter a valid prompt.";
                return;
            }

            // 生成結果をリアルタイムで表示
            await ViewModel.GeneratingText(userInput);
            UserInputBox.Text = "";
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ChangeTargetText();
        }

    }
}
