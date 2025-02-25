using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;
using AIPaste.Common;
using AIPaste.Models.Common;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AIPaste.Views
{
    public sealed partial class TrayView : UserControl
    {
        private readonly ICommand IconClicked;
        private readonly ICommand ExitClicked;
        private readonly ICommand SettingsClicked;
        public TrayView()
        {
            InitializeComponent();
            IconClicked = new RelayCommand(_ => App.MainWindow?.ShowWindow(TabName.AiPastePage));
            ExitClicked = new RelayCommand(_ =>
            {
                App.MainWindow?.RestoreDefaultClosingBehavior();
                App.MainWindow?.Close();
            });
            SettingsClicked = new RelayCommand(_ =>
            {
                App.MainWindow?.ShowWindow(TabName.Settings);
            });
        }
    }
}
