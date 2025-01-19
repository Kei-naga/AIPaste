using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using AIPaste.Services;

namespace AIPaste
{
    public sealed partial class MainWindow : Window
    {
        private readonly LLMService _llmService;
        private readonly string TargetText;

        public MainWindow()
        {
            this.InitializeComponent();

            // ウィンドウサイズを設定
            this.ExtendsContentIntoTitleBar = false; // タイトルバーのカスタマイズを無効化
            this.SetWindowSize(600, 400);

            // LLMServiceの初期化
            string modelPath = @"C:\Users\keita\llama\Llama-3-ELYZA-JP-8B-GGUF\Llama-3-ELYZA-JP-8B-q4_k_m.gguf"; // モデルファイルパス
            _llmService = new LLMService(modelPath, gpuLayerCount: 32, contextSize: 1024);

            string systemPrompt = "あなたは文章編集の専門家です。対象テキストとユーザ指示を与えるので、対象テキストをユーザ指示に厳密に従って、適切に修正してください。回答は理由等はなにも書かず、修正した文章のみを記載してください。また元の意味や意図が変わらないよう注意してください。"; // システムプロンプト
            TargetText = "モデルが不適切なやつ表示したらまずいから、特定のフレーズ避けるようにできる";
            TargetTextBlock.Text = TargetText;

            // モデル初期化とチャットセッション開始
            _llmService.Initialize();
            _llmService.StartChat(systemPrompt);
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            // ユーザー入力取得
            string userInput = UserInputBox.Text;
            var prompt = "対象テキスト：" + TargetText + Environment.NewLine + "ユーザ指示：" + userInput;

            if (string.IsNullOrWhiteSpace(userInput))
            {
                OutputTextBlock.Text = "Please enter a valid prompt.";
                return;
            }

            // 出力エリアをクリア
            OutputTextBlock.Text = "";

            // 生成結果をリアルタイムで表示
            var sb = new StringBuilder();
            await foreach (var chunk in _llmService.GeneratingText(prompt))
            {
                sb.Append(chunk);
                OutputTextBlock.Text = sb.ToString(); // リアルタイム更新
            }
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
