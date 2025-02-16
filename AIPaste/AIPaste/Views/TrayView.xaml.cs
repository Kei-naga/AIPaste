using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Windows.Input;
using AIPaste.Common;

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
            IconClicked = new RelayCommand(_ => App.MainWindow?.Activate());
            ExitClicked = new RelayCommand(_ =>
            {
                App.MainWindow?.RestoreDefaultClosingBehavior();
                App.Current.Exit();
            });
            SettingsClicked = new RelayCommand(_ =>
            {
                App.MainWindow?.SetFirstTab("Settings");
                App.MainWindow?.Activate();
            });
        }
    }
}
