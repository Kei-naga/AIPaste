using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using AIPaste.Services;
using AIPaste.ViewModels;
using System.ComponentModel;

namespace AIPaste
{
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel;

        public MainWindow()
        {
            this.InitializeComponent();
            // ウィンドウサイズを設定
            this.ExtendsContentIntoTitleBar = false; // タイトルバーのカスタマイズを無効化
            this.SetWindowSize(600, 400);

           ViewModel = new MainWindowViewModel();
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

        private void SetWindowSize(int width, int height)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                appWindow.Resize(new Windows.Graphics.SizeInt32(width, height));
            }
        }
    }
}
