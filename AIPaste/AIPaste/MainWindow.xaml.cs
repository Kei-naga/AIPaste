using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using AIPaste.Services;
using AIPaste.ViewModels;

namespace AIPaste
{
    public sealed partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _mainWindowViewModel;

        public MainWindow()
        {
            this.InitializeComponent();
            // ウィンドウサイズを設定
            this.ExtendsContentIntoTitleBar = false; // タイトルバーのカスタマイズを無効化
            this.SetWindowSize(600, 400);

            _mainWindowViewModel = new MainWindowViewModel();
            TargetTextBlock.Text = _mainWindowViewModel.TargetText;
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

            // 出力エリアをクリア
            OutputTextBlock.Text = "";

            // 生成結果をリアルタイムで表示
            var sb = new StringBuilder();
            await foreach (var chunk in _mainWindowViewModel.GeneratingText(userInput))
            {
                sb.Append(chunk);
                OutputTextBlock.Text = sb.ToString(); // リアルタイム更新
            }
            OutputTextBlock.Text = _mainWindowViewModel.outputText;
            UserInputBox.Text = "";
        }

        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            _mainWindowViewModel.ChangeTargetText();
            TargetTextBlock.Text = _mainWindowViewModel.TargetText;
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
